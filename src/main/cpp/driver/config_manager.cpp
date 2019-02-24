#include "config_manager.h"
#include "utils.h"
#include "logger.h"

#ifdef WIN32
#include <windows.h>
#include <codecvt>
#include <thread>
#include <mutex>
#endif

namespace steamvrbridge {
    ConfigManager *ConfigManager::m_instance = nullptr;

#ifdef WIN32
    // Adapted from: https://stackoverflow.com/questions/43664998/readdirectorychangesw-and-getoverlappedresult
    class ConfigManagerPlatform
    {
    public:
        ConfigManagerPlatform()
            : m_hDirectory(INVALID_HANDLE_VALUE)
            , m_hTermEvent(0)
            , m_kLoadConfigDelaySeconds(0.1)
        {

        }

        virtual ~ConfigManagerPlatform()
        {
            Dispose();
        }

        bool Init(const std::string &configFolder)
        {
            m_hDirectory = CreateFileA(
                    configFolder.c_str(),
                    FILE_LIST_DIRECTORY | GENERIC_READ,
                    FILE_SHARE_WRITE | FILE_SHARE_READ,
                    NULL,
                    OPEN_EXISTING,
                    FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OVERLAPPED,
                    NULL);

            if (m_hDirectory == INVALID_HANDLE_VALUE)
            {
                std::string lastErrorMesg= Win32Utils::GetLastErrorAsString();
                Logger::Error("ConfigManagerPlatform::Init() - Failed to open directory: %s", lastErrorMesg.c_str());
                return false;
            }

            m_hTermEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
            if (m_hTermEvent == 0)
            {
                std::string lastErrorMesg= Win32Utils::GetLastErrorAsString();
                Logger::Error("ConfigManagerPlatform::Init() - Failed to allocate thread term event: %s", lastErrorMesg.c_str());

                return false;
            }

            m_directoryMonitorThread = std::thread(&ConfigManagerPlatform::WorkerThread, this);

            return true;
        }

        void Dispose()
        {
            if (m_hTermEvent != 0)
            {
                SetEvent(m_hTermEvent);
                m_directoryMonitorThread.join();

                CloseHandle(m_hTermEvent);
                m_hTermEvent= 0;
            }

            if (m_hDirectory != INVALID_HANDLE_VALUE)
            {
                CloseHandle(m_hDirectory);
                m_hDirectory= INVALID_HANDLE_VALUE;
            }

            // Clean up all the cloned configs
            std::for_each(
                m_configList.begin(), m_configList.end(), 
                [](ConfigState &configState) {
                    delete configState.configNew;
                });
            m_configList.clear();
        }

        void RegisterConfig(Config *config)
        {
            std::lock_guard<std::mutex> scoped_lock(m_configListMutex);

            // See if there is already an entry with this config
            const auto it=
                std::find_if(
                    m_configList.begin(), m_configList.end(), 
                    [config](const ConfigState &entry) -> bool {
                        return entry.configExisting == config;
                    });

            if (it == m_configList.end()) {
                m_configList.push_back({
                    config, // configMainThread
                    config->Clone(), // configWorkerThread
                    false, // bHasWorkerThreadConfigChanged
                    std::chrono::system_clock::now() // lastUpdatedTimestamp
                });
            }
        }

        void PublishModifiedConfigs()
        {
            // Only take the lock if the atomic flag has been set by the worker thread
            if (m_bAnyWorkerThreadConfigsChanged.load() == true)
            {
                // Take the lock so that we can iterate over the config list
                std::lock_guard<std::mutex> scoped_lock(m_configListMutex);

                // Reload any dirty config, 
                // then notify the existing config that it was changed
                bool bAnyPendingConfigLoads= false;
                for(auto iter = m_configList.begin(); iter != m_configList.end(); ++iter)
                {
                    ConfigState &configState= *iter;

                    if (configState.bHasWorkerThreadConfigChanged) 
                    {
                        auto now = std::chrono::system_clock::now();
                        std::chrono::duration<double> diff = now - configState.lastUpdatedTimestamp;

                        if (diff.count() > m_kLoadConfigDelaySeconds)
                        {
                            if (configState.configNew->load())
                            {
                                configState.configExisting->OnConfigChanged(configState.configNew);
                                configState.bHasWorkerThreadConfigChanged= false;
                            }
                            else
                            {
                                // Need to retry load
                                bAnyPendingConfigLoads= true;
                            }
                        }
                        else
                        {
                            // Need to wait on timeout
                            bAnyPendingConfigLoads= true;
                        }
                    }
                }

                if (!bAnyPendingConfigLoads)
                {
                    m_bAnyWorkerThreadConfigsChanged.store(false);
                }
            }
        }

    private:
        void WorkerThread()
        {
            DWORD dwBytes = 0;
            std::vector<BYTE> buffer(1024*64);
            OVERLAPPED overlapped_state{0};
            bool bPending = false, bKeepRunning = true;

            overlapped_state.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
            if (!overlapped_state.hEvent) {
                std::string lastErrorMesg= Win32Utils::GetLastErrorAsString();
                Logger::Error("ConfigManagerPlatform::WorkerThread() - Failed to allocate event for OVERLAPPED IO: %s", lastErrorMesg.c_str());
            }

            HANDLE h[2] = {overlapped_state.hEvent, m_hTermEvent};

            do
            {
                bPending = ReadDirectoryChangesW(
                    m_hDirectory,
                    &buffer[0], (DWORD)buffer.size(),
                    TRUE, FILE_NOTIFY_CHANGE_LAST_WRITE,
                    &dwBytes, &overlapped_state, NULL) == TRUE;

                if (!bPending)
                {
                    std::string lastErrorMesg= Win32Utils::GetLastErrorAsString();
                    Logger::Error("ConfigManagerPlatform::WorkerThread() - ReadDirectoryChangesW failed: %s", lastErrorMesg.c_str());
                }

                switch (WaitForMultipleObjects(2, h, FALSE, INFINITE))
                {
                    case WAIT_OBJECT_0:
                    {
                        if (!GetOverlappedResult(m_hDirectory, &overlapped_state, &dwBytes, TRUE)) {
                            std::string lastErrorMesg= Win32Utils::GetLastErrorAsString();
                            Logger::Error("ConfigManagerPlatform::WorkerThread() - OVERLAPPED IO failed: %s", lastErrorMesg.c_str());
                        }

                        bPending = false;

                        if (dwBytes == 0)
                            break;

                        FILE_NOTIFY_INFORMATION *fni = reinterpret_cast<FILE_NOTIFY_INFORMATION*>(&buffer[0]);
                        do
                        {
                            if (fni->Action != 0)
                            {
                                std::wstring wsFileName(fni->FileName, fni->FileNameLength);
                                std::wstring_convert<std::codecvt_utf8<wchar_t>, wchar_t> converter;
                                std::string fileName= converter.to_bytes(wsFileName);

                                // Trim off any \0's at the end of the string
                                fileName.erase(std::find(fileName.begin(), fileName.end(), '\0'), fileName.end());

                                OnConfigFileChanged(fileName);
                            }

                            if (fni->NextEntryOffset == 0)
                                 break;

                            fni = reinterpret_cast<FILE_NOTIFY_INFORMATION*>(reinterpret_cast<BYTE*>(fni) + fni->NextEntryOffset);
                        }
                        while (true);

                        break;
                    }

                    case WAIT_OBJECT_0+1:
                        bKeepRunning = false;
                        break;

                    case WAIT_FAILED:
                        std::string lastErrorMesg= Win32Utils::GetLastErrorAsString();
                        Logger::Error("ConfigManagerPlatform::WorkerThread() - WaitForMultipleObjects failed: %s", lastErrorMesg.c_str());
                        break;
                }
            }
            while (bKeepRunning);

            if (bPending)
            {
                CancelIo(m_hDirectory);
                GetOverlappedResult(m_hDirectory, &overlapped_state, &dwBytes, TRUE);
            }

            CloseHandle(overlapped_state.hEvent);
        }

        void OnConfigFileChanged(const std::string &filename)
        {
            std::lock_guard<std::mutex> scoped_lock(m_configListMutex);

            auto it= std::find_if(
                m_configList.begin(), m_configList.end(), 
                [filename](const ConfigState &entry) -> bool 
                {
                    const std::string entryFilename= entry.configExisting->getConfigName();
                    return entryFilename.compare(filename) == 0;
                });

            if (it != m_configList.end())
            {
                ConfigState &entry = *it;

                entry.bHasWorkerThreadConfigChanged= true;
                entry.lastUpdatedTimestamp= std::chrono::system_clock::now();
                m_bAnyWorkerThreadConfigsChanged.store(true);
            }
        }

    protected:
        struct ConfigState
        {
            Config *configExisting;
            Config *configNew;
            bool bHasWorkerThreadConfigChanged;
            std::chrono::time_point<std::chrono::system_clock> lastUpdatedTimestamp;
        };

        HANDLE m_hDirectory;
        HANDLE m_hTermEvent;
        std::thread m_directoryMonitorThread;
        std::mutex m_configListMutex;
        std::vector<ConfigState> m_configList;
        std::atomic_bool m_bAnyWorkerThreadConfigsChanged;
        double m_kLoadConfigDelaySeconds;
    };
#endif

	ConfigManager::ConfigManager()
        : m_platformState(new ConfigManagerPlatform())
        , m_configFolderPath("")
	{
	}

    ConfigManager::~ConfigManager()
    {
        delete m_platformState;
    }

    bool ConfigManager::Init()
    {
		std::string home_dir= Utils::Path_GetHomeDirectory();

		m_configFolderPath = home_dir + "/PSMoveSteamVRBridge";   
		if (!Utils::Path_CreateDirectory(m_configFolderPath))
		{
			Logger::Error("ConfigManager::Init() - Failed to create config directory: %s", m_configFolderPath.c_str());
            return false;
		}

        if (!m_platformState->Init(m_configFolderPath))
        {
            Logger::Error("ConfigManager::Init() - Failed to init platform state.");
            return false;
        }

        m_instance= this;
        return true;
    }

    void ConfigManager::Dispose()
    {
        m_instance= nullptr;
    }

    void ConfigManager::RegisterConfig(Config *config)
    {
        m_platformState->RegisterConfig(config);
    }

    void ConfigManager::PollConfigChanges()
    {
        m_platformState->PublishModifiedConfigs();
    }

	const std::string
	ConfigManager::GetConfigDirPath() const
	{
		return m_configFolderPath;
	}
}
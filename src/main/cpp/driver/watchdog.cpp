#include "watchdog.h"
#include "logger.h"
#include <stdarg.h>

namespace steamvrbridge {

	CWatchdogDriver_PSMoveService::CWatchdogDriver_PSMoveService()
		: m_pLogger(nullptr)
		, m_loggerMutex()
		, m_bWasConnected(false)
		, m_bExitSignaled({ false })
		, m_pWatchdogThread(nullptr)
	{
		m_strPSMoveServiceAddress = PSMOVESERVICE_DEFAULT_ADDRESS;
		m_strServerPort = PSMOVESERVICE_DEFAULT_PORT;
	}

	vr::EVRInitError CWatchdogDriver_PSMoveService::Init(vr::IVRDriverContext *pDriverContext)
	{
		VR_INIT_WATCHDOG_DRIVER_CONTEXT(pDriverContext);

		m_pLogger = vr::VRDriverLog();

		WatchdogLog("CWatchdogDriver_PSMoveService::Init - Called");

		vr::IVRSettings *pSettings = vr::VRSettings();
		if (pSettings != nullptr)
		{
			char buf[256];
			vr::EVRSettingsError fetchError;

			pSettings->GetString("psmoveservice", "server_address", buf, sizeof(buf), &fetchError);
			if (fetchError == vr::VRSettingsError_None)
			{
				m_strPSMoveServiceAddress = buf;
				WatchdogLog("CWatchdogDriver_PSMoveService::Init - Overridden Server Address: %s.\n", m_strPSMoveServiceAddress.c_str());
			}
			else
			{
				WatchdogLog("CWatchdogDriver_PSMoveService::Init - Using Default Server Address: %s.\n", m_strPSMoveServiceAddress.c_str());
			}

			pSettings->GetString("psmoveservice", "server_port", buf, sizeof(buf), &fetchError);
			if (fetchError == vr::VRSettingsError_None)
			{
				m_strServerPort = buf;
				WatchdogLog("CWatchdogDriver_PSMoveService::Init - Overridden Server Port: %s.\n", m_strServerPort.c_str());
			}
			else
			{
				WatchdogLog("CWatchdogDriver_PSMoveService::Init - Using Default Server Port: %s.\n", m_strServerPort.c_str());
			}
		}
		else
		{
			WatchdogLog("CWatchdogDriver_PSMoveService::Init - Settings missing!");
		}

		// Watchdog mode on Windows starts a thread that listens for the 'Y' key on the keyboard to 
		// be pressed. A real driver should wait for a system button event or something else from the 
		// the hardware that signals that the VR system should start up.
		m_bExitSignaled = false;
		m_pWatchdogThread = new std::thread(&CWatchdogDriver_PSMoveService::WorkerThreadFunction, this);
		if (!m_pWatchdogThread)
		{
			WatchdogLog("Unable to create watchdog thread\n");
			return vr::VRInitError_Driver_Failed;
		}

		return vr::VRInitError_None;
	}

	void CWatchdogDriver_PSMoveService::Cleanup()
	{
		WatchdogLog("CWatchdogDriver_PSMoveService::Cleanup - Called");

		m_bExitSignaled = true;
		if (m_pWatchdogThread)
		{
			WatchdogLog("CWatchdogDriver_PSMoveService::Cleanup - Stopping worker thread...");
			m_pWatchdogThread->join();
			delete m_pWatchdogThread;
			m_pWatchdogThread = nullptr;
			WatchdogLog("CWatchdogDriver_PSMoveService::Cleanup - Worker thread stopped.");
		}
		else
		{
			WatchdogLog("CWatchdogDriver_PSMoveService::Cleanup - No worker thread active.");
		}

		WatchdogLog("CWatchdogDriver_PSMoveService::Cleanup - Watchdog clean up complete.");
		m_pLogger = nullptr;

		VR_CLEANUP_WATCHDOG_DRIVER_CONTEXT()
	}

	void CWatchdogDriver_PSMoveService::WorkerThreadFunction()
	{
		WatchdogLog("CWatchdogDriver_PSMoveService::WatchdogThreadFunction - Entered\n");

		while (!m_bExitSignaled)
		{
			if (!PSM_GetIsInitialized())
			{
				if (PSM_Initialize(m_strPSMoveServiceAddress.c_str(), m_strServerPort.c_str(), PSM_DEFAULT_TIMEOUT) != PSMResult_Success)
				{
					// Try re-connecting in 1 second
					std::this_thread::sleep_for(std::chrono::seconds(1));
					continue;
				}
			}

			PSM_Update();

			if (PSM_WasSystemButtonPressed())
			{
				WatchdogLog("CWatchdogDriver_PSMoveService::WatchdogThreadFunction - System button pressed. Initiating wake up.\n");
				vr::VRWatchdogHost()->WatchdogWakeUp();
			}

			std::this_thread::sleep_for(std::chrono::milliseconds(100));
		}

		PSM_Shutdown();

		vr::VRDriverLog()->Log("CWatchdogDriver_PSMoveService::WatchdogThreadFunction - Exited\n");
	}

	void CWatchdogDriver_PSMoveService::WatchdogLogVarArgs(const char *pMsgFormat, va_list args)
	{
		char buf[1024];
#if defined( WIN32 )
		vsprintf_s(buf, pMsgFormat, args);
#else
		vsnprintf(buf, sizeof(buf), pMsgFormat, args);
#endif

		if (m_pLogger)
		{
			std::lock_guard<std::mutex> guard(m_loggerMutex);

			m_pLogger->Log(buf);
		}
	}

	void CWatchdogDriver_PSMoveService::WatchdogLog(const char *pMsgFormat, ...)
	{
		va_list args;
		va_start(args, pMsgFormat);

		Logger::DriverLogVarArgs(pMsgFormat, args);

		va_end(args);
	}
}
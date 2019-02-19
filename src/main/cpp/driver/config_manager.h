#pragma once

//-- includes -----
#include <vector>
#include <string>
#include "config.h"

namespace steamvrbridge {

	//-- definitions -----
	class ConfigManager {
	public:
		ConfigManager();
        virtual ~ConfigManager();

        static ConfigManager *GetInstance() { return m_instance; }

        bool Init();
        void Dispose();

        void RegisterConfig(Config *config);
        void PollConfigChanges();

        const std::string GetConfigDirPath() const;

    private:
        static ConfigManager *m_instance;

        class ConfigManagerPlatform *m_platformState;

        std::string m_configFolderPath;
	};
}

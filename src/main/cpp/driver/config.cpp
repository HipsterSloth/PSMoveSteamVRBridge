#include "config_manager.h"
#include "config.h"
#include "utils.h"
#include "logger.h"

// Suppress unhelpful configuru warnings
#ifdef _MSC_VER
    #pragma warning (push)
    #pragma warning (disable: 4996) // This function or variable may be unsafe
    #pragma warning (disable: 4244) // 'return': conversion from 'const int64_t' to 'float', possible loss of data
    #pragma warning (disable: 4715) // configuru::Config::operator[]': not all control paths return a value
#endif
#define CONFIGURU_IMPLEMENTATION 1
#include <configuru.hpp>
#ifdef _MSC_VER
    #pragma warning (pop)
#endif

namespace steamvrbridge {

	Config::Config(const std::string &fnamebase)
	: ConfigFileBase(fnamebase) {
	}

    const std::string 
    Config::getConfigName() const
    {
        return ConfigFileBase + ".json";
    }

	const std::string
	Config::getConfigPath() const
	{
		std::string config_dir= ConfigManager::GetInstance()->GetConfigDirPath();
		std::string config_filepath = config_dir + "/" + getConfigName();

		return config_filepath;
	}

    void 
    Config::init()
    {
        ConfigManager::GetInstance()->RegisterConfig(this);
    }

	bool
	Config::load()
	{
		return load(getConfigPath());
	}

	bool 
	Config::load(const std::string &path)
	{
		bool bLoadedOk = false;

		if (Utils::Path_FileExists( path ) )
		{
			configuru::Config cfg = configuru::parse_file(path, configuru::JSON);
			ReadFromJSON(cfg);
			bLoadedOk = true;
		}

		return bLoadedOk;
	}

    void 
    Config::OnConfigChanged(Config *newConfig)
    {
        // Nothing to do
    }
}
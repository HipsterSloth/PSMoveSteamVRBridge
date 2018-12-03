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
	Config::getConfigPath()
	{
		std::string home_dir= Utils::Path_GetHomeDirectory();
		std::string config_path = home_dir + "/PSMoveSteamVRBridge";
    
		if (!Utils::Path_CreateDirectory(config_path))
		{
			Logger::Error("Config::getConfigPath() - Failed to create config directory: %s", config_path.c_str());
		}

		std::string config_filepath = config_path + "/" + ConfigFileBase + ".json";

		return config_filepath;
	}

	void
	Config::save()
	{
		save(getConfigPath());
	}

	void 
	Config::save(const std::string &path)
	{
		configuru::dump_file(path, WriteToJSON(), configuru::JSON);
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
}
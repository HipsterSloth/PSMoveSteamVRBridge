#pragma once

//-- includes -----
#include <string>

#ifdef _MSC_VER
    #pragma warning (push)
    #pragma warning (disable: 4996) // This function or variable may be unsafe
    #pragma warning (disable: 4244) // 'return': conversion from 'const int64_t' to 'float', possible loss of data
    #pragma warning (disable: 4715) // configuru::Config::operator[]': not all control paths return a value
#endif
#include <configuru.hpp>
#ifdef _MSC_VER
    #pragma warning (pop)
#endif

namespace steamvrbridge {

	//-- definitions -----
	/*
	Note that Config is an abstract class because it has 2 pure virtual functions.
	Child classes must add public member variables that store the config data,
	as well as implement writeToJSON and readFromJSON that use pt[key]= value and
	pt.get_or<type>(), respectively, to convert between member variables and the
	property tree. See tests/test_config.cpp for an example.
	*/
	class Config {
	public:
		Config(const std::string &fnamebase = std::string("Config"));
        void init();
		bool load();
		bool load(const std::string &path);
    
        const std::string getConfigName() const;
		const std::string getConfigPath() const;

		virtual bool ReadFromJSON(const configuru::Config &pt) = 0;  // Implemented by each device class' own Config

        virtual Config *Clone() = 0;
        virtual void OnConfigChanged(Config *newConfig);  // Implemented by each device class' own Config

    private:
		std::string ConfigFileBase;
	};
}

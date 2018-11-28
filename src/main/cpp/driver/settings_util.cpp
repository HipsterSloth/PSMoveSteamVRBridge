#include "settings_util.h"
#include "SharedConstants.h"

namespace steamvrbridge {

	const int ServerDriverConfig::CONFIG_VERSION = 1;

	ServerDriverConfig::ServerDriverConfig(const std::string &fnamebase)
		: Config(fnamebase)
		, is_valid(true)
		, version(CONFIG_VERSION)
		, filter_virtual_hmd_serial("") 
		, server_address(PSMOVESERVICE_DEFAULT_ADDRESS)
		, server_port(PSMOVESERVICE_DEFAULT_PORT) {
	};

	configuru::Config ServerDriverConfig::WriteToJSON() {
		configuru::Config pt{
			{"is_valid", is_valid},
			{"version", version},
			{"filter_virtual_hmd_serial", filter_virtual_hmd_serial},
			{"server_address", server_address},
			{"server_port", server_port},
		};

		return pt;
	}

	bool ServerDriverConfig::ReadFromJSON(const configuru::Config &pt) {
		if (pt.get_or<bool>("is_valid", false) == true &&
			pt.get_or<int>("version", -1) == CONFIG_VERSION) {
			filter_virtual_hmd_serial= pt.get_or<std::string>("filter_virtual_hmd_serial", "");
			server_address= pt.get_or<std::string>("server_address", server_address);
			server_port= pt.get_or<std::string>("server_port", server_port);
			return true;
		}

		return false;
	}
}
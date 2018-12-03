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
		, server_port(PSMOVESERVICE_DEFAULT_PORT)
		, auto_launch_psmove_service(true) 
		, use_installation_path(true)
		, has_calibrated_world_from_driver_pose(false)
		, world_from_driver_pose(*k_psm_pose_identity) {
	};

	configuru::Config ServerDriverConfig::WriteToJSON() {
		configuru::Config pt{
			{"is_valid", is_valid},
			{"version", version},
			{"filter_virtual_hmd_serial", filter_virtual_hmd_serial},
			{"server_address", server_address},
			{"server_port", server_port},
			{"auto_launch_psmove_service", auto_launch_psmove_service},
			{"use_installation_path", use_installation_path},
			{"has_calibrated_world_from_driver_pose", has_calibrated_world_from_driver_pose},
			{"world_from_driver_pose.orientation.w", world_from_driver_pose.Orientation.w},
			{"world_from_driver_pose.orientation.x", world_from_driver_pose.Orientation.x},
			{"world_from_driver_pose.orientation.y", world_from_driver_pose.Orientation.y},
			{"world_from_driver_pose.orientation.z", world_from_driver_pose.Orientation.z},
			{"world_from_driver_pose.position.x", world_from_driver_pose.Position.x},
			{"world_from_driver_pose.position.y", world_from_driver_pose.Position.y},
			{"world_from_driver_pose.position.z", world_from_driver_pose.Position.z},
		};

		return pt;
	}

	bool ServerDriverConfig::ReadFromJSON(const configuru::Config &pt) {
		if (pt.get_or<bool>("is_valid", false) == true &&
			pt.get_or<int>("version", -1) == CONFIG_VERSION) {
			filter_virtual_hmd_serial= pt.get_or<std::string>("filter_virtual_hmd_serial", "");
			server_address= pt.get_or<std::string>("server_address", server_address);
			server_port= pt.get_or<std::string>("server_port", server_port);
			auto_launch_psmove_service= pt.get_or<bool>("auto_launch_psmove_service", auto_launch_psmove_service);
			use_installation_path= pt.get_or<bool>("use_installation_path", use_installation_path);
			
			// By default, assume the psmove and openvr tracking spaces are the same
			has_calibrated_world_from_driver_pose= pt.get_or<bool>("has_calibrated_world_from_driver_pose", false);
			world_from_driver_pose.Orientation.w= pt.get_or<float>("world_from_driver_pose.orientation.w", 1.f);
			world_from_driver_pose.Orientation.x= pt.get_or<float>("world_from_driver_pose.orientation.x", 0.f);
			world_from_driver_pose.Orientation.y= pt.get_or<float>("world_from_driver_pose.orientation.y", 0.f);
			world_from_driver_pose.Orientation.z= pt.get_or<float>("world_from_driver_pose.orientation.z", 0.f);
			world_from_driver_pose.Position.x= pt.get_or<float>("world_from_driver_pose.position.x", 0.f);
			world_from_driver_pose.Position.y= pt.get_or<float>("world_from_driver_pose.position.y", 0.f);
			world_from_driver_pose.Position.z= pt.get_or<float>("world_from_driver_pose.position.z", 0.f);
			return true;
		}

		return false;
	}
}
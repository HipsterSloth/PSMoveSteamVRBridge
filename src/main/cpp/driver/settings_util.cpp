#include "settings_util.h"
#include "server_driver.h"
#include "SharedConstants.h"

namespace steamvrbridge {

	ServerDriverConfig::ServerDriverConfig(CServerDriver_PSMoveService *serverDriver, const std::string &fnamebase)
		: Config(fnamebase)
        , m_serverDriver(serverDriver)
		, server_address(PSMOVESERVICE_DEFAULT_ADDRESS)
		, server_port(PSMOVESERVICE_DEFAULT_PORT)
		, auto_launch_psmove_service(true) 
		, use_installation_path(true)
		, world_from_driver_pose(*k_psm_pose_identity) {
	};

    void ServerDriverConfig::OnConfigChanged(Config *newConfig) {
        ServerDriverConfig *newServerConfig = static_cast<ServerDriverConfig *>(newConfig);

        // These parameters only considered on (re)connection to PSMoveService
		this->server_address= newServerConfig->server_address;
		this->server_port= newServerConfig->server_port;
		this->auto_launch_psmove_service= newServerConfig->auto_launch_psmove_service;
		this->use_installation_path= newServerConfig->use_installation_path;

        // Update the HMD tracking space pose on all devices
        if (m_serverDriver != nullptr)
        {
            m_serverDriver->SetHMDTrackingSpace(newServerConfig->world_from_driver_pose);
        }
        else
        {
            this->world_from_driver_pose= newServerConfig->world_from_driver_pose;
        }
    }

	bool ServerDriverConfig::ReadFromJSON(const configuru::Config &pt) {
		server_address= pt.get_or<std::string>("server_address", server_address);
		server_port= pt.get_or<std::string>("server_port", server_port);
		auto_launch_psmove_service= pt.get_or<bool>("auto_launch_psmove_service", auto_launch_psmove_service);
		use_installation_path= pt.get_or<bool>("use_installation_path", use_installation_path);
			
		// By default, assume the psmove and openvr tracking spaces are the same
		world_from_driver_pose.Orientation.w= pt.get_or<float>("world_from_driver_pose.orientation.w", 1.f);
		world_from_driver_pose.Orientation.x= pt.get_or<float>("world_from_driver_pose.orientation.x", 0.f);
		world_from_driver_pose.Orientation.y= pt.get_or<float>("world_from_driver_pose.orientation.y", 0.f);
		world_from_driver_pose.Orientation.z= pt.get_or<float>("world_from_driver_pose.orientation.z", 0.f);
		world_from_driver_pose.Position.x= pt.get_or<float>("world_from_driver_pose.position.x", 0.f);
		world_from_driver_pose.Position.y= pt.get_or<float>("world_from_driver_pose.position.y", 0.f);
		world_from_driver_pose.Position.z= pt.get_or<float>("world_from_driver_pose.position.z", 0.f);
		return true;
	}
}
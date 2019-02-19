#pragma once
#include "openvr_driver.h"
#include "ClientGeometry_CAPI.h"
#include "config.h"

namespace steamvrbridge {

	class ServerDriverConfig : public Config
	{
	public:
		ServerDriverConfig(class CServerDriver_PSMoveService *serverDriver, const std::string &fnamebase = "PSMoveSteamVRBridgeConfig");

        Config *Clone() override { return new ServerDriverConfig(*this); }
        void OnConfigChanged(Config *newConfig) override;
		bool ReadFromJSON(const configuru::Config &pt) override;

		std::string server_address;
		std::string server_port;
		bool auto_launch_psmove_service;
		bool use_installation_path;

		// HMD Tracking Space
		PSMPosef world_from_driver_pose;

    private:
        CServerDriver_PSMoveService *m_serverDriver;
	};
}
#pragma once
#include "openvr_driver.h"
#include "ClientGeometry_CAPI.h"
#include "config.h"

namespace steamvrbridge {

	class ServerDriverConfig : public Config
	{
	public:
		static const int CONFIG_VERSION;

		ServerDriverConfig(const std::string &fnamebase = "PSMoveSteamVRBridgeConfig");

		virtual configuru::Config WriteToJSON();
		virtual bool ReadFromJSON(const configuru::Config &pt);

	    bool is_valid;
	    long version;

		std::string filter_virtual_hmd_serial;
		std::string server_address;
		std::string server_port;
		bool auto_launch_psmove_service;
		bool use_installation_path;

		// HMD Tracking Space
		bool has_calibrated_world_from_driver_pose;
		PSMPosef world_from_driver_pose;
	};
}
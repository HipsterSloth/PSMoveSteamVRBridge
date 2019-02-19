#pragma once
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>
#include "config_manager.h"
#include "config.h"
#include "trackable_device.h"
#include "tracker.h"
#include "logger.h"
#include "settings_util.h"

// Platform specific includes
#if defined( _WIN32 )
#include <windows.h>
#include <direct.h>
#define getcwd _getcwd // suppress "deprecation" warning
#else
#include <unistd.h>
#endif

namespace steamvrbridge {

	/* 
		IServerTrackedDeviceProvider implementation as per:
		https://github.com/ValveSoftware/openvr/wiki/IServerTrackedDeviceProvider_Overview 
	*/
	class CServerDriver_PSMoveService : public vr::IServerTrackedDeviceProvider
	{
	public:
		CServerDriver_PSMoveService();
		virtual ~CServerDriver_PSMoveService();

		static CServerDriver_PSMoveService *getInstance()
		{
			if (m_instance == nullptr)
				m_instance= new CServerDriver_PSMoveService();

			return m_instance;
		}

		// Inherited via IServerTrackedDeviceProvider
		virtual vr::EVRInitError Init(vr::IVRDriverContext *pDriverContext) override;
		virtual void Cleanup() override;
		virtual const char * const *GetInterfaceVersions() override;
		virtual void RunFrame() override;
		virtual bool ShouldBlockStandbyMode() override;
		virtual void EnterStandby() override;
		virtual void LeaveStandby() override;

		void LaunchPSMoveService();

		vr::ETrackedControllerRole AllocateControllerRole(PSMControllerHand psmControllerHand);
		void SetHMDTrackingSpace(const PSMPosef &origin_pose);
		inline PSMPosef GetWorldFromDriverPose() const { return m_config.world_from_driver_pose; }

	private:
		vr::ITrackedDeviceServerDriver * FindTrackedDeviceDriver(const char * pchId);
		void AllocateUniquePSMoveController(PSMControllerID ControllerID, PSMControllerHand psmControllerHand, const std::string &ControllerSerial);
		void AllocateUniqueVirtualController(PSMControllerID psmControllerID, PSMControllerHand psmControllerHand, const std::string &psmControllerSerial);
		void AllocateUniquePSNaviController(PSMControllerID psmControllerID, PSMControllerHand psmControllerHand, const std::string &psmControllerSerial, const std::string &psmParentControllerSerial);
		void AllocateUniqueDualShock4Controller(PSMControllerID ControllerID, PSMControllerHand psmControllerHand, const std::string &ControllerSerial);
		void AllocateUniquePSMoveTracker(const PSMClientTrackerInfo *trackerInfo);
		bool ReconnectToPSMoveService();

		// Event Handling
		void HandleClientPSMoveEvent(const PSMMessage *event);
		void HandleConnectedToPSMoveService();
		void HandleFailedToConnectToPSMoveService();
		void HandleDisconnectedFromPSMoveService();
		static void HandleServiceVersionResponse(const PSMResponseMessage *response, void *userdata);
		void HandleControllerListChanged();
		void HandleTrackerListChanged();

		// Response Handling
		void HandleClientPSMoveResponse(const PSMMessage *message);
		void HandleControllerListReponse(const PSMControllerList *controller_list, const PSMResponseHandle response_handle);
		void HandleTrackerListReponse(const PSMTrackerList *tracker_list);

        ConfigManager m_configManager;
		ServerDriverConfig m_config;

		bool m_bLaunchedPSMoveService;
		bool m_bInitialized;

		std::vector< TrackableDevice * > m_vecTrackedDevices;

		// Singleton instance of CServerDriver_PSMoveService
		static CServerDriver_PSMoveService *m_instance;
	};
}
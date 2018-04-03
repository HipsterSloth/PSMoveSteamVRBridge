#pragma once
#include "PSMoveClient_CAPI.h"
#include <openvr_driver.h>
#include "trackeddevice.h"
#include "tracker.h"
#include "logger.h"

// Platform specific includes
#if defined( _WIN32 )
#include <windows.h>
#include <direct.h>
#define getcwd _getcwd // suppress "deprecation" warning
#else
#include <unistd.h>
#endif

namespace steamvrbridge {

	class CServerDriver_PSMoveService : public vr::IServerTrackedDeviceProvider
	{
	public:
		CServerDriver_PSMoveService();
		virtual ~CServerDriver_PSMoveService();

		// Inherited via IServerTrackedDeviceProvider
		virtual vr::EVRInitError Init(vr::IVRDriverContext *pDriverContext) override;
		virtual void Cleanup() override;
		virtual const char * const *GetInterfaceVersions() override;
		virtual void RunFrame() override;
		virtual bool ShouldBlockStandbyMode() override;
		virtual void EnterStandby() override;
		virtual void LeaveStandby() override;

		void LaunchPSMoveMonitor();

		void SetHMDTrackingSpace(const PSMPosef &origin_pose);
		inline PSMPosef GetWorldFromDriverPose() const { return m_worldFromDriverPose; }

	private:
		vr::ITrackedDeviceServerDriver * FindTrackedDeviceDriver(const char * pchId);
		void AllocateUniquePSMoveController(PSMControllerID ControllerID, const std::string &ControllerSerial);
		void AllocateUniqueVirtualController(PSMControllerID psmControllerID, const std::string &psmControllerSerial);
		void AttachPSNaviToParentController(PSMControllerID ControllerID, const std::string &ControllerSerial, const std::string &ParentControllerSerial);
		void AllocateUniqueDualShock4Controller(PSMControllerID ControllerID, const std::string &ControllerSerial);
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

		void LaunchPSMoveMonitor_Internal(const char * pchDriverInstallDir);

		std::string m_strPSMoveHMDSerialNo;
		std::string m_strPSMoveServiceAddress;
		std::string m_strServerPort;

		bool m_bLaunchedPSMoveMonitor;
		bool m_bInitialized;

		std::vector< CPSMoveTrackedDeviceLatest * > m_vecTrackedDevices;

		// HMD Tracking Space
		PSMPosef m_worldFromDriverPose;
	};
}
#include "server_driver.h"
#include "ps_ds4_controller.h"
#include "ps_move_controller.h"
#include "ps_navi_controller.h"
#include "virtual_controller.h"
#include "utils.h"

#include <ctime>
#include <algorithm>
#include <assert.h>
#include <sstream>
#include <chrono>

namespace steamvrbridge {

	CServerDriver_PSMoveService * CServerDriver_PSMoveService::m_instance = nullptr;

	CServerDriver_PSMoveService::CServerDriver_PSMoveService()
		: m_bLaunchedPSMoveMonitor(false)
		, m_bLaunchedPSMoveService(false)
		, m_bInitialized(false) {
	}

	CServerDriver_PSMoveService::~CServerDriver_PSMoveService() {
		if (m_instance == this)
			m_instance = nullptr;

		Cleanup();
	}

	vr::EVRInitError CServerDriver_PSMoveService::Init(
		vr::IVRDriverContext *pDriverContext) {
		//initialise log counter for fps measurement
		VR_INIT_SERVER_DRIVER_CONTEXT(pDriverContext);

		vr::EVRInitError initError = vr::VRInitError_None;

		Logger::InitDriverLog(vr::VRDriverLog());
		Logger::Info("CServerDriver_PSMoveService::Init - called.\n");

		if (!m_bInitialized) {
			Logger::Info("CServerDriver_PSMoveService::Init - Initializing.\n");

			// Load the config file, if it exists
			m_config.load();

			// Save the config back out in case the config didn't exist or was upgraded
			m_config.save();

			// Launch PSMoveService automatically if it's not already running
			LaunchPSMoveService();

			// Note that reconnection is a non-blocking async request.
			// Returning true means we we're able to start trying to connect,
			// not that we are successfully connected yet.
			Logger::Info("CServerDriver_PSMoveService::Init - Using Default Server Address: %s.\n", m_config.server_address.c_str());
			Logger::Info("CServerDriver_PSMoveService::Init - Using Default Server Port: %s.\n", m_config.server_port.c_str());
			if (!ReconnectToPSMoveService()) {
				initError = vr::VRInitError_Driver_Failed;
			}

			m_bInitialized = true;
		} else {
			Logger::Info("CServerDriver_PSMoveService::Init - Already Initialized. Ignoring.\n");
		}

		return initError;
	}

	bool CServerDriver_PSMoveService::ReconnectToPSMoveService() {
		Logger::Info("CServerDriver_PSMoveService::ReconnectToPSMoveService - called.\n");

		if (PSM_GetIsInitialized()) {
			Logger::Info("CServerDriver_PSMoveService::ReconnectToPSMoveService - Existing PSMoveService connection active. Shutting down...\n");
			PSM_Shutdown();
			Logger::Info("CServerDriver_PSMoveService::ReconnectToPSMoveService - Existing PSMoveService connection stopped.\n");
		} else {
			Logger::Info("CServerDriver_PSMoveService::ReconnectToPSMoveService - Existing PSMoveService connection NOT active.\n");
		}

		Logger::Info("CServerDriver_PSMoveService::ReconnectToPSMoveService - Starting PSMoveService connection...\n");
		bool bSuccess = PSM_InitializeAsync(m_config.server_address.c_str(), m_config.server_port.c_str()) != PSMResult_Error;

		if (bSuccess) {
			Logger::Info("CServerDriver_PSMoveService::ReconnectToPSMoveService - Successfully requested connection\n");
		} else {
			Logger::Info("CServerDriver_PSMoveService::ReconnectToPSMoveService - Failed to request connection!\n");
		}

		return bSuccess;
	}

	void CServerDriver_PSMoveService::Cleanup() {
		if (m_bInitialized) {
			Logger::Info("CServerDriver_PSMoveService::Cleanup - Shutting down connection...\n");
			PSM_Shutdown();
			Logger::Info("CServerDriver_PSMoveService::Cleanup - Shutdown complete\n");

			m_bInitialized = false;
		}
	}

	const char * const *CServerDriver_PSMoveService::GetInterfaceVersions() {
		return vr::k_InterfaceVersions;
	}

	vr::ITrackedDeviceServerDriver * CServerDriver_PSMoveService::FindTrackedDeviceDriver(
		const char * pchId) {
		for (auto it = m_vecTrackedDevices.begin(); it != m_vecTrackedDevices.end(); ++it) {
			if (0 == strcmp((*it)->GetSteamVRIdentifier(), pchId)) {
				return *it;
			}
		}

		return nullptr;
	}

	void CServerDriver_PSMoveService::RunFrame() {

		// Update any controllers that are currently listening
		PSM_UpdateNoPollMessages();

		// Poll events queued up by the call to PSM_UpdateNoPollMessages()
		PSMMessage mesg;
		while (PSM_PollNextMessage(&mesg, sizeof(PSMMessage)) == PSMResult_Success) {
			switch (mesg.payload_type) {
				case PSMMessageType::_messagePayloadType_Response:
					HandleClientPSMoveResponse(&mesg);
					break;
				case PSMMessageType::_messagePayloadType_Event:
					HandleClientPSMoveEvent(&mesg);
					break;
			}
		}

		// Check for any OpenVR TrackedDeviceProvider events
		vr::VREvent_t event;
		while (vr::VRServerDriverHost()->PollNextEvent(&event, sizeof(event))) {
			switch (event.eventType) {
				case vr::VREvent_Input_HapticVibration:

					// haptic event details
					vr::VREvent_HapticVibration_t hapticData = event.data.hapticVibration;
					vr::TrackedDeviceIndex_t trackedDeviceIndex = event.trackedDeviceIndex;
					uint64_t handle = hapticData.containerHandle;

					//Logger::Debug("CServerDriver_PSMoveService::RunFrame: haptic event, trackedDeviceIndex=%d, durationSecs=%f, amplitude=%f, frequency=%f\n"
					//				, &trackedDeviceIndex, durationSecs, amplitude, frequency);

					// find the trackable device this vibration event is intended for by property container handle
					auto it= std::find_if(
						m_vecTrackedDevices.begin(), m_vecTrackedDevices.end(), 
						[handle](const TrackableDevice *pTrackedDevice)->bool {
							return 
								pTrackedDevice->GetTrackedDeviceClass() == vr::TrackedDeviceClass_Controller
								&& pTrackedDevice->getPropertyContainerHandle() == handle;
						});

					// If the appropriate device is found, pass on the haptic event
					if (it != m_vecTrackedDevices.end())
					{
						Controller *pController = static_cast<Controller *>(*it);

						pController->UpdateHaptics(hapticData);
					}
			}
		}

		// Update all active tracked devices
		for (auto it = m_vecTrackedDevices.begin(); it != m_vecTrackedDevices.end(); ++it) {
			TrackableDevice *pTrackedDevice = *it;

			switch (pTrackedDevice->GetTrackedDeviceClass()) {
				case vr::TrackedDeviceClass_Controller:
				{
					PSMoveController *pController = static_cast<PSMoveController *>(pTrackedDevice);

					pController->Update();
				} break;
				case vr::TrackedDeviceClass_TrackingReference:
				{
					PSMServiceTracker *pTracker = static_cast<PSMServiceTracker *>(pTrackedDevice);

					pTracker->Update();
				} break;
				default:
					assert(0 && "unreachable");
			}
		}
	}


	bool CServerDriver_PSMoveService::ShouldBlockStandbyMode() {
		return false;
	}

	void CServerDriver_PSMoveService::EnterStandby() {
	}

	void CServerDriver_PSMoveService::LeaveStandby() {
	}

	// -- Event Handling -----
	void CServerDriver_PSMoveService::HandleClientPSMoveEvent(
		const PSMMessage *message) {
		switch (message->event_data.event_type) {
			// Client Events
			case PSMEventMessageType::PSMEvent_connectedToService:
				HandleConnectedToPSMoveService();
				break;
			case PSMEventMessageType::PSMEvent_failedToConnectToService:
				HandleFailedToConnectToPSMoveService();
				break;
			case PSMEventMessageType::PSMEvent_disconnectedFromService:
				HandleDisconnectedFromPSMoveService();
				break;

				// Service Events
			case PSMEventMessageType::PSMEvent_opaqueServiceEvent:
				// We don't care about any opaque service events
				break;
			case PSMEventMessageType::PSMEvent_controllerListUpdated:
				HandleControllerListChanged();
				break;
			case PSMEventMessageType::PSMEvent_trackerListUpdated:
				HandleTrackerListChanged();
				break;
			case PSMEventMessageType::PSMEvent_hmdListUpdated:
				// don't care
				break;
			case PSMEventMessageType::PSMEvent_systemButtonPressed:
				// don't care
				break;
				//###HipsterSloth $TODO - Need a notification for when a tracker pose changes
		}
	}

	void CServerDriver_PSMoveService::HandleConnectedToPSMoveService() {
		Logger::Info("CServerDriver_PSMoveService::HandleConnectedToPSMoveService - Request controller and tracker lists\n");

		PSMRequestID request_id;
		PSM_GetServiceVersionStringAsync(&request_id);
		PSM_RegisterCallback(request_id, CServerDriver_PSMoveService::HandleServiceVersionResponse, this);
	}

	void CServerDriver_PSMoveService::HandleServiceVersionResponse(
		const PSMResponseMessage *response,
		void *userdata) {
		PSMResult ResultCode = response->result_code;
		PSMResponseHandle response_handle = response->opaque_response_handle;
		CServerDriver_PSMoveService *thisPtr = static_cast<CServerDriver_PSMoveService *>(userdata);

		switch (ResultCode) {
			case PSMResult::PSMResult_Success:
			{
				const std::string service_version = response->payload.service_version.version_string;
				const std::string local_version = PSM_GetClientVersionString();

				if (service_version == local_version) {
					Logger::Info("CServerDriver_PSMoveService::HandleServiceVersionResponse - Received expected protocol version %s\n", service_version.c_str());

					// Ask the service for a list of connected controllers
					// Response handled in HandleControllerListReponse()
					PSM_GetControllerListAsync(nullptr);

					// Ask the service for a list of connected trackers
					// Response handled in HandleTrackerListReponse()
					PSM_GetTrackerListAsync(nullptr);
				} else {
					Logger::Info("CServerDriver_PSMoveService::HandleServiceVersionResponse - Protocol mismatch! Expected %s, got %s. Please reinstall the PSMove Driver!\n",
								 local_version.c_str(), service_version.c_str());
					thisPtr->Cleanup();
				}
			} break;
			case PSMResult::PSMResult_Error:
			case PSMResult::PSMResult_Canceled:
			{
				Logger::Info("CServerDriver_PSMoveService::HandleServiceVersionResponse - Failed to get protocol version\n");
			} break;
		}
	}


	void CServerDriver_PSMoveService::HandleFailedToConnectToPSMoveService() {
		Logger::Info("CServerDriver_PSMoveService::HandleFailedToConnectToPSMoveService - Called\n");

		// Immediately attempt to reconnect to the service
		ReconnectToPSMoveService();
	}

	void CServerDriver_PSMoveService::HandleDisconnectedFromPSMoveService() {
		Logger::Info("CServerDriver_PSMoveService::HandleDisconnectedFromPSMoveService - Called\n");

		for (auto it = m_vecTrackedDevices.begin(); it != m_vecTrackedDevices.end(); ++it) {
			TrackableDevice *pDevice = *it;

			pDevice->Deactivate();
		}

		// Immediately attempt to reconnect to the service
		ReconnectToPSMoveService();
	}

	void CServerDriver_PSMoveService::HandleControllerListChanged() {
		Logger::Info("CServerDriver_PSMoveService::HandleControllerListChanged - Called\n");

		// Ask the service for a list of connected controllers
		// Response handled in HandleControllerListReponse()
		PSM_GetControllerListAsync(nullptr);
	}

	void CServerDriver_PSMoveService::HandleTrackerListChanged() {
		Logger::Info("CServerDriver_PSMoveService::HandleTrackerListChanged - Called\n");

		// Ask the service for a list of connected trackers
		// Response handled in HandleTrackerListReponse()
		PSM_GetTrackerListAsync(nullptr);
	}

	// -- Response Handling -----
	void CServerDriver_PSMoveService::HandleClientPSMoveResponse(
		const PSMMessage *message) {
		switch (message->response_data.payload_type) {
			case PSMResponsePayloadType::_responsePayloadType_Empty:
				Logger::Info("NotifyClientPSMoveResponse - request id %d returned result %s.\n",
							 message->response_data.request_id,
							 (message->response_data.result_code == PSMResult::PSMResult_Success) ? "ok" : "error");
				break;
			case PSMResponsePayloadType::_responsePayloadType_ControllerList:
				Logger::Info("NotifyClientPSMoveResponse - Controller Count = %d (request id %d).\n",
							 message->response_data.payload.controller_list.count, message->response_data.request_id);
				HandleControllerListReponse(&message->response_data.payload.controller_list, message->response_data.opaque_request_handle);
				break;
			case PSMResponsePayloadType::_responsePayloadType_TrackerList:
				Logger::Info("NotifyClientPSMoveResponse - Tracker Count = %d (request id %d).\n",
							 message->response_data.payload.tracker_list.count, message->response_data.request_id);
				HandleTrackerListReponse(&message->response_data.payload.tracker_list);
				break;
			default:
				Logger::Info("NotifyClientPSMoveResponse - Unhandled response (request id %d).\n", message->response_data.request_id);
		}
	}

	void CServerDriver_PSMoveService::HandleControllerListReponse(
		const PSMControllerList *controller_list,
		const PSMResponseHandle response_handle) {
		Logger::Info("CServerDriver_PSMoveService::HandleControllerListReponse - Received %d controllers\n", controller_list->count);

		bool bAnyNaviControllers = false;
		for (int list_index = 0; list_index < controller_list->count; ++list_index) {
			const PSMClientControllerInfo &controller_info= controller_list->controllers[list_index];
			PSMControllerID psmControllerId = controller_info.controller_id;
			PSMControllerType psmControllerType = controller_info.controller_type;
			PSMControllerHand psmControllerHand = controller_info.controller_hand;
			std::string psmControllerSerial(controller_info.controller_serial);
			std::replace(psmControllerSerial.begin(), psmControllerSerial.end(), ':', '_');

			switch (psmControllerType) {
				case PSMControllerType::PSMController_Move:
					Logger::Info("CServerDriver_PSMoveService::HandleControllerListReponse - Allocate PSMove(%d)\n", psmControllerId);
					AllocateUniquePSMoveController(psmControllerId, psmControllerHand, psmControllerSerial);
					break;
				case PSMControllerType::PSMController_Virtual:
					Logger::Info("CServerDriver_PSMoveService::HandleControllerListReponse - Allocate VirtualController(%d)\n", psmControllerId);
					AllocateUniqueVirtualController(psmControllerId, psmControllerHand, psmControllerSerial);
					break;
				case PSMControllerType::PSMController_Navi:
					// Take care of this is the second pass once all of the PSMove controllers have been setup
					bAnyNaviControllers = true;
					break;
				case PSMControllerType::PSMController_DualShock4:
					Logger::Info("CServerDriver_PSMoveService::HandleControllerListReponse - Allocate PSDualShock4(%d)\n", psmControllerId);
					AllocateUniqueDualShock4Controller(psmControllerId, psmControllerHand, psmControllerSerial);
					break;
				default:
					break;
			}
		}

		if (bAnyNaviControllers) {
			for (int list_index = 0; list_index < controller_list->count; ++list_index) {
				const PSMClientControllerInfo &controller_info= controller_list->controllers[list_index];
				PSMControllerType controller_type = controller_info.controller_type;

				if (controller_type == PSMControllerType::PSMController_Navi) {
					int psmControllerId = controller_info.controller_id;
					PSMControllerHand psmControllerHand = controller_info.controller_hand;
					std::string psmControllerSerial(controller_info.controller_serial);
					std::string psmParentControllerSerial(controller_info.parent_controller_serial);

					Logger::Info("CServerDriver_PSMoveService::HandleControllerListReponse - Allocate PSNavi(%d)\n", psmControllerId);
					AllocateUniquePSNaviController(psmControllerId, psmControllerHand, psmControllerSerial, psmParentControllerSerial);
				}
			}
		}
	}

	void CServerDriver_PSMoveService::HandleTrackerListReponse(
		const PSMTrackerList *tracker_list) {
		Logger::Info("CServerDriver_PSMoveService::HandleTrackerListReponse - Received %d trackers\n", tracker_list->count);

		for (int list_index = 0; list_index < tracker_list->count; ++list_index) {
			const PSMClientTrackerInfo *trackerInfo = &tracker_list->trackers[list_index];

			AllocateUniquePSMoveTracker(trackerInfo);
		}
	}

	void CServerDriver_PSMoveService::SetHMDTrackingSpace(
		const PSMPosef &origin_pose) {
		Logger::Info("Begin CServerDriver_PSMoveService::SetHMDTrackingSpace()\n");

		m_config.has_calibrated_world_from_driver_pose= true;
		m_config.world_from_driver_pose = origin_pose;
		m_config.save();

		// Tell all the devices that the relationship between the psmove and the OpenVR
		// tracking spaces changed
		for (auto it = m_vecTrackedDevices.begin(); it != m_vecTrackedDevices.end(); ++it) {
			TrackableDevice *pDevice = *it;

			pDevice->RefreshWorldFromDriverPose();
		}
	}

	vr::ETrackedControllerRole CServerDriver_PSMoveService::AllocateControllerRole(PSMControllerHand psmControllerHand)
	{
		vr::ETrackedControllerRole trackedControllerRole;
		switch (psmControllerHand)
		{
		case PSMControllerHand_Left:
			trackedControllerRole= vr::TrackedControllerRole_LeftHand;
			break;
		case PSMControllerHand_Right:
			trackedControllerRole= vr::TrackedControllerRole_RightHand;
			break;
		default:
			trackedControllerRole= vr::TrackedControllerRole_LeftHand;
		}

		// if we already have another controller then set this new controller's role to the right hand
		for (auto it = m_vecTrackedDevices.begin(); it != m_vecTrackedDevices.end(); ++it) {
			TrackableDevice *pDevice = *it;
			if (pDevice->GetTrackedDeviceRole() == vr::TrackedControllerRole_LeftHand)
				trackedControllerRole = vr::TrackedControllerRole_RightHand;
		}

		return trackedControllerRole;
	}

	void CServerDriver_PSMoveService::AllocateUniquePSMoveController(PSMControllerID psmControllerID, PSMControllerHand psmControllerHand, const std::string &psmControllerSerial) {
		char svrIdentifier[256];
		Utils::GenerateControllerSteamVRIdentifier(svrIdentifier, sizeof(svrIdentifier), psmControllerID);

		if (!FindTrackedDeviceDriver(svrIdentifier)) {
			std::string psmSerialNo = psmControllerSerial;
			std::transform(psmSerialNo.begin(), psmSerialNo.end(), psmSerialNo.begin(), ::toupper);

			if (0 != m_config.filter_virtual_hmd_serial.compare(psmSerialNo)) {
				Logger::Info("added new psmove controller id: %d, serial: %s\n", psmControllerID, psmSerialNo.c_str());

				vr::ETrackedControllerRole trackedControllerRole= AllocateControllerRole(psmControllerHand);
				PSMoveController *TrackedDevice =
					new PSMoveController(psmControllerID, trackedControllerRole, psmSerialNo.c_str());

				m_vecTrackedDevices.push_back(TrackedDevice);

				if (vr::VRServerDriverHost()) {
					vr::VRServerDriverHost()->TrackedDeviceAdded(TrackedDevice->GetSteamVRIdentifier(), vr::TrackedDeviceClass_Controller, TrackedDevice);
				}
			} else {
				Logger::Info("skipped new psmove controller as configured for HMD tracking, serial: %s\n", psmSerialNo.c_str());
			}
		}
	}

	void CServerDriver_PSMoveService::AllocateUniqueVirtualController(PSMControllerID psmControllerID, PSMControllerHand psmControllerHand, const std::string &psmControllerSerial) {
		char svrIdentifier[256];
		Utils::GenerateControllerSteamVRIdentifier(svrIdentifier, sizeof(svrIdentifier), psmControllerID);

		if (!FindTrackedDeviceDriver(svrIdentifier)) {
			std::string psmSerialNo = psmControllerSerial;
			std::transform(psmSerialNo.begin(), psmSerialNo.end(), psmSerialNo.begin(), ::toupper);

			if (0 != m_config.filter_virtual_hmd_serial.compare(psmSerialNo)) {
				Logger::Info("added new virtual controller id: %d, serial: %s\n", psmControllerID, psmSerialNo.c_str());

				vr::ETrackedControllerRole trackedControllerRole= AllocateControllerRole(psmControllerHand);
				VirtualController *TrackedDevice =
					new VirtualController(psmControllerID, trackedControllerRole, psmSerialNo.c_str());

				m_vecTrackedDevices.push_back(TrackedDevice);

				if (vr::VRServerDriverHost()) {
					vr::VRServerDriverHost()->TrackedDeviceAdded(TrackedDevice->GetSteamVRIdentifier(), vr::TrackedDeviceClass_Controller, TrackedDevice);
				}
			} else {
				Logger::Info("skipped new virtual controller as configured for HMD tracking, serial: %s\n", psmSerialNo.c_str());
			}
		}
	}

	void CServerDriver_PSMoveService::AllocateUniqueDualShock4Controller(PSMControllerID psmControllerID, PSMControllerHand psmControllerHand, const std::string &psmControllerSerial) {
		char svrIdentifier[256];
		Utils::GenerateControllerSteamVRIdentifier(svrIdentifier, sizeof(svrIdentifier), psmControllerID);

		if (!FindTrackedDeviceDriver(svrIdentifier)) {
			std::string psmSerialNo = psmControllerSerial;
			std::transform(psmSerialNo.begin(), psmSerialNo.end(), psmSerialNo.begin(), ::toupper);

			if (0 != m_config.filter_virtual_hmd_serial.compare(psmSerialNo)) {
				Logger::Info("added new dualshock4 controller id: %d, serial: %s\n", psmControllerID, psmSerialNo.c_str());

				vr::ETrackedControllerRole trackedControllerRole= AllocateControllerRole(psmControllerHand);
				PSDualshock4Controller *TrackedDevice =
					new PSDualshock4Controller(psmControllerID, trackedControllerRole, psmSerialNo.c_str());

				m_vecTrackedDevices.push_back(TrackedDevice);

				if (vr::VRServerDriverHost()) {
					vr::VRServerDriverHost()->TrackedDeviceAdded(TrackedDevice->GetSteamVRIdentifier(), vr::TrackedDeviceClass_Controller, TrackedDevice);
				}
			} else {
				Logger::Info("skipped new dualshock4 controller as configured for HMD tracking, serial: %s\n", psmSerialNo.c_str());
			}
		}
	}

	void CServerDriver_PSMoveService::AllocateUniquePSNaviController(
		PSMControllerID psmControllerID, 
		PSMControllerHand psmControllerHand, 
		const std::string &psmControllerSerial, 
		const std::string &psmParentControllerSerial) {

		// Try and find the parent controller by serial number
		Controller *parent_controller = nullptr;
		if (psmParentControllerSerial.length() > 0)
		{
			std::string naviSerialNo = psmControllerSerial;
			std::string parentSerialNo = psmParentControllerSerial;
			std::transform(naviSerialNo.begin(), naviSerialNo.end(), naviSerialNo.begin(), ::toupper);
			std::transform(parentSerialNo.begin(), parentSerialNo.end(), parentSerialNo.begin(), ::toupper);

			for (TrackableDevice *trackedDevice : m_vecTrackedDevices) {
				if (trackedDevice->GetTrackedDeviceClass() == vr::TrackedDeviceClass_Controller) {
					Controller *test_controller = static_cast<Controller *>(trackedDevice);
					const std::string testSerialNo = test_controller->GetPSMControllerSerialNo();

					if (testSerialNo == parentSerialNo) {
						parent_controller= test_controller;

						Logger::Info("Attached navi controller serial %s to controller serial %s\n", 
							naviSerialNo.c_str(), parentSerialNo.c_str());

						break;
					}
				}
			}

			if (parent_controller == nullptr) {
				Logger::Info("Failed to find parent controller serial %s for navi controller serial %s\n", parentSerialNo.c_str(), naviSerialNo.c_str());
			}
		}

		char svrIdentifier[256];
		Utils::GenerateControllerSteamVRIdentifier(svrIdentifier, sizeof(svrIdentifier), psmControllerID);

		if (!FindTrackedDeviceDriver(svrIdentifier)) {
			std::string psmSerialNo = psmControllerSerial;
			std::transform(psmSerialNo.begin(), psmSerialNo.end(), psmSerialNo.begin(), ::toupper);

			if (0 != m_config.filter_virtual_hmd_serial.compare(psmSerialNo)) {
				Logger::Info("added new psnavi controller id: %d, serial: %s\n", psmControllerID, psmSerialNo.c_str());

				vr::ETrackedControllerRole trackedControllerRole= AllocateControllerRole(psmControllerHand);
				PSNaviController *naviController =
					new PSNaviController(psmControllerID, trackedControllerRole, psmSerialNo.c_str());

				m_vecTrackedDevices.push_back(naviController);

				if (parent_controller != nullptr)
				{
					// Attached controllers aren't registered with SteamVR.
					// They route their button events to their parent controllers
					naviController->AttachToController(parent_controller);
				}
				else
				{
					// Not attached to another controller
					// So register just like any other controller
					if (vr::VRServerDriverHost()) {
						vr::VRServerDriverHost()->TrackedDeviceAdded(naviController->GetSteamVRIdentifier(), vr::TrackedDeviceClass_Controller, naviController);
					}
				}
			} else {
				Logger::Info("skipped new virtual controller as configured for HMD tracking, serial: %s\n", psmSerialNo.c_str());
			}
		}
	}

	void CServerDriver_PSMoveService::AllocateUniquePSMoveTracker(const PSMClientTrackerInfo *trackerInfo) {
		char svrIdentifier[256];
		Utils::GenerateTrackerSerialNumber(svrIdentifier, sizeof(svrIdentifier), trackerInfo->tracker_id);

		if (!FindTrackedDeviceDriver(svrIdentifier)) {
			Logger::Info("added new tracker device %s\n", svrIdentifier);
			PSMServiceTracker *TrackerDevice = new PSMServiceTracker(trackerInfo);

			m_vecTrackedDevices.push_back(TrackerDevice);

			if (vr::VRServerDriverHost()) {
				vr::VRServerDriverHost()->TrackedDeviceAdded(TrackerDevice->GetSteamVRIdentifier(), vr::TrackedDeviceClass_TrackingReference, TrackerDevice);
			}
		}
	}

	void CServerDriver_PSMoveService::LaunchPSMoveService() {
		if (m_bLaunchedPSMoveService) {
			return;
		}

		// Only attempt an auto-launch once
		m_bLaunchedPSMoveService = true;

		if (m_config.auto_launch_psmove_service) {
			#if defined( _WIN32 ) || defined( _WIN64 )
			std::string psmServiceProcessName("PSMoveService.exe");
			#else 
			std::string psmServiceProcessName("PSMoveService");
			#endif

			if (!Utils::IsProcessRunning(psmServiceProcessName)) {
				std::string psmInstallDir = Utils::Path_GetPSMoveServiceInstallPath(&m_config);

				if (psmInstallDir.length() > 0) {
					// We'll spin on connection attempts independent of PSMoveService start-up state
					std::vector<std::string> args;
					Utils::LaunchProcess(psmInstallDir, psmServiceProcessName, args);
				} else {
					Logger::Error("CServerDriver_PSMoveService::LaunchPSMoveService() - Failed to fetch PSMoveServices paths\n");
				}
			}
			else {
				Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveService() - Skipping auto-launch of PSMoveService: Alread running.\n");
			}
		} else {
			Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveService() - Skipping auto-launch of PSMoveService: auto_launch_psmove_service set to 'false'.\n");
		}
	}

	/** Launch monitor_psmove if needed (requested by devices as they activate) */
	void CServerDriver_PSMoveService::LaunchPSMoveMonitor() {
		if (m_bLaunchedPSMoveMonitor) {
			return;
		}

		Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor() - Attempting to launch monitor_psmove\n");
		m_bLaunchedPSMoveMonitor = true;

		std::string driverBinDir = Utils::Path_GetPSMoveSteamVRBridgeDriverBinPath(&m_config);
		std::string driverResourcesDir = Utils::Path_GetPSMoveSteamVRBridgeDriverResourcesPath(&m_config);
		if (driverBinDir.length() > 0 && driverResourcesDir.length() > 0) {
			Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor() - driver bin directory: %s\n", driverBinDir.c_str());
			Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor() - driver resources directory: %s\n", driverResourcesDir.c_str());

			#if defined( _WIN32 ) || defined( _WIN64 )
			std::string process_name("monitor_psmove.exe");
			#else 
			std::string process_name("monitor_psmove");
			#endif

			std::vector<std::string> args;
			args.push_back(driverResourcesDir);

			// The monitor_psmove is a companion program which can display overlay prompts for us
			// and tell us the pose of the HMD at the moment we want to calibrate.
			Utils::LaunchProcess(driverBinDir, process_name, args);

		} else {
			Logger::Error("CServerDriver_PSMoveService::LaunchPSMoveMonitor() - Failed to fetch PSMoveSteamVRBridge paths\n");
		}
	}
}
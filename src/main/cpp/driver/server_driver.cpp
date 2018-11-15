#include "server_driver.h"
#include "ps_move_controller.h"
#include "virtual_controller.h"
#include "utils.h"

#include <ctime>
#include <algorithm>
#include <assert.h>
#include <sstream>
#include <chrono>

namespace steamvrbridge {

	CServerDriver_PSMoveService::CServerDriver_PSMoveService()
		: m_bLaunchedPSMoveMonitor(false)
		, m_bInitialized(false) {
		m_strPSMoveServiceAddress = PSMOVESERVICE_DEFAULT_ADDRESS;
		m_strServerPort = PSMOVESERVICE_DEFAULT_PORT;
	}

	CServerDriver_PSMoveService::~CServerDriver_PSMoveService() {
		// 10/10/2015 benj:  vrserver is exiting without calling Cleanup() to balance Init()
		// causing std::thread to call std::terminate
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
			vr::IVRSettings *pSettings = vr::VRSettings();
			if (pSettings != nullptr) {
				char buf[256];
				vr::EVRSettingsError fetchError;

				pSettings->GetString("psmove_settings", "psmove_filter_hmd_serial", buf, sizeof(buf), &fetchError);
				if (fetchError == vr::VRSettingsError_None) {
					m_strPSMoveHMDSerialNo = buf;
					std::transform(m_strPSMoveHMDSerialNo.begin(), m_strPSMoveHMDSerialNo.end(), m_strPSMoveHMDSerialNo.begin(), ::toupper);
				}

				pSettings->GetString("psmoveservice", "server_address", buf, sizeof(buf), &fetchError);
				if (fetchError == vr::VRSettingsError_None) {
					m_strPSMoveServiceAddress = buf;
					Logger::Info("CServerDriver_PSMoveService::Init - Overridden Server Address: %s.\n", m_strPSMoveServiceAddress.c_str());
				} else {
					Logger::Info("CServerDriver_PSMoveService::Init - Using Default Server Address: %s.\n", m_strPSMoveServiceAddress.c_str());
				}

				pSettings->GetString("psmoveservice", "server_port", buf, sizeof(buf), &fetchError);
				if (fetchError == vr::VRSettingsError_None) {
					m_strServerPort = buf;
					Logger::Info("CServerDriver_PSMoveService::Init - Overridden Server Port: %s.\n", m_strServerPort.c_str());
				} else {
					Logger::Info("CServerDriver_PSMoveService::Init - Using Default Server Port: %s.\n", m_strServerPort.c_str());
				}
			} else {
				Logger::Info("CServerDriver_PSMoveService::Init - NULL settings!.\n");
			}

			Logger::Info("CServerDriver_PSMoveService::Init - Initializing.\n");

			// By default, assume the psmove and openvr tracking spaces are the same
			m_worldFromDriverPose = *k_psm_pose_identity;

			// Note that reconnection is a non-blocking async request.
			// Returning true means we we're able to start trying to connect,
			// not that we are successfully connected yet.
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
		bool bSuccess = PSM_InitializeAsync(m_strPSMoveServiceAddress.c_str(), m_strServerPort.c_str()) != PSMResult_Error;

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
				case PSMMessage::_messagePayloadType_Response:
					HandleClientPSMoveResponse(&mesg);
					break;
				case PSMMessage::_messagePayloadType_Event:
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

						pController->SetPendingHapticVibration(hapticData);
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
			case PSMEventMessage::PSMEvent_connectedToService:
				HandleConnectedToPSMoveService();
				break;
			case PSMEventMessage::PSMEvent_failedToConnectToService:
				HandleFailedToConnectToPSMoveService();
				break;
			case PSMEventMessage::PSMEvent_disconnectedFromService:
				HandleDisconnectedFromPSMoveService();
				break;

				// Service Events
			case PSMEventMessage::PSMEvent_opaqueServiceEvent:
				// We don't care about any opaque service events
				break;
			case PSMEventMessage::PSMEvent_controllerListUpdated:
				HandleControllerListChanged();
				break;
			case PSMEventMessage::PSMEvent_trackerListUpdated:
				HandleTrackerListChanged();
				break;
			case PSMEventMessage::PSMEvent_hmdListUpdated:
				// don't care
				break;
			case PSMEventMessage::PSMEvent_systemButtonPressed:
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
			case PSMResponseMessage::_responsePayloadType_Empty:
				Logger::Info("NotifyClientPSMoveResponse - request id %d returned result %s.\n",
							 message->response_data.request_id,
							 (message->response_data.result_code == PSMResult::PSMResult_Success) ? "ok" : "error");
				break;
			case PSMResponseMessage::_responsePayloadType_ControllerList:
				Logger::Info("NotifyClientPSMoveResponse - Controller Count = %d (request id %d).\n",
							 message->response_data.payload.controller_list.count, message->response_data.request_id);
				HandleControllerListReponse(&message->response_data.payload.controller_list, message->response_data.opaque_request_handle);
				break;
			case PSMResponseMessage::_responsePayloadType_TrackerList:
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
			PSMControllerID psmControllerId = controller_list->controller_id[list_index];
			PSMControllerType psmControllerType = controller_list->controller_type[list_index];
			PSMControllerHand psmControllerHand = controller_list->controller_hand[list_index];
			std::string psmControllerSerial(controller_list->controller_serial[list_index]);

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
				int controller_id = controller_list->controller_id[list_index];
				PSMControllerType controller_type = controller_list->controller_type[list_index];
				std::string ControllerSerial(controller_list->controller_serial[list_index]);
				std::string ParentControllerSerial(controller_list->parent_controller_serial[list_index]);

				if (controller_type == PSMControllerType::PSMController_Navi) {
					Logger::Info("CServerDriver_PSMoveService::HandleControllerListReponse - Attach PSNavi(%d)\n", controller_id);
					AttachPSNaviToParentController(controller_id, ControllerSerial, ParentControllerSerial);
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

		m_worldFromDriverPose = origin_pose;

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

			if (0 != m_strPSMoveHMDSerialNo.compare(psmSerialNo)) {
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

			if (0 != m_strPSMoveHMDSerialNo.compare(psmSerialNo)) {
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
		/* TODO use ds4controller.cpp
		char svrIdentifier[256];
		Utils::GenerateControllerSteamVRIdentifier(svrIdentifier, sizeof(svrIdentifier), psmControllerID);

		if (!FindTrackedDeviceDriver(svrIdentifier))
		{
			std::string psmSerialNo = psmControllerSerial;
			std::transform(psmSerialNo.begin(), psmSerialNo.end(), psmSerialNo.begin(), ::toupper);

			Logger::Info("added new dualshock4 controller id: %d, serial: %s\n", psmControllerID, psmSerialNo.c_str());

			PSMoveController *TrackedDevice =
				new PSMoveController(psmControllerID, PSMControllerType::PSMController_DualShock4, psmSerialNo.c_str());
			m_vecTrackedDevices.push_back(TrackedDevice);

			if (vr::VRServerDriverHost())
			{
				vr::VRServerDriverHost()->TrackedDeviceAdded(TrackedDevice->GetSteamVRIdentifier(), vr::TrackedDeviceClass_Controller, TrackedDevice);
			}
		}*/
	}

	void CServerDriver_PSMoveService::AttachPSNaviToParentController(PSMControllerID NaviControllerID, const std::string &NaviControllerSerial, const std::string &ParentControllerSerial) {
		bool bFoundParent = false;

		std::string naviSerialNo = NaviControllerSerial;
		std::string parentSerialNo = ParentControllerSerial;
		std::transform(naviSerialNo.begin(), naviSerialNo.end(), naviSerialNo.begin(), ::toupper);
		std::transform(parentSerialNo.begin(), parentSerialNo.end(), parentSerialNo.begin(), ::toupper);

		for (TrackableDevice *trackedDevice : m_vecTrackedDevices) {
			if (trackedDevice->GetTrackedDeviceClass() == vr::TrackedDeviceClass_Controller) {
				PSMoveController *test_controller = static_cast<PSMoveController *>(trackedDevice);
				const std::string testSerialNo = test_controller->GetPSMControllerSerialNo();

				if (testSerialNo == parentSerialNo) {
					bFoundParent = true;

					if (test_controller->GetPSMControllerType() == PSMController_Move ||
						test_controller->GetPSMControllerType() == PSMController_Virtual) {
						/*if (test_controller->AttachChildPSMController(NaviControllerID, PSMController_Navi, naviSerialNo))
						{
							Logger::Info("Attached navi controller serial %s to controller serial %s\n", naviSerialNo.c_str(), parentSerialNo.c_str());
						}
						else
						{
							Logger::Info("Failed to attach navi controller serial %s to controller serial %s\n", naviSerialNo.c_str(), parentSerialNo.c_str());
						}*/
					} else {
						Logger::Info("Failed to attach navi controller serial %s to non-psmove controller serial %s\n", naviSerialNo.c_str(), parentSerialNo.c_str());
					}

					break;
				}
			}
		}

		if (!bFoundParent) {
			Logger::Info("Failed to find parent controller serial %s for navi controller serial %s\n", parentSerialNo.c_str(), naviSerialNo.c_str());
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


	// The monitor_psmove is a companion program which can display overlay prompts for us
	// and tell us the pose of the HMD at the moment we want to calibrate.
	void CServerDriver_PSMoveService::LaunchPSMoveMonitor_Internal(const char * pchDriverInstallDir) {
		Logger::Info("Entered CServerDriver_PSMoveService::LaunchPSMoveMonitor_Internal(%s)\n", pchDriverInstallDir);

		m_bLaunchedPSMoveMonitor = true;

		std::ostringstream path_and_executable_string_builder;

		path_and_executable_string_builder << pchDriverInstallDir;
		#if defined( _WIN64 )
		path_and_executable_string_builder << "\\bin\\win64";
		#elif defined( _WIN32 )
		path_and_executable_string_builder << "\\bin\\win32";
		#elif defined(__APPLE__) 
		path_and_executable_string_builder << "/bin/osx";
		#else 
		#error Do not know how to launch psmove_monitor
		#endif


		#if defined( _WIN32 ) || defined( _WIN64 )
		path_and_executable_string_builder << "\\monitor_psmove.exe";
		const std::string monitor_path_and_exe = path_and_executable_string_builder.str();

		std::ostringstream args_string_builder;
		args_string_builder << "monitor_psmove.exe \"" << pchDriverInstallDir << "\\resources\"";
		const std::string monitor_args = args_string_builder.str();

		char monitor_args_cstr[1024];
		strncpy_s(monitor_args_cstr, monitor_args.c_str(), sizeof(monitor_args_cstr) - 1);
		monitor_args_cstr[sizeof(monitor_args_cstr) - 1] = '\0';

		Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor_Internal() monitor_psmove windows full path: %s\n", monitor_path_and_exe.c_str());
		Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor_Internal() monitor_psmove windows args: %s\n", monitor_args_cstr);

		STARTUPINFOA sInfoProcess = { 0 };
		sInfoProcess.cb = sizeof(STARTUPINFOW);
		PROCESS_INFORMATION pInfoStartedProcess;
		BOOL bSuccess = CreateProcessA(monitor_path_and_exe.c_str(), monitor_args_cstr, NULL, NULL, FALSE, 0, NULL, NULL, &sInfoProcess, &pInfoStartedProcess);
		DWORD ErrorCode = (bSuccess == TRUE) ? 0 : GetLastError();

		Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor_Internal() Start monitor_psmove CreateProcessA() result: %d.\n", ErrorCode);

		#elif defined(__APPLE__) 
		pid_t processId;
		if ((processId = fork()) == 0) {
			path_and_executable_string_builder << "\\monitor_psmove";

			const std::string monitor_exe_path = path_and_executable_string_builder.str();
			const char * argv[] = { monitor_exe_path.c_str(), pchDriverInstallDir, NULL };

			if (execv(app, argv) < 0) {
				Logger::DriverLog("Failed to exec child process\n");
			}
		} else if (processId < 0) {
			Logger::DriverLog("Failed to fork child process\n");
			perror("fork error");
		}
		#else 
		#error Do not know how to launch psmove config tool
		#endif
	}

	/** Launch monitor_psmove if needed (requested by devices as they activate) */
	void CServerDriver_PSMoveService::LaunchPSMoveMonitor() {
		if (m_bLaunchedPSMoveMonitor) {
			return;
		}

		Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor() - Called\n");

		//###HipsterSloth $TODO - Ideally we would get the install path as a property, but this property fetch doesn't seem to work...
		//vr::ETrackedPropertyError errorCode;
		//std::string driverInstallDir= vr::VRProperties()->GetStringProperty(requestingDevicePropertyHandle, vr::Prop_InstallPath_String, &errorCode);

		//...so for now, just assume that we're running out of the steamvr folder
		std::string driver_dll_path = Utils::Path_StripFilename(Utils::Path_GetThisModulePath(), 0);
		if (driver_dll_path.length() > 0) {
			Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor() - driver dll directory: %s\n", driver_dll_path);

			std::ostringstream driverInstallDirBuilder;
			driverInstallDirBuilder << driver_dll_path;
			#if defined( _WIN64 ) || defined( _WIN32 )
			driverInstallDirBuilder << "\\..\\..";
			#else
			driverInstallDirBuilder << "/../..";
			#endif
			const std::string driverInstallDir = driverInstallDirBuilder.str();

			LaunchPSMoveMonitor_Internal(driverInstallDir.c_str());
		} else {
			Logger::Info("CServerDriver_PSMoveService::LaunchPSMoveMonitor() - Failed to fetch current directory\n");
		}
	}
}
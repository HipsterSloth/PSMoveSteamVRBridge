#define _USE_MATH_DEFINES

#include "constants.h"
#include "server_driver.h"
#include "utils.h"
#include "settings_util.h"
#include "driver.h"
#include "facing_handsolver.h"
#include "virtual_controller.h"
#include "trackable_device.h"
#include <assert.h>

#if _MSC_VER
#define strcasecmp(a, b) stricmp(a,b)
#pragma warning (disable: 4996) // 'This function or variable may be unsafe': snprintf
#define snprintf _snprintf
#endif

namespace steamvrbridge {

	// -- VirtualControllerConfig -----
	VirtualControllerConfig::VirtualControllerConfig(VirtualController *ownerController, const std::string &fnamebase)
		: ControllerConfig(ownerController, fnamebase)
		, extend_Y_meters(0.f)
		, extend_Z_meters(0.f)
		, z_rotate_90_degrees(false)
		, delay_after_touchpad_press(false)
		, meters_per_touchpad_axis_units()
		, steamvr_trigger_axis_index(1)
		, virtual_touchpad_XAxis_index(-1)
		, virtual_touchpad_YAxis_index(-1)
		, thumbstick_deadzone(k_defaultThumbstickDeadZoneRadius)
		, thumbstick_touch_as_press(true)
		, linear_velocity_multiplier(1.f)
		, linear_velocity_exponent(0.f)
		, system_button_id(k_PSMButtonID_Virtual_4) // "Start" button on a xbox 360 controller
	{
	}

    void VirtualControllerConfig::OnConfigChanged(Config *newConfig)
    {
        VirtualControllerConfig *newPSMoveConfig = static_cast<VirtualControllerConfig *>(newConfig);

		// Settings that can simply be copied and require no update callback
		this->extend_Y_meters= newPSMoveConfig->extend_Y_meters;
		this->extend_Z_meters= newPSMoveConfig->extend_Z_meters;
		this->z_rotate_90_degrees= newPSMoveConfig->z_rotate_90_degrees;
		this->linear_velocity_multiplier= newPSMoveConfig->linear_velocity_multiplier;
		this->linear_velocity_exponent= newPSMoveConfig->linear_velocity_exponent;
		this->delay_after_touchpad_press= newPSMoveConfig->delay_after_touchpad_press;
		this->meters_per_touchpad_axis_units= newPSMoveConfig->meters_per_touchpad_axis_units;
		this->steamvr_trigger_axis_index= newPSMoveConfig->steamvr_trigger_axis_index;
		this->virtual_touchpad_XAxis_index = newPSMoveConfig->virtual_touchpad_XAxis_index;
		this->virtual_touchpad_YAxis_index = newPSMoveConfig->virtual_touchpad_YAxis_index;
		this->thumbstick_deadzone = newPSMoveConfig->thumbstick_deadzone;
		this->thumbstick_touch_as_press = newPSMoveConfig->thumbstick_touch_as_press;
		this->system_button_id = newPSMoveConfig->system_button_id;

        ControllerConfig::OnConfigChanged(newConfig);
    }

	bool VirtualControllerConfig::ReadFromJSON(const configuru::Config &pt) {

		if (!ControllerConfig::ReadFromJSON(pt))
			return false;

		// Touch pad settings
		delay_after_touchpad_press = pt.get_or<bool>("delay_after_touchpad_press", delay_after_touchpad_press);
		meters_per_touchpad_axis_units = pt.get_or<float>("cm_per_touchpad_units", 7.5f) / 100.f;

		// Throwing power settings
		linear_velocity_multiplier = pt.get_or<float>("linear_velocity_multiplier",  linear_velocity_multiplier);
		linear_velocity_exponent = pt.get_or<float>("linear_velocity_exponent",  linear_velocity_exponent);

		// General Settings
		extend_Y_meters = pt.get_or<float>("extend_y_cm",  0.f) / 100.f;
		extend_Z_meters = pt.get_or<float>("extend_x_cm",  0.f) / 100.f;
		z_rotate_90_degrees = pt.get_or<bool>("rotate_z_90", z_rotate_90_degrees);
		thumbstick_deadzone= pt.get_or<float>("thumbstick_deadzone",  thumbstick_deadzone);
		thumbstick_touch_as_press= pt.get_or<bool>("thumbstick_touch_as_press", thumbstick_touch_as_press);

		// Axis mapping
		virtual_touchpad_XAxis_index = pt.get_or<int>("touchpad_x_axis_index", -1);
		virtual_touchpad_YAxis_index = pt.get_or<int>("touchpad_y_axis_index", -1);

		// Controller button mappings
		for (int button_index = 0; button_index < PSM_MAX_VIRTUAL_CONTROLLER_BUTTONS; ++button_index)
		{
			ReadEmulatedTouchpadAction(pt, (ePSMButtonID)(k_PSMButtonID_Virtual_0+button_index));
		}

		// System button mapping
		{
			std::string systemButtonString= pt.get_or<std::string>("system_button", "");

			if (systemButtonString.length() > 0)
			{
				int button_index = StringUtils::FindIndexInTable(k_PSMButtonNames, k_PSMButtonID_Count, systemButtonString.c_str());
				if (button_index != -1)
				{
					system_button_id = static_cast<ePSMButtonID>(button_index);
				}
				else
				{
					Logger::Info("Invalid virtual controller system button: %s\n", systemButtonString.c_str());
				}
			}
		}

		return true;
	}

	// -- VirtualController -----
	VirtualController::VirtualController(
		PSMControllerID psmControllerId,
		PSMControllerHand psmControllerHand,
		const char *psmSerialNo)
		: Controller(psmControllerHand)
		, m_nPSMControllerId(psmControllerId)
		, m_PSMServiceController(nullptr)
		, m_nPoseSequenceNumber(0)
		, m_resetPoseButtonPressTime()
		, m_bResetPoseRequestSent(false)
		, m_resetAlignButtonPressTime()
		, m_bResetAlignRequestSent(false)
		, m_bTouchpadWasActive(false)
		, m_touchpadDirectionsUsed(false)
		, m_posMetersAtTouchpadPressTime(*k_psm_float_vector3_zero)
		, m_driverSpaceRotationAtTouchpadPressTime(*k_psm_quaternion_identity)
		, m_orientationSolver(new CFacingHandOrientationSolver) {
		char svrIdentifier[256];
		Utils::GenerateControllerSteamVRIdentifier(svrIdentifier, sizeof(svrIdentifier), psmControllerId);
		m_strSteamVRSerialNo = svrIdentifier;

		m_lastTouchpadPressTime = std::chrono::high_resolution_clock::now();

		if (psmSerialNo != NULL) {
			m_strPSMControllerSerialNo = psmSerialNo;
		}

		// Tell PSM Client API that we are listening to this controller id
		PSM_AllocateControllerListener(psmControllerId);
		m_PSMServiceController = PSM_GetController(psmControllerId);

		m_trackingStatus = vr::TrackingResult_Uninitialized;

	}

	VirtualController::~VirtualController() {
		PSM_FreeControllerListener(m_PSMServiceController->ControllerID);
		m_PSMServiceController = nullptr;

		if (m_orientationSolver != nullptr) {
			delete m_orientationSolver;
			m_orientationSolver = nullptr;
		}
	}

    void VirtualController::OnControllerModelChanged()
    {
        vr::CVRPropertyHelpers *properties = vr::VRProperties();

		// The {psmove} syntax lets us refer to rendermodels that are installed
		// in the driver's own resources/rendermodels directory.  The driver can
		// still refer to SteamVR models like "generic_hmd".
		if (getConfig()->override_model.length() > 0) {
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, getConfig()->override_model.c_str());
		} else {
			properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "generic_controller");
		}
    }

	vr::EVRInitError VirtualController::Activate(vr::TrackedDeviceIndex_t unObjectId) {
		vr::EVRInitError result = Controller::Activate(unObjectId);

		if (result == vr::VRInitError_None) {
			Logger::Info("VirtualController::Activate - Controller %d Activated\n", unObjectId);

			m_trackingStatus = vr::TrackingResult_Running_OK;

			PSMRequestID requestId;
			if (PSM_StartControllerDataStreamAsync(
				m_PSMServiceController->ControllerID,
				PSMStreamFlags_includePositionData | PSMStreamFlags_includePhysicsData,
				&requestId) != PSMResult_Error) {
				PSM_RegisterCallback(requestId, VirtualController::start_controller_response_callback, this);
			}

			// Setup controller properties
			{
				vr::CVRPropertyHelpers *properties = vr::VRProperties();

				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceOff_String, "{psmove}gamepad_status_off.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceSearching_String, "{psmove}gamepad_status_ready.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceSearchingAlert_String, "{psmove}gamepad_status_ready_alert.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceReady_String, "{psmove}gamepad_status_ready.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceReadyAlert_String, "{psmove}gamepad_status_ready_alert.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceNotReady_String, "{psmove}gamepad_status_error.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceStandby_String, "{psmove}gamepad_status_ready.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceAlertLow_String, "{psmove}gamepad_status_ready_low.png");

				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_WillDriftInYaw_Bool, false);
				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceIsWireless_Bool, true);
				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceProvidesBatteryStatus_Bool, false);

				properties->SetInt32Property(m_ulPropertyContainer, vr::Prop_DeviceClass_Int32, vr::TrackedDeviceClass_Controller);

				// Set device properties
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_ManufacturerName_String, "Unknown");

				// Fake Vive for motion controllers
                properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_ControllerType_String, "psmoveservice_virtual");
				properties->SetUint64Property(m_ulPropertyContainer, vr::Prop_HardwareRevision_Uint64, 1313);
				properties->SetUint64Property(m_ulPropertyContainer, vr::Prop_FirmwareVersion_Uint64, 1315);
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_ModelNumber_String, "Virtual Controller");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_SerialNumber_String, m_strPSMControllerSerialNo.c_str());

                OnControllerModelChanged();
			}
		}

		return result;
	}

	void VirtualController::start_controller_response_callback(
		const PSMResponseMessage *response, void *userdata) {
		VirtualController *controller = reinterpret_cast<VirtualController *>(userdata);

		if (response->result_code == PSMResult::PSMResult_Success) {
			const PSMVirtualController *VirtualController = &controller->m_PSMServiceController->ControllerState.VirtualController;

			Logger::Info("VirtualController::start_controller_response_callback - Controller stream started\n");

			// Create a system button (bound to one of the virtual buttons)
			controller->CreateButtonComponent(k_PSMButtonID_System);

			// Create buttons components
			for (int button_id = k_PSMButtonID_Virtual_0; button_id <= k_PSMButtonID_Virtual_31; ++button_id)
			{
				controller->CreateButtonComponent((ePSMButtonID)button_id);
			}

			// Create axis components
			for (int axis_id = k_PSMAxisID_Virtual_0; axis_id <= k_PSMAxisID_Virtual_31; ++axis_id)
			{
				controller->CreateAxisComponent((ePSMAxisID)axis_id);
			}

			// Create components for emulated trackpad
			controller->CreateButtonComponent(k_PSMButtonID_EmulatedTrackpadTouched);
			controller->CreateButtonComponent(k_PSMButtonID_EmulatedTrackpadPressed);
			controller->CreateAxisComponent(k_PSMAxisID_EmulatedTrackpad_X);
			controller->CreateAxisComponent(k_PSMAxisID_EmulatedTrackpad_Y);
		}
	}

	void VirtualController::Deactivate() {
		Logger::Info("VirtualController::Deactivate - Controller stream stopped\n");
		PSM_StopControllerDataStreamAsync(m_PSMServiceController->ControllerID, nullptr);

		Controller::Deactivate();
	}

	void VirtualController::UpdateControllerState() {
		static const uint64_t s_kTouchpadButtonMask = vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);

		assert(m_PSMServiceController != nullptr);
		assert(m_PSMServiceController->IsConnected);

		const PSMVirtualController &clientView = m_PSMServiceController->ControllerState.VirtualController;

		// Provide a system button update from one of the virtual buttons
		if (getConfig()->system_button_id >= k_PSMButtonID_Virtual_0 && getConfig()->system_button_id <= k_PSMButtonID_Virtual_31)
		{
			int buttonIndex= getConfig()->system_button_id - k_PSMButtonID_Virtual_0;
			const PSMButtonState button_state= 
				m_PSMServiceController->ControllerState.VirtualController.buttonStates[buttonIndex];

			UpdateButton(k_PSMButtonID_System, button_state);
		}

		int buttonCount = m_PSMServiceController->ControllerState.VirtualController.numButtons;
		for (int buttonIndex = 0; buttonIndex < buttonCount; ++buttonIndex)
		{
			const PSMButtonState button_state= 
				m_PSMServiceController->ControllerState.VirtualController.buttonStates[buttonIndex];

			UpdateButton((ePSMButtonID)(k_PSMButtonID_Virtual_0+buttonIndex), button_state);
		}

		int axisCount = m_PSMServiceController->ControllerState.VirtualController.numAxes;
		for (int axisIndex = 0; axisIndex < axisCount; ++axisIndex)
		{
			const float triggerValue = (float)m_PSMServiceController->ControllerState.VirtualController.axisStates[axisIndex] / 255.f;

			UpdateAxis((ePSMAxisID)(k_PSMButtonID_Virtual_0+axisIndex), triggerValue);
		}

		// Touchpad handling
		if (getConfig()->virtual_touchpad_XAxis_index >= 0 && getConfig()->virtual_touchpad_XAxis_index < axisCount &&
			getConfig()->virtual_touchpad_YAxis_index >= 0 && getConfig()->virtual_touchpad_YAxis_index < axisCount)
		{
			const unsigned char rawThumbStickX = m_PSMServiceController->ControllerState.VirtualController.axisStates[getConfig()->virtual_touchpad_XAxis_index];
			const unsigned char rawThumbStickY = m_PSMServiceController->ControllerState.VirtualController.axisStates[getConfig()->virtual_touchpad_YAxis_index];
			float thumbStickX = ((float)rawThumbStickX - 127.f) / 127.f;
			float thumbStickY = ((float)rawThumbStickY - 127.f) / 127.f;

			const float thumbStickAngle = atanf(abs(thumbStickY / thumbStickX));
			const float thumbStickRadialDist = sqrtf(thumbStickX*thumbStickX + thumbStickY * thumbStickY);

			bool bTouchpadTouched = false;
			bool bTouchpadPressed = false;

			// Moving a thumbstick outside of the deadzone is consider a touchpad touch
			if (thumbStickRadialDist >= getConfig()->thumbstick_deadzone)
			{
				// Rescale the thumbstick position to hide the dead zone
				const float rescaledRadius = (thumbStickRadialDist - getConfig()->thumbstick_deadzone) / (1.f - getConfig()->thumbstick_deadzone);

				// Set the thumbstick axis
				thumbStickX = (rescaledRadius / thumbStickRadialDist) * thumbStickX * abs(cosf(thumbStickAngle));
				thumbStickY = (rescaledRadius / thumbStickRadialDist) * thumbStickY * abs(sinf(thumbStickAngle));

				// Also make sure the touchpad is considered "touched" 
				// if the thumbstick is outside of the deadzone
				bTouchpadTouched = true;

				// If desired, also treat the touch as a press
				bTouchpadPressed = getConfig()->thumbstick_touch_as_press;
			}

			Controller::UpdateButton(k_PSMButtonID_EmulatedTrackpadTouched, bTouchpadTouched ? PSMButtonState_DOWN : PSMButtonState_UP);
			Controller::UpdateButton(k_PSMButtonID_EmulatedTrackpadPressed, bTouchpadPressed ? PSMButtonState_DOWN : PSMButtonState_UP);

			Controller::UpdateAxis(k_PSMAxisID_EmulatedTrackpad_X, thumbStickX);
			Controller::UpdateAxis(k_PSMAxisID_EmulatedTrackpad_Y, thumbStickY);
		}
		else
		{
			UpdateEmulatedTrackpad();
		}
	}

	// Updates the state of the controllers touchpad axis relative to its position over time and active state.
	void VirtualController::UpdateEmulatedTrackpad() {
		// Bail if the config hasn't enabled the emulated trackpad
		if (!HasButton(k_PSMButtonID_EmulatedTrackpadPressed) && !HasButton(k_PSMButtonID_EmulatedTrackpadPressed))
			return;

		// Find the highest priority emulated touch pad action (if any)
		eEmulatedTrackpadAction highestPriorityAction= k_EmulatedTrackpadAction_None;
		for (int buttonIndex = 0; buttonIndex < static_cast<int>(k_PSMButtonID_Count); ++buttonIndex) {
			eEmulatedTrackpadAction action= getConfig()->ps_button_id_to_emulated_touchpad_action[buttonIndex];
			if (action != k_EmulatedTrackpadAction_None) {
				PSMButtonState button_state= PSMButtonState_UP;
				if (Controller::GetButtonState((ePSMButtonID)buttonIndex, button_state)) {
					if (button_state == PSMButtonState_DOWN || button_state == PSMButtonState_PRESSED) {
						if (action >= highestPriorityAction) {
							highestPriorityAction= action;
						}

						if (action >= k_EmulatedTrackpadAction_Press) {
							break;
						}
					}
				}
			}
		}

		float touchpad_x = 0.f;
		float touchpad_y = 0.f;
		PSMButtonState emulatedTouchPadTouchedState= PSMButtonState_UP;
		PSMButtonState emulatedTouchPadPressedState= PSMButtonState_UP;

		if (highestPriorityAction == k_EmulatedTrackpadAction_Touch)
		{
			emulatedTouchPadTouchedState= PSMButtonState_DOWN;
		}
		else if (highestPriorityAction == k_EmulatedTrackpadAction_Press)
		{
			emulatedTouchPadTouchedState= PSMButtonState_DOWN;
			emulatedTouchPadPressedState= PSMButtonState_DOWN;
		}

		// If the action specifies a specific trackpad direction,
		// then use the given trackpad axis
		if (highestPriorityAction > k_EmulatedTrackpadAction_Press)
		{
			emulatedTouchPadTouchedState= PSMButtonState_DOWN;
			emulatedTouchPadPressedState= PSMButtonState_DOWN;

			switch (highestPriorityAction)
			{
			case k_EmulatedTrackpadAction_Left:
				touchpad_x= -1.f;
				touchpad_y= 0.f;
				break;
			case k_EmulatedTrackpadAction_Up:
				touchpad_x= 0.f;
				touchpad_y= 1.f;
				break;
			case k_EmulatedTrackpadAction_Right:
				touchpad_x= 1.f;
				touchpad_y= 0.f;
				break;
			case k_EmulatedTrackpadAction_Down:
				touchpad_x= 0.f;
				touchpad_y= -1.f;
				break;
			case k_EmulatedTrackpadAction_UpLeft:
				touchpad_x = -0.707f;
				touchpad_y = 0.707f;
				break;
			case k_EmulatedTrackpadAction_UpRight:
				touchpad_x = 0.707f;
				touchpad_y = 0.707f;
				break;
			case k_EmulatedTrackpadAction_DownLeft:
				touchpad_x = -0.707f;
				touchpad_y = -0.707f;
				break;
			case k_EmulatedTrackpadAction_DownRight:
				touchpad_x = 0.707f;
				touchpad_y = -0.707f;
				break;
			}
		}
		// Otherwise if the action was just a touch or press,
		// then use spatial offset method for determining touchpad axis
		if (highestPriorityAction == k_EmulatedTrackpadAction_Touch || 
			highestPriorityAction == k_EmulatedTrackpadAction_Press) {

			bool bIsNewTouchpadLocation = true;

			if (getConfig()->delay_after_touchpad_press) {
				std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();

				if (!m_bTouchpadWasActive) {
					const float k_max_touchpad_press = 2000.0; // time until coordinates are reset, otherwise assume in last location.
					std::chrono::duration<double, std::milli> timeSinceActivated = now - m_lastTouchpadPressTime;

					// true if the touchpad has been active for more than the max time required to hold it
					bIsNewTouchpadLocation = timeSinceActivated.count() >= k_max_touchpad_press;
				}
				m_lastTouchpadPressTime = now;
			}

			if (bIsNewTouchpadLocation) {
				if (!m_bTouchpadWasActive) {
					// Just pressed.
					const PSMPSMove &view = m_PSMServiceController->ControllerState.PSMoveState;
					m_driverSpaceRotationAtTouchpadPressTime = view.Pose.Orientation;

					Utils::GetMetersPosInRotSpace(&m_driverSpaceRotationAtTouchpadPressTime, &m_posMetersAtTouchpadPressTime, m_PSMServiceController->ControllerState.PSMoveState);

					#if LOG_TOUCHPAD_EMULATION != 0
					Logger::Info("Touchpad pressed! At (%f, %f, %f) meters relative to orientation\n",
									m_posMetersAtTouchpadPressTime.x, m_posMetersAtTouchpadPressTime.y, m_posMetersAtTouchpadPressTime.z);
					#endif
				} else {
					// Held!

					PSMVector3f newPosMeters;
					Utils::GetMetersPosInRotSpace(&m_driverSpaceRotationAtTouchpadPressTime, &newPosMeters, m_PSMServiceController->ControllerState.PSMoveState);

					PSMVector3f offsetMeters = PSM_Vector3fSubtract(&newPosMeters, &m_posMetersAtTouchpadPressTime);

					#if LOG_TOUCHPAD_EMULATION != 0
					Logger::Info("Touchpad held! Relative position (%f, %f, %f) meters\n",
									offsetMeters.x, offsetMeters.y, offsetMeters.z);
					#endif

					touchpad_x = fminf(fmaxf(offsetMeters.x / getConfig()->meters_per_touchpad_axis_units, -1.0f), 1.0f);
					touchpad_y = fminf(fmaxf(-offsetMeters.z / getConfig()->meters_per_touchpad_axis_units, -1.0f), 1.0f);

					#if LOG_TOUCHPAD_EMULATION != 0
					Logger::Info("Touchpad axis at (%f, %f) \n", touchpad_x, touchpad_x);
					#endif
				}
			}
		}

		Controller::UpdateButton(k_PSMButtonID_EmulatedTrackpadTouched, emulatedTouchPadTouchedState);
		Controller::UpdateButton(k_PSMButtonID_EmulatedTrackpadPressed, emulatedTouchPadPressedState);

		Controller::UpdateAxis(k_PSMAxisID_EmulatedTrackpad_X, touchpad_x);
		Controller::UpdateAxis(k_PSMAxisID_EmulatedTrackpad_Y, touchpad_y);

		// Remember if the touchpad was active the previous frame for edge detection
		m_bTouchpadWasActive = highestPriorityAction != k_EmulatedTrackpadAction_None;
	}

	void VirtualController::UpdateTrackingState() {
		assert(m_PSMServiceController != nullptr);
		assert(m_PSMServiceController->IsConnected);

		// The tracking status will be one of the following states:
		m_Pose.result = m_trackingStatus;

		m_Pose.deviceIsConnected = m_PSMServiceController->IsConnected;

		// These should always be false from any modern driver.  These are for Oculus DK1-like
		// rotation-only tracking.  Support for that has likely rotted in vrserver.
		m_Pose.willDriftInYaw = false;
		m_Pose.shouldApplyHeadModel = false;

		const PSMPSMove &view = m_PSMServiceController->ControllerState.PSMoveState;

		// No prediction since that's already handled in the psmove service
		m_Pose.poseTimeOffset = -0.016f;

		// No transform due to the current HMD orientation
		m_Pose.qDriverFromHeadRotation.w = 1.f;
		m_Pose.qDriverFromHeadRotation.x = 0.0f;
		m_Pose.qDriverFromHeadRotation.y = 0.0f;
		m_Pose.qDriverFromHeadRotation.z = 0.0f;
		m_Pose.vecDriverFromHeadTranslation[0] = 0.f;
		m_Pose.vecDriverFromHeadTranslation[1] = 0.f;
		m_Pose.vecDriverFromHeadTranslation[2] = 0.f;

		// Set position
		{
			const PSMVector3f &position = view.Pose.Position;
			m_Pose.vecPosition[0] = position.x * k_fScalePSMoveAPIToMeters;
			m_Pose.vecPosition[1] = position.y * k_fScalePSMoveAPIToMeters;
			m_Pose.vecPosition[2] = position.z * k_fScalePSMoveAPIToMeters;
		}

		// virtual extend controllers
		if (getConfig()->extend_Y_meters != 0.0f || getConfig()->extend_Z_meters != 0.0f) {
			const PSMQuatf &orientation = view.Pose.Orientation;

			PSMVector3f shift = { (float)m_Pose.vecPosition[0], (float)m_Pose.vecPosition[1], (float)m_Pose.vecPosition[2] };

			if (getConfig()->extend_Z_meters != 0.0f) {

				PSMVector3f local_forward = { 0, 0, -1 };
				PSMVector3f global_forward = PSM_QuatfRotateVector(&orientation, &local_forward);

				shift = PSM_Vector3fScaleAndAdd(&global_forward, getConfig()->extend_Z_meters, &shift);
			}

			if (getConfig()->extend_Y_meters != 0.0f) {

				PSMVector3f local_forward = { 0, -1, 0 };
				PSMVector3f global_forward = PSM_QuatfRotateVector(&orientation, &local_forward);

				shift = PSM_Vector3fScaleAndAdd(&global_forward, getConfig()->extend_Y_meters, &shift);
			}

			m_Pose.vecPosition[0] = shift.x;
			m_Pose.vecPosition[1] = shift.y;
			m_Pose.vecPosition[2] = shift.z;
		}

		// Set rotational coordinates
		{
			const PSMQuatf &orientation = view.Pose.Orientation;

			m_Pose.qRotation.w = getConfig()->z_rotate_90_degrees ? -orientation.w : orientation.w;
			m_Pose.qRotation.x = orientation.x;
			m_Pose.qRotation.y = orientation.y;
			m_Pose.qRotation.z = getConfig()->z_rotate_90_degrees ? -orientation.z : orientation.z;
		}

		// Set the physics state of the controller
		/*{
			const PSMPhysicsData &physicsData = view.PhysicsData;

			m_Pose.vecVelocity[0] = physicsData.LinearVelocityCmPerSec.x
				* abs(pow(abs(physicsData.LinearVelocityCmPerSec.x), m_fLinearVelocityExponent))
				* k_fScalePSMoveAPIToMeters * m_fLinearVelocityMultiplier;
			m_Pose.vecVelocity[1] = physicsData.LinearVelocityCmPerSec.y
				* abs(pow(abs(physicsData.LinearVelocityCmPerSec.y), m_fLinearVelocityExponent))
				* k_fScalePSMoveAPIToMeters * m_fLinearVelocityMultiplier;
			m_Pose.vecVelocity[2] = physicsData.LinearVelocityCmPerSec.z
				* abs(pow(abs(physicsData.LinearVelocityCmPerSec.z), m_fLinearVelocityExponent))
				* k_fScalePSMoveAPIToMeters * m_fLinearVelocityMultiplier;

			m_Pose.vecAcceleration[0] = physicsData.LinearAccelerationCmPerSecSqr.x * k_fScalePSMoveAPIToMeters;
			m_Pose.vecAcceleration[1] = physicsData.LinearAccelerationCmPerSecSqr.y * k_fScalePSMoveAPIToMeters;
			m_Pose.vecAcceleration[2] = physicsData.LinearAccelerationCmPerSecSqr.z * k_fScalePSMoveAPIToMeters;

			m_Pose.vecAngularVelocity[0] = physicsData.AngularVelocityRadPerSec.x;
			m_Pose.vecAngularVelocity[1] = physicsData.AngularVelocityRadPerSec.y;
			m_Pose.vecAngularVelocity[2] = physicsData.AngularVelocityRadPerSec.z;

			m_Pose.vecAngularAcceleration[0] = physicsData.AngularAccelerationRadPerSecSqr.x;
			m_Pose.vecAngularAcceleration[1] = physicsData.AngularAccelerationRadPerSecSqr.y;
			m_Pose.vecAngularAcceleration[2] = physicsData.AngularAccelerationRadPerSecSqr.z;
		}*/

		m_Pose.poseIsValid =
			m_PSMServiceController->ControllerState.PSMoveState.bIsPositionValid &&
			m_PSMServiceController->ControllerState.PSMoveState.bIsOrientationValid;

		// This call posts this pose to shared memory, where all clients will have access to it the next
		// moment they want to predict a pose.
		vr::VRServerDriverHost()->TrackedDevicePoseUpdated(m_unSteamVRTrackedDeviceId, m_Pose, sizeof(vr::DriverPose_t));
	}

	void VirtualController::Update() {
		Controller::Update();

		if (IsActivated() && m_PSMServiceController->IsConnected) {
			int seq_num = m_PSMServiceController->OutputSequenceNum;

			// Only other updating incoming state if it actually changed and is due for one
			if (m_nPoseSequenceNumber < seq_num) {
				m_nPoseSequenceNumber = seq_num;

				UpdateTrackingState();
				UpdateControllerState();
			}
		}
	}

	void VirtualController::RefreshWorldFromDriverPose() {
		TrackableDevice::RefreshWorldFromDriverPose();

		// Mark the calibration process as done once we have setup the world from driver pose
		m_trackingStatus = vr::TrackingResult_Running_OK;
	}
}
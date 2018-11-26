#define _USE_MATH_DEFINES

#include "constants.h"
#include "server_driver.h"
#include "utils.h"
#include "settings_util.h"
#include "driver.h"
#include "facing_handsolver.h"
#include "ps_ds4_controller.h"
#include "trackable_device.h"
#include <assert.h>

namespace steamvrbridge {

	PSDualshock4Controller::PSDualshock4Controller(
		PSMControllerID psmControllerId,
		vr::ETrackedControllerRole trackedControllerRole,
		const char *psmSerialNo)
		: Controller()
		, m_parentController(nullptr)
		, m_nPSMControllerId(psmControllerId)
		, m_PSMServiceController(nullptr)
		, m_nPoseSequenceNumber(0)
		, m_bRumbleSuppressed(false)
		, m_resetPoseButtonPressTime()
		, m_bResetPoseRequestSent(false)
		, m_resetAlignButtonPressTime()
		, m_bResetAlignRequestSent(false)
		, m_touchpadDirectionsUsed(false)
		, m_fControllerMetersInFrontOfHmdAtCalibration(0.f)
		, m_bDisableHMDAlignmentGesture(false)
		, m_bUseControllerOrientationInHMDAlignment(false)
		, m_lastSanitizedLeftThumbstick_X(0.f)
		, m_lastSanitizedLeftThumbstick_Y(0.f)
		, m_lastSanitizedRightThumbstick_X(0.f)
		, m_lastSanitizedRightThumbstick_Y(0.f)
		, m_thumbstickDeadzone(k_defaultThumbstickDeadZoneRadius)
		, m_fLinearVelocityMultiplier(1.f)
		, m_fLinearVelocityExponent(0.f)
		, m_hmdAlignPSButtonID(k_PSMButtonID_Select) {
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

		m_TrackedControllerRole = trackedControllerRole;

		// Map every button to not be associated with any touchpad direction, initially
		memset(m_psButtonIDToEmulatedTouchpadAction, k_EmulatedTrackpadAction_None, k_PSMButtonID_Count * sizeof(vr::EVRButtonId));

		m_trackingStatus = vr::TrackingResult_Running_OK;
	}

	PSDualshock4Controller::~PSDualshock4Controller() {
		PSM_FreeControllerListener(m_PSMServiceController->ControllerID);
		m_PSMServiceController = nullptr;
	}

	void PSDualshock4Controller::LoadSettings(vr::IVRSettings *pSettings) {
		Controller::LoadSettings(pSettings);

		const char * szModelName= GetControllerSettingsPrefix();

		// DS4 controller button -> fake touchpad mappings
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_PS, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_Triangle, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_Circle, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_Cross, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_Square, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_DPad_Left, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_DPad_Up, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_DPad_Right, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_DPad_Down, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_Options, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_Share, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_Touchpad, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_LeftJoystick, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_RightJoystick, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_LeftShoulder, m_nPSMControllerId);
		LoadEmulatedTouchpadActions(pSettings, k_PSMButtonID_RightShoulder, m_nPSMControllerId);

		// Throwing power settings
		m_fLinearVelocityMultiplier =
			SettingsUtil::LoadFloat(pSettings, szModelName, "linear_velocity_multiplier", 1.f);
		m_fLinearVelocityExponent =
			SettingsUtil::LoadFloat(pSettings, szModelName, "linear_velocity_exponent", 0.f);

		// General Settings
		m_bRumbleSuppressed= 
			SettingsUtil::LoadBool(pSettings, szModelName, "rumble_suppressed", m_bRumbleSuppressed);
		m_fVirtuallExtendControllersYMeters = 
			SettingsUtil::LoadFloat(pSettings, szModelName, "extend_y", 0.0f);
		m_fVirtuallExtendControllersZMeters = 
			SettingsUtil::LoadFloat(pSettings, szModelName, "extend_z", 0.0f);
		m_fVirtuallyRotateController = 
			SettingsUtil::LoadBool(pSettings, szModelName, "rotate", false);
        m_bDisableHMDAlignmentGesture= 
			SettingsUtil::LoadBool(pSettings, szModelName, "disable_alignment_gesture", false);
		m_bUseControllerOrientationInHMDAlignment= 
			SettingsUtil::LoadBool(pSettings, szModelName, "use_orientation_in_alignment", true);
		m_fControllerMetersInFrontOfHmdAtCalibration= 
			SettingsUtil::LoadFloat(pSettings, szModelName, "calibration_cm_offset", 16.f) / 100.f;
		m_thumbstickDeadzone =
			fminf(fmaxf(SettingsUtil::LoadFloat(pSettings, szModelName, "thumbstick_deadzone_radius", k_defaultThumbstickDeadZoneRadius), 0.f), 0.99f);
	}

	vr::EVRInitError PSDualshock4Controller::Activate(vr::TrackedDeviceIndex_t unObjectId) {
		vr::EVRInitError result = Controller::Activate(unObjectId);

		if (result == vr::VRInitError_None) {
			Logger::Info("PSDualshock4Controller::Activate - Controller %d Activated\n", unObjectId);

			CServerDriver_PSMoveService::getInstance()->LaunchPSMoveMonitor();

			PSMRequestID requestId;
			if (PSM_StartControllerDataStreamAsync(
				m_PSMServiceController->ControllerID,
				PSMStreamFlags_includePositionData | PSMStreamFlags_includePhysicsData,
				&requestId) != PSMResult_Error) {
				PSM_RegisterCallback(requestId, PSDualshock4Controller::start_controller_response_callback, this);
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

				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_WillDriftInYaw_Bool, true);
				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceIsWireless_Bool, true);
				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceProvidesBatteryStatus_Bool, false);

				properties->SetInt32Property(m_ulPropertyContainer, vr::Prop_DeviceClass_Int32, vr::TrackedDeviceClass_Controller);

				// The {psmove} syntax lets us refer to rendermodels that are installed
				// in the driver's own resources/rendermodels directory.  The driver can
				// still refer to SteamVR models like "generic_hmd".
				if (m_overrideModel.length() > 0)
				{
					properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, m_overrideModel.c_str());
				}
				else
				{
					properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "{psmove}dualshock4_controller");
				}

				// Set device properties
				vr::VRProperties()->SetInt32Property(m_ulPropertyContainer, vr::Prop_ControllerRoleHint_Int32, m_TrackedControllerRole);
				vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_ManufacturerName_String, "Sony");
				vr::VRProperties()->SetUint64Property(m_ulPropertyContainer, vr::Prop_HardwareRevision_Uint64, 1313);
				vr::VRProperties()->SetUint64Property(m_ulPropertyContainer, vr::Prop_FirmwareVersion_Uint64, 1315);
				vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_ModelNumber_String, "Dualshock4");
				vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_SerialNumber_String, m_strPSMControllerSerialNo.c_str());
			}
		}

		return result;
	}

	void PSDualshock4Controller::start_controller_response_callback(
		const PSMResponseMessage *response, void *userdata) {
		PSDualshock4Controller *controller = reinterpret_cast<PSDualshock4Controller *>(userdata);

		if (response->result_code == PSMResult::PSMResult_Success) {

			Logger::Info("PSDualshock4Controller::start_controller_response_callback - Controller stream started\n");

			// Create the special case system button (bound to PS button)
			controller->CreateButtonComponent(k_PSMButtonID_System);

			// Create buttons components
			controller->CreateButtonComponent(k_PSMButtonID_PS);
			controller->CreateButtonComponent(k_PSMButtonID_Triangle);
			controller->CreateButtonComponent(k_PSMButtonID_Circle);
			controller->CreateButtonComponent(k_PSMButtonID_Cross);
			controller->CreateButtonComponent(k_PSMButtonID_Square);
			controller->CreateButtonComponent(k_PSMButtonID_DPad_Up);
			controller->CreateButtonComponent(k_PSMButtonID_DPad_Down);
			controller->CreateButtonComponent(k_PSMButtonID_DPad_Left);
			controller->CreateButtonComponent(k_PSMButtonID_DPad_Right);
			controller->CreateButtonComponent(k_PSMButtonID_Options);
			controller->CreateButtonComponent(k_PSMButtonID_Share);
			controller->CreateButtonComponent(k_PSMButtonID_Touchpad);
			controller->CreateButtonComponent(k_PSMButtonID_LeftJoystick);
			controller->CreateButtonComponent(k_PSMButtonID_RightJoystick);
			controller->CreateButtonComponent(k_PSMButtonID_LeftShoulder);
			controller->CreateButtonComponent(k_PSMButtonID_RightShoulder);

			// Create axis components
			controller->CreateAxisComponent(k_PSMAxisID_LeftTrigger);
			controller->CreateAxisComponent(k_PSMAxisID_RightTrigger);
			controller->CreateAxisComponent(k_PSMAxisID_LeftJoystick_X);
			controller->CreateAxisComponent(k_PSMAxisID_LeftJoystick_Y);
			controller->CreateAxisComponent(k_PSMAxisID_RightJoystick_X);
			controller->CreateAxisComponent(k_PSMAxisID_RightJoystick_Y);

			// Create components for emulated trackpad
			controller->CreateButtonComponent(k_PSMButtonID_EmulatedTrackpadTouched);
			controller->CreateButtonComponent(k_PSMButtonID_EmulatedTrackpadPressed);
			controller->CreateAxisComponent(k_PSMAxisID_EmulatedTrackpad_X);
			controller->CreateAxisComponent(k_PSMAxisID_EmulatedTrackpad_Y);

			// Create haptic components
			controller->CreateHapticComponent(k_PSMHapticID_LeftRumble);
			controller->CreateHapticComponent(k_PSMHapticID_RightRumble);
		}
	}

	void PSDualshock4Controller::Deactivate() {
		Logger::Info("PSDualshock4Controller::Deactivate - Controller stream stopped\n");
		PSM_StopControllerDataStreamAsync(m_PSMServiceController->ControllerID, nullptr);
		Controller::Deactivate();
	}

	void PSDualshock4Controller::UpdateControllerState() {
		static const uint64_t s_kTouchpadButtonMask = vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);

		assert(m_PSMServiceController != nullptr);
		assert(m_PSMServiceController->IsConnected);

        const PSMDualShock4 &clientView = m_PSMServiceController->ControllerState.PSDS4State;

		const bool bStartRealignHMDTriggered =
			(clientView.ShareButton == PSMButtonState_PRESSED && clientView.OptionsButton == PSMButtonState_PRESSED) ||
			(clientView.ShareButton == PSMButtonState_PRESSED && clientView.OptionsButton == PSMButtonState_DOWN) ||
			(clientView.ShareButton == PSMButtonState_DOWN && clientView.OptionsButton == PSMButtonState_PRESSED);

		// See if the recenter button has been held for the requisite amount of time
		bool bRecenterRequestTriggered = false;
		{
			PSMButtonState resetPoseButtonState = clientView.OptionsButton;

			switch (resetPoseButtonState)
			{
			case PSMButtonState_PRESSED:
				{
					m_resetPoseButtonPressTime = std::chrono::high_resolution_clock::now();
				} break;
			case PSMButtonState_DOWN:
			{
				if (!m_bResetPoseRequestSent)
				{
					const float k_hold_duration_milli = 250.f;
					std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();
					std::chrono::duration<float, std::milli> pressDurationMilli = now - m_resetPoseButtonPressTime;

					if (pressDurationMilli.count() >= k_hold_duration_milli)
					{
						bRecenterRequestTriggered = true;
					}
				}
			} break;
			case PSMButtonState_RELEASED:
				{
					m_bResetPoseRequestSent = false;
				} break;
			}
		}

		// If SHARE was just pressed while and OPTIONS was held or vice versa,
		// recenter the controller orientation pose and start the realignment of the controller to HMD tracking space.
		if (bStartRealignHMDTriggered && !m_bDisableHMDAlignmentGesture)
		{
			Logger::Info("PSDualshock4Controller::UpdateControllerState(): Calling StartRealignHMDTrackingSpace() in response to controller chord.\n");

			PSM_ResetControllerOrientationAsync(m_PSMServiceController->ControllerID, k_psm_quaternion_identity, nullptr);
			m_bResetPoseRequestSent = true;

			// We have the transform of the HMD in world space. 
			// The controller's position is a few inches ahead of the HMD's on the HMD's local -Z axis. 
			PSMVector3f controllerLocalOffsetFromHmdPosition = *k_psm_float_vector3_zero;
			controllerLocalOffsetFromHmdPosition = { 0.0f, 0.0f, -1.0f * m_fControllerMetersInFrontOfHmdAtCalibration };

			try {
				PSMPosef hmdPose = Utils::GetHMDPoseInMeters();
				PSMPosef realignedPose = Utils::RealignHMDTrackingSpace(*k_psm_quaternion_identity,
																		controllerLocalOffsetFromHmdPosition,
																		m_PSMServiceController->ControllerID,
																		hmdPose,
																		m_bUseControllerOrientationInHMDAlignment);
				CServerDriver_PSMoveService::getInstance()->SetHMDTrackingSpace(realignedPose);
			} catch (std::exception & e) {
				// Log an error message and safely carry on
				Logger::Error(e.what());
			}

			m_bResetAlignRequestSent = true;
		}
		else if (bRecenterRequestTriggered)
		{
			Logger::Info("PSDualshock4Controller::UpdateControllerState(): Calling ClientPSMoveAPI::reset_orientation() in response to controller button press.\n");

			PSM_ResetControllerOrientationAsync(m_PSMServiceController->ControllerID, k_psm_quaternion_identity, nullptr);
			m_bResetPoseRequestSent = true;
		}
		else
		{
			// System Button hard-coded to PS button
			Controller::UpdateButton(k_PSMButtonID_System, clientView.PSButton);

			// Process all the native buttons 
			Controller::UpdateButton(k_PSMButtonID_PS, clientView.PSButton);
			Controller::UpdateButton(k_PSMButtonID_Triangle, clientView.TriangleButton);
			Controller::UpdateButton(k_PSMButtonID_Circle, clientView.CircleButton);
			Controller::UpdateButton(k_PSMButtonID_Cross, clientView.CrossButton);
			Controller::UpdateButton(k_PSMButtonID_Square, clientView.SquareButton);
			Controller::UpdateButton(k_PSMButtonID_DPad_Up, clientView.DPadUpButton);
			Controller::UpdateButton(k_PSMButtonID_DPad_Down, clientView.DPadDownButton);
			Controller::UpdateButton(k_PSMButtonID_DPad_Left, clientView.DPadLeftButton);
			Controller::UpdateButton(k_PSMButtonID_DPad_Right, clientView.DPadRightButton);
			Controller::UpdateButton(k_PSMButtonID_Options, clientView.OptionsButton);
			Controller::UpdateButton(k_PSMButtonID_Share, clientView.ShareButton);
			Controller::UpdateButton(k_PSMButtonID_Touchpad, clientView.TrackPadButton);
			Controller::UpdateButton(k_PSMButtonID_LeftJoystick, clientView.L3Button);
			Controller::UpdateButton(k_PSMButtonID_RightJoystick, clientView.R3Button);
			Controller::UpdateButton(k_PSMButtonID_LeftShoulder, clientView.L1Button);
			Controller::UpdateButton(k_PSMButtonID_RightShoulder, clientView.R1Button);

			// Thumbstick handling
			UpdateThumbsticks();

			// Touchpad handling
			UpdateEmulatedTrackpad();

			// Trigger handling
			Controller::UpdateAxis(k_PSMAxisID_LeftTrigger, clientView.LeftTriggerValue / 255.f);
			Controller::UpdateAxis(k_PSMAxisID_RightTrigger, clientView.RightTriggerValue / 255.f);
		}
	}

	void PSDualshock4Controller::RemapThumbstick(
		const float thumb_stick_x, const float thumb_stick_y,
		float &out_sanitized_x, float &out_sanitized_y)
	{
		const float thumb_stick_radius = sqrtf(thumb_stick_x*thumb_stick_x + thumb_stick_y * thumb_stick_y);

		// Moving a thumb-stick outside of the deadzone is consider a touchpad touch
		if (thumb_stick_radius >= m_thumbstickDeadzone)
		{
			// Rescale the thumb-stick position to hide the dead zone
			const float rescaledRadius = (thumb_stick_radius - m_thumbstickDeadzone) / (1.f - m_thumbstickDeadzone);

			// Set the thumb-stick axis
			out_sanitized_x = (rescaledRadius / thumb_stick_radius) * thumb_stick_x;
			out_sanitized_y = (rescaledRadius / thumb_stick_radius) * thumb_stick_y;
		}
		else
		{
			out_sanitized_x= 0.f;
			out_sanitized_y= 0.f;
		}
	}

	void PSDualshock4Controller::UpdateThumbsticks()
	{
		const PSMDualShock4 &clientView = m_PSMServiceController->ControllerState.PSDS4State;

		RemapThumbstick(
			clientView.LeftAnalogX, clientView.LeftAnalogY,
			m_lastSanitizedLeftThumbstick_X, m_lastSanitizedLeftThumbstick_Y);
		RemapThumbstick(
			clientView.RightAnalogX, clientView.RightAnalogY,
			m_lastSanitizedRightThumbstick_X, m_lastSanitizedRightThumbstick_Y);

		Controller::UpdateAxis(k_PSMAxisID_LeftJoystick_X, m_lastSanitizedLeftThumbstick_X);
		Controller::UpdateAxis(k_PSMAxisID_LeftJoystick_Y, m_lastSanitizedLeftThumbstick_Y);
		Controller::UpdateAxis(k_PSMAxisID_RightJoystick_X, m_lastSanitizedRightThumbstick_X);
		Controller::UpdateAxis(k_PSMAxisID_RightJoystick_Y, m_lastSanitizedRightThumbstick_Y);
	}

	// Updates the state of the controllers touchpad axis relative to its position over time and active state.
	void PSDualshock4Controller::UpdateEmulatedTrackpad() {
		// Bail if the config hasn't enabled the emulated trackpad
		if (!HasButton(k_PSMButtonID_EmulatedTrackpadPressed) && !HasButton(k_PSMButtonID_EmulatedTrackpadPressed))
			return;

		// Find the highest priority emulated touch pad action (if any)
		eEmulatedTrackpadAction highestPriorityAction= k_EmulatedTrackpadAction_None;
		for (int buttonIndex = 0; buttonIndex < static_cast<int>(k_PSMButtonID_Count); ++buttonIndex) {
			eEmulatedTrackpadAction action= m_psButtonIDToEmulatedTouchpadAction[buttonIndex];
			if (action != k_EmulatedTrackpadAction_None) {
				PSMButtonState button_state= PSMButtonState_UP;
				if (Controller::GetButtonState((ePSMButtonID)buttonIndex, button_state))
				{
					if (action >= highestPriorityAction)
					{
						highestPriorityAction= action;
					}

					if (action >= k_EmulatedTrackpadAction_Press)
					{
						break;
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

		if (highestPriorityAction > k_EmulatedTrackpadAction_Press)
		{
			emulatedTouchPadTouchedState= PSMButtonState_DOWN;
			emulatedTouchPadPressedState= PSMButtonState_DOWN;

			// If the action specifies a specific trackpad direction,
			// then use the given trackpad axis
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
		} else if (m_psButtonIDToEmulatedTouchpadAction[k_PSMButtonID_LeftJoystick] != k_EmulatedTrackpadAction_None) {
			// Consider the touchpad pressed if the left thumbstick is deflected at all.
			const bool bIsTouched= fabsf(m_lastSanitizedLeftThumbstick_X) + fabsf(m_lastSanitizedLeftThumbstick_Y) > 0.f;

			if (bIsTouched)
			{
				emulatedTouchPadTouchedState= PSMButtonState_DOWN;
				emulatedTouchPadPressedState= PSMButtonState_DOWN;
				touchpad_x= m_lastSanitizedLeftThumbstick_X;
				touchpad_y= m_lastSanitizedLeftThumbstick_Y;
			}
		} else if (m_psButtonIDToEmulatedTouchpadAction[k_PSMButtonID_RightJoystick] != k_EmulatedTrackpadAction_None) {
			// Consider the touchpad pressed if the right thumbstick is deflected at all.
			const bool bIsTouched= fabsf(m_lastSanitizedRightThumbstick_X) + fabsf(m_lastSanitizedRightThumbstick_Y) > 0.f;

			if (bIsTouched)
			{
				emulatedTouchPadTouchedState= PSMButtonState_DOWN;
				emulatedTouchPadPressedState= PSMButtonState_DOWN;
				touchpad_x= m_lastSanitizedRightThumbstick_X;
				touchpad_y= m_lastSanitizedRightThumbstick_Y;
			}
		}

		Controller::UpdateButton(k_PSMButtonID_EmulatedTrackpadTouched, emulatedTouchPadTouchedState);
		Controller::UpdateButton(k_PSMButtonID_EmulatedTrackpadPressed, emulatedTouchPadPressedState);

		Controller::UpdateAxis(k_PSMAxisID_EmulatedTrackpad_X, touchpad_x);
		Controller::UpdateAxis(k_PSMAxisID_EmulatedTrackpad_Y, touchpad_y);
	}

	void PSDualshock4Controller::UpdateTrackingState() {
		assert(m_PSMServiceController != nullptr);
		assert(m_PSMServiceController->IsConnected);

		const PSMDualShock4 &view = m_PSMServiceController->ControllerState.PSDS4State;

		// The tracking status will be one of the following states:
		m_Pose.result = m_trackingStatus;

		m_Pose.deviceIsConnected = m_PSMServiceController->IsConnected;

		// These should always be false from any modern driver.  These are for Oculus DK1-like
		// rotation-only tracking.  Support for that has likely rotted in vrserver.
		m_Pose.willDriftInYaw = false;
		m_Pose.shouldApplyHeadModel = false;

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
		if (m_fVirtuallExtendControllersYMeters != 0.0f || m_fVirtuallExtendControllersZMeters != 0.0f) {
			const PSMQuatf &orientation = view.Pose.Orientation;

			PSMVector3f shift = { (float)m_Pose.vecPosition[0], (float)m_Pose.vecPosition[1], (float)m_Pose.vecPosition[2] };

			if (m_fVirtuallExtendControllersZMeters != 0.0f) {

				PSMVector3f local_forward = { 0, 0, -1 };
				PSMVector3f global_forward = PSM_QuatfRotateVector(&orientation, &local_forward);

				shift = PSM_Vector3fScaleAndAdd(&global_forward, m_fVirtuallExtendControllersZMeters, &shift);
			}

			if (m_fVirtuallExtendControllersYMeters != 0.0f) {

				PSMVector3f local_forward = { 0, -1, 0 };
				PSMVector3f global_forward = PSM_QuatfRotateVector(&orientation, &local_forward);

				shift = PSM_Vector3fScaleAndAdd(&global_forward, m_fVirtuallExtendControllersYMeters, &shift);
			}

			m_Pose.vecPosition[0] = shift.x;
			m_Pose.vecPosition[1] = shift.y;
			m_Pose.vecPosition[2] = shift.z;
		}

		// Set rotational coordinates
		{
			const PSMQuatf &orientation = view.Pose.Orientation;

			m_Pose.qRotation.w = m_fVirtuallyRotateController ? -orientation.w : orientation.w;
			m_Pose.qRotation.x = orientation.x;
			m_Pose.qRotation.y = orientation.y;
			m_Pose.qRotation.z = m_fVirtuallyRotateController ? -orientation.z : orientation.z;
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

		m_Pose.poseIsValid = view.bIsPositionValid && view.bIsOrientationValid;

		// This call posts this pose to shared memory, where all clients will have access to it the next
		// moment they want to predict a pose.
		vr::VRServerDriverHost()->TrackedDevicePoseUpdated(m_unSteamVRTrackedDeviceId, m_Pose, sizeof(vr::DriverPose_t));
	}

	// TODO - Make use of amplitude and frequency for Buffered Haptics, will give us patterning and panning vibration
	// See: https://developer.oculus.com/documentation/pcsdk/latest/concepts/dg-input-touch-haptic/
	void PSDualshock4Controller::UpdateRumbleState(PSMControllerRumbleChannel channel) {
		Controller::HapticState *haptic_state= 
			GetHapticState(
				channel == PSMControllerRumbleChannel_Left
				? k_PSMHapticID_LeftRumble
				: k_PSMHapticID_RightRumble);

		if (haptic_state == nullptr)
			return;

		if (!m_bRumbleSuppressed) {
			// pulse duration - the length of each pulse
			// amplitude - strength of vibration
			// frequency - speed of each pulse

			// convert to microseconds, the max duration received from OpenVR appears to be 5 micro seconds
			uint16_t pendingHapticPulseDurationMicroSecs = 
				static_cast<uint16_t>(haptic_state->pendingHapticDurationSecs * 1000000);

			const float k_max_rumble_update_rate = 33.f; // Don't bother trying to update the rumble faster than 30fps (33ms)
			const float k_max_pulse_microseconds = 5000.f; // Docs suggest max pulse duration of 5ms, but we'll call 1ms max

			std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();
			bool bTimoutElapsed = true;

			if (haptic_state->lastTimeRumbleSentValid) {
				std::chrono::duration<double, std::milli> timeSinceLastSend = now - haptic_state->lastTimeRumbleSent;

				bTimoutElapsed = timeSinceLastSend.count() >= k_max_rumble_update_rate;
			}

			// See if a rumble request hasn't come too recently
			if (bTimoutElapsed) {
				float rumble_fraction = 
					(static_cast<float>(pendingHapticPulseDurationMicroSecs) / k_max_pulse_microseconds) 
					* haptic_state->pendingHapticAmplitude;

				if (rumble_fraction > 0)
				{
					steamvrbridge::Logger::Debug(
						"PSDualshock4Controller::UpdateRumble: m_pendingHapticPulseDurationSecs=%f, pendingHapticPulseDurationMicroSecs=%d, rumble_fraction=%f\n", 
						haptic_state->pendingHapticDurationSecs, pendingHapticPulseDurationMicroSecs, rumble_fraction);
				}

				// Unless a zero rumble intensity was explicitly set, 
				// don't rumble less than 35% (no enough to feel)
				if (haptic_state->pendingHapticDurationSecs != 0) {
					if (rumble_fraction < 0.35f) {
						// rumble values less 35% isn't noticeable
						rumble_fraction = 0.35f;
					}
				}

				// Keep the pulse intensity within reasonable bounds
				if (rumble_fraction > 1.f) {
					rumble_fraction = 1.f;
				}

				// Actually send the rumble to the server
				PSM_SetControllerRumble(m_PSMServiceController->ControllerID, channel, rumble_fraction);

				// Remember the last rumble we went and when we sent it
				haptic_state->lastTimeRumbleSent = now;
				haptic_state->lastTimeRumbleSentValid = true;

				// Reset the pending haptic pulse duration.
				// If another call to TriggerHapticPulse() is made later, it will stomp this value.
				// If no future haptic event is received by ServerDriver then the next call to UpdateRumbleState()
				// in k_max_rumble_update_rate milliseconds will set the rumble_fraction to 0.f
				// This effectively makes the shortest rumble pulse k_max_rumble_update_rate milliseconds.
				haptic_state->pendingHapticDurationSecs = DEFAULT_HAPTIC_DURATION;
				haptic_state->pendingHapticAmplitude = DEFAULT_HAPTIC_AMPLITUDE;
				haptic_state->pendingHapticFrequency = DEFAULT_HAPTIC_FREQUENCY;
			}
		} else {
			// Reset the pending haptic pulse duration since rumble is suppressed.
			haptic_state->pendingHapticDurationSecs = DEFAULT_HAPTIC_DURATION;
			haptic_state->pendingHapticAmplitude = DEFAULT_HAPTIC_AMPLITUDE;
			haptic_state->pendingHapticFrequency = DEFAULT_HAPTIC_FREQUENCY;
		}
	}

	void PSDualshock4Controller::Update() {
		Controller::Update();

		if (IsActivated() && m_PSMServiceController->IsConnected) {
			int seq_num = m_PSMServiceController->OutputSequenceNum;

			// Only other updating incoming state if it actually changed and is due for one
			if (m_nPoseSequenceNumber < seq_num) {
				m_nPoseSequenceNumber = seq_num;

				UpdateTrackingState();
				UpdateControllerState();
			}

			// Update the outgoing state
			UpdateRumbleState(PSMControllerRumbleChannel_Left);
			UpdateRumbleState(PSMControllerRumbleChannel_Right);
		}
	}

	void PSDualshock4Controller::RefreshWorldFromDriverPose() {
		TrackableDevice::RefreshWorldFromDriverPose();
		// Mark the calibration process as done once we have setup the world from driver pose
		m_trackingStatus = vr::TrackingResult_Running_OK;
	}
}
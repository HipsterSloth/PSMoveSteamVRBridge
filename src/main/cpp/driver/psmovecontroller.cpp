#define _USE_MATH_DEFINES

#include "constants.h"
#include "serverdriver.h"
#include "utils.h"
#include "settingsutil.h"
#include "driver.h"
#include "facinghandsolver.h"
#include "psmovecontroller.h"
#include <assert.h>

#if _MSC_VER
#define strcasecmp(a, b) stricmp(a,b)
#pragma warning (disable: 4996) // 'This function or variable may be unsafe': snprintf
#define snprintf _snprintf
#endif

namespace steamvrbridge {

	PSMoveController::PSMoveController(
		PSMControllerID psmControllerId,
		PSMControllerType psmControllerType,
		vr::ETrackedControllerRole trackedControllerRole,
		const char *psmSerialNo)
		: TrackableDevice()
		, m_nPSMControllerId(psmControllerId)
		, m_PSMControllerType(psmControllerType)
		, m_PSMServiceController(nullptr)
		, m_nPoseSequenceNumber(0)
		, m_bIsBatteryCharging(false)
		, m_fBatteryChargeFraction(0.f)
		, m_bRumbleSuppressed(false)
		, m_pendingHapticPulseDurationSecs(0)
		, m_lastTimeRumbleSent()
		, m_lastTimeRumbleSentValid(false)
		, m_resetPoseButtonPressTime()
		, m_bResetPoseRequestSent(false)
		, m_resetAlignButtonPressTime()
		, m_bResetAlignRequestSent(false)
		, m_fVirtuallExtendControllersZMeters(0.0f)
		, m_fVirtuallExtendControllersYMeters(0.0f)
		, m_fVirtuallyRotateController(false)
		, m_bDelayAfterTouchpadPress(false)
		, m_bTouchpadWasActive(false)
		, m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis(false)
		, m_fControllerMetersInFrontOfHmdAtCalibration(0.f)
		, m_posMetersAtTouchpadPressTime(*k_psm_float_vector3_zero)
		, m_driverSpaceRotationAtTouchpadPressTime(*k_psm_quaternion_identity)
		, m_bDisableHMDAlignmentGesture(false)
		, m_bUseControllerOrientationInHMDAlignment(false)
		, m_steamVRTriggerAxisIndex(1)
		, m_virtualTriggerAxisIndex(-1)
		, m_virtualTouchpadXAxisIndex(-1)
		, m_virtualTouchpadYAxisIndex(-1)
		, m_fLinearVelocityMultiplier(1.f)
		, m_fLinearVelocityExponent(0.f)
		, m_hmdAlignPSButtonID(k_EPSButtonID_Select)
		, m_overrideModel("")
		, m_orientationSolver(nullptr) {
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

		// Load config from steamvr.vrsettings
		vr::IVRSettings *pSettings = vr::VRSettings();

		// Map every button to the system button initially
		memset(psButtonIDToVRButtonID, vr::k_EButton_SteamVR_Trigger, k_EPSButtonID_Count * sizeof(vr::EVRButtonId));

		// Map every button to not be associated with any touchpad direction, initially
		memset(psButtonIDToVrTouchpadDirection, k_EVRTouchpadDirection_None, k_EPSButtonID_Count * sizeof(vr::EVRButtonId));

		if (pSettings != nullptr) {
			//PSMove controller button mappings

			LoadButtonMapping(pSettings, k_EPSButtonID_PS, vr::k_EButton_System, k_EVRTouchpadDirection_None, psmControllerId);
			LoadButtonMapping(pSettings, k_EPSButtonID_Move, vr::k_EButton_SteamVR_Touchpad, k_EVRTouchpadDirection_None, psmControllerId);
			LoadButtonMapping(pSettings, k_EPSButtonID_Trigger, vr::k_EButton_SteamVR_Trigger, k_EVRTouchpadDirection_None, psmControllerId);
			LoadButtonMapping(pSettings, k_EPSButtonID_Triangle, (vr::EVRButtonId)8, k_EVRTouchpadDirection_None, psmControllerId);
			LoadButtonMapping(pSettings, k_EPSButtonID_Square, (vr::EVRButtonId)9, k_EVRTouchpadDirection_None, psmControllerId);
			LoadButtonMapping(pSettings, k_EPSButtonID_Circle, (vr::EVRButtonId)10, k_EVRTouchpadDirection_None, psmControllerId);
			LoadButtonMapping(pSettings, k_EPSButtonID_Cross, (vr::EVRButtonId)11, k_EVRTouchpadDirection_None, psmControllerId);
			LoadButtonMapping(pSettings, k_EPSButtonID_Select, vr::k_EButton_Grip, k_EVRTouchpadDirection_None, psmControllerId);
			LoadButtonMapping(pSettings, k_EPSButtonID_Start, vr::k_EButton_ApplicationMenu, k_EVRTouchpadDirection_None, psmControllerId);

			// Touch pad settings
			m_bDelayAfterTouchpadPress =
				SettingsUtil::LoadBool(pSettings, "psmove_touchpad", "delay_after_touchpad_press", m_bDelayAfterTouchpadPress);
			m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis =
				SettingsUtil::LoadBool(pSettings, "psmove", "use_spatial_offset_after_touchpad_press_as_touchpad_axis", false);
			m_fMetersPerTouchpadAxisUnits =
				SettingsUtil::LoadFloat(pSettings, "psmove", "meters_per_touchpad_units", .075f);

			// Throwing power settings
			m_fLinearVelocityMultiplier =
				SettingsUtil::LoadFloat(pSettings, "psmove_settings", "linear_velocity_multiplier", 1.f);
			m_fLinearVelocityExponent =
				SettingsUtil::LoadFloat(pSettings, "psmove_settings", "linear_velocity_exponent", 0.f);

			// General Settings
			m_bRumbleSuppressed = SettingsUtil::LoadBool(pSettings, "psmove_settings", "rumble_suppressed", m_bRumbleSuppressed);
			m_fVirtuallExtendControllersYMeters = SettingsUtil::LoadFloat(pSettings, "psmove_settings", "psmove_extend_y", 0.0f);
			m_fVirtuallExtendControllersZMeters = SettingsUtil::LoadFloat(pSettings, "psmove_settings", "psmove_extend_z", 0.0f);
			m_fVirtuallyRotateController = SettingsUtil::LoadBool(pSettings, "psmove_settings", "psmove_rotate", false);
			m_fControllerMetersInFrontOfHmdAtCalibration =
				SettingsUtil::LoadFloat(pSettings, "psmove", "m_fControllerMetersInFrontOfHmdAtCallibration", 0.06f);
			m_bDisableHMDAlignmentGesture = SettingsUtil::LoadBool(pSettings, "psmove_settings", "disable_alignment_gesture", false);
			m_bUseControllerOrientationInHMDAlignment = SettingsUtil::LoadBool(pSettings, "psmove_settings", "use_orientation_in_alignment", true);

			#if LOG_TOUCHPAD_EMULATION != 0
			Logger::DriverLog("use_spatial_offset_after_touchpad_press_as_touchpad_axis: %d\n", m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis);
			Logger::DriverLog("meters_per_touchpad_units: %f\n", m_fMetersPerTouchpadAxisUnits);
			#endif

			Logger::Logger::Info("m_fControllerMetersInFrontOfHmdAtCalibration(psmove): %f\n", m_fControllerMetersInFrontOfHmdAtCalibration);
		}

		m_trackingStatus = m_bDisableHMDAlignmentGesture ? vr::TrackingResult_Running_OK : vr::TrackingResult_Uninitialized;

	}

	PSMoveController::~PSMoveController() {
		PSM_FreeControllerListener(m_PSMServiceController->ControllerID);
		m_PSMServiceController = nullptr;

		if (m_orientationSolver != nullptr) {
			delete m_orientationSolver;
			m_orientationSolver = nullptr;
		}
	}

	void PSMoveController::LoadButtonMapping(
		vr::IVRSettings *pSettings,
		const ePSButtonID psButtonID,
		const vr::EVRButtonId defaultVRButtonID,
		const eVRTouchpadDirection defaultTouchpadDirection,
		int controllerId) {

		vr::EVRButtonId vrButtonID = defaultVRButtonID;
		eVRTouchpadDirection vrTouchpadDirection = defaultTouchpadDirection;

		if (pSettings != nullptr) {
			char remapButtonToButtonString[32];
			vr::EVRSettingsError fetchError;

			const char *szPSButtonName = "";
			const char *szButtonSectionName = "";
			const char *szTouchpadSectionName = "";
			szPSButtonName = k_PSButtonNames[psButtonID];
			szButtonSectionName = "psmove";
			szTouchpadSectionName = "psmove_touchpad_directions";

			pSettings->GetString(szButtonSectionName, szPSButtonName, remapButtonToButtonString, 32, &fetchError);

			if (fetchError == vr::VRSettingsError_None) {
				for (int vr_button_index = 0; vr_button_index < k_max_vr_buttons; ++vr_button_index) {
					if (strcasecmp(remapButtonToButtonString, k_VRButtonNames[vr_button_index]) == 0) {
						vrButtonID = static_cast<vr::EVRButtonId>(vr_button_index);
						break;
					}
				}
			}

			const char *numId = "";
			if (controllerId == 0) numId = "0";
			else if (controllerId == 1) numId = "1";
			else if (controllerId == 2) numId = "2";
			else if (controllerId == 3) numId = "3";
			else if (controllerId == 4) numId = "4";
			else if (controllerId == 5) numId = "5";
			else if (controllerId == 6) numId = "6";
			else if (controllerId == 7) numId = "7";
			else if (controllerId == 8) numId = "8";
			else if (controllerId == 9) numId = "9";

			if (strcmp(numId, "") != 0) {
				char buffer[64];
				strcpy(buffer, szButtonSectionName);
				strcat(buffer, "_");
				strcat(buffer, numId);
				szButtonSectionName = buffer;
				pSettings->GetString(szButtonSectionName, szPSButtonName, remapButtonToButtonString, 32, &fetchError);

				if (fetchError == vr::VRSettingsError_None) {
					for (int vr_button_index = 0; vr_button_index < k_max_vr_buttons; ++vr_button_index) {
						if (strcasecmp(remapButtonToButtonString, k_VRButtonNames[vr_button_index]) == 0) {
							vrButtonID = static_cast<vr::EVRButtonId>(vr_button_index);
							break;
						}
					}
				}
			}

			char remapButtonToTouchpadDirectionString[32];
			pSettings->GetString(szTouchpadSectionName, szPSButtonName, remapButtonToTouchpadDirectionString, 32, &fetchError);

			if (fetchError == vr::VRSettingsError_None) {
				for (int vr_touchpad_direction_index = 0; vr_touchpad_direction_index < k_max_vr_touchpad_directions; ++vr_touchpad_direction_index) {
					if (strcasecmp(remapButtonToTouchpadDirectionString, k_VRTouchpadDirectionNames[vr_touchpad_direction_index]) == 0) {
						vrTouchpadDirection = static_cast<eVRTouchpadDirection>(vr_touchpad_direction_index);
						break;
					}
				}
			}

			if (strcmp(numId, "") != 0) {
				char buffer[64];
				strcpy(buffer, szTouchpadSectionName);
				strcat(buffer, "_");
				strcat(buffer, numId);
				szTouchpadSectionName = buffer;
				pSettings->GetString(szTouchpadSectionName, szPSButtonName, remapButtonToTouchpadDirectionString, 32, &fetchError);

				if (fetchError == vr::VRSettingsError_None) {
					for (int vr_touchpad_direction_index = 0; vr_touchpad_direction_index < k_max_vr_touchpad_directions; ++vr_touchpad_direction_index) {
						if (strcasecmp(remapButtonToTouchpadDirectionString, k_VRTouchpadDirectionNames[vr_touchpad_direction_index]) == 0) {
							vrTouchpadDirection = static_cast<eVRTouchpadDirection>(vr_touchpad_direction_index);
							break;
						}
					}
				}
			}
		}

		// Save the mapping
		assert(psButtonID >= 0 && psButtonID < k_EPSButtonID_Count);
		psButtonIDToVRButtonID[psButtonID] = vrButtonID;
		psButtonIDToVrTouchpadDirection[psButtonID] = vrTouchpadDirection;
	}

	vr::EVRInitError PSMoveController::Activate(vr::TrackedDeviceIndex_t unObjectId) {
		vr::EVRInitError result = TrackableDevice::Activate(unObjectId);

		if (result == vr::VRInitError_None) {
			Logger::Info("CPSMoveControllerLatest::Activate - Controller %d Activated\n", unObjectId);

			g_ServerTrackedDeviceProvider.LaunchPSMoveMonitor();

			PSMRequestID requestId;
			if (PSM_StartControllerDataStreamAsync(
				m_PSMServiceController->ControllerID,
				PSMStreamFlags_includePositionData | PSMStreamFlags_includePhysicsData,
				&requestId) == PSMResult_Success) {
				PSM_RegisterCallback(requestId, PSMoveController::start_controller_response_callback, this);
			}

			// Setup controller properties
			{
				vr::CVRPropertyHelpers *properties = vr::VRProperties();

				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceOff_String, "{psmove}controller_status_off.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceSearching_String, "{psmove}controller_status_ready.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceSearchingAlert_String, "{psmove}controller_status_ready_alert.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceReady_String, "{psmove}controller_status_ready.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceReadyAlert_String, "{psmove}controller_status_ready_alert.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceNotReady_String, "{psmove}controller_status_error.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceStandby_String, "{psmove}controller_status_ready.png");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_NamedIconPathDeviceAlertLow_String, "{psmove}controller_status_ready_low.png");

				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_WillDriftInYaw_Bool, false);
				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceIsWireless_Bool, true);
				properties->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceProvidesBatteryStatus_Bool, m_PSMControllerType == PSMController_Move);

				properties->SetInt32Property(m_ulPropertyContainer, vr::Prop_DeviceClass_Int32, vr::TrackedDeviceClass_Controller);
				// We are reporting a "trackpad" type axis for better compatibility with Vive games
				properties->SetInt32Property(m_ulPropertyContainer, vr::Prop_Axis0Type_Int32, vr::k_eControllerAxis_TrackPad);
				properties->SetInt32Property(m_ulPropertyContainer, vr::Prop_Axis1Type_Int32, vr::k_eControllerAxis_Trigger);

				uint64_t ulRetVal = 0;
				for (int buttonIndex = 0; buttonIndex < static_cast<int>(k_EPSButtonID_Count); ++buttonIndex) {
					ulRetVal |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[buttonIndex]);

					if (psButtonIDToVrTouchpadDirection[buttonIndex] != k_EVRTouchpadDirection_None) {
						ulRetVal |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
					}
				}
				properties->SetUint64Property(m_ulPropertyContainer, vr::Prop_SupportedButtons_Uint64, ulRetVal);

				// The {psmove} syntax lets us refer to rendermodels that are installed
				// in the driver's own resources/rendermodels directory.  The driver can
				// still refer to SteamVR models like "generic_hmd".
				char model_label[32] = "\0";
				snprintf(model_label, sizeof(model_label), "psmove_%d", m_PSMServiceController->ControllerID);
				//properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "{psmove}psmove_controller");
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_ModeLabel_String, model_label);

				// Set device properties
				vr::VRProperties()->SetInt32Property(m_ulPropertyContainer, vr::Prop_ControllerRoleHint_Int32, m_TrackedControllerRole);
				vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_ManufacturerName_String, "HTC");

				// Fake Vive for motion controllers
				vr::VRProperties()->SetUint64Property(m_ulPropertyContainer, vr::Prop_HardwareRevision_Uint64, 1313);
				vr::VRProperties()->SetUint64Property(m_ulPropertyContainer, vr::Prop_FirmwareVersion_Uint64, 1315);
				vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_ModelNumber_String, "PS Move");
				vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_SerialNumber_String, m_strPSMControllerSerialNo.c_str());
				vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "vr_controller_vive_1_5");
				//vr::VRProperties()->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "{psmove}psmove_controller");
			}
		}

		return result;
	}

	void PSMoveController::start_controller_response_callback(
		const PSMResponseMessage *response, void *userdata) {
		PSMoveController *controller = reinterpret_cast<PSMoveController *>(userdata);

		if (response->result_code == PSMResult::PSMResult_Success) {
			Logger::Info("CPSMoveControllerLatest::start_controller_response_callback - Controller stream started\n");
		}
	}

	void PSMoveController::Deactivate() {
		Logger::Info("CPSMoveControllerLatest::Deactivate - Controller stream stopped\n");
		PSM_StopControllerDataStreamAsync(m_PSMServiceController->ControllerID, nullptr);
	}


	// Legacy IVRControllerComponent
	//void *CPSMoveControllerLatest::GetComponent(const char *pchComponentNameAndVersion)
	//{
	//    if (!strcasecmp(pchComponentNameAndVersion, vr::IVRControllerComponent_Version))
	//    {
	//        return (vr::IVRControllerComponent*)this;
	//    }
	//
	//    return NULL;
	//}
	// Legacy IVRControllerComponent
	//vr::VRControllerState_t CPSMoveControllerLatest::GetControllerState()
	//{
	//    return m_ControllerState;
	//}
	// Legacy IVRControllerComponent
	//bool CPSMoveControllerLatest::TriggerHapticPulse( uint32_t unAxisId, uint16_t usPulseDurationMicroseconds )
	//{
	//    m_pendingHapticPulseDuration = usPulseDurationMicroseconds;
	//    UpdateRumbleState();
	//
	//    return true;
	//}

	void PSMoveController::UpdateControllerState() {
		static const uint64_t s_kTouchpadButtonMask = vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);

		assert(m_PSMServiceController != nullptr);
		assert(m_PSMServiceController->IsConnected);

		const PSMPSMove &clientView = m_PSMServiceController->ControllerState.PSMoveState;

		bool bStartRealignHMDTriggered =
			(clientView.StartButton == PSMButtonState_PRESSED && clientView.SelectButton == PSMButtonState_PRESSED) ||
			(clientView.StartButton == PSMButtonState_PRESSED && clientView.SelectButton == PSMButtonState_DOWN) ||
			(clientView.StartButton == PSMButtonState_DOWN && clientView.SelectButton == PSMButtonState_PRESSED);

		// See if the recenter button has been held for the requisite amount of time
		bool bRecenterRequestTriggered = false;
		{
			PSMButtonState resetPoseButtonState = clientView.SelectButton;

			switch (resetPoseButtonState) {
				case PSMButtonState_PRESSED:
				{
					m_resetPoseButtonPressTime = std::chrono::high_resolution_clock::now();
				} break;
				case PSMButtonState_DOWN:
				{
					if (!m_bResetPoseRequestSent) {
						const float k_hold_duration_milli = 250.f;
						std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();
						std::chrono::duration<float, std::milli> pressDurationMilli = now - m_resetPoseButtonPressTime;

						if (pressDurationMilli.count() >= k_hold_duration_milli) {
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

		// If START was just pressed while and SELECT was held or vice versa,
		// recenter the controller orientation pose and start the realignment of the controller to HMD tracking space.
		if (bStartRealignHMDTriggered) {
			PSMVector3f controllerBallPointedUpEuler = { (float)M_PI_2, 0.0f, 0.0f };

			PSMQuatf controllerBallPointedUpQuat = PSM_QuatfCreateFromAngles(&controllerBallPointedUpEuler);

			Logger::Info("CPSMoveControllerLatest::UpdateControllerState(): Calling StartRealignHMDTrackingSpace() in response to controller chord.\n");

			PSM_ResetControllerOrientationAsync(m_PSMServiceController->ControllerID, &controllerBallPointedUpQuat, nullptr);
			m_bResetPoseRequestSent = true;

			RealignHMDTrackingSpace();
			m_bResetAlignRequestSent = true;
		} else if (bRecenterRequestTriggered) {
			Logger::Info("CPSMoveControllerLatest::UpdateControllerState(): Calling ClientPSMoveAPI::reset_orientation() in response to controller button press.\n");

			PSM_ResetControllerOrientationAsync(m_PSMServiceController->ControllerID, k_psm_quaternion_identity, nullptr);
			m_bResetPoseRequestSent = true;
		} else {

			// Process all the button mappings 
			// Check all PSMoveService controller button and update the controller state if it's changed.
			UpdateButtonState(k_EPSButtonID_Circle, clientView.CircleButton);
			UpdateButtonState(k_EPSButtonID_Cross, clientView.CrossButton);
			UpdateButtonState(k_EPSButtonID_Move, clientView.MoveButton);
			UpdateButtonState(k_EPSButtonID_PS, clientView.PSButton);
			UpdateButtonState(k_EPSButtonID_Select, clientView.SelectButton);
			UpdateButtonState(k_EPSButtonID_Square, clientView.SquareButton);
			UpdateButtonState(k_EPSButtonID_Start, clientView.StartButton);
			UpdateButtonState(k_EPSButtonID_Triangle, clientView.TriangleButton);
			UpdateButtonState(k_EPSButtonID_Trigger, clientView.TriggerButton);

			// Touchpad handling
			HandleTouchPadDirection();

			// PSMove Trigger handling
			float latestTriggerValue = clientView.TriggerValue / 255.f;
			HandleTrigger(latestTriggerValue);

			// Update the battery charge state
			UpdateBatteryChargeState(m_PSMServiceController->ControllerState.PSMoveState.BatteryValue);
		}

		// TODO rename?
		// Report trackable state to SteamVR Runtime
		TrackableDevice::Update();
	}

	// Updates this controllers trackable state for the given controller button id. The mappings should not be configured
	// via vr_settings.xml anymore as IVRDriverInput is aiming to offer a UI config interface for this in SteamVR Runtime, 
	// therefore they should be set in stone here.
	void PSMoveController::UpdateButtonState(ePSButtonID button, bool buttonState) {
		switch (button) {
			case k_EPSButtonID_PS:
				if (state.system.isPressed != buttonState)
					state.system.isPressed = buttonState;
				break;
			case k_EPSButtonID_Triangle:
				if (state.application.isPressed != buttonState)
					state.application.isPressed = buttonState;
				break;
			case k_EPSButtonID_Cross:
				if (state.grip.isPressed != buttonState)
					state.grip.isPressed = buttonState;
				break;
			case k_EPSButtonID_Trigger:
				if (state.trigger.isPressed != buttonState)
					state.trigger.isPressed = buttonState;
				if (state.trigger.isTouched != buttonState)
					state.trigger.isTouched = buttonState;
				break;
			case k_EPSButtonID_Square:
				if (state.guide.isPressed != buttonState)
					state.guide.isPressed = buttonState;
				break;
			case k_EPSButtonID_Circle:
				if (state.back.isPressed != buttonState)
					state.back.isPressed = buttonState;
				break;
			case k_EPSButtonID_Move:
				if (state.trackpad.isPressed != buttonState)
					state.trackpad.isPressed = buttonState;
				if (state.trackpad.isTouched != buttonState)
					state.trackpad.isTouched = buttonState;
				break;
		}
	}

	// TODO - migrate this to PS Navi Controller class.
	//void PSMoveController::UpdateTouchpadState(
	//	ePSButtonID buttonId,
	//	PSMButtonState buttonState) {

	//	/* Now check if the button pressed was a directional touchpad button. If it is was then fake a change
	//	in the touchpad axis. i.e. pretend a finger touched the touchpad in the given direction */
	//	if (buttonState == PSMButtonState_DOWN || buttonState == PSMButtonState_PRESSED) {
	//		if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_Left) {
	//			m_touchpadDirectionsUsed = true;
	//			state.trackpad.axis.x = -1.0f;
	//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_Right) {
	//			m_touchpadDirectionsUsed = true;
	//			state.trackpad.axis.x = 1.0f;
	//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_Up) {
	//			m_touchpadDirectionsUsed = true;
	//			state.trackpad.axis.y = -1.0f;
	//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_Down) {
	//			m_touchpadDirectionsUsed = true;
	//			state.trackpad.axis.y = 1.0f;
	//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_UpLeft) {
	//			m_touchpadDirectionsUsed = true;
	//			state.trackpad.axis.x = -0.707f;
	//			state.trackpad.axis.y = 0.707f;
	//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_UpRight) {
	//			m_touchpadDirectionsUsed = true;
	//			state.trackpad.axis.x = 0.707f;
	//			state.trackpad.axis.y = 0.707f;
	//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_DownLeft) {
	//			m_touchpadDirectionsUsed = true;
	//			state.trackpad.axis.x = -0.707f;
	//			state.trackpad.axis.y = -0.707f;
	//		} else if (psButtonIDToVrTouchpadDirection[buttonId] == k_EVRTouchpadDirection_DownRight) {
	//			m_touchpadDirectionsUsed = true;
	//			state.trackpad.axis.x = 0.707f;
	//			state.trackpad.axis.y = -0.707f;
	//		}
	//	}

	//	if (buttonState == PSMButtonState_DOWN || buttonState == PSMButtonState_PRESSED) {
	//		m_touchpadDirectionsUsed = true;
	//		state.trackpad.axis.x = 0.f;
	//		state.trackpad.axis.y = 0.f;
	//	}
	//}

	void PSMoveController::RealignHMDTrackingSpace() {
		if (m_bDisableHMDAlignmentGesture) {
			Logger::Info("Ignoring RealignHMDTrackingSpace request. Disabled.\n");
			return;
		}

		Logger::Info("Begin CPSMoveControllerLatest::RealignHMDTrackingSpace()\n");

		vr::TrackedDeviceIndex_t hmd_device_index = vr::k_unTrackedDeviceIndexInvalid;
		if (Utils::GetHMDDeviceIndex(&hmd_device_index)) {
			Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - HMD Device Index= %u\n", hmd_device_index);
		} else {
			Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - Failed to get HMD Device Index\n");
			return;
		}

		PSMPosef hmd_pose_meters;
		if (Utils::GetTrackedDevicePose(hmd_device_index, &hmd_pose_meters)) {
			Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - hmd_pose_meters: %s \n", Utils::PSMPosefToString(hmd_pose_meters).c_str());
		} else {
			Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - Failed to get HMD Pose\n");
			return;
		}

		// Make the HMD orientation only contain a yaw
		hmd_pose_meters.Orientation = Utils::ExtractHMDYawQuaternion(hmd_pose_meters.Orientation);
		Logger::Info("hmd_pose_meters(yaw-only): %s \n", Utils::PSMPosefToString(hmd_pose_meters).c_str());

		// We have the transform of the HMD in world space. 
		// However the HMD and the controller aren't quite aligned depending on the controller type:
		PSMQuatf controllerOrientationInHmdSpaceQuat = *k_psm_quaternion_identity;
		PSMVector3f controllerLocalOffsetFromHmdPosition = *k_psm_float_vector3_zero;
		if (m_PSMControllerType == PSMControllerType::PSMController_Move) {
			// Rotation) The controller's local -Z axis (from the center to the glowing ball) is currently pointed 
			//    in the direction of the HMD's local +Y axis, 
			// Translation) The controller's position is a few inches ahead of the HMD's on the HMD's local -Z axis. 
			PSMVector3f eulerPitch = { (float)M_PI_2, 0.0f, 0.0f };
			controllerOrientationInHmdSpaceQuat = PSM_QuatfCreateFromAngles(&eulerPitch);
			controllerLocalOffsetFromHmdPosition = { 0.0f, 0.0f, -1.0f * m_fControllerMetersInFrontOfHmdAtCalibration };
		}

		// Transform the HMD's world space transform to where we expect the controller's world space transform to be.
		PSMPosef controllerPoseRelativeToHMD =
			PSM_PosefCreate(&controllerLocalOffsetFromHmdPosition, &controllerOrientationInHmdSpaceQuat);

		Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controllerPoseRelativeToHMD: %s \n", Utils::PSMPosefToString(controllerPoseRelativeToHMD).c_str());

		// Compute the expected controller pose in HMD tracking space (i.e. "World Space")
		PSMPosef controller_world_space_pose = PSM_PosefConcat(&controllerPoseRelativeToHMD, &hmd_pose_meters);
		Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_world_space_pose: %s \n", Utils::PSMPosefToString(controller_world_space_pose).c_str());

		/*
		We now have the transform of the controller in world space -- controller_world_space_pose

		We also have the transform of the controller in driver space -- psmove_pose_meters

		We need the transform that goes from driver space to world space -- driver_pose_to_world_pose
		psmove_pose_meters * driver_pose_to_world_pose = controller_world_space_pose
		psmove_pose_meters.inverse() * psmove_pose_meters * driver_pose_to_world_pose = psmove_pose_meters.inverse() * controller_world_space_pose
		driver_pose_to_world_pose = psmove_pose_meters.inverse() * controller_world_space_pose
		*/

		// Get the current pose from the controller view instead of using the driver's cached
		// value because the user may have triggered a pose reset, in which case the driver's
		// cached pose might not yet be up to date by the time this callback is triggered.
		PSMPosef controller_pose_meters = *k_psm_pose_identity;
		PSM_GetControllerPose(m_PSMServiceController->ControllerID, &controller_pose_meters);
		Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_meters(raw): %s \n", Utils::PSMPosefToString(controller_pose_meters).c_str());

		// PSMove Position is in cm, but OpenVR stores position in meters
		controller_pose_meters.Position = PSM_Vector3fScale(&controller_pose_meters.Position, k_fScalePSMoveAPIToMeters);

		if (m_bUseControllerOrientationInHMDAlignment) {
			// Extract only the yaw from the controller orientation (assume it's mostly held upright)
			controller_pose_meters.Orientation = Utils::ExtractPSMoveYawQuaternion(controller_pose_meters.Orientation);
			Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_meters(yaw-only): %s \n", Utils::PSMPosefToString(controller_pose_meters).c_str());
		} else {
			const PSMVector3f eulerPitch = { (float)M_PI_2, 0.0f, 0.0f };

			controller_pose_meters.Orientation = PSM_QuatfCreateFromAngles(&eulerPitch);
			Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_meters(no-rotation): %s \n", Utils::PSMPosefToString(controller_pose_meters).c_str());
		}

		PSMPosef controller_pose_inv = PSM_PosefInverse(&controller_pose_meters);
		Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_inv: %s \n", Utils::PSMPosefToString(controller_pose_inv).c_str());

		PSMPosef driver_pose_to_world_pose = PSM_PosefConcat(&controller_pose_inv, &controller_world_space_pose);
		Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - driver_pose_to_world_pose: %s \n", Utils::PSMPosefToString(driver_pose_to_world_pose).c_str());

		PSMPosef test_composed_controller_world_space = PSM_PosefConcat(&controller_pose_meters, &driver_pose_to_world_pose);
		Logger::Info("CPSMoveControllerLatest::RealignHMDTrackingSpace() - test_composed_controller_world_space: %s \n", Utils::PSMPosefToString(test_composed_controller_world_space).c_str());

		g_ServerTrackedDeviceProvider.SetHMDTrackingSpace(driver_pose_to_world_pose);
	}

	// TODO - release of the Move button isn't stopping the TouchpadIsActive
	void PSMoveController::HandleTouchPadDirection() {
		// Virtual TouchPad Handling (i.e. controller spatial offset as touchpad)
		if (m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis) {
			bool bTouchpadIsActive = state.trackpad.isPressed || state.trackpad.isTouched;

			if (bTouchpadIsActive) {
				bool bIsNewTouchpadLocation = true;

				if (m_bDelayAfterTouchpadPress) {
					std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();

					if (!m_bTouchpadWasActive) {
						const float k_max_touchpad_press = 2000.0; // time until coordinates are reset, otherwise assume in last location.
						std::chrono::duration<double, std::milli> timeSinceActivated = now - m_lastTouchpadPressTime;

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
						Logger::DriverLog("Touchpad pressed! At (%f, %f, %f) meters relative to orientation\n",
										  m_posMetersAtTouchpadPressTime.x, m_posMetersAtTouchpadPressTime.y, m_posMetersAtTouchpadPressTime.z);
						#endif
					} else {
						// Held!
						PSMVector3f newPosMeters;
						Utils::GetMetersPosInRotSpace(&m_driverSpaceRotationAtTouchpadPressTime, &newPosMeters, m_PSMServiceController->ControllerState.PSMoveState);

						PSMVector3f offsetMeters = PSM_Vector3fSubtract(&newPosMeters, &m_posMetersAtTouchpadPressTime);

						#if LOG_TOUCHPAD_EMULATION != 0
						Logger::DriverLog("Touchpad held! Relative position (%f, %f, %f) meters\n",
										  offsetMeters.x, offsetMeters.y, offsetMeters.z);
						#endif

						state.trackpad.axis.x = offsetMeters.x / m_fMetersPerTouchpadAxisUnits;
						state.trackpad.axis.x = fminf(fmaxf(state.trackpad.axis.x, -1.0f), 1.0f);

						state.trackpad.axis.y = -offsetMeters.z / m_fMetersPerTouchpadAxisUnits;
						state.trackpad.axis.y = fminf(fmaxf(state.trackpad.axis.y, -1.0f), 1.0f);

						#if LOG_TOUCHPAD_EMULATION != 0
						Logger::DriverLog("Touchpad axis at (%f, %f) \n",
										  state.trackpad.axis.x, state.trackpad.axis.y);
						#endif
					}
				}
				// Remember if the touchpad was active the previous frame for edge detection
				m_bTouchpadWasActive = bTouchpadIsActive;
			} else { // trackpad is no longer active to reset the trackpad axis
				state.trackpad.axis.x = 0.f;
				state.trackpad.axis.y = 0.f;
			}

		}
	}

	void PSMoveController::HandleTrigger(float latestTriggerValue) {

		/* Check if the Trigger value has changed, this value represents how far the trigger has been physically pressed.
		It's considered touched at any value greater than 0.1 but it's only considered touched/pressed when it's nearly
		completely pressed.
		*/
		if (state.trigger.value != latestTriggerValue) {
			if (latestTriggerValue > 0.1f) {
				state.trigger.isTouched = true;
			}

			// Send the button was pressed event only when it's almost fully pressed
			if (latestTriggerValue > 0.8f) {
				state.trigger.isPressed = true;
			}

			state.trigger.value = latestTriggerValue;
		}
	}

	void PSMoveController::UpdateTrackingState() {
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
		{
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
		}

		m_Pose.poseIsValid =
			m_PSMServiceController->ControllerState.PSMoveState.bIsPositionValid &&
			m_PSMServiceController->ControllerState.PSMoveState.bIsOrientationValid;

		// This call posts this pose to shared memory, where all clients will have access to it the next
		// moment they want to predict a pose.
		vr::VRServerDriverHost()->TrackedDevicePoseUpdated(m_unSteamVRTrackedDeviceId, m_Pose, sizeof(vr::DriverPose_t));
	}

	void PSMoveController::setPendingPulseDurationSecs(float durationSecs) {
		m_pendingHapticPulseDurationSecs = durationSecs;
	}

	// TODO - Make use of amplitude and frequency for Buffered Haptics, will give us patterning and panning vibration.
	// See: https://developer.oculus.com/documentation/pcsdk/latest/concepts/dg-input-touch-haptic/
	void PSMoveController::UpdateRumbleState() {
		if (!m_bRumbleSuppressed) {

			// convert to microseconds
			uint16_t pendingHapticPulseDurationMicroSecs = static_cast<uint16_t>(m_pendingHapticPulseDurationSecs * 1000000);

			const float k_max_rumble_update_rate = 33.f; // Don't bother trying to update the rumble faster than 30fps (33ms)
			const float k_max_pulse_microseconds = 1000.f; // Docs suggest max pulse duration of 5ms, but we'll call 1ms max

			std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();
			bool bTimoutElapsed = true;

			if (m_lastTimeRumbleSentValid) {
				std::chrono::duration<double, std::milli> timeSinceLastSend = now - m_lastTimeRumbleSent;

				bTimoutElapsed = timeSinceLastSend.count() >= k_max_rumble_update_rate;
			}

			// See if a rumble request hasn't come too recently
			if (bTimoutElapsed) {
				float rumble_fraction = static_cast<float>(pendingHapticPulseDurationMicroSecs) / k_max_pulse_microseconds;

				if (rumble_fraction > 0)
					steamvrbridge::Logger::Debug("PSMoveController::UpdateRumble: m_pendingHapticPulseDurationSecs=%f ,pendingHapticPulseDurationMicroSecs=%d, rumble_fraction=%f\n", m_pendingHapticPulseDurationSecs, pendingHapticPulseDurationMicroSecs, rumble_fraction);

				// Unless a zero rumble intensity was explicitly set, 
				// don't rumble less than 35% (no enough to feel)
				if (m_pendingHapticPulseDurationSecs != 0) {
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
				PSM_SetControllerRumble(m_PSMServiceController->ControllerID, PSMControllerRumbleChannel_All, rumble_fraction);

				// Remember the last rumble we went and when we sent it
				m_lastTimeRumbleSent = now;
				m_lastTimeRumbleSentValid = true;

				// Reset the pending haptic pulse duration.
				// If another call to TriggerHapticPulse() is made later, it will stomp this value.
				// If no future haptic event is received by ServerDriver then the next call to UpdateRumbleState()
				// in k_max_rumble_update_rate milliseconds will set the rumble_fraction to 0.f
				// This effectively makes the shortest rumble pulse k_max_rumble_update_rate milliseconds.
				m_pendingHapticPulseDurationSecs = 0;
			}
		} else {
			// Reset the pending haptic pulse duration since rumble is suppressed.
			m_pendingHapticPulseDurationSecs = 0;
		}
	}

	void PSMoveController::UpdateBatteryChargeState(
		PSMBatteryState newBatteryEnum) {
		bool bIsBatteryCharging = false;
		float fBatteryChargeFraction = 0.f;

		switch (newBatteryEnum) {
			case PSMBattery_0:
				bIsBatteryCharging = false;
				fBatteryChargeFraction = 0.f;
				break;
			case PSMBattery_20:
				bIsBatteryCharging = false;
				fBatteryChargeFraction = 0.2f;
				break;
			case PSMBattery_40:
				bIsBatteryCharging = false;
				fBatteryChargeFraction = 0.4f;
				break;
			case PSMBattery_60:
				bIsBatteryCharging = false;
				fBatteryChargeFraction = 0.6f;
				break;
			case PSMBattery_80:
				bIsBatteryCharging = false;
				fBatteryChargeFraction = 0.8f;
				break;
			case PSMBattery_100:
				bIsBatteryCharging = false;
				fBatteryChargeFraction = 1.f;
				break;
			case PSMBattery_Charging:
				bIsBatteryCharging = true;
				fBatteryChargeFraction = 0.99f; // Don't really know the charge amount in this case
				break;
			case PSMBattery_Charged:
				bIsBatteryCharging = true;
				fBatteryChargeFraction = 1.f;
				break;
		}

		if (bIsBatteryCharging != m_bIsBatteryCharging) {
			m_bIsBatteryCharging = bIsBatteryCharging;
			vr::VRProperties()->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceIsCharging_Bool, m_bIsBatteryCharging);
		}

		if (fBatteryChargeFraction != m_fBatteryChargeFraction) {
			m_fBatteryChargeFraction = fBatteryChargeFraction;
			vr::VRProperties()->SetFloatProperty(m_ulPropertyContainer, vr::Prop_DeviceBatteryPercentage_Float, m_fBatteryChargeFraction);
		}
	}

	void PSMoveController::Update() {
		if (IsActivated() && m_PSMServiceController->IsConnected) {
			int seq_num = m_PSMServiceController->OutputSequenceNum;

			// Only other updating incoming state if it actually changed and is due for one
			if (m_nPoseSequenceNumber < seq_num) {
				m_nPoseSequenceNumber = seq_num;

				UpdateTrackingState();
				UpdateControllerState();
				TrackableDevice::Update();
			}

			// Update the outgoing state
			UpdateRumbleState();
		}
	}

	void PSMoveController::RefreshWorldFromDriverPose() {
		TrackableDevice::RefreshWorldFromDriverPose();

		// Mark the calibration process as done once we have setup the world from driver pose
		m_trackingStatus = vr::TrackingResult_Running_OK;
	}
}
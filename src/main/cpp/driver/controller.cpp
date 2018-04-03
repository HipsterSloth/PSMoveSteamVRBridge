#define _USE_MATH_DEFINES

#include "controller.h"
#include "constants.h"
#include "serverdriver.h"
#include "utils.h"
#include <assert.h>
#include "driver.h"
#include "facinghandsolver.h"

#if _MSC_VER
#define strcasecmp(a, b) stricmp(a,b)
#pragma warning (disable: 4996) // 'This function or variable may be unsafe': snprintf
#define snprintf _snprintf
#endif

namespace steamvrbridge {

	//==================================================================================================
	// Controller Driver
	//==================================================================================================

	CPSMoveControllerLatest::CPSMoveControllerLatest(
		PSMControllerID psmControllerId,
		PSMControllerType psmControllerType,
		const char *psmSerialNo)
		: CPSMoveTrackedDeviceLatest()
		, m_nPSMControllerId(psmControllerId)
		, m_PSMControllerType(psmControllerType)
		, m_PSMControllerView(nullptr)
		, m_nPSMChildControllerId(-1)
		, m_PSMChildControllerType(PSMControllerType::PSMController_None)
		, m_PSMChildControllerView(nullptr)
		, m_nPoseSequenceNumber(0)
		, m_bIsBatteryCharging(false)
		, m_fBatteryChargeFraction(0.f)
		, m_bRumbleSuppressed(false)
		, m_pendingHapticPulseDuration(0)
		, m_lastTimeRumbleSent()
		, m_lastTimeRumbleSentValid(false)
		, m_resetPoseButtonPressTime()
		, m_bResetPoseRequestSent(false)
		, m_resetAlignButtonPressTime()
		, m_bResetAlignRequestSent(false)
		, m_bUsePSNaviDPadRecenter(false)
		, m_bUsePSNaviDPadRealign(false)
		, m_fVirtuallExtendControllersZMeters(0.0f)
		, m_fVirtuallExtendControllersYMeters(0.0f)
		, m_fVirtuallyRotateController(false)
		, m_bDelayAfterTouchpadPress(false)
		, m_bTouchpadWasActive(false)
		, m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis(false)
		, m_touchpadDirectionsUsed(false)
		, m_fControllerMetersInFrontOfHmdAtCalibration(0.f)
		, m_posMetersAtTouchpadPressTime(*k_psm_float_vector3_zero)
		, m_driverSpaceRotationAtTouchpadPressTime(*k_psm_quaternion_identity)
		, m_bDisableHMDAlignmentGesture(false)
		, m_bUseControllerOrientationInHMDAlignment(false)
		, m_steamVRTriggerAxisIndex(1)
		, m_steamVRNaviTriggerAxisIndex(1)
		, m_virtualTriggerAxisIndex(-1)
		, m_virtualTouchpadXAxisIndex(-1)
		, m_virtualTouchpadYAxisIndex(-1)
		, m_thumbstickDeadzone(k_defaultThumbstickDeadZoneRadius)
		, m_bThumbstickTouchAsPress(true)
		, m_fLinearVelocityMultiplier(1.f)
		, m_fLinearVelocityExponent(0.f)
		, m_hmdAlignPSButtonID(k_EPSButtonID_Select)
		, m_overrideModel("")
		, m_orientationSolver(nullptr)
	{
		char svrIdentifier[256];
		Utils::GenerateControllerSteamVRIdentifier(svrIdentifier, sizeof(svrIdentifier), psmControllerId);
		m_strSteamVRSerialNo = svrIdentifier;

		m_lastTouchpadPressTime = std::chrono::high_resolution_clock::now();

		if (psmSerialNo != NULL) {
			m_strPSMControllerSerialNo = psmSerialNo;
		}

		// Tell PSM Client API that we are listening to this controller id
		PSM_AllocateControllerListener(psmControllerId);
		m_PSMControllerView = PSM_GetController(psmControllerId);

		// Load config from steamvr.vrsettings
		vr::IVRSettings *pSettings = vr::VRSettings();

		// Map every button to the system button initially
		memset(psButtonIDToVRButtonID, vr::k_EButton_SteamVR_Trigger, k_EPSControllerType_Count*k_EPSButtonID_Count * sizeof(vr::EVRButtonId));

		// Map every button to not be associated with any touchpad direction, initially
		memset(psButtonIDToVrTouchpadDirection, k_EVRTouchpadDirection_None, k_EPSControllerType_Count*k_EPSButtonID_Count * sizeof(vr::EVRButtonId));

		if (pSettings != nullptr)
		{
			// Load PSMove button/touchpad remapping from the settings for all possible controller buttons
			if (psmControllerType == PSMController_Move)
			{
				// Parent controller button mappings

				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_PS, vr::k_EButton_System, k_EVRTouchpadDirection_None, psmControllerId);
				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_Move, vr::k_EButton_SteamVR_Touchpad, k_EVRTouchpadDirection_None, psmControllerId);
				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_Trigger, vr::k_EButton_SteamVR_Trigger, k_EVRTouchpadDirection_None, psmControllerId);
				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_Triangle, (vr::EVRButtonId)8, k_EVRTouchpadDirection_None, psmControllerId);
				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_Square, (vr::EVRButtonId)9, k_EVRTouchpadDirection_None, psmControllerId);
				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_Circle, (vr::EVRButtonId)10, k_EVRTouchpadDirection_None, psmControllerId);
				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_Cross, (vr::EVRButtonId)11, k_EVRTouchpadDirection_None, psmControllerId);
				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_Select, vr::k_EButton_Grip, k_EVRTouchpadDirection_None, psmControllerId);
				LoadButtonMapping(pSettings, k_EPSControllerType_Move, k_EPSButtonID_Start, vr::k_EButton_ApplicationMenu, k_EVRTouchpadDirection_None, psmControllerId);

				// Attached child controller button mappings
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_PS, vr::k_EButton_System, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_Left, vr::k_EButton_DPad_Left, k_EVRTouchpadDirection_Left);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_Up, (vr::EVRButtonId)10, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_Right, vr::k_EButton_DPad_Right, k_EVRTouchpadDirection_Right);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_Down, (vr::EVRButtonId)10, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_Move, vr::k_EButton_SteamVR_Touchpad, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_Circle, (vr::EVRButtonId)10, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_Cross, (vr::EVRButtonId)11, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_L1, vr::k_EButton_SteamVR_Trigger, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_L2, vr::k_EButton_SteamVR_Trigger, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_Navi, k_EPSButtonID_L3, vr::k_EButton_Grip, k_EVRTouchpadDirection_None);

				// Trigger mapping
				m_steamVRTriggerAxisIndex = LoadInt(pSettings, "psmove", "trigger_axis_index", 1);
				m_steamVRNaviTriggerAxisIndex = LoadInt(pSettings, "psnavi_button", "trigger_axis_index", m_steamVRTriggerAxisIndex);

				// Touch pad settings
				m_bDelayAfterTouchpadPress =
					LoadBool(pSettings, "psmove_touchpad", "delay_after_touchpad_press", m_bDelayAfterTouchpadPress);
				m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis =
					LoadBool(pSettings, "psmove", "use_spatial_offset_after_touchpad_press_as_touchpad_axis", false);
				m_fMetersPerTouchpadAxisUnits =
					LoadFloat(pSettings, "psmove", "meters_per_touchpad_units", .075f);

				// Throwing power settings
				m_fLinearVelocityMultiplier =
					LoadFloat(pSettings, "psmove_settings", "linear_velocity_multiplier", 1.f);
				m_fLinearVelocityExponent =
					LoadFloat(pSettings, "psmove_settings", "linear_velocity_exponent", 0.f);

				// Check for PSNavi up/down mappings
				char remapButtonToButtonString[32];
				vr::EVRSettingsError fetchError;

				pSettings->GetString("psnavi_button", k_PSButtonNames[k_EPSButtonID_Up], remapButtonToButtonString, 32, &fetchError);
				if (fetchError != vr::VRSettingsError_None)
				{
					pSettings->GetString("psnavi_touchpad", k_PSButtonNames[k_EPSButtonID_Up], remapButtonToButtonString, 32, &fetchError);
					if (fetchError != vr::VRSettingsError_None)
					{
						m_bUsePSNaviDPadRealign = true;
					}
				}

				pSettings->GetString("psnavi_button", k_PSButtonNames[k_EPSButtonID_Down], remapButtonToButtonString, 32, &fetchError);
				if (fetchError != vr::VRSettingsError_None)
				{
					pSettings->GetString("psnavi_touchpad", k_PSButtonNames[k_EPSButtonID_Down], remapButtonToButtonString, 32, &fetchError);
					if (fetchError != vr::VRSettingsError_None)
					{
						m_bUsePSNaviDPadRecenter = true;
					}
				}

				// General Settings
				m_bRumbleSuppressed = LoadBool(pSettings, "psmove_settings", "rumble_suppressed", m_bRumbleSuppressed);
				m_fVirtuallExtendControllersYMeters = LoadFloat(pSettings, "psmove_settings", "psmove_extend_y", 0.0f);
				m_fVirtuallExtendControllersZMeters = LoadFloat(pSettings, "psmove_settings", "psmove_extend_z", 0.0f);
				m_fVirtuallyRotateController = LoadBool(pSettings, "psmove_settings", "psmove_rotate", false);
				m_fControllerMetersInFrontOfHmdAtCalibration =
					LoadFloat(pSettings, "psmove", "m_fControllerMetersInFrontOfHmdAtCallibration", 0.06f);
				m_bDisableHMDAlignmentGesture = LoadBool(pSettings, "psmove_settings", "disable_alignment_gesture", false);
				m_bUseControllerOrientationInHMDAlignment = LoadBool(pSettings, "psmove_settings", "use_orientation_in_alignment", true);

				m_thumbstickDeadzone =
					fminf(fmaxf(LoadFloat(pSettings, "psnavi_settings", "thumbstick_deadzone_radius", k_defaultThumbstickDeadZoneRadius), 0.f), 0.99f);
				m_bThumbstickTouchAsPress = LoadBool(pSettings, "psnavi_settings", "thumbstick_touch_as_press", true);

#if LOG_TOUCHPAD_EMULATION != 0
				Logger::DriverLog("use_spatial_offset_after_touchpad_press_as_touchpad_axis: %d\n", m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis);
				Logger::DriverLog("meters_per_touchpad_units: %f\n", m_fMetersPerTouchpadAxisUnits);
#endif

				Logger::Logger::DriverLog("m_fControllerMetersInFrontOfHmdAtCalibration(psmove): %f\n", m_fControllerMetersInFrontOfHmdAtCalibration);
			}
			else if (psmControllerType == PSMController_DualShock4)
			{
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_PS, vr::k_EButton_System, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Left, vr::k_EButton_DPad_Left, k_EVRTouchpadDirection_Left);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Up, vr::k_EButton_DPad_Up, k_EVRTouchpadDirection_Up);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Right, vr::k_EButton_DPad_Right, k_EVRTouchpadDirection_Right);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Down, vr::k_EButton_DPad_Down, k_EVRTouchpadDirection_Down);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Trackpad, vr::k_EButton_SteamVR_Touchpad, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Triangle, (vr::EVRButtonId)8, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Square, (vr::EVRButtonId)9, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Circle, (vr::EVRButtonId)10, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Cross, (vr::EVRButtonId)11, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Share, vr::k_EButton_ApplicationMenu, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_Options, vr::k_EButton_ApplicationMenu, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_L1, vr::k_EButton_SteamVR_Trigger, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_L2, vr::k_EButton_SteamVR_Trigger, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_L3, vr::k_EButton_Grip, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_R1, vr::k_EButton_SteamVR_Trigger, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_R2, vr::k_EButton_SteamVR_Trigger, k_EVRTouchpadDirection_None);
				LoadButtonMapping(pSettings, k_EPSControllerType_DS4, k_EPSButtonID_R3, vr::k_EButton_Grip, k_EVRTouchpadDirection_None);

				// General Settings
				m_bRumbleSuppressed = LoadBool(pSettings, "dualshock4_settings", "rumble_suppressed", m_bRumbleSuppressed);
				m_bDisableHMDAlignmentGesture = LoadBool(pSettings, "dualshock4_settings", "disable_alignment_gesture", false);
				m_fControllerMetersInFrontOfHmdAtCalibration =
					LoadFloat(pSettings, "dualshock4_settings", "cm_in_front_of_hmd_at_calibration", 16.f) / 100.f;

				Logger::DriverLog("m_fControllerMetersInFrontOfHmdAtCalibration(ds4): %f\n", m_fControllerMetersInFrontOfHmdAtCalibration);
			}
			else if (psmControllerType == PSMController_Virtual)
			{
				// Controller button mappings
				for (int button_index = 0; button_index < k_EPSButtonID_Count; ++button_index)
				{
					LoadButtonMapping(
						pSettings,
						k_EPSControllerType_Virtual,
						(CPSMoveControllerLatest::ePSButtonID)button_index,
						(vr::EVRButtonId)button_index,
						k_EVRTouchpadDirection_None,
						psmControllerId);
				}

				// Axis mapping
				m_virtualTriggerAxisIndex = LoadInt(pSettings, "virtual_axis", "trigger_axis_index", -1);
				m_virtualTouchpadXAxisIndex = LoadInt(pSettings, "virtual_axis", "touchpad_x_axis_index", -1);
				m_virtualTouchpadYAxisIndex = LoadInt(pSettings, "virtual_axis", "touchpad_y_axis_index", -1);

				// HMD align button mapping
				{
					char alignButtonString[32];
					vr::EVRSettingsError fetchError;

					m_hmdAlignPSButtonID = k_EPSButtonID_0;
					pSettings->GetString("virtual_controller", "hmd_align_button", alignButtonString, 32, &fetchError);

					if (fetchError == vr::VRSettingsError_None)
					{
						int button_index = Utils::find_index_of_string_in_table(k_VirtualButtonNames, CPSMoveControllerLatest::k_EPSButtonID_Count, alignButtonString);
						if (button_index != -1)
						{
							m_hmdAlignPSButtonID = static_cast<CPSMoveControllerLatest::ePSButtonID>(button_index);
						}
						else
						{
							Logger::DriverLog("Invalid virtual controller hmd align button: %s\n", alignButtonString);
						}
					}
				}

				// Get the controller override model to use, if any
				{
					char modelString[64];
					vr::EVRSettingsError fetchError;

					pSettings->GetString("virtual_controller", "override_model", modelString, 64, &fetchError);
					if (fetchError == vr::VRSettingsError_None)
					{
						m_overrideModel = modelString;
					}
				}

				// Touch pad settings
				m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis =
					LoadBool(pSettings, "virtual_controller", "use_spatial_offset_after_touchpad_press_as_touchpad_axis", false);
				m_fMetersPerTouchpadAxisUnits =
					LoadFloat(pSettings, "virtual_controller", "meters_per_touchpad_units", .075f);

				// Throwing power settings
				m_fLinearVelocityMultiplier =
					LoadFloat(pSettings, "virtual_controller_settings", "linear_velocity_multiplier", 1.f);
				m_fLinearVelocityExponent =
					LoadFloat(pSettings, "virtual_controller_settings", "linear_velocity_exponent", 0.f);

				// General Settings
				m_bDisableHMDAlignmentGesture = LoadBool(pSettings, "virtual_controller_settings", "disable_alignment_gesture", false);
				m_fVirtuallExtendControllersYMeters = LoadFloat(pSettings, "virtual_controller_settings", "psmove_extend_y", 0.0f);
				m_fVirtuallExtendControllersZMeters = LoadFloat(pSettings, "virtual_controller_settings", "psmove_extend_z", 0.0f);
				m_fControllerMetersInFrontOfHmdAtCalibration =
					LoadFloat(pSettings, "virtual_controller_settings", "m_fControllerMetersInFrontOfHmdAtCallibration", 0.06f);

				m_thumbstickDeadzone =
					fminf(fmaxf(LoadFloat(pSettings, "virtual_controller_settings", "thumbstick_deadzone_radius", k_defaultThumbstickDeadZoneRadius), 0.f), 0.99f);
				m_bThumbstickTouchAsPress = LoadBool(pSettings, "virtual_controller_settings", "thumbstick_touch_as_press", true);

				// IK solver
				if (LoadBool(pSettings, "virtual_controller_ik", "enable_ik", false))
				{
					char handString[16];
					vr::EVRSettingsError fetchError;
					vr::ETrackedControllerRole hand;

					if ((int)psmControllerId % 2 == 0)
					{
						hand = vr::TrackedControllerRole_RightHand;

						pSettings->GetString("virtual_controller_ik", "first_hand", handString, 16, &fetchError);
						if (fetchError == vr::VRSettingsError_None)
						{
							if (strcasecmp(handString, "left") == 0)
							{
								hand = vr::TrackedControllerRole_LeftHand;
							}
						}
					}
					else
					{
						hand = vr::TrackedControllerRole_LeftHand;

						pSettings->GetString("virtual_controller_ik", "second_hand", handString, 16, &fetchError);
						if (fetchError == vr::VRSettingsError_None)
						{
							if (strcasecmp(handString, "right") == 0)
							{
								hand = vr::TrackedControllerRole_RightHand;
							}
						}
					}

					float neckLength = LoadFloat(pSettings, "virtual_controller_ik", "neck_length", 0.2f); // meters
					float halfShoulderLength = LoadFloat(pSettings, "virtual_controller_ik", "half_shoulder_length", 0.22f); // meters
					float upperArmLength = LoadFloat(pSettings, "virtual_controller_ik", "upper_arm_length", 0.3f); // meters
					float lowerArmLength = LoadFloat(pSettings, "virtual_controller_ik", "lower_arm_length", 0.35f); // meters

					//TODO: Select solver method
					//m_orientationSolver = new CFABRIKArmSolver(hand, neckLength, halfShoulderLength, upperArmLength, lowerArmLength);
					//m_orientationSolver = new CRadialHandOrientationSolver(hand, neckLength, halfShoulderLength);
					m_orientationSolver = new CFacingHandOrientationSolver;
				}
				else
				{
					m_orientationSolver = new CFacingHandOrientationSolver;
				}
			}
		}

		memset(&m_ControllerState, 0, sizeof(vr::VRControllerState_t));
		m_trackingStatus = m_bDisableHMDAlignmentGesture ? vr::TrackingResult_Running_OK : vr::TrackingResult_Uninitialized;
	}

	CPSMoveControllerLatest::~CPSMoveControllerLatest()
	{
		if (m_PSMChildControllerView != nullptr)
		{
			PSM_FreeControllerListener(m_PSMChildControllerView->ControllerID);
			m_PSMChildControllerView = nullptr;
		}

		PSM_FreeControllerListener(m_PSMControllerView->ControllerID);
		m_PSMControllerView = nullptr;

		if (m_orientationSolver != nullptr)
		{
			delete m_orientationSolver;
			m_orientationSolver = nullptr;
		}
	}

	void CPSMoveControllerLatest::LoadButtonMapping(
		vr::IVRSettings *pSettings,
		const CPSMoveControllerLatest::ePSControllerType controllerType,
		const CPSMoveControllerLatest::ePSButtonID psButtonID,
		const vr::EVRButtonId defaultVRButtonID,
		const eVRTouchpadDirection defaultTouchpadDirection,
		int controllerId)
	{

		vr::EVRButtonId vrButtonID = defaultVRButtonID;
		eVRTouchpadDirection vrTouchpadDirection = defaultTouchpadDirection;

		if (pSettings != nullptr)
		{
			char remapButtonToButtonString[32];
			vr::EVRSettingsError fetchError;

			const char *szPSButtonName = "";
			const char *szButtonSectionName = "";
			const char *szTouchpadSectionName = "";
			switch (controllerType)
			{
			case CPSMoveControllerLatest::k_EPSControllerType_Move:
				szPSButtonName = k_PSButtonNames[psButtonID];
				szButtonSectionName = "psmove";
				szTouchpadSectionName = "psmove_touchpad_directions";
				break;
			case CPSMoveControllerLatest::k_EPSControllerType_DS4:
				szPSButtonName = k_PSButtonNames[psButtonID];
				szButtonSectionName = "dualshock4_button";
				szTouchpadSectionName = "dualshock4_touchpad";
				break;
			case CPSMoveControllerLatest::k_EPSControllerType_Navi:
				szPSButtonName = k_PSButtonNames[psButtonID];
				szButtonSectionName = "psnavi_button";
				szTouchpadSectionName = "psnavi_touchpad";
				break;
			case CPSMoveControllerLatest::k_EPSControllerType_Virtual:
				szPSButtonName = k_VirtualButtonNames[psButtonID];
				szButtonSectionName = "virtual_button";
				szTouchpadSectionName = "virtual_touchpad";
				break;
			}

			pSettings->GetString(szButtonSectionName, szPSButtonName, remapButtonToButtonString, 32, &fetchError);

			if (fetchError == vr::VRSettingsError_None)
			{
				for (int vr_button_index = 0; vr_button_index < k_max_vr_buttons; ++vr_button_index)
				{
					if (strcasecmp(remapButtonToButtonString, k_VRButtonNames[vr_button_index]) == 0)
					{
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

			if (strcmp(numId, "") != 0)
			{
				char buffer[64];
				strcpy(buffer, szButtonSectionName);
				strcat(buffer, "_");
				strcat(buffer, numId);
				szButtonSectionName = buffer;
				pSettings->GetString(szButtonSectionName, szPSButtonName, remapButtonToButtonString, 32, &fetchError);

				if (fetchError == vr::VRSettingsError_None)
				{
					for (int vr_button_index = 0; vr_button_index < k_max_vr_buttons; ++vr_button_index)
					{
						if (strcasecmp(remapButtonToButtonString, k_VRButtonNames[vr_button_index]) == 0)
						{
							vrButtonID = static_cast<vr::EVRButtonId>(vr_button_index);
							break;
						}
					}
				}
			}

			char remapButtonToTouchpadDirectionString[32];
			pSettings->GetString(szTouchpadSectionName, szPSButtonName, remapButtonToTouchpadDirectionString, 32, &fetchError);

			if (fetchError == vr::VRSettingsError_None)
			{
				for (int vr_touchpad_direction_index = 0; vr_touchpad_direction_index < k_max_vr_touchpad_directions; ++vr_touchpad_direction_index)
				{
					if (strcasecmp(remapButtonToTouchpadDirectionString, k_VRTouchpadDirectionNames[vr_touchpad_direction_index]) == 0)
					{
						vrTouchpadDirection = static_cast<eVRTouchpadDirection>(vr_touchpad_direction_index);
						break;
					}
				}
			}

			if (strcmp(numId, "") != 0)
			{
				char buffer[64];
				strcpy(buffer, szTouchpadSectionName);
				strcat(buffer, "_");
				strcat(buffer, numId);
				szTouchpadSectionName = buffer;
				pSettings->GetString(szTouchpadSectionName, szPSButtonName, remapButtonToTouchpadDirectionString, 32, &fetchError);

				if (fetchError == vr::VRSettingsError_None)
				{
					for (int vr_touchpad_direction_index = 0; vr_touchpad_direction_index < k_max_vr_touchpad_directions; ++vr_touchpad_direction_index)
					{
						if (strcasecmp(remapButtonToTouchpadDirectionString, k_VRTouchpadDirectionNames[vr_touchpad_direction_index]) == 0)
						{
							vrTouchpadDirection = static_cast<eVRTouchpadDirection>(vr_touchpad_direction_index);
							break;
						}
					}
				}
			}
		}

		// Save the mapping
		assert(controllerType >= 0 && controllerType < k_EPSControllerType_Count);
		assert(psButtonID >= 0 && psButtonID < k_EPSButtonID_Count);
		psButtonIDToVRButtonID[controllerType][psButtonID] = vrButtonID;
		psButtonIDToVrTouchpadDirection[controllerType][psButtonID] = vrTouchpadDirection;
	}

	bool CPSMoveControllerLatest::LoadBool(
		vr::IVRSettings *pSettings,
		const char *pchSection,
		const char *pchSettingsKey,
		const bool bDefaultValue)
	{
		vr::EVRSettingsError eError;
		bool bResult = pSettings->GetBool(pchSection, pchSettingsKey, &eError);

		if (eError != vr::VRSettingsError_None)
		{
			bResult = bDefaultValue;
		}

		return bResult;
	}

	int CPSMoveControllerLatest::LoadInt(
		vr::IVRSettings *pSettings,
		const char *pchSection,
		const char *pchSettingsKey,
		const int iDefaultValue)
	{
		vr::EVRSettingsError eError;
		int iResult = pSettings->GetInt32(pchSection, pchSettingsKey, &eError);

		if (eError != vr::VRSettingsError_None)
		{
			iResult = iDefaultValue;
		}

		return iResult;
	}

	float CPSMoveControllerLatest::LoadFloat(
		vr::IVRSettings *pSettings,
		const char *pchSection,
		const char *pchSettingsKey,
		const float fDefaultValue)
	{
		vr::EVRSettingsError eError;
		float fResult = pSettings->GetFloat(pchSection, pchSettingsKey, &eError);

		if (eError != vr::VRSettingsError_None)
		{
			fResult = fDefaultValue;
		}

		return fResult;
	}

	vr::EVRInitError CPSMoveControllerLatest::Activate(vr::TrackedDeviceIndex_t unObjectId)
	{
		vr::EVRInitError result = CPSMoveTrackedDeviceLatest::Activate(unObjectId);

		if (result == vr::VRInitError_None)
		{
			Logger::DriverLog("CPSMoveControllerLatest::Activate - Controller %d Activated\n", unObjectId);

			g_ServerTrackedDeviceProvider.LaunchPSMoveMonitor();

			PSMRequestID requestId;
			if (PSM_StartControllerDataStreamAsync(
				m_PSMControllerView->ControllerID,
				PSMStreamFlags_includePositionData | PSMStreamFlags_includePhysicsData,
				&requestId) == PSMResult_Success)
			{
				PSM_RegisterCallback(requestId, CPSMoveControllerLatest::start_controller_response_callback, this);
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
				for (int buttonIndex = 0; buttonIndex < static_cast<int>(k_EPSButtonID_Count); ++buttonIndex)
				{
					ulRetVal |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[m_PSMControllerType][buttonIndex]);

					if (psButtonIDToVrTouchpadDirection[m_PSMControllerType][buttonIndex] != k_EVRTouchpadDirection_None)
					{
						ulRetVal |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
					}
				}
				properties->SetUint64Property(m_ulPropertyContainer, vr::Prop_SupportedButtons_Uint64, ulRetVal);

				// The {psmove} syntax lets us refer to rendermodels that are installed
				// in the driver's own resources/rendermodels directory.  The driver can
				// still refer to SteamVR models like "generic_hmd".
				char model_label[32] = "\0";
				switch (m_PSMControllerType)
				{
				case PSMController_Move:
				{
					snprintf(model_label, sizeof(model_label), "psmove_%d", m_PSMControllerView->ControllerID);
					properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "{psmove}psmove_controller");
				} break;
				case PSMController_DualShock4:
				{
					snprintf(model_label, sizeof(model_label), "dualshock4_%d", m_PSMControllerView->ControllerID);
					properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "{psmove}dualshock4_controller");

				} break;
				case PSMController_Virtual:
				{
					snprintf(model_label, sizeof(model_label), "virtual_%d", m_PSMControllerView->ControllerID);
					if (m_overrideModel.length() > 0)
					{
						properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, m_overrideModel.c_str());
					}
					else
					{
						properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "vr_controller_01_mrhat");
					}
				} break;
				default:
				{
					snprintf(model_label, sizeof(model_label), "unknown");
					properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_RenderModelName_String, "generic_controller");
				}
				}
				properties->SetStringProperty(m_ulPropertyContainer, vr::Prop_ModeLabel_String, model_label);
			}
		}

		return result;
	}

	void CPSMoveControllerLatest::start_controller_response_callback(
		const PSMResponseMessage *response, void *userdata)
	{
		CPSMoveControllerLatest *controller = reinterpret_cast<CPSMoveControllerLatest *>(userdata);

		if (response->result_code == PSMResult::PSMResult_Success)
		{
			Logger::DriverLog("CPSMoveControllerLatest::start_controller_response_callback - Controller stream started\n");
		}
	}

	void CPSMoveControllerLatest::Deactivate()
	{
		Logger::DriverLog("CPSMoveControllerLatest::Deactivate - Controller stream stopped\n");
		PSM_StopControllerDataStreamAsync(m_PSMControllerView->ControllerID, nullptr);
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

	void CPSMoveControllerLatest::SendBooleanUpdates(bool pressed, uint64_t ulMask)
	{
		if (!ulMask)
			return;

		for (int i = 0; i < vr::k_EButton_Max; i++)
		{
			vr::EVRButtonId button = (vr::EVRButtonId)i;

			uint64_t bit = ButtonMaskFromId(button);

			if (bit & ulMask)
			{
				//( vr::VRServerDriverHost()->*ButtonEvent )( m_unSteamVRTrackedDeviceId, button, 0.0 );
				// must now call update on the boolean component instead
				vr::VRDriverInput()->UpdateBooleanComponent(m_ulBoolComponentsMap[button], pressed, 0.0);
			}
		}
	}

	void CPSMoveControllerLatest::SendScalarUpdates(float val, uint64_t ulMask)
	{
		if (!ulMask)
			return;

		for (int i = 0; i < vr::k_EButton_Max; i++)
		{
			vr::EVRButtonId button = (vr::EVRButtonId)i;

			uint64_t bit = ButtonMaskFromId(button);

			if (bit & ulMask)
			{
				//( vr::VRServerDriverHost()->*ButtonEvent )( m_unSteamVRTrackedDeviceId, button, 0.0 );
				// must now call update on the scalar component instead
				vr::VRDriverInput()->UpdateScalarComponent(m_ulScalarComponentsMap[button], val, 0.0);
			}
		}
	}

	void CPSMoveControllerLatest::UpdateControllerState()
	{
		static const uint64_t s_kTouchpadButtonMask = vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);

		assert(m_PSMControllerView != nullptr);
		assert(m_PSMControllerView->IsConnected);

		vr::VRControllerState_t NewState = { 0 };

		// Changing unPacketNum tells anyone polling state that something might have
		// changed.  We don't try to be precise about that here.
		NewState.unPacketNum = m_ControllerState.unPacketNum + 1;

		switch (m_PSMControllerView->ControllerType)
		{
		case PSMController_Move:
		{
			const PSMPSMove &clientView = m_PSMControllerView->ControllerState.PSMoveState;

			bool bStartRealignHMDTriggered =
				(clientView.StartButton == PSMButtonState_PRESSED && clientView.SelectButton == PSMButtonState_PRESSED) ||
				(clientView.StartButton == PSMButtonState_PRESSED && clientView.SelectButton == PSMButtonState_DOWN) ||
				(clientView.StartButton == PSMButtonState_DOWN && clientView.SelectButton == PSMButtonState_PRESSED);

			// Check if the PSMove has a PSNavi child
			const bool bHasChildNavi =
				m_PSMChildControllerView != nullptr &&
				m_PSMChildControllerView->ControllerType == PSMController_Navi;

			// See if the recenter button has been held for the requisite amount of time
			bool bRecenterRequestTriggered = false;
			{
				PSMButtonState resetPoseButtonState = clientView.SelectButton;
				PSMButtonState resetAlignButtonState;

				// Use PSNavi D-pad up/down if they are free
				if (bHasChildNavi)
				{
					if (m_bUsePSNaviDPadRealign)
					{
						resetAlignButtonState = m_PSMChildControllerView->ControllerState.PSNaviState.DPadUpButton;

						switch (resetAlignButtonState)
						{
						case PSMButtonState_PRESSED:
						{
							m_resetAlignButtonPressTime = std::chrono::high_resolution_clock::now();
						} break;
						case PSMButtonState_DOWN:
						{
							if (!m_bResetAlignRequestSent)
							{
								const float k_hold_duration_milli = 1000.f;
								std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();
								std::chrono::duration<float, std::milli> pressDurationMilli = now - m_resetAlignButtonPressTime;

								if (pressDurationMilli.count() >= k_hold_duration_milli)
								{
									bStartRealignHMDTriggered = true;
								}
							}
						} break;
						case PSMButtonState_RELEASED:
						{
							m_bResetAlignRequestSent = false;
						} break;
						}
					}

					if (m_bUsePSNaviDPadRecenter)
					{
						resetPoseButtonState = m_PSMChildControllerView->ControllerState.PSNaviState.DPadDownButton;
					}
				}

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
						const float k_hold_duration_milli = (bHasChildNavi) ? 1000.f : 250.f;
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

			// If START was just pressed while and SELECT was held or vice versa,
			// recenter the controller orientation pose and start the realignment of the controller to HMD tracking space.
			if (bStartRealignHMDTriggered)
			{
				PSMVector3f controllerBallPointedUpEuler = { (float)M_PI_2, 0.0f, 0.0f };

				PSMQuatf controllerBallPointedUpQuat = PSM_QuatfCreateFromAngles(&controllerBallPointedUpEuler);

				Logger::DriverLog("CPSMoveControllerLatest::UpdateControllerState(): Calling StartRealignHMDTrackingSpace() in response to controller chord.\n");

				PSM_ResetControllerOrientationAsync(m_PSMControllerView->ControllerID, &controllerBallPointedUpQuat, nullptr);
				m_bResetPoseRequestSent = true;

				RealignHMDTrackingSpace();
				m_bResetAlignRequestSent = true;
			}
			else if (bRecenterRequestTriggered)
			{
				Logger::DriverLog("CPSMoveControllerLatest::UpdateControllerState(): Calling ClientPSMoveAPI::reset_orientation() in response to controller button press.\n");

				PSM_ResetControllerOrientationAsync(m_PSMControllerView->ControllerID, k_psm_quaternion_identity, nullptr);
				m_bResetPoseRequestSent = true;
			}
			else
			{
				// Process all the button mappings 
				// ------

				// Handle buttons/virtual touchpad buttons on the psmove
				m_touchpadDirectionsUsed = false;
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_Circle, clientView.CircleButton, &NewState);
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_Cross, clientView.CrossButton, &NewState);
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_Move, clientView.MoveButton, &NewState);
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_PS, clientView.PSButton, &NewState);
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_Select, clientView.SelectButton, &NewState);
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_Square, clientView.SquareButton, &NewState);
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_Start, clientView.StartButton, &NewState);
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_Triangle, clientView.TriangleButton, &NewState);
				UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Move, k_EPSButtonID_Trigger, clientView.TriggerButton, &NewState);

				// Handle buttons/virtual touchpad buttons on the psnavi
				if (bHasChildNavi)
				{
					const PSMPSNavi &naviClientView = m_PSMChildControllerView->ControllerState.PSNaviState;

					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_Circle, naviClientView.CircleButton, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_Cross, naviClientView.CrossButton, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_PS, naviClientView.PSButton, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_Up, naviClientView.DPadUpButton, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_Down, naviClientView.DPadDownButton, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_Left, naviClientView.DPadLeftButton, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_Right, naviClientView.DPadRightButton, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_L1, naviClientView.L1Button, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_L2, naviClientView.L2Button, &NewState);
					UpdateControllerStateFromPsMoveButtonState(k_EPSControllerType_Navi, k_EPSButtonID_L3, naviClientView.L3Button, &NewState);
				}

				// Touchpad handling
				if (!m_touchpadDirectionsUsed)
				{
					// PSNavi TouchPad Handling (i.e. thumbstick as touchpad)
					if (bHasChildNavi)
					{
						const PSMPSNavi &naviClientView = m_PSMChildControllerView->ControllerState.PSNaviState;
						const float thumbStickX = (naviClientView.Stick_XAxis / 128.f) - 1.f;
						const float thumbStickY = (naviClientView.Stick_YAxis / 128.f) - 1.f;
						const float thumbStickAngle = atanf(abs(thumbStickY / thumbStickX));
						const float thumbStickRadialDist = sqrtf(thumbStickX*thumbStickX + thumbStickY * thumbStickY);

						if (thumbStickRadialDist >= m_thumbstickDeadzone)
						{
							// Rescale the thumbstick position to hide the dead zone
							const float rescaledRadius = (thumbStickRadialDist - m_thumbstickDeadzone) / (1.f - m_thumbstickDeadzone);

							// Set the thumbstick axis
							NewState.rAxis[0].x = (rescaledRadius / thumbStickRadialDist) * thumbStickX * abs(cosf(thumbStickAngle));
							NewState.rAxis[0].y = (rescaledRadius / thumbStickRadialDist) * thumbStickY * abs(sinf(thumbStickAngle));

							// Also make sure the touchpad is considered "touched" 
							// if the thumbstick is outside of the deadzone
							NewState.ulButtonTouched |= s_kTouchpadButtonMask;

							// If desired, also treat the touch as a press
							if (m_bThumbstickTouchAsPress)
							{
								NewState.ulButtonPressed |= s_kTouchpadButtonMask;
							}
						}
					}
					// Virtual TouchPad h=Handling (i.e. controller spatial offset as touchpad) 
					else if (m_bUseSpatialOffsetAfterTouchpadPressAsTouchpadAxis)
					{
						bool bTouchpadIsActive = (NewState.ulButtonPressed & s_kTouchpadButtonMask) || (NewState.ulButtonTouched & s_kTouchpadButtonMask);

						if (bTouchpadIsActive)
						{
							bool bIsNewTouchpadLocation = true;

							if (m_bDelayAfterTouchpadPress)
							{
								std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();

								if (!m_bTouchpadWasActive)
								{
									const float k_max_touchpad_press = 2000.0; // time until coordinates are reset, otherwise assume in last location.
									std::chrono::duration<double, std::milli> timeSinceActivated = now - m_lastTouchpadPressTime;

									bIsNewTouchpadLocation = timeSinceActivated.count() >= k_max_touchpad_press;
								}
								m_lastTouchpadPressTime = now;

							}

							if (bIsNewTouchpadLocation)
							{
								if (!m_bTouchpadWasActive)
								{
									// Just pressed.
									const PSMPSMove &view = m_PSMControllerView->ControllerState.PSMoveState;
									m_driverSpaceRotationAtTouchpadPressTime = view.Pose.Orientation;

									GetMetersPosInRotSpace(&m_driverSpaceRotationAtTouchpadPressTime, &m_posMetersAtTouchpadPressTime);

#if LOG_TOUCHPAD_EMULATION != 0
									Logger::DriverLog("Touchpad pressed! At (%f, %f, %f) meters relative to orientation\n",
										m_posMetersAtTouchpadPressTime.x, m_posMetersAtTouchpadPressTime.y, m_posMetersAtTouchpadPressTime.z);
#endif
								}
								else
								{
									// Held!
									PSMVector3f newPosMeters;
									GetMetersPosInRotSpace(&m_driverSpaceRotationAtTouchpadPressTime, &newPosMeters);

									PSMVector3f offsetMeters = PSM_Vector3fSubtract(&newPosMeters, &m_posMetersAtTouchpadPressTime);

#if LOG_TOUCHPAD_EMULATION != 0
									Logger::DriverLog("Touchpad held! Relative position (%f, %f, %f) meters\n",
										offsetMeters.x, offsetMeters.y, offsetMeters.z);
#endif

									NewState.rAxis[0].x = offsetMeters.x / m_fMetersPerTouchpadAxisUnits;
									NewState.rAxis[0].x = fminf(fmaxf(NewState.rAxis[0].x, -1.0f), 1.0f);

									NewState.rAxis[0].y = -offsetMeters.z / m_fMetersPerTouchpadAxisUnits;
									NewState.rAxis[0].y = fminf(fmaxf(NewState.rAxis[0].y, -1.0f), 1.0f);

#if LOG_TOUCHPAD_EMULATION != 0
									Logger::DriverLog("Touchpad axis at (%f, %f) \n",
										NewState.rAxis[0].x, NewState.rAxis[0].y);
#endif
								}
							}
						}

						// Remember if the touchpad was active the previous frame for edge detection
						m_bTouchpadWasActive = bTouchpadIsActive;
					}
				}

				// Touchpad SteamVR Events
				if (NewState.rAxis[0].x != m_ControllerState.rAxis[0].x ||
					NewState.rAxis[0].y != m_ControllerState.rAxis[0].y)
				{
					// TODO change this to update a registered touch controller component i.e. /input/trackpad/x /input/trackpad/y
					//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, 0, NewState.rAxis[0]);
				}

				// PSMove Trigger handling
				NewState.rAxis[m_steamVRTriggerAxisIndex].x = clientView.TriggerValue / 255.f;
				NewState.rAxis[m_steamVRTriggerAxisIndex].y = 0.f;

				if (m_steamVRTriggerAxisIndex != 1)
				{
					static const uint64_t s_kTriggerButtonMask = vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Trigger);
					if ((NewState.ulButtonPressed & s_kTriggerButtonMask) || (NewState.ulButtonTouched & s_kTriggerButtonMask))
					{
						NewState.rAxis[1].x = 1.f;
					}
					else
					{
						NewState.rAxis[1].x = 0.f;
					}
					NewState.rAxis[1].y = 0.f;
				}

				// Attached PSNavi Trigger handling
				if (bHasChildNavi)
				{
					const PSMPSNavi &naviClientView = m_PSMChildControllerView->ControllerState.PSNaviState;

					NewState.rAxis[m_steamVRNaviTriggerAxisIndex].x = fmaxf(NewState.rAxis[m_steamVRNaviTriggerAxisIndex].x, naviClientView.TriggerValue / 255.f);
					if (m_steamVRNaviTriggerAxisIndex != m_steamVRTriggerAxisIndex)
						NewState.rAxis[m_steamVRNaviTriggerAxisIndex].y = 0.f;
				}

				// Trigger SteamVR Events
				if (NewState.rAxis[m_steamVRTriggerAxisIndex].x != m_ControllerState.rAxis[m_steamVRTriggerAxisIndex].x)
				{

					if (NewState.rAxis[m_steamVRTriggerAxisIndex].x > 0.1f)
					{
						NewState.ulButtonTouched |= vr::ButtonMaskFromId(static_cast<vr::EVRButtonId>(vr::k_EButton_Axis0 + m_steamVRTriggerAxisIndex));
					}

					// Send the button was press event only when it's almost fully pressed
					if (NewState.rAxis[m_steamVRTriggerAxisIndex].x > 0.8f)
					{
						NewState.ulButtonPressed |= vr::ButtonMaskFromId(static_cast<vr::EVRButtonId>(vr::k_EButton_Axis0 + m_steamVRTriggerAxisIndex));
					}

					//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, m_steamVRTriggerAxisIndex, NewState.rAxis[m_steamVRTriggerAxisIndex]);
					SendScalarUpdates(NewState.rAxis[m_steamVRTriggerAxisIndex].x, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[m_steamVRTriggerAxisIndex].x, NewState.ulButtonPressed);

				}
				if (m_steamVRTriggerAxisIndex != 1 && NewState.rAxis[1].x != m_ControllerState.rAxis[1].x)
				{
					if (NewState.rAxis[1].x > 0.1f)
					{
						NewState.ulButtonTouched |= vr::ButtonMaskFromId(static_cast<vr::EVRButtonId>(vr::k_EButton_Axis0 + 1));
					}

					if (NewState.rAxis[1].x > 0.8f)
					{
						NewState.ulButtonPressed |= vr::ButtonMaskFromId(static_cast<vr::EVRButtonId>(vr::k_EButton_Axis0 + 1));
					}

					//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, 1, NewState.rAxis[1]);
					SendScalarUpdates(NewState.rAxis[1].x, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[1].x, NewState.ulButtonPressed);
				}
				if (m_steamVRNaviTriggerAxisIndex != m_steamVRTriggerAxisIndex && (NewState.rAxis[m_steamVRNaviTriggerAxisIndex].x != m_ControllerState.rAxis[m_steamVRNaviTriggerAxisIndex].x))
				{
					if (NewState.rAxis[m_steamVRNaviTriggerAxisIndex].x > 0.1f)
					{
						NewState.ulButtonTouched |= vr::ButtonMaskFromId(static_cast<vr::EVRButtonId>(vr::k_EButton_Axis0 + m_steamVRNaviTriggerAxisIndex));
					}

					if (NewState.rAxis[m_steamVRNaviTriggerAxisIndex].x > 0.8f)
					{
						NewState.ulButtonPressed |= vr::ButtonMaskFromId(static_cast<vr::EVRButtonId>(vr::k_EButton_Axis0 + m_steamVRNaviTriggerAxisIndex));
					}

					// vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, m_steamVRNaviTriggerAxisIndex, NewState.rAxis[m_steamVRNaviTriggerAxisIndex]);
					SendScalarUpdates(NewState.rAxis[m_steamVRNaviTriggerAxisIndex].x, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[m_steamVRNaviTriggerAxisIndex].x, NewState.ulButtonPressed);
				}

				// Update the battery charge state
				UpdateBatteryChargeState(m_PSMControllerView->ControllerState.PSMoveState.BatteryValue);
			}
		} break;
		case PSMController_DualShock4:
		{
			const PSMDualShock4 &clientView = m_PSMControllerView->ControllerState.PSDS4State;

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
			if (bStartRealignHMDTriggered)
			{
				Logger::DriverLog("CPSMoveControllerLatest::UpdateControllerState(): Calling StartRealignHMDTrackingSpace() in response to controller chord.\n");

				PSM_ResetControllerOrientationAsync(m_PSMControllerView->ControllerID, k_psm_quaternion_identity, nullptr);
				m_bResetPoseRequestSent = true;

				RealignHMDTrackingSpace();
			}
			else if (bRecenterRequestTriggered)
			{
				Logger::DriverLog("CPSMoveControllerLatest::UpdateControllerState(): Calling ClientPSMoveAPI::reset_orientation() in response to controller button press.\n");

				PSM_ResetControllerOrientationAsync(m_PSMControllerView->ControllerID, k_psm_quaternion_identity, nullptr);
				m_bResetPoseRequestSent = true;
			}
			else
			{
				if (clientView.L1Button)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_L1]);
				if (clientView.L2Button)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_L2]);
				if (clientView.L3Button)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_L3]);
				if (clientView.R1Button)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_R1]);
				if (clientView.R2Button)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_R2]);
				if (clientView.R3Button)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_R3]);

				if (clientView.CircleButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Circle]);
				if (clientView.CrossButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Cross]);
				if (clientView.SquareButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Square]);
				if (clientView.TriangleButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Triangle]);

				if (clientView.DPadUpButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Up]);
				if (clientView.DPadDownButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Down]);
				if (clientView.DPadLeftButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Left]);
				if (clientView.DPadRightButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Right]);

				if (clientView.OptionsButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Options]);
				if (clientView.ShareButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Share]);
				if (clientView.TrackPadButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_Trackpad]);
				if (clientView.PSButton)
					NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_DS4][k_EPSButtonID_PS]);

				NewState.rAxis[0].x = clientView.LeftAnalogX;
				NewState.rAxis[0].y = -clientView.LeftAnalogY;

				NewState.rAxis[1].x = clientView.LeftTriggerValue;
				NewState.rAxis[1].y = 0.f;

				NewState.rAxis[2].x = clientView.RightAnalogX;
				NewState.rAxis[2].y = -clientView.RightAnalogY;

				NewState.rAxis[3].x = clientView.RightTriggerValue;
				NewState.rAxis[3].y = 0.f;

				if (NewState.rAxis[0].x != m_ControllerState.rAxis[0].x || NewState.rAxis[0].y != m_ControllerState.rAxis[0].y) {
					//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, 0, NewState.rAxis[0]);
					SendScalarUpdates(NewState.rAxis[0].x, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[0].x, NewState.ulButtonPressed);
					SendScalarUpdates(NewState.rAxis[0].y, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[0].y, NewState.ulButtonPressed);
				}

				if (NewState.rAxis[1].x != m_ControllerState.rAxis[1].x) {
					//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, 1, NewState.rAxis[1]);
					SendScalarUpdates(NewState.rAxis[1].x, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[1].x, NewState.ulButtonPressed);
					SendScalarUpdates(NewState.rAxis[1].y, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[1].y, NewState.ulButtonPressed);
				}

				if (NewState.rAxis[2].x != m_ControllerState.rAxis[2].x || NewState.rAxis[2].y != m_ControllerState.rAxis[2].y) {
					//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, 2, NewState.rAxis[2]);
					SendScalarUpdates(NewState.rAxis[2].x, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[2].x, NewState.ulButtonPressed);
					SendScalarUpdates(NewState.rAxis[2].y, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[2].y, NewState.ulButtonPressed);
				}

				if (NewState.rAxis[3].x != m_ControllerState.rAxis[3].x) {
					//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, 3, NewState.rAxis[3]);
					SendScalarUpdates(NewState.rAxis[3].x, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[3].x, NewState.ulButtonPressed);
					SendScalarUpdates(NewState.rAxis[3].y, NewState.ulButtonTouched);
					SendScalarUpdates(NewState.rAxis[3].y, NewState.ulButtonPressed);
				}

			}
		} break;
		case PSMController_Virtual:
		{
			const PSMVirtualController &clientView = m_PSMControllerView->ControllerState.VirtualController;

			if (clientView.buttonStates[m_hmdAlignPSButtonID] == PSMButtonState_PRESSED)
			{
				Logger::DriverLog("CPSMoveControllerLatest::UpdateControllerState(): Calling StartRealignHMDTrackingSpace() in response to controller chord.\n");

				RealignHMDTrackingSpace();
			}
			else
			{
				int buttonCount = m_PSMControllerView->ControllerState.VirtualController.numButtons;
				int axisCount = m_PSMControllerView->ControllerState.VirtualController.numAxes;

				for (int buttonIndex = 0; buttonIndex < buttonCount; ++buttonIndex)
				{
					if (m_PSMControllerView->ControllerState.VirtualController.buttonStates[buttonIndex])
					{
						NewState.ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[k_EPSControllerType_Virtual][buttonIndex]);
					}
				}

				if (m_virtualTouchpadXAxisIndex >= 0 && m_virtualTouchpadXAxisIndex < axisCount &&
					m_virtualTouchpadYAxisIndex >= 0 && m_virtualTouchpadYAxisIndex < axisCount)
				{
					const unsigned char rawThumbStickX = m_PSMControllerView->ControllerState.VirtualController.axisStates[m_virtualTouchpadXAxisIndex];
					const unsigned char rawThumbStickY = m_PSMControllerView->ControllerState.VirtualController.axisStates[m_virtualTouchpadYAxisIndex];
					float thumbStickX = ((float)rawThumbStickX - 127.f) / 127.f;
					float thumbStickY = ((float)rawThumbStickY - 127.f) / 127.f;

					const float thumbStickAngle = atanf(abs(thumbStickY / thumbStickX));
					const float thumbStickRadialDist = sqrtf(thumbStickX*thumbStickX + thumbStickY * thumbStickY);

					bool bTouchpadTouched = false;
					bool bTouchpadPressed = false;

					// Moving a thumbstick outside of the deadzone is consider a touchpad touch
					if (thumbStickRadialDist >= m_thumbstickDeadzone)
					{
						// Rescale the thumbstick position to hide the dead zone
						const float rescaledRadius = (thumbStickRadialDist - m_thumbstickDeadzone) / (1.f - m_thumbstickDeadzone);

						// Set the thumbstick axis
						thumbStickX = (rescaledRadius / thumbStickRadialDist) * thumbStickX * abs(cosf(thumbStickAngle));
						thumbStickY = (rescaledRadius / thumbStickRadialDist) * thumbStickY * abs(sinf(thumbStickAngle));

						// Also make sure the touchpad is considered "touched" 
						// if the thumbstick is outside of the deadzone
						bTouchpadTouched = true;

						// If desired, also treat the touch as a press
						bTouchpadPressed = m_bThumbstickTouchAsPress;
					}

					if (bTouchpadTouched)
					{
						NewState.ulButtonTouched |= s_kTouchpadButtonMask;
					}

					if (bTouchpadPressed)
					{
						NewState.ulButtonPressed |= s_kTouchpadButtonMask;
					}

					NewState.rAxis[0].x = thumbStickX;
					NewState.rAxis[0].y = thumbStickY;

					// when either the x or y axis of the virtual controller change send a scalar component update
					if (NewState.rAxis[0].x != m_ControllerState.rAxis[0].x || NewState.rAxis[0].y != m_ControllerState.rAxis[0].y)
					{
						//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, 0, NewState.rAxis[0]);
						SendScalarUpdates(NewState.rAxis[0].x, NewState.ulButtonTouched);
						SendScalarUpdates(NewState.rAxis[0].x, NewState.ulButtonPressed);
						SendScalarUpdates(NewState.rAxis[0].y, NewState.ulButtonTouched);
						SendScalarUpdates(NewState.rAxis[0].y, NewState.ulButtonPressed);
					}
				}

				if (m_virtualTriggerAxisIndex >= 0 && m_virtualTriggerAxisIndex < axisCount)
				{
					// Remap trigger axis from [0, 255]
					const float triggerValue = (float)m_PSMControllerView->ControllerState.VirtualController.axisStates[m_virtualTriggerAxisIndex] / 255.f;

					NewState.rAxis[1].x = triggerValue;
					NewState.rAxis[1].y = 0.f;

					if (NewState.rAxis[1].x != m_ControllerState.rAxis[1].x)
					{
						//vr::VRServerDriverHost()->TrackedDeviceAxisUpdated(m_unSteamVRTrackedDeviceId, 1, NewState.rAxis[1]);
						SendScalarUpdates(NewState.rAxis[1].x, NewState.ulButtonTouched);
						SendScalarUpdates(NewState.rAxis[1].x, NewState.ulButtonPressed);
						SendScalarUpdates(NewState.rAxis[1].y, NewState.ulButtonTouched);
						SendScalarUpdates(NewState.rAxis[1].y, NewState.ulButtonPressed);

					}
				}
			}
		} break;
		}

		// All pressed buttons are touched
		NewState.ulButtonTouched |= NewState.ulButtonPressed;

		uint64_t ulChangedTouched = NewState.ulButtonTouched ^ m_ControllerState.ulButtonTouched;
		uint64_t ulChangedPressed = NewState.ulButtonPressed ^ m_ControllerState.ulButtonPressed;

		SendBooleanUpdates(true, ulChangedPressed &  NewState.ulButtonPressed);
		SendBooleanUpdates(false, ulChangedPressed & ~NewState.ulButtonPressed);
		//SendButtonUpdates( &vr::IVRServerDriverHost::TrackedDeviceButtonTouched, ulChangedTouched & NewState.ulButtonTouched );
		//SendButtonUpdates( &vr::IVRServerDriverHost::TrackedDeviceButtonUntouched, ulChangedTouched & ~NewState.ulButtonTouched );

		m_ControllerState = NewState;
	}

	void CPSMoveControllerLatest::UpdateControllerStateFromPsMoveButtonState(
		ePSControllerType controllerType,
		ePSButtonID buttonId,
		PSMButtonState buttonState,
		vr::VRControllerState_t* pControllerStateToUpdate)
	{
		if (buttonState & PSMButtonState_PRESSED || buttonState & PSMButtonState_DOWN)
		{
			if (psButtonIDToVRButtonID[controllerType][buttonId] == k_touchpadTouchMapping) {
				pControllerStateToUpdate->ulButtonTouched |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
			}
			else {
				pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(psButtonIDToVRButtonID[controllerType][buttonId]);

				if (psButtonIDToVrTouchpadDirection[controllerType][buttonId] == k_EVRTouchpadDirection_Left)
				{
					m_touchpadDirectionsUsed = true;
					pControllerStateToUpdate->rAxis[0].x = -1.0f;
					pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
				}
				else if (psButtonIDToVrTouchpadDirection[controllerType][buttonId] == k_EVRTouchpadDirection_Right)
				{
					m_touchpadDirectionsUsed = true;
					pControllerStateToUpdate->rAxis[0].x = 1.0f;
					pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
				}
				else if (psButtonIDToVrTouchpadDirection[controllerType][buttonId] == k_EVRTouchpadDirection_Up)
				{
					m_touchpadDirectionsUsed = true;
					pControllerStateToUpdate->rAxis[0].y = 1.0f;
					pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
				}
				else if (psButtonIDToVrTouchpadDirection[controllerType][buttonId] == k_EVRTouchpadDirection_Down)
				{
					m_touchpadDirectionsUsed = true;
					pControllerStateToUpdate->rAxis[0].y = -1.0f;
					pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
				}
				else if (psButtonIDToVrTouchpadDirection[controllerType][buttonId] == k_EVRTouchpadDirection_UpLeft)
				{
					m_touchpadDirectionsUsed = true;
					pControllerStateToUpdate->rAxis[0].x = -0.707f;
					pControllerStateToUpdate->rAxis[0].y = 0.707f;
					pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
				}
				else if (psButtonIDToVrTouchpadDirection[controllerType][buttonId] == k_EVRTouchpadDirection_UpRight)
				{
					m_touchpadDirectionsUsed = true;
					pControllerStateToUpdate->rAxis[0].x = 0.707f;
					pControllerStateToUpdate->rAxis[0].y = 0.707f;
					pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
				}
				else if (psButtonIDToVrTouchpadDirection[controllerType][buttonId] == k_EVRTouchpadDirection_DownLeft)
				{
					m_touchpadDirectionsUsed = true;
					pControllerStateToUpdate->rAxis[0].x = -0.707f;
					pControllerStateToUpdate->rAxis[0].y = -0.707f;
					pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
				}
				else if (psButtonIDToVrTouchpadDirection[controllerType][buttonId] == k_EVRTouchpadDirection_DownRight)
				{
					m_touchpadDirectionsUsed = true;
					pControllerStateToUpdate->rAxis[0].x = 0.707f;
					pControllerStateToUpdate->rAxis[0].y = -0.707f;
					pControllerStateToUpdate->ulButtonPressed |= vr::ButtonMaskFromId(vr::k_EButton_SteamVR_Touchpad);
				}
			}
		}
	}

	PSMQuatf ExtractHMDYawQuaternion(const PSMQuatf &q)
	{
		// Convert the quaternion to a basis matrix
		const PSMMatrix3f hmd_orientation = PSM_Matrix3fCreateFromQuatf(&q);

		// Extract the forward (z-axis) vector from the basis
		const PSMVector3f forward = PSM_Matrix3fBasisZ(&hmd_orientation);
		PSMVector3f forward2d = { forward.x, 0.f, forward.z };
		forward2d = PSM_Vector3fNormalizeWithDefault(&forward2d, k_psm_float_vector3_k);

		// Compute the yaw angle (amount the z-axis has been rotated to it's current facing)
		const float cos_yaw = PSM_Vector3fDot(&forward, k_psm_float_vector3_k);
		float half_yaw = acosf(fminf(fmaxf(cos_yaw, -1.f), 1.f)) / 2.f;

		// Flip the sign of the yaw angle depending on if forward2d is to the left or right of global forward
		PSMVector3f yaw_axis = PSM_Vector3fCross(k_psm_float_vector3_k, &forward2d);
		if (PSM_Vector3fDot(&yaw_axis, k_psm_float_vector3_j) < 0)
		{
			half_yaw = -half_yaw;
		}

		// Convert this yaw rotation back into a quaternion
		PSMQuatf yaw_quaternion =
			PSM_QuatfCreate(
				cosf(half_yaw), // w = cos(theta/2)
				0.f, sinf(half_yaw), 0.f); // (x, y, z) = sin(theta/2)*axis, where axis = (0, 1, 0)

		return yaw_quaternion;
	}

	PSMQuatf ExtractPSMoveYawQuaternion(const PSMQuatf &q)
	{
		// Convert the quaternion to a basis matrix
		const PSMMatrix3f psmove_basis = PSM_Matrix3fCreateFromQuatf(&q);

		// Extract the forward (negative z-axis) vector from the basis
		const PSMVector3f global_forward = { 0.f, 0.f, -1.f };
		const PSMVector3f &forward = PSM_Matrix3fBasisY(&psmove_basis);
		PSMVector3f forward2d = { forward.x, 0.f, forward.z };
		forward2d = PSM_Vector3fNormalizeWithDefault(&forward2d, &global_forward);

		// Compute the yaw angle (amount the z-axis has been rotated to it's current facing)
		const float cos_yaw = PSM_Vector3fDot(&forward, &global_forward);
		float yaw = acosf(fminf(fmaxf(cos_yaw, -1.f), 1.f));

		// Flip the sign of the yaw angle depending on if forward2d is to the left or right of global forward
		const PSMVector3f &global_up = *k_psm_float_vector3_j;
		PSMVector3f yaw_axis = PSM_Vector3fCross(&global_forward, &forward2d);
		if (PSM_Vector3fDot(&yaw_axis, &global_up) < 0)
		{
			yaw = -yaw;
		}

		// Convert this yaw rotation back into a quaternion
		PSMVector3f eulerPitch = { (float)1.57079632679489661923, 0.f, 0.f }; // pitch 90 up first
		PSMVector3f eulerYaw = { 0, yaw, 0 };
		PSMQuatf quatPitch = PSM_QuatfCreateFromAngles(&eulerPitch);
		PSMQuatf quatYaw = PSM_QuatfCreateFromAngles(&eulerYaw);
		PSMQuatf yaw_quaternion =
			PSM_QuatfConcat(
				&quatPitch, // pitch 90 up first
				&quatYaw); // Then apply the yaw

		return yaw_quaternion;
	}

	void CPSMoveControllerLatest::GetMetersPosInRotSpace(const PSMQuatf *rotation, PSMVector3f* out_position)
	{
		const PSMPSMove &view = m_PSMControllerView->ControllerState.PSMoveState;
		const PSMVector3f &position = view.Pose.Position;

		PSMVector3f unrotatedPositionMeters = PSM_Vector3fScale(&position, k_fScalePSMoveAPIToMeters);
		PSMQuatf viewOrientationInverse = PSM_QuatfConjugate(rotation);

		*out_position = PSM_QuatfRotateVector(&viewOrientationInverse, &unrotatedPositionMeters);
	}

	void CPSMoveControllerLatest::RealignHMDTrackingSpace()
	{
		if (m_bDisableHMDAlignmentGesture)
		{
			Logger::DriverLog("Ignoring RealignHMDTrackingSpace request. Disabled.\n");
			return;
		}

		Logger::DriverLog("Begin CPSMoveControllerLatest::RealignHMDTrackingSpace()\n");

		vr::TrackedDeviceIndex_t hmd_device_index = vr::k_unTrackedDeviceIndexInvalid;
		if (Utils::GetHMDDeviceIndex(&hmd_device_index))
		{
			Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - HMD Device Index= %u\n", hmd_device_index);
		}
		else
		{
			Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - Failed to get HMD Device Index\n");
			return;
		}

		PSMPosef hmd_pose_meters;
		if (Utils::GetTrackedDevicePose(hmd_device_index, &hmd_pose_meters))
		{
			Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - hmd_pose_meters: %s \n", Utils::PSMPosefToString(hmd_pose_meters).c_str());
		}
		else
		{
			Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - Failed to get HMD Pose\n");
			return;
		}

		// Make the HMD orientation only contain a yaw
		hmd_pose_meters.Orientation = ExtractHMDYawQuaternion(hmd_pose_meters.Orientation);
		Logger::DriverLog("hmd_pose_meters(yaw-only): %s \n", Utils::PSMPosefToString(hmd_pose_meters).c_str());

		// We have the transform of the HMD in world space. 
		// However the HMD and the controller aren't quite aligned depending on the controller type:
		PSMQuatf controllerOrientationInHmdSpaceQuat = *k_psm_quaternion_identity;
		PSMVector3f controllerLocalOffsetFromHmdPosition = *k_psm_float_vector3_zero;
		if (m_PSMControllerType == PSMControllerType::PSMController_Move)
		{
			// Rotation) The controller's local -Z axis (from the center to the glowing ball) is currently pointed 
			//    in the direction of the HMD's local +Y axis, 
			// Translation) The controller's position is a few inches ahead of the HMD's on the HMD's local -Z axis. 
			PSMVector3f eulerPitch = { (float)M_PI_2, 0.0f, 0.0f };
			controllerOrientationInHmdSpaceQuat = PSM_QuatfCreateFromAngles(&eulerPitch);
			controllerLocalOffsetFromHmdPosition = { 0.0f, 0.0f, -1.0f * m_fControllerMetersInFrontOfHmdAtCalibration };
		}
		else if (m_PSMControllerType == PSMControllerType::PSMController_DualShock4 ||
			m_PSMControllerType == PSMControllerType::PSMController_Virtual)
		{
			// Translation) The controller's position is a few inches ahead of the HMD's on the HMD's local -Z axis. 
			controllerLocalOffsetFromHmdPosition = { 0.0f, 0.0f, -1.0f * m_fControllerMetersInFrontOfHmdAtCalibration };
		}

		// Transform the HMD's world space transform to where we expect the controller's world space transform to be.
		PSMPosef controllerPoseRelativeToHMD =
			PSM_PosefCreate(&controllerLocalOffsetFromHmdPosition, &controllerOrientationInHmdSpaceQuat);

		Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controllerPoseRelativeToHMD: %s \n", Utils::PSMPosefToString(controllerPoseRelativeToHMD).c_str());

		// Compute the expected controller pose in HMD tracking space (i.e. "World Space")
		PSMPosef controller_world_space_pose = PSM_PosefConcat(&controllerPoseRelativeToHMD, &hmd_pose_meters);
		Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_world_space_pose: %s \n", Utils::PSMPosefToString(controller_world_space_pose).c_str());

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
		PSM_GetControllerPose(m_PSMControllerView->ControllerID, &controller_pose_meters);
		Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_meters(raw): %s \n", Utils::PSMPosefToString(controller_pose_meters).c_str());

		// PSMove Position is in cm, but OpenVR stores position in meters
		controller_pose_meters.Position = PSM_Vector3fScale(&controller_pose_meters.Position, k_fScalePSMoveAPIToMeters);

		if (m_PSMControllerType == PSMControllerType::PSMController_Move)
		{
			if (m_bUseControllerOrientationInHMDAlignment)
			{
				// Extract only the yaw from the controller orientation (assume it's mostly held upright)
				controller_pose_meters.Orientation = ExtractPSMoveYawQuaternion(controller_pose_meters.Orientation);
				Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_meters(yaw-only): %s \n", Utils::PSMPosefToString(controller_pose_meters).c_str());
			}
			else
			{
				const PSMVector3f eulerPitch = { (float)M_PI_2, 0.0f, 0.0f };

				controller_pose_meters.Orientation = PSM_QuatfCreateFromAngles(&eulerPitch);
				Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_meters(no-rotation): %s \n", Utils::PSMPosefToString(controller_pose_meters).c_str());
			}
		}
		else if (m_PSMControllerType == PSMControllerType::PSMController_DualShock4 ||
			m_PSMControllerType == PSMControllerType::PSMController_Virtual)
		{
			controller_pose_meters.Orientation = *k_psm_quaternion_identity;
			Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_meters(no-rotation): %s \n", Utils::PSMPosefToString(controller_pose_meters).c_str());
		}

		PSMPosef controller_pose_inv = PSM_PosefInverse(&controller_pose_meters);
		Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - controller_pose_inv: %s \n", Utils::PSMPosefToString(controller_pose_inv).c_str());

		PSMPosef driver_pose_to_world_pose = PSM_PosefConcat(&controller_pose_inv, &controller_world_space_pose);
		Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - driver_pose_to_world_pose: %s \n", Utils::PSMPosefToString(driver_pose_to_world_pose).c_str());

		PSMPosef test_composed_controller_world_space = PSM_PosefConcat(&controller_pose_meters, &driver_pose_to_world_pose);
		Logger::DriverLog("CPSMoveControllerLatest::RealignHMDTrackingSpace() - test_composed_controller_world_space: %s \n", Utils::PSMPosefToString(test_composed_controller_world_space).c_str());

		g_ServerTrackedDeviceProvider.SetHMDTrackingSpace(driver_pose_to_world_pose);
	}

	void CPSMoveControllerLatest::UpdateTrackingState()
	{
		assert(m_PSMControllerView != nullptr);
		assert(m_PSMControllerView->IsConnected);

		// The tracking status will be one of the following states:
		m_Pose.result = m_trackingStatus;

		m_Pose.deviceIsConnected = m_PSMControllerView->IsConnected;

		// These should always be false from any modern driver.  These are for Oculus DK1-like
		// rotation-only tracking.  Support for that has likely rotted in vrserver.
		m_Pose.willDriftInYaw = false;
		m_Pose.shouldApplyHeadModel = false;

		switch (m_PSMControllerView->ControllerType)
		{
		case PSMControllerType::PSMController_Move:
		{
			const PSMPSMove &view = m_PSMControllerView->ControllerState.PSMoveState;

			// No prediction since that's already handled in the psmove service
			m_Pose.poseTimeOffset = 0.f;

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
			if (m_fVirtuallExtendControllersYMeters != 0.0f || m_fVirtuallExtendControllersZMeters != 0.0f)
			{
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
				m_PSMControllerView->ControllerState.PSMoveState.bIsPositionValid &&
				m_PSMControllerView->ControllerState.PSMoveState.bIsOrientationValid;

			// This call posts this pose to shared memory, where all clients will have access to it the next
			// moment they want to predict a pose.
			vr::VRServerDriverHost()->TrackedDevicePoseUpdated(m_unSteamVRTrackedDeviceId, m_Pose, sizeof(vr::DriverPose_t));
		} break;
		case PSMControllerType::PSMController_DualShock4:
		{
			const PSMDualShock4 &view = m_PSMControllerView->ControllerState.PSDS4State;

			// No prediction since that's already handled in the psmove service
			m_Pose.poseTimeOffset = 0.f;

			// Rotate -90 degrees about the x-axis from the current HMD orientation
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

			// Set rotational coordinates
			{
				const PSMQuatf &orientation = view.Pose.Orientation;

				m_Pose.qRotation.w = orientation.w;
				m_Pose.qRotation.x = orientation.x;
				m_Pose.qRotation.y = orientation.y;
				m_Pose.qRotation.z = orientation.z;
			}

			// Set the physics state of the controller
			// TODO: Physics data is too noisy for the DS4 right now, causes jitter
			{
				const PSMPhysicsData &physicsData = view.PhysicsData;

				m_Pose.vecVelocity[0] = 0.f; // physicsData.Velocity.i * k_fScalePSMoveAPIToMeters;
				m_Pose.vecVelocity[1] = 0.f; // physicsData.Velocity.j * k_fScalePSMoveAPIToMeters;
				m_Pose.vecVelocity[2] = 0.f; // physicsData.Velocity.k * k_fScalePSMoveAPIToMeters;

				m_Pose.vecAcceleration[0] = 0.f; // physicsData.Acceleration.i * k_fScalePSMoveAPIToMeters;
				m_Pose.vecAcceleration[1] = 0.f; // physicsData.Acceleration.j * k_fScalePSMoveAPIToMeters;
				m_Pose.vecAcceleration[2] = 0.f; // physicsData.Acceleration.k * k_fScalePSMoveAPIToMeters;

				m_Pose.vecAngularVelocity[0] = 0.f; // physicsData.AngularVelocity.i;
				m_Pose.vecAngularVelocity[1] = 0.f; // physicsData.AngularVelocity.j;
				m_Pose.vecAngularVelocity[2] = 0.f; // physicsData.AngularVelocity.k;

				m_Pose.vecAngularAcceleration[0] = 0.f; // physicsData.AngularAcceleration.i;
				m_Pose.vecAngularAcceleration[1] = 0.f; // physicsData.AngularAcceleration.j;
				m_Pose.vecAngularAcceleration[2] = 0.f; // physicsData.AngularAcceleration.k;
			}

			m_Pose.poseIsValid =
				m_PSMControllerView->ControllerState.PSDS4State.bIsPositionValid &&
				m_PSMControllerView->ControllerState.PSDS4State.bIsOrientationValid;

			// This call posts this pose to shared memory, where all clients will have access to it the next
			// moment they want to predict a pose.
			vr::VRServerDriverHost()->TrackedDevicePoseUpdated(m_unSteamVRTrackedDeviceId, m_Pose, sizeof(vr::DriverPose_t));
		} break;
		case PSMControllerType::PSMController_Virtual:
		{
			const PSMVirtualController &view = m_PSMControllerView->ControllerState.VirtualController;

			// No prediction since that's already handled in the psmove service
			m_Pose.poseTimeOffset = 0.f;

			// No transform due to the current HMD orientation
			m_Pose.qDriverFromHeadRotation.w = 1.f;
			m_Pose.qDriverFromHeadRotation.x = 0.0f;
			m_Pose.qDriverFromHeadRotation.y = 0.0f;
			m_Pose.qDriverFromHeadRotation.z = 0.0f;
			m_Pose.vecDriverFromHeadTranslation[0] = 0.f;
			m_Pose.vecDriverFromHeadTranslation[1] = 0.f;
			m_Pose.vecDriverFromHeadTranslation[2] = 0.f;

			// Set position
			const PSMVector3f psm_hand_position_meters = PSM_Vector3fScale(&view.Pose.Position, k_fScalePSMoveAPIToMeters);

			m_Pose.vecPosition[0] = psm_hand_position_meters.x;
			m_Pose.vecPosition[1] = psm_hand_position_meters.y;
			m_Pose.vecPosition[2] = psm_hand_position_meters.z;

			// Compute the orientation of the controller
			PSMQuatf orientation = view.Pose.Orientation;

			if (m_orientationSolver != nullptr)
			{
				vr::TrackedDeviceIndex_t hmd_device_index = vr::k_unTrackedDeviceIndexInvalid;

				if (Utils::GetHMDDeviceIndex(&hmd_device_index))
				{
					PSMPosef openvr_hmd_pose_meters;

					if (Utils::GetTrackedDevicePose(hmd_device_index, &openvr_hmd_pose_meters))
					{
						// Convert the HMD pose that's in OpenVR tracking space into PSM tracking space.
						// The HMD alignment calibration already gave us the tracking space conversion.
						const PSMPosef psmToOpenVRPose = GetWorldFromDriverPose();
						const PSMPosef openVRToPsmPose = PSM_PosefInverse(&psmToOpenVRPose);
						PSMPosef psm_hmd_pose_meters = PSM_PosefConcat(&openvr_hmd_pose_meters, &openVRToPsmPose);

						orientation = m_orientationSolver->solveHandOrientation(psm_hmd_pose_meters, psm_hand_position_meters);
					}
				}
			}

			// Set rotational coordinates
			m_Pose.qRotation.w = m_fVirtuallyRotateController ? -orientation.w : orientation.w;
			m_Pose.qRotation.x = orientation.x;
			m_Pose.qRotation.y = orientation.y;
			m_Pose.qRotation.z = m_fVirtuallyRotateController ? -orientation.z : orientation.z;

			// virtual extend controller position
			if (m_fVirtuallExtendControllersYMeters != 0.0f || m_fVirtuallExtendControllersZMeters != 0.0f)
			{
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

				m_Pose.vecAngularVelocity[0] = 0.f;
				m_Pose.vecAngularVelocity[1] = 0.f;
				m_Pose.vecAngularVelocity[2] = 0.f;

				m_Pose.vecAngularAcceleration[0] = 0.f;
				m_Pose.vecAngularAcceleration[1] = 0.f;
				m_Pose.vecAngularAcceleration[2] = 0.f;
			}

			m_Pose.poseIsValid = m_PSMControllerView->ControllerState.VirtualController.bIsPositionValid;

			// This call posts this pose to shared memory, where all clients will have access to it the next
			// moment they want to predict a pose.
			vr::VRServerDriverHost()->TrackedDevicePoseUpdated(m_unSteamVRTrackedDeviceId, m_Pose, sizeof(vr::DriverPose_t));
		} break;
		}
	}

	void CPSMoveControllerLatest::UpdateRumbleState()
	{
		if (!m_bRumbleSuppressed)
		{
			const float k_max_rumble_update_rate = 33.f; // Don't bother trying to update the rumble faster than 30fps (33ms)
			const float k_max_pulse_microseconds = 1000.f; // Docs suggest max pulse duration of 5ms, but we'll call 1ms max

			std::chrono::time_point<std::chrono::high_resolution_clock> now = std::chrono::high_resolution_clock::now();
			bool bTimoutElapsed = true;

			if (m_lastTimeRumbleSentValid)
			{
				std::chrono::duration<double, std::milli> timeSinceLastSend = now - m_lastTimeRumbleSent;

				bTimoutElapsed = timeSinceLastSend.count() >= k_max_rumble_update_rate;
			}

			// See if a rumble request hasn't come too recently
			if (bTimoutElapsed)
			{
				float rumble_fraction = static_cast<float>(m_pendingHapticPulseDuration) / k_max_pulse_microseconds;

				// Unless a zero rumble intensity was explicitly set, 
				// don't rumble less than 35% (no enough to feel)
				if (m_pendingHapticPulseDuration != 0)
				{
					if (rumble_fraction < 0.35f)
					{
						// rumble values less 35% isn't noticeable
						rumble_fraction = 0.35f;
					}
				}

				// Keep the pulse intensity within reasonable bounds
				if (rumble_fraction > 1.f)
				{
					rumble_fraction = 1.f;
				}

				// Actually send the rumble to the server
				PSM_SetControllerRumble(m_PSMControllerView->ControllerID, PSMControllerRumbleChannel_All, rumble_fraction);

				// Remember the last rumble we went and when we sent it
				m_lastTimeRumbleSent = now;
				m_lastTimeRumbleSentValid = true;

				// Reset the pending haptic pulse duration.
				// If another call to TriggerHapticPulse() is made later, it will stomp this value.
				// If no call to TriggerHapticPulse() is made later, then the next call to UpdateRumbleState()
				// in k_max_rumble_update_rate milliseconds will set the rumble_fraction to 0.f
				// This effectively makes the shortest rumble pulse k_max_rumble_update_rate milliseconds.
				m_pendingHapticPulseDuration = 0;
			}
		}
		else
		{
			// Reset the pending haptic pulse duration since rumble is suppressed.
			m_pendingHapticPulseDuration = 0;
		}
	}

	void CPSMoveControllerLatest::UpdateBatteryChargeState(
		PSMBatteryState newBatteryEnum)
	{
		bool bIsBatteryCharging = false;
		float fBatteryChargeFraction = 0.f;

		switch (newBatteryEnum)
		{
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

		if (bIsBatteryCharging != m_bIsBatteryCharging)
		{
			m_bIsBatteryCharging = bIsBatteryCharging;
			vr::VRProperties()->SetBoolProperty(m_ulPropertyContainer, vr::Prop_DeviceIsCharging_Bool, m_bIsBatteryCharging);
		}

		if (fBatteryChargeFraction != m_fBatteryChargeFraction)
		{
			m_fBatteryChargeFraction = fBatteryChargeFraction;
			vr::VRProperties()->SetFloatProperty(m_ulPropertyContainer, vr::Prop_DeviceBatteryPercentage_Float, m_fBatteryChargeFraction);
		}
	}

	void CPSMoveControllerLatest::Update()
	{
		CPSMoveTrackedDeviceLatest::Update();

		if (IsActivated() && m_PSMControllerView->IsConnected)
		{
			int seq_num = m_PSMControllerView->OutputSequenceNum;

			// Only other updating incoming state if it actually changed
			if (m_nPoseSequenceNumber != seq_num)
			{
				m_nPoseSequenceNumber = seq_num;

				UpdateTrackingState();
				UpdateControllerState();
			}

			// Update the outgoing state
			UpdateRumbleState();
		}
	}

	void CPSMoveControllerLatest::RefreshWorldFromDriverPose()
	{
		CPSMoveTrackedDeviceLatest::RefreshWorldFromDriverPose();

		// Mark the calibration process as done
		// once we have setup the world from driver pose
		m_trackingStatus = vr::TrackingResult_Running_OK;
	}

	bool CPSMoveControllerLatest::AttachChildPSMController(
		int ChildControllerId,
		PSMControllerType ChildControllerType,
		const std::string &ChildControllerSerialNo)
	{
		bool bSuccess = false;

		if (m_nPSMChildControllerId == -1 &&
			m_nPSMChildControllerId != m_nPSMControllerId &&
			PSM_AllocateControllerListener(ChildControllerId) == PSMResult_Success)
		{
			m_nPSMChildControllerId = ChildControllerId;
			m_PSMChildControllerType = ChildControllerType;
			m_PSMChildControllerView = PSM_GetController(m_nPSMChildControllerId);

			PSMRequestID request_id;
			if (PSM_StartControllerDataStreamAsync(ChildControllerId, PSMStreamFlags_defaultStreamOptions, &request_id))
			{
				PSM_RegisterCallback(request_id, CPSMoveControllerLatest::start_controller_response_callback, this);
				bSuccess = true;
			}
			else
			{
				PSM_FreeControllerListener(ChildControllerId);
			}
		}

		return bSuccess;
	}
}
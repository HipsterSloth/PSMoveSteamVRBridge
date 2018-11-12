#pragma once
#include "openvr_driver.h"

namespace steamvrbridge {
	//==================================================================================================
	// Constants
	//==================================================================================================
	static const float k_fScalePSMoveAPIToMeters = 0.01f;  // psmove driver in cm
	static const float k_fRadiansToDegrees = 180.f / 3.14159265f;

	static const int k_touchpadTouchMapping = (vr::EVRButtonId)31;
	static const float k_defaultThumbstickDeadZoneRadius = 0.1f;

	static const float DEFAULT_HAPTIC_DURATION = 0.f;
	static const float DEFAULT_HAPTIC_AMPLITUDE = 1.f;
	static const float DEFAULT_HAPTIC_FREQUENCY = 200.f;

	/* PSMoveService button IDs*/
	enum ePSButtonID {
		k_EPSButtonID_0,
		k_EPSButtonID_1,
		k_EPSButtonID_2,
		k_EPSButtonID_3,
		k_EPSButtonID_4,
		k_EPSButtonID_5,
		k_EPSButtonID_6,
		k_EPSButtonID_7,
		k_EPSButtonID_8,
		k_EPSButtonID_9,
		k_EPSButtonID_10,
		k_EPSButtonID_11,
		k_EPSButtonID_12,
		k_EPSButtonID_13,
		k_EPSButtonID_14,
		k_EPSButtonID_15,
		k_EPSButtonID_16,
		k_EPSButtonID_17,
		k_EPSButtonID_18,
		k_EPSButtonID_19,
		k_EPSButtonID_20,
		k_EPSButtonID_21,
		k_EPSButtonID_22,
		k_EPSButtonID_23,
		k_EPSButtonID_24,
		k_EPSButtonID_25,
		k_EPSButtonID_26,
		k_EPSButtonID_27,
		k_EPSButtonID_28,
		k_EPSButtonID_29,
		k_EPSButtonID_30,
		k_EPSButtonID_31,

		k_EPSButtonID_Count,

		k_EPSButtonID_PS = k_EPSButtonID_0,
		k_EPSButtonID_Left = k_EPSButtonID_1,
		k_EPSButtonID_Up = k_EPSButtonID_2,
		k_EPSButtonID_Right = k_EPSButtonID_3,
		k_EPSButtonID_Down = k_EPSButtonID_4,
		k_EPSButtonID_Move = k_EPSButtonID_5,
		k_EPSButtonID_Trackpad = k_EPSButtonID_6,
		k_EPSButtonID_Trigger = k_EPSButtonID_7,
		k_EPSButtonID_Triangle = k_EPSButtonID_8,
		k_EPSButtonID_Square = k_EPSButtonID_9,
		k_EPSButtonID_Circle = k_EPSButtonID_10,
		k_EPSButtonID_Cross = k_EPSButtonID_11,
		k_EPSButtonID_Select = k_EPSButtonID_12,
		k_EPSButtonID_Share = k_EPSButtonID_13,
		k_EPSButtonID_Start = k_EPSButtonID_14,
		k_EPSButtonID_Options = k_EPSButtonID_15,
		k_EPSButtonID_L1 = k_EPSButtonID_16,
		k_EPSButtonID_L2 = k_EPSButtonID_17,
		k_EPSButtonID_L3 = k_EPSButtonID_18,
		k_EPSButtonID_R1 = k_EPSButtonID_19,
		k_EPSButtonID_R2 = k_EPSButtonID_20,
		k_EPSButtonID_R3 = k_EPSButtonID_21,
	};

	static const char *k_PSButtonNames[k_EPSButtonID_Count] = {
		"ps",
		"left",
		"up",
		"down",
		"right",
		"move",
		"trackpad",
		"trigger",
		"triangle",
		"square",
		"circle",
		"cross",
		"select",
		"share",
		"start",
		"options",
		"l1",
		"l2",
		"l3",
		"r1",
		"r2",
		"r3",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
	};

	static const char *k_VirtualButtonNames[k_EPSButtonID_Count] = {
		"gamepad_button_0",
		"gamepad_button_1",
		"gamepad_button_2",
		"gamepad_button_3",
		"gamepad_button_4",
		"gamepad_button_5",
		"gamepad_button_6",
		"gamepad_button_7",
		"gamepad_button_8",
		"gamepad_button_9",
		"gamepad_button_10",
		"gamepad_button_11",
		"gamepad_button_12",
		"gamepad_button_13",
		"gamepad_button_14",
		"gamepad_button_15",
		"gamepad_button_16",
		"gamepad_button_17",
		"gamepad_button_18",
		"gamepad_button_19",
		"gamepad_button_20",
		"gamepad_button_21",
		"gamepad_button_22",
		"gamepad_button_23",
		"gamepad_button_24",
		"gamepad_button_25",
		"gamepad_button_26",
		"gamepad_button_27",
		"gamepad_button_28",
		"gamepad_button_29",
		"gamepad_button_30",
		"gamepad_button_31"
	};

	static const int k_max_vr_buttons = 37;
	static const char *k_VRButtonNames[k_max_vr_buttons] = {
		"system",               // k_EButton_System
		"application_menu",     // k_EButton_ApplicationMenu
		"grip",                 // k_EButton_Grip
		"dpad_left",            // k_EButton_DPad_Left
		"dpad_up",              // k_EButton_DPad_Up
		"dpad_right",           // k_EButton_DPad_Right
		"dpad_down",            // k_EButton_DPad_Down
		"a",                    // k_EButton_A
		"button_8",              // (vr::EVRButtonId)8
		"button_9",              // (vr::EVRButtonId)9
		"button_10",              // (vr::EVRButtonId)10
		"button_11",              // (vr::EVRButtonId)11
		"button_12",              // (vr::EVRButtonId)12
		"button_13",              // (vr::EVRButtonId)13
		"button_14",              // (vr::EVRButtonId)14
		"button_15",              // (vr::EVRButtonId)15
		"button_16",              // (vr::EVRButtonId)16
		"button_17",              // (vr::EVRButtonId)17
		"button_18",              // (vr::EVRButtonId)18
		"button_19",              // (vr::EVRButtonId)19
		"button_20",              // (vr::EVRButtonId)20
		"button_21",              // (vr::EVRButtonId)21
		"button_22",              // (vr::EVRButtonId)22
		"button_23",              // (vr::EVRButtonId)23
		"button_24",              // (vr::EVRButtonId)24
		"button_25",              // (vr::EVRButtonId)25
		"button_26",              // (vr::EVRButtonId)26
		"button_27",              // (vr::EVRButtonId)27
		"button_28",              // (vr::EVRButtonId)28
		"button_29",              // (vr::EVRButtonId)29
		"button_30",              // (vr::EVRButtonId)30
		"touchpad_touched",       // (vr::EVRButtonId)31 used to map to touchpad touched state in vr
		"touchpad",               // k_EButton_Axis0, k_EButton_SteamVR_Touchpad
		"trigger",                // k_EButton_Axis1, k_EButton_SteamVR_Trigger
		"axis_2",                 // k_EButton_Axis2
		"axis_3",                 // k_EButton_Axis3
		"axis_4",                 // k_EButton_Axis4
	};

	enum eVRTouchpadDirection {
		k_EVRTouchpadDirection_None,

		k_EVRTouchpadDirection_Left,
		k_EVRTouchpadDirection_Up,
		k_EVRTouchpadDirection_Right,
		k_EVRTouchpadDirection_Down,

		k_EVRTouchpadDirection_UpLeft,
		k_EVRTouchpadDirection_UpRight,
		k_EVRTouchpadDirection_DownLeft,
		k_EVRTouchpadDirection_DownRight,

		k_EVRTouchpadDirection_Count
	};

	static const int k_max_vr_touchpad_directions = k_EVRTouchpadDirection_Count;
	static const char *k_VRTouchpadDirectionNames[k_max_vr_touchpad_directions] = {
		"none",
		"touchpad_left",
		"touchpad_up",
		"touchpad_right",
		"touchpad_down",
		"touchpad_up-left",
		"touchpad_up-right",
		"touchpad_down-left",
		"touchpad_down-right",
	};
}
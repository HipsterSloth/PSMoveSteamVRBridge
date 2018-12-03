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
	enum ePSMButtonID {
		/* Special-Case System Button (bound to PS button by default) */
		k_PSMButtonID_System,
		
		/* Common Buttons */
		k_PSMButtonID_PS,
		k_PSMButtonID_Triangle,
		k_PSMButtonID_Circle,
		k_PSMButtonID_Cross,
		k_PSMButtonID_Square,
		k_PSMButtonID_DPad_Up,
		k_PSMButtonID_DPad_Down,
		k_PSMButtonID_DPad_Left,
		k_PSMButtonID_DPad_Right,

		/* PSMove Specific Buttons */
		k_PSMButtonID_Move,
		k_PSMButtonID_Select,
		k_PSMButtonID_Start,

		/* PSNavi Specific Buttons */
		k_PSMButtonID_Shoulder,
		k_PSMButtonID_Joystick,

		/* Dualshock4 Specific Buttons */
		k_PSMButtonID_Options,
		k_PSMButtonID_Share,
		k_PSMButtonID_Touchpad,
		k_PSMButtonID_LeftJoystick,
		k_PSMButtonID_RightJoystick,
		k_PSMButtonID_LeftShoulder,
		k_PSMButtonID_RightShoulder,

		/* Emulated Trackpad Buttons */
		k_PSMButtonID_EmulatedTrackpadTouched,
		k_PSMButtonID_EmulatedTrackpadPressed,

		/* Virtual Controller Specific Buttons */
		k_PSMButtonID_Virtual_0,
		k_PSMButtonID_Virtual_1,
		k_PSMButtonID_Virtual_2,
		k_PSMButtonID_Virtual_3,
		k_PSMButtonID_Virtual_4,
		k_PSMButtonID_Virtual_5,
		k_PSMButtonID_Virtual_6,
		k_PSMButtonID_Virtual_7,
		k_PSMButtonID_Virtual_8,
		k_PSMButtonID_Virtual_9,
		k_PSMButtonID_Virtual_10,
		k_PSMButtonID_Virtual_11,
		k_PSMButtonID_Virtual_12,
		k_PSMButtonID_Virtual_13,
		k_PSMButtonID_Virtual_14,
		k_PSMButtonID_Virtual_15,
		k_PSMButtonID_Virtual_16,
		k_PSMButtonID_Virtual_17,
		k_PSMButtonID_Virtual_18,
		k_PSMButtonID_Virtual_19,
		k_PSMButtonID_Virtual_20,
		k_PSMButtonID_Virtual_21,
		k_PSMButtonID_Virtual_22,
		k_PSMButtonID_Virtual_23,
		k_PSMButtonID_Virtual_24,
		k_PSMButtonID_Virtual_25,
		k_PSMButtonID_Virtual_26,
		k_PSMButtonID_Virtual_27,
		k_PSMButtonID_Virtual_28,
		k_PSMButtonID_Virtual_29,
		k_PSMButtonID_Virtual_30,
		k_PSMButtonID_Virtual_31,

		k_PSMButtonID_Count
	};

	static const char *k_PSMButtonNames[k_PSMButtonID_Count] = {
		/* System Button */
		"system",

		/* Common Buttons */
		"ps",
		"triangle",
		"circle",
		"cross",
		"square",
		"dpad_up",
		"dpad_down",
		"dpad_right",
		"dpad_left",

		/* PSMove Specific Buttons */
		"move",
		"select",
		"start",

		/* PSNavi Specific Buttons */
		"shoulder",
		"joystick",

		/* Dualshock4 Specific Buttons */
		"options",
		"share",
		"touchpad",
		"joystick_left",
		"joystick_right",
		"shoulder_left",
		"shoulder_right",

		/* Emulated Trackpad Buttons */
		"emulated_trackpad_touched",
		"emulated_trackpad_pressed",

		/* Virtual Controller Specific Buttons */
		"virtual_button_0",
		"virtual_button_1",
		"virtual_button_2",
		"virtual_button_3",
		"virtual_button_4",
		"virtual_button_5",
		"virtual_button_6",
		"virtual_button_7",
		"virtual_button_8",
		"virtual_button_9",
		"virtual_button_10",
		"virtual_button_11",
		"virtual_button_12",
		"virtual_button_13",
		"virtual_button_14",
		"virtual_button_15",
		"virtual_button_16",
		"virtual_button_17",
		"virtual_button_18",
		"virtual_button_19",
		"virtual_button_20",
		"virtual_button_21",
		"virtual_button_22",
		"virtual_button_23",
		"virtual_button_24",
		"virtual_button_25",
		"virtual_button_26",
		"virtual_button_27",
		"virtual_button_28",
		"virtual_button_29",
		"virtual_button_30",
		"virtual_button_31"
	};

	static const char *k_PSMButtonPaths[k_PSMButtonID_Count] = {
		/* System Button */
		"/input/system/click",

		/* Common Buttons */
		"/input/ps/click",
		"/input/triangle/click",
		"/input/circle/click",
		"/input/cross/click",
		"/input/square/click",
		"/input/dpad_up/click",
		"/input/dpad_down/click",
		"/input/dpad_right/click",
		"/input/dpad_left/click",

		/* PSMove Specific Buttons */
		"/input/move/click",
		"/input/select/click",
		"/input/start/click",

		/* PSNavi Specific Buttons */
		"/input/shoulder/click",
		"/input/joystick/click",

		/* Dualshock4 Specific Buttons */
		"/input/options/click",
		"/input/share/click",
		"/input/touchpad/click",
		"/input/joystick_left/click",
		"/input/joystick_right/click",
		"/input/shoulder_left/click",
		"/input/shoulder_right/click",

		/* Emulated Trackpad Buttons */
		"/input/trackpad/touch",
		"/input/trackpad/click",

		/* Virtual Controller Specific Buttons */
		"/input/virtual_button_0/click",
		"/input/virtual_button_1/click",
		"/input/virtual_button_2/click",
		"/input/virtual_button_3/click",
		"/input/virtual_button_4/click",
		"/input/virtual_button_5/click",
		"/input/virtual_button_6/click",
		"/input/virtual_button_7/click",
		"/input/virtual_button_8/click",
		"/input/virtual_button_9/click",
		"/input/virtual_button_10/click",
		"/input/virtual_button_11/click",
		"/input/virtual_button_12/click",
		"/input/virtual_button_13/click",
		"/input/virtual_button_14/click",
		"/input/virtual_button_15/click",
		"/input/virtual_button_16/click",
		"/input/virtual_button_17/click",
		"/input/virtual_button_18/click",
		"/input/virtual_button_19/click",
		"/input/virtual_button_20/click",
		"/input/virtual_button_21/click",
		"/input/virtual_button_22/click",
		"/input/virtual_button_23/click",
		"/input/virtual_button_24/click",
		"/input/virtual_button_25/click",
		"/input/virtual_button_26/click",
		"/input/virtual_button_27/click",
		"/input/virtual_button_28/click",
		"/input/virtual_button_29/click",
		"/input/virtual_button_30/click",
		"/input/virtual_button_31/click"
	};
	

	enum ePSMAxisID {
		/* Common Axes */
		k_PSMAxisID_Trigger,

		/* PSNavi Specific Axes */
		k_PSMAxisID_Joystick_X,
		k_PSMAxisID_Joystick_Y,

		/* Dualshock4 Specific Axes */
		k_PSMAxisID_LeftTrigger,
		k_PSMAxisID_RightTrigger,
		k_PSMAxisID_LeftJoystick_X,
		k_PSMAxisID_LeftJoystick_Y,
		k_PSMAxisID_RightJoystick_X,
		k_PSMAxisID_RightJoystick_Y,

		/* Emulated Trackpad Specific Axes */
		k_PSMAxisID_EmulatedTrackpad_X,
		k_PSMAxisID_EmulatedTrackpad_Y,

		/* Virtual Controller Specific Axes */
		k_PSMAxisID_Virtual_0,
		k_PSMAxisID_Virtual_1,
		k_PSMAxisID_Virtual_2,
		k_PSMAxisID_Virtual_3,
		k_PSMAxisID_Virtual_4,
		k_PSMAxisID_Virtual_5,
		k_PSMAxisID_Virtual_6,
		k_PSMAxisID_Virtual_7,
		k_PSMAxisID_Virtual_8,
		k_PSMAxisID_Virtual_9,
		k_PSMAxisID_Virtual_10,
		k_PSMAxisID_Virtual_11,
		k_PSMAxisID_Virtual_12,
		k_PSMAxisID_Virtual_13,
		k_PSMAxisID_Virtual_14,
		k_PSMAxisID_Virtual_15,
		k_PSMAxisID_Virtual_16,
		k_PSMAxisID_Virtual_17,
		k_PSMAxisID_Virtual_18,
		k_PSMAxisID_Virtual_19,
		k_PSMAxisID_Virtual_20,
		k_PSMAxisID_Virtual_21,
		k_PSMAxisID_Virtual_22,
		k_PSMAxisID_Virtual_23,
		k_PSMAxisID_Virtual_24,
		k_PSMAxisID_Virtual_25,
		k_PSMAxisID_Virtual_26,
		k_PSMAxisID_Virtual_27,
		k_PSMAxisID_Virtual_28,
		k_PSMAxisID_Virtual_29,
		k_PSMAxisID_Virtual_30,
		k_PSMAxisID_Virtual_31,

		k_PSMAxisID_Count
	};

	static bool k_PSMAxisTwoSided[k_PSMAxisID_Count] = {
		/* Common Axes */
		false, // trigger

		/* PSNavi Specific Axes */
		true, // joystick x
		true, // joystick y

		/* Dualshock4 Specific Axes */
		false, // trigger left
		false, // trigger right
		true, // joystick left x
		true, // joystick left y
		true, // joystick right x
		true, // joystick right x

		/* Emulated Trackpad Specific Axes */
		true, // emulated trackpad x
		true, // emulated trackpad y

		/* Virtual Controller Specific Axes */
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false,  // virtual axis
		false  // virtual axis
	};

	static const char *k_PSMAxisPaths[k_PSMAxisID_Count] = {
		/* Common Axes */
		"/input/trigger/value",

		/* PSNavi Specific Axes */
		"/input/joystick/x",
		"/input/joystick/y",

		/* Dualshock4 Specific Axes */
		"/input/trigger_left/value",
		"/input/trigger_right/value",
		"/input/joystick_left/x",
		"/input/joystick_left/y",
		"/input/joystick_right/x",
		"/input/joystick_right/y",

		/* Emulated Trackpad Specific Axes */
		"/input/trackpad/x",
		"/input/trackpad/y",

		/* Virtual Controller Specific Axes */
		"/input/virtual_axis_0/value",
		"/input/virtual_axis_1/value",
		"/input/virtual_axis_2/value",
		"/input/virtual_axis_3/value",
		"/input/virtual_axis_4/value",
		"/input/virtual_axis_5/value",
		"/input/virtual_axis_6/value",
		"/input/virtual_axis_7/value",
		"/input/virtual_axis_8/value",
		"/input/virtual_axis_9/value",
		"/input/virtual_axis_10/value",
		"/input/virtual_axis_11/value",
		"/input/virtual_axis_12/value",
		"/input/virtual_axis_13/value",
		"/input/virtual_axis_14/value",
		"/input/virtual_axis_15/value",
		"/input/virtual_axis_16/value",
		"/input/virtual_axis_17/value",
		"/input/virtual_axis_18/value",
		"/input/virtual_axis_19/value",
		"/input/virtual_axis_20/value",
		"/input/virtual_axis_21/value",
		"/input/virtual_axis_22/value",
		"/input/virtual_axis_23/value",
		"/input/virtual_axis_24/value",
		"/input/virtual_axis_25/value",
		"/input/virtual_axis_26/value",
		"/input/virtual_axis_27/value",
		"/input/virtual_axis_28/value",
		"/input/virtual_axis_29/value",
		"/input/virtual_axis_30/value",
		"/input/virtual_axis_31/value"
	};

	enum ePSMHapicID {
		/* Common Rumble */
		k_PSMHapticID_Rumble,

		/* Dualshock4 Specific Rumble */
		k_PSMHapticID_LeftRumble,
		k_PSMHapticID_RightRumble,

		k_PSMHapticID_Count
	};

	static const char *k_PSMHapticPaths[k_PSMHapticID_Count] = {
		/* Common Rumble */
		"/output/haptic",

		/* Dualshock4 Specific Rumble */
		"/output/left_rumble",
		"/output/right_rumble",
	};

	enum eEmulatedTrackpadAction {
		k_EmulatedTrackpadAction_None,

		k_EmulatedTrackpadAction_Touch,
		k_EmulatedTrackpadAction_Press,

		k_EmulatedTrackpadAction_Left,
		k_EmulatedTrackpadAction_Up,
		k_EmulatedTrackpadAction_Right,
		k_EmulatedTrackpadAction_Down,

		k_EmulatedTrackpadAction_UpLeft,
		k_EmulatedTrackpadAction_UpRight,
		k_EmulatedTrackpadAction_DownLeft,
		k_EmulatedTrackpadAction_DownRight,

		k_EmulatedTrackpadAction_Count
	};

	static const int k_max_vr_touchpad_actions = k_EmulatedTrackpadAction_Count;
	static const char *k_VRTouchpadActionNames[k_max_vr_touchpad_actions] = {
		"none",
		"touchpad_touch",
		"touchpad_press",
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    /* PSMoveService button IDs*/
    public enum ePSMButtonID
    {
        INVALID= -1,

        /* Special-Case System Button (bound to PS button by default) */
        System,

        /* Common Buttons */
        PS,
        Triangle,
        Circle,
        Cross,
        Square,
        DPad_Up,
        DPad_Down,
        DPad_Left,
        DPad_Right,

        /* PSMove Specific Buttons */
        Move,
        Select,
        Start,

        /* PSNavi Specific Buttons */
        Shoulder,
        Joystick,

        /* Dualshock4 Specific Buttons */
        Options,
        Share,
        Touchpad,
        LeftJoystick,
        RightJoystick,
        LeftShoulder,
        RightShoulder,

        /* Emulated Trackpad Buttons */
        EmulatedTrackpadTouched,
        EmulatedTrackpadPressed,

        /* Virtual Controller Specific Buttons */
        Virtual_0,
        Virtual_1,
        Virtual_2,
        Virtual_3,
        Virtual_4,
        Virtual_5,
        Virtual_6,
        Virtual_7,
        Virtual_8,
        Virtual_9,
        Virtual_10,
        Virtual_11,
        Virtual_12,
        Virtual_13,
        Virtual_14,
        Virtual_15,
        Virtual_16,
        Virtual_17,
        Virtual_18,
        Virtual_19,
        Virtual_20,
        Virtual_21,
        Virtual_22,
        Virtual_23,
        Virtual_24,
        Virtual_25,
        Virtual_26,
        Virtual_27,
        Virtual_28,
        Virtual_29,
        Virtual_30,
        Virtual_31,

        Count
    };

    public enum ePSMAxisID
    {
        /* Common Axes */
        Trigger,

        /* PSNavi Specific Axes */
        Joystick_X,
        Joystick_Y,

        /* Dualshock4 Specific Axes */
        LeftTrigger,
        RightTrigger,
        LeftJoystick_X,
        LeftJoystick_Y,
        RightJoystick_X,
        RightJoystick_Y,

        /* Emulated Trackpad Specific Axes */
        EmulatedTrackpad_X,
        EmulatedTrackpad_Y,

        /* Virtual Controller Specific Axes */
        Virtual_0,
        Virtual_1,
        Virtual_2,
        Virtual_3,
        Virtual_4,
        Virtual_5,
        Virtual_6,
        Virtual_7,
        Virtual_8,
        Virtual_9,
        Virtual_10,
        Virtual_11,
        Virtual_12,
        Virtual_13,
        Virtual_14,
        Virtual_15,
        Virtual_16,
        Virtual_17,
        Virtual_18,
        Virtual_19,
        Virtual_20,
        Virtual_21,
        Virtual_22,
        Virtual_23,
        Virtual_24,
        Virtual_25,
        Virtual_26,
        Virtual_27,
        Virtual_28,
        Virtual_29,
        Virtual_30,
        Virtual_31,

        Count
    };

    public enum eEmulatedTrackpadAction
    {
        None,

        Touch,
        Press,

        Left,
        Up,
        Right,
        Down,

        UpLeft,
        UpRight,
        DownLeft,
        DownRight,

        Count
    };

    public class Constants
    {
        public static string[] PSMButtonNames = new string[(int)ePSMButtonID.Count] {
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

        public static string[] VRTouchpadActionNames= new string[(int)eEmulatedTrackpadAction.Count] {
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

        public static Dictionary<string, eEmulatedTrackpadAction> MakeTrackpadActionDictionary()
        {
            Dictionary<string, eEmulatedTrackpadAction> dictionary = new Dictionary<string, eEmulatedTrackpadAction>();

            for (int action_index = 0; action_index < (int)eEmulatedTrackpadAction.Count; ++action_index) {
                dictionary.Add(VRTouchpadActionNames[action_index], (eEmulatedTrackpadAction)action_index);
            }

            return dictionary;
        }
    }
}

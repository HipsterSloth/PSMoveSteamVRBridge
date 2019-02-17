using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSMoveService;

namespace SystemTrayApp
{
    public class VirtualControllerConfig : ControllerConfig
    {
        public VirtualControllerConfig(string controller_serial) : base("virtual_controller_"+controller_serial.ToUpper())
        {
            setTrackpadActionForButton(ePSMButtonID.Move, eEmulatedTrackpadAction.Press);
        }

        // Virtual extend controller in meters.
        float extend_Y_meters;
        float extend_Z_meters;
        public float ExtendYMeters
        {
            get { return extend_Y_meters; }
            set { extend_Y_meters = value; IsDirty = true; }
        }
        public float ExtendZMeters
        {
            get { return extend_Z_meters; }
            set { extend_Z_meters = value; IsDirty = true; }
        }

        // Rotate controllers orientation 90 degrees about the z-axis (for gun style games).
        bool z_rotate_90_degrees;
        public bool ZRotate90Degrees
        {
            get { return z_rotate_90_degrees; }
            set { z_rotate_90_degrees = value; IsDirty = true; }
        }

        // Delay in resetting touchpad position after touchpad press.
        bool delay_after_touchpad_press;
        public bool DelayAfterTouchpadPress
        {
            get { return delay_after_touchpad_press; }
            set { delay_after_touchpad_press = value; IsDirty = true; }
        }

        // Settings values. Used to determine whether we'll map controller movement after touchpad
        // presses to touchpad axis values.
        float meters_per_touchpad_axis_units;
        public float MetersPerTouchpadAxisUnits
        {
            get { return meters_per_touchpad_axis_units; }
            set { meters_per_touchpad_axis_units = value; IsDirty = true; }
        }

        // The axis to use for trigger input
        int steamvr_trigger_axis_index;
        public int SteamVRTriggerAxisIndex
        {
            get { return steamvr_trigger_axis_index; }
            set { steamvr_trigger_axis_index = value; IsDirty = true; }
        }

        // The axis to use for trigger input
        int virtual_touchpad_XAxis_index;
        int virtual_touchpad_YAxis_index;
        public int VirtualTouchpadXAxisIndex
        {
            get { return virtual_touchpad_XAxis_index; }
            set { virtual_touchpad_XAxis_index = value; IsDirty = true; }
        }
        public int VirtualTouchpadYAxisIndex
        {
            get { return virtual_touchpad_YAxis_index; }
            set { virtual_touchpad_YAxis_index = value; IsDirty = true; }
        }

        // Rumble state
        float thumbstick_deadzone;
        public float ThumbstickDeadzone
        {
            get { return thumbstick_deadzone; }
            set { thumbstick_deadzone = value; IsDirty = true; }
        }

        // Treat a thumbstick touch also as a press
        bool thumbstick_touch_as_press;
        public bool ThumbstickTouchAsPress
        {
            get { return thumbstick_touch_as_press; }
            set { thumbstick_touch_as_press = value; IsDirty = true; }
        }

        // Settings values. Used to adjust throwing power using linear velocity and acceleration.
        float linear_velocity_multiplier;
        float linear_velocity_exponent;
        public float LinearVelocityMultiplier
        {
            get { return linear_velocity_multiplier; }
            set { linear_velocity_multiplier = value; IsDirty = true; }
        }
        public float LinearVelocityExponent
        {
            get { return linear_velocity_exponent; }
            set { linear_velocity_exponent = value; IsDirty = true; }
        }

        // The button to use as the system button
        ePSMButtonID system_button_id;
        public ePSMButtonID SystemButtonID
        {
            get { return system_button_id; }
            set { system_button_id = value; IsDirty = true; }
        }

        public override void WriteToJSON(JsonValue pt)
        {
            base.WriteToJSON(pt);

            pt["extend_Y_meters"] = extend_Y_meters;
            pt["extend_Z_meters"] = extend_Z_meters;
            pt["z_rotate_90_degrees"] = z_rotate_90_degrees;
            pt["delay_after_touchpad_press"] = delay_after_touchpad_press;
            pt["meters_per_touchpad_axis_units"] = meters_per_touchpad_axis_units;
            pt["steamvr_trigger_axis_index"] = steamvr_trigger_axis_index;
            pt["virtual_touchpad_XAxis_index"] = virtual_touchpad_XAxis_index;
            pt["virtual_touchpad_YAxis_index"] = virtual_touchpad_YAxis_index;
            pt["thumbstick_deadzone"] = thumbstick_deadzone;
            pt["thumbstick_touch_as_press"] = thumbstick_touch_as_press;
            pt["linear_velocity_multiplier"] = linear_velocity_multiplier;
            pt["linear_velocity_exponent"] = linear_velocity_exponent;

            // System button mapping
            pt["system_button"] = Constants.PSMButtonNames[(int)system_button_id];

            //PSMove controller button -> fake touchpad mappings
            for (int button_index = 0; button_index < PSMoveClient.PSM_MAX_VIRTUAL_CONTROLLER_BUTTONS; ++button_index) {
                WriteEmulatedTouchpadAction(pt, (ePSMButtonID)((int)ePSMButtonID.Virtual_0 + button_index));
            }
        }

        public override bool ReadFromJSON(JsonValue pt)
        {
            if (base.ReadFromJSON(pt)) {
                if (pt.ContainsKey("extend_Y_meters")) {
                    extend_Y_meters = pt["extend_Y_meters"];
                }
                if (pt.ContainsKey("extend_Z_meters")) {
                    extend_Z_meters = pt["extend_Z_meters"];
                }
                if (pt.ContainsKey("z_rotate_90_degrees")) {
                    z_rotate_90_degrees = pt["z_rotate_90_degrees"];
                }
                if (pt.ContainsKey("delay_after_touchpad_press")) {
                    delay_after_touchpad_press = pt["delay_after_touchpad_press"];
                }
                if (pt.ContainsKey("meters_per_touchpad_axis_units")) {
                    meters_per_touchpad_axis_units = pt["meters_per_touchpad_axis_units"];
                }
                if (pt.ContainsKey("steamvr_trigger_axis_index")) {
                    steamvr_trigger_axis_index = pt["steamvr_trigger_axis_index"];
                }
                if (pt.ContainsKey("virtual_touchpad_XAxis_index")) {
                    virtual_touchpad_XAxis_index = pt["virtual_touchpad_XAxis_index"];
                }
                if (pt.ContainsKey("virtual_touchpad_YAxis_index")) {
                    virtual_touchpad_YAxis_index = pt["virtual_touchpad_YAxis_index"];
                }
                if (pt.ContainsKey("thumbstick_deadzone")) {
                    thumbstick_deadzone = pt["thumbstick_deadzone"];
                }
                if (pt.ContainsKey("thumbstick_touch_as_press")) {
                    thumbstick_touch_as_press = pt["thumbstick_touch_as_press"];
                }
                if (pt.ContainsKey("linear_velocity_multiplier")) {
                    linear_velocity_multiplier = pt["linear_velocity_multiplier"];
                }
                if (pt.ContainsKey("linear_velocity_exponent")) {
                    linear_velocity_exponent = pt["linear_velocity_exponent"];
                }

                // Controller button mappings
                for (int button_index = 0; button_index < PSMoveClient.PSM_MAX_VIRTUAL_CONTROLLER_BUTTONS; ++button_index) {
                    ReadEmulatedTouchpadAction(pt, (ePSMButtonID)((int)ePSMButtonID.Virtual_0 + button_index));
                }

                // System button mapping
                if (pt.ContainsKey("system_button"))
                {
                    string systemButtonString = pt["system_button"];

                    if (systemButtonString.Length > 0) {
                        int button_index = Array.FindIndex(Constants.PSMButtonNames, 0, x => x == systemButtonString);

                        if (button_index != -1) {
                            system_button_id = (ePSMButtonID)button_index;
                        } else {
                            Trace.TraceWarning(string.Format("Invalid virtual controller system button: {0}", systemButtonString));
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}

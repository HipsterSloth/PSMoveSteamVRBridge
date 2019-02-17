using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class DS4ControllerConfig : ControllerConfig
    {
        public DS4ControllerConfig(string controller_serial) : base("ds4_" + controller_serial.ToUpper())
        {
        }

        // Rumble state
        bool rumble_suppressed;
        public bool RumbleSuppressed
        {
            get { return rumble_suppressed; }
            set { rumble_suppressed = value; IsDirty = true; }
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

        // Settings value: used to determine how many meters in front of the HMD the controller
        // is held when it's being calibrated.
        float calibration_offset_meters;
        public float CalibrationOffsetMeters
        {
            get { return calibration_offset_meters; }
            set { calibration_offset_meters= value; IsDirty = true; }
        }

        // Rumble state
        float thumbstick_deadzone;
        public float ThumbstickDeadzone
        {
            get { return thumbstick_deadzone; }
            set { thumbstick_deadzone = value; IsDirty = true; }
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

        public override void WriteToJSON(JsonValue pt)
        {
            base.WriteToJSON(pt);
            pt["rumble_suppressed"] = rumble_suppressed;
            pt["extend_Y_meters"] = extend_Y_meters;
            pt["extend_Z_meters"] = extend_Z_meters;
            pt["z_rotate_90_degrees"] = z_rotate_90_degrees;
            pt["calibration_offset_meters"] = calibration_offset_meters;
            pt["thumbstick_deadzone"] = thumbstick_deadzone;
            pt["linear_velocity_multiplier"] = linear_velocity_multiplier;
            pt["linear_velocity_exponent"] = linear_velocity_exponent;

            //PSMove controller button -> fake touchpad mappings
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.PS);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Triangle);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Circle);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Cross);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Square);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Left);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Up);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Right);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Down);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Options);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Share);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Touchpad);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.LeftJoystick);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.RightJoystick);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.LeftShoulder);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.RightShoulder);
        }

        public override bool ReadFromJSON(JsonValue pt)
        {
            if (base.ReadFromJSON(pt)) {
                if (pt.ContainsKey("rumble_suppressed")) {
                    rumble_suppressed = pt["rumble_suppressed"];
                }
                if (pt.ContainsKey("extend_Y_meters")) {
                    extend_Y_meters = pt["extend_Y_meters"];
                }
                if (pt.ContainsKey("extend_Z_meters")) {
                    extend_Z_meters = pt["extend_Z_meters"];
                }
                if (pt.ContainsKey("z_rotate_90_degrees")) {
                    z_rotate_90_degrees = pt["z_rotate_90_degrees"];
                }
                if (pt.ContainsKey("thumbstick_deadzone")) {
                    thumbstick_deadzone = pt["thumbstick_deadzone"];
                }
                if (pt.ContainsKey("calibration_offset_meters")) {
                    calibration_offset_meters = pt["calibration_offset_meters"];
                }
                if (pt.ContainsKey("linear_velocity_multiplier")) {
                    linear_velocity_multiplier = pt["linear_velocity_multiplier"];
                }
                if (pt.ContainsKey("linear_velocity_exponent")) {
                    linear_velocity_exponent = pt["linear_velocity_exponent"];
                }

                //PSMove controller button -> fake touchpad mappings
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.PS);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Triangle);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Circle);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Cross);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Square);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Left);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Up);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Right);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Down);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Options);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Share);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Touchpad);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.LeftJoystick);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.RightJoystick);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.LeftShoulder);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.RightShoulder);

                return true;
            }

            return false;
        }
    }
}

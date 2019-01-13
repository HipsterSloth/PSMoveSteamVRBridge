using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class PSMoveControllerConfig : ControllerConfig
    {
        public PSMoveControllerConfig(string psmSerialNo) : base("psmove_"+psmSerialNo.ToUpper())
        {
            setTrackpadActionForButton(ePSMButtonID.Move, eEmulatedTrackpadAction.Press);
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

        // Settings value: used to determine how many meters in front of the HMD the controller
        // is held when it's being calibrated.
        float calibration_offset_meters;
        public float CalibrationOffsetMeters
        {
            get { return calibration_offset_meters; }
            set { calibration_offset_meters= value; IsDirty = true; }
        }

        // Flag used to completely disable the alignment gesture.
        bool disable_alignment_gesture;
        public bool DisableAlignmentGesture
        {
            get { return disable_alignment_gesture; }
            set { disable_alignment_gesture = value; IsDirty = true; }
        }

        // Flag to tell if we should use the controller orientation as part of the controller alignment.
        bool use_orientation_in_hmd_alignment;
        public bool UseOrientationInHmdAlignment
        {
            get { return use_orientation_in_hmd_alignment; }
            set { use_orientation_in_hmd_alignment = value; IsDirty = true; }
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
            pt["delay_after_touchpad_press"] = delay_after_touchpad_press;
            pt["meters_per_touchpad_axis_units"] = meters_per_touchpad_axis_units;
            pt["calibration_offset_meters"] = calibration_offset_meters;
            pt["disable_alignment_gesture"] = disable_alignment_gesture;
            pt["use_orientation_in_hmd_alignment"] = use_orientation_in_hmd_alignment;
            pt["linear_velocity_multiplier"] = linear_velocity_multiplier;
            pt["linear_velocity_exponent"] = linear_velocity_exponent;

            //PSMove controller button -> fake touchpad mappings
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.PS);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Move);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Triangle);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Square);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Circle);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Cross);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Select);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Start);
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
                if (pt.ContainsKey("delay_after_touchpad_press")) {
                    delay_after_touchpad_press = pt["delay_after_touchpad_press"];
                }
                if (pt.ContainsKey("meters_per_touchpad_axis_units")) {
                    meters_per_touchpad_axis_units = pt["meters_per_touchpad_axis_units"];
                }
                if (pt.ContainsKey("calibration_offset_meters")) {
                    calibration_offset_meters = pt["calibration_offset_meters"];
                }
                if (pt.ContainsKey("disable_alignment_gesture")) {
                    disable_alignment_gesture = pt["disable_alignment_gesture"];
                }
                if (pt.ContainsKey("use_orientation_in_hmd_alignment")) {
                    use_orientation_in_hmd_alignment = pt["use_orientation_in_hmd_alignment"];
                }
                if (pt.ContainsKey("linear_velocity_multiplier")) {
                    linear_velocity_multiplier = pt["linear_velocity_multiplier"];
                }
                if (pt.ContainsKey("linear_velocity_exponent")) {
                    linear_velocity_exponent = pt["linear_velocity_exponent"];
                }

                //PSMove controller button -> fake touchpad mappings
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.PS);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Move);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Triangle);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Square);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Circle);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Cross);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Select);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Start);

                return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class PSNaviControllerConfig : ControllerConfig
    {
        public PSNaviControllerConfig(string controller_serial) : base("psnavi_" + controller_serial.ToUpper())
        {
            setTrackpadActionForButton(ePSMButtonID.Move, eEmulatedTrackpadAction.Press);
        }

        // Rumble state
        float thumbstick_deadzone;
        public float ThumbstickDeadzone
        {
            get { return thumbstick_deadzone; }
            set { thumbstick_deadzone = value; IsDirty = true; }
        }

        public override void WriteToJSON(JsonValue pt)
        {
            base.WriteToJSON(pt);

            // General Settings
            pt["thumbstick_deadzone"] = thumbstick_deadzone;

            //PSMove controller button -> fake touchpad mappings
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.PS);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Left);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Up);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Right);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Down);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Circle);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Cross);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Joystick);
            WriteEmulatedTouchpadAction(pt, ePSMButtonID.Shoulder);
        }

        public override bool ReadFromJSON(JsonValue pt)
        {
            if (base.ReadFromJSON(pt)) {
                if (pt.ContainsKey("thumbstick_deadzone")) {
                    thumbstick_deadzone = pt["thumbstick_deadzone"];
                }

                // DS4 controller button -> fake touchpad mappings
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.PS);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Left);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Up);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Right);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.DPad_Down);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Circle);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Cross);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Joystick);
                ReadEmulatedTouchpadAction(pt, ePSMButtonID.Shoulder);

                return true;
            }

            return false;
        }
    }
}

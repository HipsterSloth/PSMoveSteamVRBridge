using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class ControllerConfig : ConfigBase
    {
        public ControllerConfig(string fnamebase) : base(fnamebase)
        {
            ps_button_id_to_emulated_touchpad_action = new eEmulatedTrackpadAction[(int)ePSMButtonID.Count];
            for (int buttonId = 0; buttonId < (int)ePSMButtonID.Count; ++buttonId) {
                ps_button_id_to_emulated_touchpad_action[buttonId] = eEmulatedTrackpadAction.None;
            }
        }

        public static int GetControllerCount()
        {
            return ConfigManager.Instance.GetControllerCount();
        }

        public static ControllerConfig GetControllerConfigByIndex(int ControllerIndex)
        {
            return ConfigManager.Instance.GetControllerConfig(ControllerIndex);
        }

        public string ControllerName
        {
            get { return ConfigFileBase; }
        }

        private string override_model = "";
        public string OverrideModel
        {
            get { return override_model; }
            set { override_model = value; IsDirty = true; }
        }

        // Used to map buttons to the emulated touchpad
        private eEmulatedTrackpadAction[] ps_button_id_to_emulated_touchpad_action;
        public eEmulatedTrackpadAction getTrackpadActionForButton(ePSMButtonID buttonId)
        {
            return ps_button_id_to_emulated_touchpad_action[(int)buttonId];
        }
        public void setTrackpadActionForButton(ePSMButtonID buttonId, eEmulatedTrackpadAction action)
        {
            ps_button_id_to_emulated_touchpad_action[(int)buttonId] = action;
        }

        protected void ReadEmulatedTouchpadAction(JsonValue pt, ePSMButtonID psButtonID)
        {
            eEmulatedTrackpadAction vrTouchpadDirection = eEmulatedTrackpadAction.None;

            if (pt.ContainsKey("trackpad_mappings")) {
                JsonValue trackpad_pt = pt["trackpad_mappings"];
                string PSButtonName = Constants.PSMButtonNames[(int)psButtonID];

                if (trackpad_pt.ContainsKey(PSButtonName)) {
                    string remapButtonToTouchpadDirectionString = trackpad_pt[PSButtonName];

                    for (int vr_touchpad_direction_index = 0; vr_touchpad_direction_index < (int)eEmulatedTrackpadAction.Count; ++vr_touchpad_direction_index) {
                        if (string.Compare(remapButtonToTouchpadDirectionString, Constants.VRTouchpadActionNames[vr_touchpad_direction_index], true) == 0) {
                            vrTouchpadDirection = (eEmulatedTrackpadAction)vr_touchpad_direction_index;
                            break;
                        }
                    }
                }
            }

            // Load the mapping
            ps_button_id_to_emulated_touchpad_action[(int)psButtonID] = vrTouchpadDirection;
        }

        protected void WriteEmulatedTouchpadAction(JsonValue pt, ePSMButtonID psButtonID)
        {
            JsonValue trackpad_pt;

            string szPSButtonName = Constants.PSMButtonNames[(int)psButtonID];
            string szTouchpadAction = Constants.VRTouchpadActionNames[(int)ps_button_id_to_emulated_touchpad_action[(int)psButtonID]];

            if (pt.ContainsKey("trackpad_mappings")) {
                trackpad_pt = pt["trackpad_mappings"];
                trackpad_pt[szPSButtonName] = szTouchpadAction;
            }
            else {
                trackpad_pt = new JsonObject();
                trackpad_pt[szPSButtonName] = szTouchpadAction;
                pt["trackpad_mappings"] = trackpad_pt;
            }
        }

        public override void WriteToJSON(JsonValue pt)
        {
            base.WriteToJSON(pt);
            pt["override_model"] = override_model;
        }

        public override bool ReadFromJSON(JsonValue pt)
        {
            if (base.ReadFromJSON(pt)) {
                if (pt.ContainsKey("override_model")) {
                    override_model = pt["override_model"];
                }

                return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class FreePIEControllerProperty
    {
        public eControllerSource controllerSource;
        public eControllerPropertySource controllerPropertySource;

        public FreePIEControllerProperty(eControllerSource source, eControllerPropertySource property)
        {
            controllerSource = source;
            controllerPropertySource = property;
        }

        public JsonObject WriteToJSON()
        {
            JsonObject pt = new JsonObject();

            pt["controller_source"] = PSMDevicePool.ControllerSourceNames[(int)controllerSource];
            pt["controller_property_source"] = PSMDevicePool.ControllerPropertySourceNames[(int)controllerPropertySource];

            return pt;
        }

        public bool ReadFromJSON(JsonValue pt)
        {
            if (!pt.ContainsKey("controller_source"))
                return false;
            if (!pt.ContainsKey("controller_property_source"))
                return false;
            if (!Enum.TryParse(pt["controller_source"], out controllerSource))
                return false;
            if (!Enum.TryParse(pt["controller_property_source"], out controllerPropertySource))
                return false;

            return true;
        }
    }

    public class FreePIEHmdProperty
    {
        public eHmdSource hmdSource;
        public eHmdPropertySource hmdPropertySource;

        public FreePIEHmdProperty(eHmdSource source, eHmdPropertySource property)
        {
            hmdSource = source;
            hmdPropertySource = property;
        }

        public JsonObject WriteToJSON()
        {
            JsonObject pt = new JsonObject();

            pt["hmd_source"] = PSMDevicePool.HmdSourceNames[(int)hmdSource];
            pt["hmd_property_source"] = PSMDevicePool.HmdPropertySourceNames[(int)hmdPropertySource];

            return pt;
        }

        public bool ReadFromJSON(JsonValue pt)
        {
            if (!pt.ContainsKey("hmd_source"))
                return false;
            if (!pt.ContainsKey("hmd_property_source"))
                return false;
            if (!Enum.TryParse(pt["hmd_source"], out hmdSource))
                return false;
            if (!Enum.TryParse(pt["hmd_property_source"], out hmdPropertySource))
                return false;

            return true;
        }
    }

    public class FreePIESlotDefinition
    {
        public int SlotIndex;

        public FreePIESlotDefinition(int slotIndex)
        {
            SlotIndex = slotIndex;
        }

        public virtual void WriteToJSON(JsonValue pt)
        {
            pt["slot_index"] = SlotIndex;
        }

        public virtual bool ReadFromJSON(JsonValue pt)
        {
            if (pt.ContainsKey("slot_index"))
            {
                SlotIndex = pt["slot_index"];
                return true;
            }
            return false;
        }
    }

    public class FreePIEControllerSlotDefinition : FreePIESlotDefinition
    {
        public FreePIEControllerProperty xProperty;
        public FreePIEControllerProperty yProperty;
        public FreePIEControllerProperty zProperty;
        public FreePIEControllerProperty pitchProperty;
        public FreePIEControllerProperty rollProperty;
        public FreePIEControllerProperty yawProperty;

        public FreePIEControllerSlotDefinition(int slotIndex) : base(slotIndex)
        {            
            xProperty = new FreePIEControllerProperty(eControllerSource.CONTROLLER_0, eControllerPropertySource.POSITION_X);
            yProperty = new FreePIEControllerProperty(eControllerSource.CONTROLLER_0, eControllerPropertySource.POSITION_Y);
            zProperty = new FreePIEControllerProperty(eControllerSource.CONTROLLER_0, eControllerPropertySource.POSITION_Z);
            pitchProperty = new FreePIEControllerProperty(eControllerSource.CONTROLLER_0, eControllerPropertySource.ORIENTATION_PITCH);
            rollProperty = new FreePIEControllerProperty(eControllerSource.CONTROLLER_0, eControllerPropertySource.ORIENTATION_ROLL);
            yawProperty = new FreePIEControllerProperty(eControllerSource.CONTROLLER_0, eControllerPropertySource.ORIENTATION_YAW);
        }

        public override void WriteToJSON(JsonValue pt)
        {
            pt["slot_type"] = "controller";
            pt["x"] = xProperty.WriteToJSON();
            pt["y"] = yProperty.WriteToJSON();
            pt["z"] = zProperty.WriteToJSON();
            pt["pitch"] = pitchProperty.WriteToJSON();
            pt["roll"] = rollProperty.WriteToJSON();
            pt["yaw"] = yawProperty.WriteToJSON();

            base.WriteToJSON(pt);
        }

        public override bool ReadFromJSON(JsonValue pt)
        {
            if (!base.ReadFromJSON(pt))
                return false;
            if (!pt.ContainsKey("x") || !xProperty.ReadFromJSON(pt["x"]))
                return false;
            if (!pt.ContainsKey("y") || !yProperty.ReadFromJSON(pt["y"]))
                return false;
            if (!pt.ContainsKey("z") || !zProperty.ReadFromJSON(pt["z"]))
                return false;
            if (!pt.ContainsKey("pitch") || !pitchProperty.ReadFromJSON(pt["pitch"]))
                return false;
            if (!pt.ContainsKey("roll") || !rollProperty.ReadFromJSON(pt["roll"]))
                return false;
            if (!pt.ContainsKey("yaw") || !yawProperty.ReadFromJSON(pt["yaw"]))
                return false;

            return true;
        }
    }

    public class FreePIEHmdSlotDefinition : FreePIESlotDefinition
    {
        public FreePIEHmdProperty xProperty;
        public FreePIEHmdProperty yProperty;
        public FreePIEHmdProperty zProperty;
        public FreePIEHmdProperty pitchProperty;
        public FreePIEHmdProperty rollProperty;
        public FreePIEHmdProperty yawProperty;

        public FreePIEHmdSlotDefinition(int slotIndex) : base(slotIndex)
        {
            xProperty = new FreePIEHmdProperty(eHmdSource.HMD_0, eHmdPropertySource.POSITION_X);
            yProperty = new FreePIEHmdProperty(eHmdSource.HMD_0, eHmdPropertySource.POSITION_Y);
            zProperty = new FreePIEHmdProperty(eHmdSource.HMD_0, eHmdPropertySource.POSITION_Z);
            pitchProperty = new FreePIEHmdProperty(eHmdSource.HMD_0, eHmdPropertySource.ORIENTATION_PITCH);
            rollProperty = new FreePIEHmdProperty(eHmdSource.HMD_0, eHmdPropertySource.ORIENTATION_ROLL);
            yawProperty = new FreePIEHmdProperty(eHmdSource.HMD_0, eHmdPropertySource.ORIENTATION_YAW);
        }

        public override void WriteToJSON(JsonValue pt)
        {
            pt["slot_type"] = "hmd";
            pt["x"] = xProperty.WriteToJSON();
            pt["y"] = yProperty.WriteToJSON();
            pt["z"] = zProperty.WriteToJSON();
            pt["pitch"] = pitchProperty.WriteToJSON();
            pt["roll"] = rollProperty.WriteToJSON();
            pt["yaw"] = yawProperty.WriteToJSON();

            base.WriteToJSON(pt);
        }

        public override bool ReadFromJSON(JsonValue pt)
        {
            if (!base.ReadFromJSON(pt))
                return false;
            if (!pt.ContainsKey("x") || !xProperty.ReadFromJSON(pt["x"]))
                return false;
            if (!pt.ContainsKey("y") || !yProperty.ReadFromJSON(pt["y"]))
                return false;
            if (!pt.ContainsKey("z") || !zProperty.ReadFromJSON(pt["z"]))
                return false;
            if (!pt.ContainsKey("pitch") || !pitchProperty.ReadFromJSON(pt["pitch"]))
                return false;
            if (!pt.ContainsKey("roll") || !rollProperty.ReadFromJSON(pt["roll"]))
                return false;
            if (!pt.ContainsKey("yaw") || !yawProperty.ReadFromJSON(pt["yaw"]))
                return false;

            return true;
        }
    }

    public class FreePIEConfig : ConfigBase
    {
        public FreePIEConfig() : base("FreePIEConfig")
        {
            _slotDefinitions = new FreePIESlotDefinition[0];
        }

        public static FreePIEConfig Instance
        {
            get { return ConfigManager.Instance.FreePIEConfig; }
        }

        private int _virtualControllerTriggerAxisIndex = -1;
        public int VirtualControllerTriggerAxisIndex
        {
            get { return _virtualControllerTriggerAxisIndex; }
            set { _virtualControllerTriggerAxisIndex = value; IsDirty = true; }
        }

        private FreePIESlotDefinition[] _slotDefinitions;
        public FreePIESlotDefinition[] SlotDefinitions
        {
            get { return _slotDefinitions; }
            set { _slotDefinitions = value; IsDirty = true; }
        }

        public override void WriteToJSON(JsonValue pt)
        {
            pt["virtual_controller_trigger_axis_index"] = _virtualControllerTriggerAxisIndex;

            JsonArray slotDefinitionJsonArray = new JsonArray();
            foreach (FreePIESlotDefinition slotDefinition in _slotDefinitions)
            {
                JsonObject slotDefinitionJson = new JsonObject();

                slotDefinition.WriteToJSON(slotDefinitionJson);
                slotDefinitionJsonArray.Add(slotDefinitionJson);
            }
            pt["slots"] = slotDefinitionJsonArray;
        }

        public override bool ReadFromJSON(JsonValue pt)
        {
            if (!base.ReadFromJSON(pt))
                return false;

            if (pt.ContainsKey("virtual_controller_trigger_axis_index"))
            {
                _virtualControllerTriggerAxisIndex = pt["virtual_controller_trigger_axis_index"];
            }

            if (pt.ContainsKey("slots") && pt["slots"].GetType() == typeof(JsonArray))
            {
                List<FreePIESlotDefinition> slotDefinitions = new List<FreePIESlotDefinition>();
                JsonArray slotDefinitionJsonArray= pt["slots"] as JsonArray;

                for (int slotIndex= 0; slotIndex < slotDefinitionJsonArray.Count; ++slotIndex)
                {
                    JsonValue slotDefinitionJson = slotDefinitionJsonArray[slotIndex];

                    if (slotDefinitionJson.ContainsKey("slot_type"))
                    {
                        string slot_type = slotDefinitionJson["slot_type"];

                        if (slot_type == "hmd")
                        {
                            FreePIEHmdSlotDefinition hmdDefinition = new FreePIEHmdSlotDefinition(slotIndex);

                            if (hmdDefinition.ReadFromJSON(slotDefinitionJson))
                                slotDefinitions.Add(hmdDefinition);
                            else
                                return false;
                        }
                        else if (slot_type == "controller")
                        {
                            FreePIEControllerSlotDefinition controllerDefinition = new FreePIEControllerSlotDefinition(slotIndex);

                            if (controllerDefinition.ReadFromJSON(slotDefinitionJson))
                                slotDefinitions.Add(controllerDefinition);
                            else
                                return false;
                        }
                    }
                }

                _slotDefinitions= slotDefinitions.ToArray();
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}

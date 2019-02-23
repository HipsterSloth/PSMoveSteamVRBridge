using System;
using PSMoveService;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using System.ComponentModel;
using Valve.VR;
using System.Text;

namespace SystemTrayApp
{
    public class SteamVRController : SteamVRTrackedDevice
    {
        public static string ControllerType_PSMove = "playstation_move";
        public static string ControllerType_DS4 = "playstation_ds4";
        public static string ControllerType_PSNavi = "playstation_navi";
        public static string ControllerType_Virtual = "psmoveservice_virtual";

        private string _controllerType;
        public string ControllerType
        {
            get { return _controllerType; }
        }

        public SteamVRController(uint deviceID) : base(deviceID, ETrackedDeviceClass.Controller)
        {
        }

        public override void UpdateProperties(CVRSystem SteamVRSystem)
        {
            base.UpdateProperties(SteamVRSystem);

            _controllerType = FetchStringProperty(SteamVRSystem, ETrackedDeviceProperty.Prop_ControllerType_String, "");

            if (_controllerType == "playstation_move" && _renderModelName == "")
            {
                _renderModelName = "{psmove}psmove_controller";
                UpdateRenderModel();
            }
        }

        public void TriggerHapticPulse(float intensityFraction)
        {
            ushort usDuration= (ushort)(3999.0f * intensityFraction);

            SteamVRContext.Instance.SteamVRSystem.TriggerHapticPulse(_deviceID, (uint)EVRButtonId.k_EButton_Axis0, usDuration);
        }
    }
}
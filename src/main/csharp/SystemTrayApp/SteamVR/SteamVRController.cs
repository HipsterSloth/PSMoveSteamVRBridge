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

        private ETrackedControllerRole _controllerRole;
        public ETrackedControllerRole ControllerRole
        {
            get { return _controllerRole; }
        }

        ulong _hapticActionHandle = OpenVR.k_ulInvalidActionHandle;

        public SteamVRController(uint deviceID) : base(deviceID, ETrackedDeviceClass.Controller)
        {
        }

        public override void UpdateProperties(CVRSystem SteamVRSystem)
        {
            base.UpdateProperties(SteamVRSystem);

            _controllerRole = SteamVRSystem.GetControllerRoleForTrackedDeviceIndex(DeviceID);
            _controllerType = FetchStringProperty(SteamVRSystem, ETrackedDeviceProperty.Prop_ControllerType_String, "");

            if (_controllerType == "playstation_move" && _renderModelName == "")
            {
                _renderModelName = "{psmove}psmove_controller";
                UpdateRenderModel();
            }

            if (_controllerRole == ETrackedControllerRole.LeftHand)
            {
                OpenVR.Input.GetActionHandle("/actions/trayapp/out/haptic_left", ref _hapticActionHandle);
            }
            else if (_controllerRole == ETrackedControllerRole.RightHand)
            {
                OpenVR.Input.GetActionHandle("/actions/trayapp/out/haptic_right", ref _hapticActionHandle);
            }
            else
            {
                _hapticActionHandle = OpenVR.k_ulInvalidActionHandle;
            }
        }

        public void TriggerHapticPulse(float intensityFraction, float durationSeconds)
        {
            if (OpenVR.Input != null && _hapticActionHandle != OpenVR.k_ulInvalidActionHandle)
            {
                OpenVR.Input.TriggerHapticVibrationAction(
                    _hapticActionHandle,
                    0.0f, // Start delay
                    durationSeconds,
                    4.0f, // The frequency in cycles per second of the haptic event
                    intensityFraction, // The magnitude of the haptic event. This value must be between 0.0 and 1.0.
                    OpenVR.k_ulInvalidInputValueHandle);
            }
        }
    }
}
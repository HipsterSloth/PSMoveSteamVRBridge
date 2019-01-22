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
        public SteamVRController(uint deviceID) : base(deviceID, ETrackedDeviceClass.Controller)
        {
        }

        public override void UpdateProperties(CVRSystem SteamVRSystem)
        {
            base.UpdateProperties(SteamVRSystem);
        }
    }
}
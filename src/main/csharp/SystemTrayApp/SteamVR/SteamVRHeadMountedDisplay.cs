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
    public class SteamVRHeadMountedDisplay : SteamVRTrackedDevice
    {
        public SteamVRHeadMountedDisplay(uint deviceID) : base(deviceID, ETrackedDeviceClass.HMD)
        {
        }

        public override void UpdateProperties(CVRSystem SteamVRSystem)
        {
            base.UpdateProperties(SteamVRSystem);
        }
    }
}
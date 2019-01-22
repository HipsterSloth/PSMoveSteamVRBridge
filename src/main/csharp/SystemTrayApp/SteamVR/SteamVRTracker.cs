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
    public class SteamVRTracker : SteamVRTrackedDevice
    {
        public SteamVRTracker(uint deviceID) : base(deviceID, ETrackedDeviceClass.TrackingReference)
        {
        }

        public override void UpdateProperties(CVRSystem SteamVRSystem)
        {
            base.UpdateProperties(SteamVRSystem);
        }
    }
}
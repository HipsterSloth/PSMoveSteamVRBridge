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
    //Reference: https://github.com/ValveSoftware/openvr/blob/master/samples/hellovr_opengl/hellovr_opengl_main.cpp

    public class SteamVRContext : SynchronizedContext
    {
        private static uint VREventSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));
        private static double POLL_INTERVAL_60FPS = 1.0 / 60.0; // ms

        private static readonly Lazy<SteamVRContext> lazy = new Lazy<SteamVRContext>(() => new SteamVRContext());
        public static SteamVRContext Instance { get { return lazy.Value; } }

        private Timer PollTimer;

        private CVRSystem SteamVRSystem;
        private TrackedDevicePose_t[] DevicePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

        private List<SteamVRTrackedDevice> PendingDeviceList = new List<SteamVRTrackedDevice>();
        private Dictionary<uint, SteamVRTrackedDevice> LoadedDeviceTable = new Dictionary<uint, SteamVRTrackedDevice>();

        private SteamVRContext()
        {
            PollTimer = new System.Timers.Timer();
            bIsConnected = false;
        }

        private bool bIsConnected;
        public bool IsConnected
        { 
            get { return bIsConnected; }
        }

        private string runtimePath;
        public string SteamVRRuntimePath
        {
            get { return runtimePath; }
        }

        private SteamVRResourceManager resourceManager = new SteamVRResourceManager();
        public SteamVRResourceManager ResourceManager
        {
            get { return resourceManager; }
        }

        public delegate void ConnectedToSteamVR();
        public event ConnectedToSteamVR ConnectedToSteamVREvent;

        public delegate void DisconnectedFromSteamVR();
        public event DisconnectedFromSteamVR DisconnectedFromSteamVREvent;

        public delegate void TrackedDeviceActivated(SteamVRTrackedDevice device);
        public event TrackedDeviceActivated TrackedDeviceActivatedEvent;

        public delegate void TrackedDeviceDeactivated(SteamVRTrackedDevice device);
        public event TrackedDeviceDeactivated TrackedDeviceDeactivatedEvent;

        public delegate void TrackedDevicesPoseUpdate(Dictionary<uint, OpenGL.ModelMatrix> poses);
        public event TrackedDevicesPoseUpdate TrackedDevicesPoseUpdateEvent;

        public void Init()
        {
            runtimePath = OpenVR.RuntimePath();
            resourceManager.Init();
        }

        public void Cleanup()
        {
            resourceManager.Cleanup();
        }

        public bool Connect()
        {
            if (!bIsConnected) {

                // Start as "background" application.  This prevents vrserver from being started on our behalf, 
                // and prevents us from keeping vrserver alive when everything else exits.
                EVRInitError eVRInitError= EVRInitError.None;
                SteamVRSystem= OpenVR.Init(ref eVRInitError, EVRApplicationType.VRApplication_Overlay);
                if (SteamVRSystem == null || eVRInitError != EVRInitError.None)
                {
                    return false;
                }

                // Fetch the initial set of connected tracked devices
                SteamVRSystem.GetDeviceToAbsoluteTrackingPose(
                    ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, 
                    0.0f, 
                    DevicePoses);

                for (uint DeviceIndex = 0; DeviceIndex < OpenVR.k_unMaxTrackedDeviceCount; ++DeviceIndex)
                {
                    if (DevicePoses[DeviceIndex].bDeviceIsConnected)
                    {
                        HandleTrackedDeviceActivated(DeviceIndex);
                    }
                }

                if (ConnectedToSteamVREvent != null)
                {
                    ConnectedToSteamVREvent();
                }

                // Create a timer to poll PSMoveService state with
                PollTimer.Elapsed += RunFrame;
                PollTimer.AutoReset = false; // NO AUTO RESET! Restart in RunFrame().
                PollTimer.Enabled = true;
                PollTimer.Interval = POLL_INTERVAL_60FPS;
                PollTimer.Start();

                bIsConnected = true;
            }

            return true;
        }

        public void Disconnect()
        {
            if (bIsConnected) {
                if (DisconnectedFromSteamVREvent != null)
                {
                    DisconnectedFromSteamVREvent();
                }

                // Forget about all tracked devices
                PendingDeviceList.Clear();
                LoadedDeviceTable.Clear();

                // Disconnected the timer callback
                PollTimer.Elapsed -= RunFrame;
                PollTimer.Stop();

                // Shutdown SteamVR connection
                SteamVRSystem = null;
                OpenVR.Shutdown();
                bIsConnected = false;
            }
        }

        private void RunFrame(object sender, ElapsedEventArgs e)
        {
            VREvent_t Event = new VREvent_t();
            while (SteamVRSystem.PollNextEvent(ref Event, VREventSize))
            {
                switch((EVREventType)Event.eventType)
                {
                    case EVREventType.VREvent_Quit:
                        Disconnect();
                        break;
                    //  A tracked device was plugged in or otherwise detected by the system. 
                    // There is no data, but the trackedDeviceIndex will be the index of the new device.
                    case EVREventType.VREvent_TrackedDeviceActivated:
                        HandleTrackedDeviceActivated(Event.trackedDeviceIndex);
                        break;
                    // One or more of the properties of a tracked device have changed. Data is not used for this event.
                    case EVREventType.VREvent_TrackedDeviceUpdated:
                        HandleTrackedDevicePropertyChanged(Event.trackedDeviceIndex);
                        break;
                    // A tracked device was unplugged or the system is no longer able to contact it in some other way. 
                    // Data is not used for this event.
                    case EVREventType.VREvent_TrackedDeviceDeactivated:
                        HandleTrackedDeviceDeactivated(Event.trackedDeviceIndex);
                        break;
                    case EVREventType.VREvent_ChaperoneDataHasChanged:
                        break;
                    case EVREventType.VREvent_ChaperoneUniverseHasChanged:
                        break;
                    case EVREventType.VREvent_ChaperoneSettingsHaveChanged:
                        break;
                }
            }

            if (IsConnected)
            {
                // Update any pending resource loads
                resourceManager.PollAsyncLoadRequests();

                // See if any pending resource loads have completed
                for (int ListIndex= PendingDeviceList.Count - 1; ListIndex >= 0; --ListIndex)
                {
                    SteamVRTrackedDevice device = PendingDeviceList[ListIndex];

                    if (!device.GetIsLoadingResources())
                    {
                        // Move the device into the loaded set
                        PendingDeviceList.Remove(device);
                        LoadedDeviceTable.Add(device.DeviceID, device);

                        // Now it's safe to notify clients that the device has been added
                        if (TrackedDeviceActivatedEvent != null)
                        {
                            TrackedDeviceActivatedEvent(device);
                        }
                    }
                }

                // Fetch the latest controller pose data
                if (TrackedDevicesPoseUpdateEvent != null)
                {
                    Dictionary<uint, OpenGL.ModelMatrix> UpdatePosesTable = new Dictionary<uint, OpenGL.ModelMatrix>();

                    SteamVRSystem.GetDeviceToAbsoluteTrackingPose(
                        ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, 0.0f, DevicePoses);

                    for (uint DeviceIndex = 0; DeviceIndex < OpenVR.k_unMaxTrackedDeviceCount; ++DeviceIndex)
                    {
                        if (DevicePoses[DeviceIndex].bDeviceIsConnected)
                        {
                            if (HandleTrackedDevicePoseUpdated(DeviceIndex, DevicePoses[DeviceIndex]))
                            {
                                UpdatePosesTable.Add(DeviceIndex, LoadedDeviceTable[DeviceIndex].Transform);
                            }
                        }
                    }

                    if (UpdatePosesTable.Count > 0)
                    {
                        TrackedDevicesPoseUpdateEvent(UpdatePosesTable);
                    }
                }

                // Restart the timer once event handling is complete.
                // This prevents overlapping callings to RunFrame.
                // In practice this is only an issue when debugging.
                PollTimer.Start();
            }
        }

        public List<SteamVRTrackedDevice> FetchLoadedTrackedDeviceList()
        {
            return new List<SteamVRTrackedDevice>(LoadedDeviceTable.Values);
        }

        private void HandleTrackedDeviceActivated(uint trackedDeviceIndex)
        {
            SteamVRTrackedDevice device = null;

            switch (SteamVRSystem.GetTrackedDeviceClass(trackedDeviceIndex))
            {
                case ETrackedDeviceClass.HMD:
                    if (!LoadedDeviceTable.ContainsKey(trackedDeviceIndex) &&
                        !PendingDeviceList.Exists(x => x.DeviceID == trackedDeviceIndex))
                    {
                        device = new SteamVRHeadMountedDisplay(trackedDeviceIndex);
                    }
                    break;
                case ETrackedDeviceClass.Controller:
                    if (!LoadedDeviceTable.ContainsKey(trackedDeviceIndex) &&
                        !PendingDeviceList.Exists(x => x.DeviceID == trackedDeviceIndex))
                    {
                        device = new SteamVRController(trackedDeviceIndex);
                    }
                    break;
                case ETrackedDeviceClass.TrackingReference:
                    if (!LoadedDeviceTable.ContainsKey(trackedDeviceIndex) &&
                        !PendingDeviceList.Exists(x => x.DeviceID == trackedDeviceIndex))
                    {
                        device = new SteamVRTracker(trackedDeviceIndex);
                    }
                    break;
            }

            if (device != null)
            {
                // Fetch all relevant device properties from SteamVR
                device.UpdateProperties(SteamVRSystem);

                // Add the device to the pending-load set
                PendingDeviceList.Add(device);
            }
        }

        private void HandleTrackedDevicePropertyChanged(uint trackedDeviceIndex)
        {
            if (LoadedDeviceTable.ContainsKey(trackedDeviceIndex)) 
            {
                SteamVRTrackedDevice device= LoadedDeviceTable[trackedDeviceIndex];

                // Refresh all the properties on the device
                device.UpdateProperties(SteamVRSystem);

                // See if the device has to load resources as a result of property changes
                if (device.GetIsLoadingResources())
                {
                    // Tell any clients that the device has been "disconnected" during the load phase
                    HandleTrackedDeviceDeactivated(trackedDeviceIndex);

                    // Put the device back in the pending list
                    PendingDeviceList.Add(device);
                }
            }
        }

        private void HandleTrackedDeviceDeactivated(uint trackedDeviceIndex)
        {
            PendingDeviceList.RemoveAll(x => x.DeviceID == trackedDeviceIndex);

            if (LoadedDeviceTable.ContainsKey(trackedDeviceIndex)) {
                if (TrackedDeviceDeactivatedEvent != null) {
                    TrackedDeviceDeactivatedEvent(LoadedDeviceTable[trackedDeviceIndex]);
                }

                LoadedDeviceTable.Remove(trackedDeviceIndex);
            }
        }

        private bool HandleTrackedDevicePoseUpdated(uint trackedDeviceIndex, TrackedDevicePose_t pose)
        {
            if (LoadedDeviceTable.ContainsKey(trackedDeviceIndex))
            {
                LoadedDeviceTable[trackedDeviceIndex].ApplyPoseUpdate(pose);

                return true;
            }

            return false;
        }
    }
}
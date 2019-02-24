using System;
using PSMoveService;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using Valve.VR;
using System.Text;

namespace SystemTrayApp
{
    //Reference: https://github.com/ValveSoftware/openvr/blob/master/samples/hellovr_opengl/hellovr_opengl_main.cpp

    public class SteamVRContext
    {
        private static uint VREventSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));
        private static int POLL_INTERVAL_60FPS = 1000 / 60; // ms

        private static readonly Lazy<SteamVRContext> lazy = new Lazy<SteamVRContext>(() => new SteamVRContext());
        public static SteamVRContext Instance { get { return lazy.Value; } }

        private Timer PollTimer;

        private TrackedDevicePose_t[] _devicePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        public TrackedDevicePose_t[] DevicePoses
        {
            get { return _devicePoses; }
        }

        private TrackedDevicePose_t[] _rawDevicePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        public TrackedDevicePose_t[] RawDevicePoses
        {
            get { return _rawDevicePoses; }
        }

        private Dictionary<uint, SteamVRTrackedDevice> _deviceTable = new Dictionary<uint, SteamVRTrackedDevice>();
        public Dictionary<uint, SteamVRTrackedDevice> DeviceTable
        {
            get { return _deviceTable; }
        }

        private SteamVRContext()
        {
            PollTimer = new System.Windows.Forms.Timer();
            bIsConnected = false;
        }

        private CVRSystem _steamVRSystem;
        public CVRSystem SteamVRSystem
        {
            get { return _steamVRSystem; }
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
        public event TrackedDevicesPoseUpdate TrackedDevicesRawPoseUpdateEvent;

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
                _steamVRSystem= OpenVR.Init(ref eVRInitError, EVRApplicationType.VRApplication_Overlay);
                if (_steamVRSystem == null || eVRInitError != EVRInitError.None)
                {
                    Trace.TraceWarning(string.Format("SystemTrayApp.Connect - Failed to initialize SteamVR: {0}", eVRInitError.ToString()));
                    return false;
                }

                // Tell SteamVR where to look for our action manifest settings
                EVRInputError eVRInputError= OpenVR.Input.SetActionManifestPath(PathUtility.GetActionManifestPathPath());
                if (eVRInitError != EVRInitError.None)
                {
                    Trace.TraceWarning(string.Format("SystemTrayApp.Connect - Failed to set action manifest path: {0}", eVRInputError.ToString()));
                }

                // Fetch the initial set of connected tracked devices
                _steamVRSystem.GetDeviceToAbsoluteTrackingPose(
                    ETrackingUniverseOrigin.TrackingUniverseStanding, 
                    0.0f, 
                    _devicePoses);
                _steamVRSystem.GetDeviceToAbsoluteTrackingPose(
                    ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated,
                    0.0f,
                    _rawDevicePoses);

                for (uint DeviceIndex = 0; DeviceIndex < OpenVR.k_unMaxTrackedDeviceCount; ++DeviceIndex)
                {
                    if (_devicePoses[DeviceIndex].bDeviceIsConnected)
                    {
                        HandleTrackedDeviceActivated(DeviceIndex);
                    }
                }

                if (ConnectedToSteamVREvent != null)
                {
                    ConnectedToSteamVREvent();
                }

                // Create a timer to poll PSMoveService state with
                PollTimer.Tick += RunFrame;
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
                _deviceTable.Clear();

                // Disconnected the timer callback
                PollTimer.Tick -= RunFrame;
                PollTimer.Stop();

                // Shutdown SteamVR connection
                _steamVRSystem = null;
                OpenVR.Shutdown();
                bIsConnected = false;
            }
        }

        private void RunFrame(object sender, EventArgs e)
        {
            VREvent_t Event = new VREvent_t();
            while (IsConnected && _steamVRSystem.PollNextEvent(ref Event, VREventSize))
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
                // Fetch the latest controller pose data
                if (TrackedDevicesPoseUpdateEvent != null)
                {
                    Dictionary<uint, OpenGL.ModelMatrix> UpdatePosesTable = new Dictionary<uint, OpenGL.ModelMatrix>();

                    _steamVRSystem.GetDeviceToAbsoluteTrackingPose(
                        ETrackingUniverseOrigin.TrackingUniverseStanding, 0.0f, _devicePoses);

                    for (uint DeviceIndex = 0; DeviceIndex < OpenVR.k_unMaxTrackedDeviceCount; ++DeviceIndex)
                    {
                        if (_devicePoses[DeviceIndex].bDeviceIsConnected)
                        {
                            if (HandleTrackedDevicePoseUpdated(DeviceIndex, _devicePoses[DeviceIndex]))
                            {
                                UpdatePosesTable.Add(DeviceIndex, _deviceTable[DeviceIndex].Transform);
                            }
                        }
                    }

                    if (UpdatePosesTable.Count > 0)
                    {
                        TrackedDevicesPoseUpdateEvent(UpdatePosesTable);
                    }
                }

                // Fetch the latest raw controller pose data
                if (TrackedDevicesRawPoseUpdateEvent != null)
                {
                    Dictionary<uint, OpenGL.ModelMatrix> UpdatePosesTable = new Dictionary<uint, OpenGL.ModelMatrix>();

                    _steamVRSystem.GetDeviceToAbsoluteTrackingPose(
                        ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, 0.0f, _rawDevicePoses);

                    for (uint DeviceIndex = 0; DeviceIndex < OpenVR.k_unMaxTrackedDeviceCount; ++DeviceIndex)
                    {
                        if (_rawDevicePoses[DeviceIndex].bDeviceIsConnected)
                        {
                            UpdatePosesTable.Add(DeviceIndex, _deviceTable[DeviceIndex].Transform);
                        }
                    }

                    if (UpdatePosesTable.Count > 0)
                    {
                        TrackedDevicesRawPoseUpdateEvent(UpdatePosesTable);
                    }
                }
            }
        }

        public List<SteamVRTrackedDevice> FetchLoadedTrackedDeviceList()
        {
            return new List<SteamVRTrackedDevice>(_deviceTable.Values);
        }

        public SteamVRHeadMountedDisplay FetchFirstHMD()
        {
            SteamVRHeadMountedDisplay hmd = null;
            foreach (SteamVRTrackedDevice device in _deviceTable.Values)
            {
                if (device is SteamVRHeadMountedDisplay)
                {
                    hmd = device as SteamVRHeadMountedDisplay;
                    break;
                }
            }

            return hmd;
        }

        public SteamVRController FetchFirstControllerOfType(string controllerType)
        {
            SteamVRController controller = null;
            foreach (SteamVRTrackedDevice device in _deviceTable.Values)
            {
                if (device is SteamVRController)
                {
                    SteamVRController testController = device as SteamVRController;

                    if (testController.ControllerType == controllerType)
                    {
                        controller = testController;
                        break;
                    }
                }
            }

            return controller;
        }

        public void TriggerHapticPulse(uint DeviceId, float intensityFraction, float durationSeconds)
        {
            if (_deviceTable.ContainsKey(DeviceId))
            {
                SteamVRTrackedDevice device = _deviceTable[DeviceId];

                if (device is SteamVRController)
                {
                    SteamVRController Controller = device as SteamVRController;

                    Controller.TriggerHapticPulse(intensityFraction, durationSeconds);
                }
            }
        }

        private void HandleTrackedDeviceActivated(uint trackedDeviceIndex)
        {
            SteamVRTrackedDevice device = null;

            switch (_steamVRSystem.GetTrackedDeviceClass(trackedDeviceIndex))
            {
                case ETrackedDeviceClass.HMD:
                    if (!_deviceTable.ContainsKey(trackedDeviceIndex))
                    {
                        device = new SteamVRHeadMountedDisplay(trackedDeviceIndex);
                    }
                    break;
                case ETrackedDeviceClass.Controller:
                    if (!_deviceTable.ContainsKey(trackedDeviceIndex))
                    {
                        device = new SteamVRController(trackedDeviceIndex);
                    }
                    break;
                case ETrackedDeviceClass.TrackingReference:
                    if (!_deviceTable.ContainsKey(trackedDeviceIndex))
                    {
                        device = new SteamVRTracker(trackedDeviceIndex);
                    }
                    break;
            }

            if (device != null)
            {
                // Fetch all relevant device properties from SteamVR
                device.UpdateProperties(_steamVRSystem);

                // Add the device to the pending-load set
                _deviceTable.Add(trackedDeviceIndex, device);
            }
        }

        private void HandleTrackedDevicePropertyChanged(uint trackedDeviceIndex)
        {
            if (_deviceTable.ContainsKey(trackedDeviceIndex)) 
            {
                SteamVRTrackedDevice device= _deviceTable[trackedDeviceIndex];

                // Refresh all the properties on the device
                device.UpdateProperties(_steamVRSystem);

                if (TrackedDeviceActivatedEvent != null) {
                    TrackedDeviceActivatedEvent(device);
                }
            }
        }

        private void HandleTrackedDeviceDeactivated(uint trackedDeviceIndex)
        {
            if (_deviceTable.ContainsKey(trackedDeviceIndex)) {
                if (TrackedDeviceDeactivatedEvent != null) {
                    TrackedDeviceDeactivatedEvent(_deviceTable[trackedDeviceIndex]);
                }

                _deviceTable.Remove(trackedDeviceIndex);
            }
        }

        private bool HandleTrackedDevicePoseUpdated(uint trackedDeviceIndex, TrackedDevicePose_t pose)
        {
            if (_deviceTable.ContainsKey(trackedDeviceIndex))
            {
                _deviceTable[trackedDeviceIndex].ApplyPoseUpdate(pose);

                return true;
            }

            return false;
        }
    }
}
using System;
using PSMoveService;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using System.ComponentModel;

namespace SystemTrayApp
{
    public class PSMoveServiceContext : SynchronizedContext
    {
        private static string PSMOVESTEAMVRBRIDE_REGKEY_PATH = @"SOFTWARE\WOW6432Node\PSMoveSteamVRBridge\PSMoveSteamVRBridge";
        private static string PSMOVESERVICE_PROCESS_NAME = "PSMoveService";
        private static double POLL_INTERVAL_5FPS = 1.0 / 5.0; // ms
        //private static double POLL_INTERVAL_60FPS = 1.0 / 60.0; // ms

        private static readonly Lazy<PSMoveServiceContext> _lazyInstance = 
            new Lazy<PSMoveServiceContext>(() => new PSMoveServiceContext());
        public static PSMoveServiceContext Instance 
        { 
            get { return _lazyInstance.Value; }
        }

        private bool _bInitialized;
        private Timer _pollTimer;

        public enum PSMConnectionState
        {
            disconnected,
            waitingForConnectionResponse,
            connected
        }

        private PSMConnectionState _connectionState;
        public PSMConnectionState ConnectionState
        {
            get { return _connectionState; }
        }

        private PSMClientControllerInfo[] _controllerInfoList;
        public PSMClientControllerInfo[] ControllerInfoList
        {
            get { return _controllerInfoList; }
        }

        private PSMClientHMDInfo[] _hmdInfoList;
        public PSMClientHMDInfo[] HmdInfoList
        {
            get { return _hmdInfoList; }
        }

        private PSMClientTrackerInfo[] _trackerInfoList;
        public PSMClientTrackerInfo[] TrackerInfoList
        {
            get { return _trackerInfoList; }
        }

        public delegate void ConnectedToPSMService();
        public event ConnectedToPSMService ConnectedToPSMServiceEvent;

        public delegate void DisconnectedFromPSMService();
        public event DisconnectedFromPSMService DisconnectedFromPSMServiceEvent;

        public delegate void PSMMessagesPolled();
        public event PSMMessagesPolled PSMMessagesPolledEvent;

        public delegate void ControllerListUpdated();
        public event ControllerListUpdated ControllerListUpdatedEvent;

        public delegate void TrackerListUpdated();
        public event TrackerListUpdated TrackerListUpdatedEvent;

        public delegate void HmdListUpdated();
        public event HmdListUpdated HmdListUpdatedEvent;

        public delegate void ControllerConfigListPreUpdate();
        public event ControllerConfigListPreUpdate ControllerConfigListPreUpdateEvent;

        public delegate void ControllerConfigListPostUpdate(List<ControllerConfig> newControllerConfigList);
        public event ControllerConfigListPostUpdate ControllerConfigListPostUpdateEvent;

        private PSMoveServiceContext()
        {
            _controllerInfoList = new PSMClientControllerInfo[0];
            _hmdInfoList = new PSMClientHMDInfo[0];
            _trackerInfoList = new PSMClientTrackerInfo[0];
            _pollTimer = new System.Timers.Timer();
            _bInitialized = false;
            _connectionState = PSMConnectionState.disconnected;
        }

        public bool Init()
        {
            if (!_bInitialized)
            {
                // Create a timer to poll PSMoveService state with
                _pollTimer.Elapsed += RunFrame;
                _pollTimer.AutoReset = false; // NO AUTO RESET! Restart in RunFrame().
                _pollTimer.Enabled = true;
                _pollTimer.Interval = POLL_INTERVAL_5FPS;
                _pollTimer.Start();

                _bInitialized = true;
            }

            return true;
        }

        public void Cleanup()
        {
            if (_bInitialized)
            {
                // Disconnected the timer callback
                _pollTimer.Elapsed -= RunFrame;

                // Shutdown PSMove Client API
                PSMoveClient.PSM_Shutdown();
                _connectionState = PSMConnectionState.disconnected;
                _bInitialized = false;
            }
        }

        private void RunFrame(object sender, ElapsedEventArgs e)
        {
            if (_connectionState == PSMConnectionState.connected ||
                _connectionState == PSMConnectionState.waitingForConnectionResponse)
            {
                // Update any controllers that are currently listening
                PSMoveClient.PSM_UpdateNoPollMessages();

                // Poll events queued up by the call to PSM_UpdateNoPollMessages()
                PSMMessage mesg = new PSMMessage();
                while (PSMoveClient.PSM_PollNextMessage(mesg) == PSMResult.PSMResult_Success) {
                    switch (mesg.payload_type) {
                        case PSMMessageType._messagePayloadType_Response:
                            HandleClientPSMoveResponse(mesg);
                            break;
                        case PSMMessageType._messagePayloadType_Event:
                            HandleClientPSMoveEvent(mesg);
                            break;
                    }
                }

                if (PSMMessagesPolledEvent != null) {
                    PSMMessagesPolledEvent();
                }
            }
            else
            {
                TryConnectToPSMoveService();
            }

            // Restart the timer once event handling is complete.
            // This prevents overlapping callings to RunFrame.
            // In practice this is only an issue when debugging.
            _pollTimer.Start();
        }

        private void HandleClientPSMoveResponse(PSMMessage message)
        {
            switch (message.response_data.payload_type)
            {
                case PSMResponsePayloadType._responsePayloadType_Empty:
                    Trace.TraceInformation(
                        string.Format(
                            "NotifyClientPSMoveResponse - request id {0} returned result {1}.",
                            message.response_data.request_id,
                            (message.response_data.result_code == PSMResult.PSMResult_Success) ? "ok" : "error"));
                    break;
                case PSMResponsePayloadType._responsePayloadType_ControllerList:
                    Trace.TraceInformation(
                        string.Format(
                            "NotifyClientPSMoveResponse - Controller Count = {0} (request id {1}).",
                                    message.response_data.payload.controller_list.count, 
                                    message.response_data.request_id));
                    HandleControllerListReponse(
                        message.response_data.payload.controller_list, 
                        message.response_data.opaque_request_handle);
                    break;
                case PSMResponsePayloadType._responsePayloadType_TrackerList:
                    Trace.TraceInformation(
                        string.Format(
                            "NotifyClientPSMoveResponse - Tracker Count = {0} (request id {1}).",
                            message.response_data.payload.tracker_list.count, 
                            message.response_data.request_id));
                    HandleTrackerListReponse(
                        message.response_data.payload.tracker_list,
                        message.response_data.opaque_request_handle);
                    break;
                case PSMResponsePayloadType._responsePayloadType_HmdList:
                    Trace.TraceInformation(
                        string.Format(
                            "NotifyClientPSMoveResponse - HMD Count = {0} (request id {1}).",
                            message.response_data.payload.hmd_list.count,
                            message.response_data.request_id));
                    HandleHmdListReponse(
                        message.response_data.payload.hmd_list,
                        message.response_data.opaque_request_handle);
                    break;
                default:
                    Trace.TraceInformation(
                        string.Format(
                            "NotifyClientPSMoveResponse - Unhandled response (request id {0}).", 
                            message.response_data.request_id));
                    break;
            }
        }

        private void HandleClientPSMoveEvent(PSMMessage message)
        {
            switch (message.event_data.event_type)
            {
                // Client Events
                case PSMEventMessageType.PSMEvent_connectedToService:
                    HandleConnectedToPSMoveService();
                    break;
                case PSMEventMessageType.PSMEvent_failedToConnectToService:
                    HandleFailedToConnectToPSMoveService();
                    break;
                case PSMEventMessageType.PSMEvent_disconnectedFromService:
                    HandleDisconnectedFromPSMoveService();
                    break;

                    // Service Events
                case PSMEventMessageType.PSMEvent_opaqueServiceEvent:
                    // We don't care about any opaque service events
                    break;
                case PSMEventMessageType.PSMEvent_controllerListUpdated:
                    HandleControllerListChanged();
                    break;
                case PSMEventMessageType.PSMEvent_trackerListUpdated:
                    // don't care
                    break;
                case PSMEventMessageType.PSMEvent_hmdListUpdated:
                    // don't care
                    break;
                case PSMEventMessageType.PSMEvent_systemButtonPressed:
                    // don't care
                    break;
                    //###HipsterSloth $TODO - Need a notification for when a tracker pose changes
            }
        }

        void HandleConnectedToPSMoveService()
        {
            Trace.TraceInformation("HandleConnectedToPSMoveService - Request service version");

            int request_id;
            PSMoveClient.PSM_GetServiceVersionStringAsync(out request_id);
            PSMoveClient.PSM_RegisterDelegate(request_id, this.HandleServiceVersionResponse);
        }

        void HandleServiceVersionResponse(PSMResponseMessage response)
        {
            PSMResult ResultCode = response.result_code;

            switch (ResultCode)
            {
                case PSMResult.PSMResult_Success: 
                {
                    string service_version = response.payload.service_version.version_string;
                    string local_version = PSMoveClient.PSM_GetClientVersionString();

                    if (service_version == local_version)
                    {
                        Trace.TraceInformation(
                            string.Format(
                            "HandleServiceVersionResponse - Received expected protocol version {0}",
                            service_version));

                        // Connection is now ready
                        _connectionState = PSMConnectionState.connected;

                        // Fire off connection delegate
                        ConnectedToPSMServiceEvent();

                        // Ask the service for a list of connected controllers
                        // Response handled in HandleControllerListReponse()
                        int request_id = -1;
                        PSMoveClient.PSM_GetControllerListAsync(out request_id);
                    }
                    else
                    {
                        Trace.TraceInformation(
                            string.Format(
                                "HandleServiceVersionResponse - Protocol mismatch! Expected {0}, got {1}. Please reinstall the PSMove Driver!",
                                local_version, service_version));
                        Cleanup();
                    }
                }
                break;
                case PSMResult.PSMResult_Error:
                case PSMResult.PSMResult_Canceled:
                {
                    Trace.TraceInformation("HandleServiceVersionResponse - Failed to get protocol version\n");
                }
                break;
            }
        }

        void HandleFailedToConnectToPSMoveService()
        {
            Trace.TraceInformation("HandleFailedToConnectToPSMoveService - Called");
            _connectionState = PSMConnectionState.disconnected;
        }

        void HandleDisconnectedFromPSMoveService()
        {
            Trace.TraceInformation("HandleDisconnectedFromPSMoveService - Called");

            // Fire off disconnection delegate
            DisconnectedFromPSMServiceEvent();

            _connectionState = PSMConnectionState.disconnected;
        }

        void HandleControllerListChanged()
        {
            Trace.TraceInformation("CServerDriver_PSMoveService::HandleControllerListChanged - Called");

            // Ask the service for a list of connected controllers
            // Response handled in HandleControllerListReponse()
            int request_id = -1;
            PSMoveClient.PSM_GetControllerListAsync(out request_id);
        }

        void HandleControllerListReponse(PSMControllerList controller_list, IntPtr response_handle)
        {
            Trace.TraceInformation(
                string.Format(
                    "CServerDriver_PSMoveService::HandleControllerListReponse - Received {0} controllers", 
                    controller_list.count));

            // Notify any listeners that the controller list is about to change changed
            if (ControllerConfigListPreUpdateEvent != null) {
                ControllerConfigListPreUpdateEvent();
            }

            // Rebuild the controller list and assigning to the config manager
            List<ControllerConfig> controllerConfigList = new List<ControllerConfig>();
            {
                // Update the controller info list
                _controllerInfoList = controller_list.controllers;

                for (int list_index = 0; list_index < _controllerInfoList.Length; ++list_index) {
                    PSMClientControllerInfo controller_info = _controllerInfoList[list_index];
                    int psmControllerId = controller_info.controller_id;
                    PSMControllerType psmControllerType = controller_info.controller_type;
                    string psmControllerSerial = controller_info.controller_serial.Replace(":", "_");

                    switch (psmControllerType) {
                        case PSMControllerType.PSMController_Move:
                            Trace.TraceInformation(
                                string.Format(
                                    "CServerDriver_PSMoveService::HandleControllerListReponse - Allocate PSMove({0})",
                                    psmControllerId));
                            controllerConfigList.Add(new PSMoveControllerConfig(psmControllerSerial));
                            break;
                        case PSMControllerType.PSMController_Virtual:
                            Trace.TraceInformation(
                                string.Format(
                                    "CServerDriver_PSMoveService::HandleControllerListReponse - Allocate VirtualController({0})",
                                    psmControllerId));
                            controllerConfigList.Add(new VirtualControllerConfig(psmControllerSerial));
                            break;
                        case PSMControllerType.PSMController_Navi:
                            Trace.TraceInformation(
                                string.Format(
                                    "CServerDriver_PSMoveService::HandleControllerListReponse - Allocate NaviController({0})",
                                    psmControllerId));
                            controllerConfigList.Add(new PSNaviControllerConfig(psmControllerSerial));
                            break;
                        case PSMControllerType.PSMController_DualShock4:
                            Trace.TraceInformation(
                                string.Format(
                                    "CServerDriver_PSMoveService::HandleControllerListReponse - Allocate PSDualShock4({0})",
                                    psmControllerId));
                            controllerConfigList.Add(new PSNaviControllerConfig(psmControllerSerial));
                            break;
                        default:
                            break;
                    }
                }

            }

            // Notify any listeners that the controller list changed
            if (ControllerConfigListPostUpdateEvent != null) {
                ControllerConfigListPostUpdateEvent(controllerConfigList);
            }
            if (ControllerListUpdatedEvent != null) {
                ControllerListUpdatedEvent();
            } 
        }

        void HandleTrackerListReponse(PSMTrackerList tracker_list, IntPtr response_handle)
        {
            Trace.TraceInformation(
                string.Format(
                    "CServerDriver_PSMoveService::HandleTrackerListReponse - Received {0} trackers",
                    tracker_list.count));

            // Update the tracker info list
            _trackerInfoList = tracker_list.trackers;

            // Notify any listeners that the tracker list changed
            if (TrackerListUpdatedEvent != null) {
                TrackerListUpdatedEvent();
            }
        }

        void HandleHmdListReponse(PSMHmdList hmd_list, IntPtr response_handle)
        {
            Trace.TraceInformation(
                string.Format(
                    "CServerDriver_PSMoveService::HandleHmdListReponse - Received {0} HMDs",
                    hmd_list.count));

            // Update the HMD info list
            _hmdInfoList = hmd_list.hmds;

            // Notify any listeners that the tracker list changed
            if (HmdListUpdatedEvent != null) {
                HmdListUpdatedEvent();
            }
        }

        void AllocateUniquePSMoveController(string psmControllerSerial)
        {
            PSMoveControllerConfig config = new PSMoveControllerConfig(psmControllerSerial);
        }

        public bool IsConnected
        {
            get { return PSMoveClient.PSM_GetIsConnected(); }
        }

        public bool TryConnectToPSMoveService()
        {
            // Only attempt connection once this is a window listening for a connection
            if (ConnectedToPSMServiceEvent != null && DisconnectedFromPSMServiceEvent != null)
            {
                if (GetIsPSMoveServiceRunning()) {
                    PSMoveSteamVRBridgeConfig config = PSMoveSteamVRBridgeConfig.Instance;

                    if (PSMoveClient.PSM_InitializeAsync(config.ServerAddress, config.ServerPort) != PSMResult.PSMResult_Error) {
                        _connectionState = PSMConnectionState.waitingForConnectionResponse;
                        return true;
                    }
                    else {
                        Trace.TraceInformation("TryConnectToPSMoveService - Error initializing PSMoveService connection");
                    }
                }
            }

            return false;
        }

        public void DisconnectFromPSMoveService()
        {
            if (ConnectionState != PSMConnectionState.connected ||
                ConnectionState != PSMConnectionState.waitingForConnectionResponse)
            {
                if (ConnectionState == PSMConnectionState.connected) 
                {
                    HandleDisconnectedFromPSMoveService();
                }

                PSMoveClient.PSM_Shutdown();
                _connectionState = PSMConnectionState.disconnected;
            }
        }

        public bool LaunchPSMoveServiceProcess()
        {
            if (!GetIsPSMoveServiceRunning())
            {
                string psm_path = GetPSMoveServicePath();
                if (psm_path.Length > 0)
                {
                    bool bSuccess = false;

                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = psm_path;
                        process.StartInfo.CreateNoWindow = false;
                        process.StartInfo.UseShellExecute = true;
                        bSuccess = process.Start();
                    }
                    catch (Exception e)
                    {
                        Trace.TraceInformation(
                            string.Format(
                                "LaunchPSMoveServiceProcess - Error launching PSMoveService: {0}",
                                e.Message));
                    }

                    return bSuccess;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public void TerminatePSMoveServiceProcess()
        {
            DisconnectFromPSMoveService();

            foreach (Process process in Process.GetProcessesByName(PSMOVESERVICE_PROCESS_NAME))
            {
                if (!process.HasExited)
                {
                    process.CloseMainWindow();
                }
            }
        }

        public static string GetPSMoveSteamVRBridgeInstallPath()
        {
            string install_path= "";

            if (PSMoveSteamVRBridgeConfig.Instance.UseInstallationPath) 
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(PSMOVESTEAMVRBRIDE_REGKEY_PATH)) 
                    {
                        if (key != null)
                        {
                            object value = key.GetValue("Location");

                            if (value is string)
                            {
                                install_path = (string)value;
                            }
                        }
                    }                
                }
            }

            if (install_path.Length == 0)
            {
                install_path = SystemTrayApp.Program.GetExecutingDirectoryName();
            }

            return install_path;
        }

        public string GetPSMoveServicePath()
        {
            string install_path = GetPSMoveSteamVRBridgeInstallPath();
            string psm_path = "";

            if (install_path.Length > 0)
            {
                psm_path = Path.Combine(install_path, PSMOVESERVICE_PROCESS_NAME);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    psm_path = psm_path + ".exe";
                }

                if (!File.Exists(psm_path))
                {
                    psm_path = "";
                }
            }

            return psm_path;
        }

        public static bool GetIsPSMoveServiceRunning()
        {
            foreach (Process process in Process.GetProcessesByName(PSMOVESERVICE_PROCESS_NAME)) {
                if (!process.HasExited) {
                    return true;
                }
            }

            return false;
        }
    }
}
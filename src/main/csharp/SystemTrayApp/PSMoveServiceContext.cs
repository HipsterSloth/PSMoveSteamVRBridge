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
    public class PSMoveServiceContext
    {
        private static string PSMOVESTEAMVRBRIDE_REGKEY_PATH = @"SOFTWARE\WOW6432Node\PSMoveSteamVRBridge\PSMoveSteamVRBridge";
        private static string PSMOVESERVICE_PROCESS_NAME = "PSMoveService";
        private static int POLL_INTERVAL = 100; // ms

        private static readonly Lazy<PSMoveServiceContext> lazy = 
            new Lazy<PSMoveServiceContext>(() => new PSMoveServiceContext());
        public static PSMoveServiceContext Instance { get { return lazy.Value; } }        

        private bool bInitialized;
        private Timer PollTimer;

        private PSMoveServiceContext()
        {
            PollTimer = new System.Timers.Timer(POLL_INTERVAL);
            bInitialized = false;
        }

        public bool Init()
        {
            if (!bInitialized)
            {
                // Note that reconnection is a non-blocking async request.
                // Returning true means we we're able to start trying to connect,
                // not that we are successfully connected yet.
                if (!ReconnectToPSMoveService())
                {
                    return false;
                }

                // Create a timer to poll PSMoveService state with
                PollTimer.Elapsed += RunFrame;
                PollTimer.AutoReset = false; // NO AUTO RESET! Restart in RunFrame().
                PollTimer.Enabled = true;

                bInitialized = true;
            }

            return true;
        }

        public void Cleanup()
        {
            if (bInitialized)
            {
                // Disconnected the timer callback
                PollTimer.Elapsed -= RunFrame;

                // Shutdown PSMove Client API
                PSMoveClient.PSM_Shutdown();
                bInitialized = false;
            }
        }

        private void RunFrame(object sender, ElapsedEventArgs e)
        {
            // Update any controllers that are currently listening
            PSMoveClient.PSM_UpdateNoPollMessages();

            // Poll events queued up by the call to PSM_UpdateNoPollMessages()
            PSMMessage mesg = new PSMMessage();
            while (PSMoveClient.PSM_PollNextMessage(mesg) == PSMResult.PSMResult_Success) 
            {
                switch (mesg.payload_type) 
                {
                    case PSMMessageType._messagePayloadType_Response:
                        HandleClientPSMoveResponse(mesg);
                        break;
                    case PSMMessageType._messagePayloadType_Event:
                        HandleClientPSMoveEvent(mesg);
                        break;
                }
            }

            // Restart the timer once event handling is complete.
            // This prevents overlaping callings to RunFrame.
            // In practice this is only an issue when debugging.
            PollTimer.Start();
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

        public delegate void ConnectedToPSMService();
        public event ConnectedToPSMService ConnectedToPSMServiceEvent;

        public delegate void DisconnectedFromPSMService();
        public event DisconnectedFromPSMService DisconnectedFromPSMServiceEvent;

        void HandleConnectedToPSMoveService()
        {
            Trace.TraceInformation("CServerDriver_PSMoveService::HandleConnectedToPSMoveService - Request controller and tracker lists");

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
                            "CServerDriver_PSMoveService::HandleServiceVersionResponse - Received expected protocol version {0}",
                            service_version));

                        // Fire off connection delegate
                        if (ConnectedToPSMServiceEvent != null)
                        {
                            ConnectedToPSMServiceEvent();
                        }

                        // Ask the service for a list of connected controllers
                        // Response handled in HandleControllerListReponse()
                        int request_id = -1;
                        PSMoveClient.PSM_GetControllerListAsync(out request_id);
                    }
                    else
                    {
                        Trace.TraceInformation(
                            string.Format(
                                "CServerDriver_PSMoveService::HandleServiceVersionResponse - Protocol mismatch! Expected {0}, got {1}. Please reinstall the PSMove Driver!",
                                local_version, service_version));
                        Cleanup();
                    }
                }
                break;
                case PSMResult.PSMResult_Error:
                case PSMResult.PSMResult_Canceled:
                {
                    Trace.TraceInformation("CServerDriver_PSMoveService::HandleServiceVersionResponse - Failed to get protocol version\n");
                }
                break;
            }
        }

        void HandleFailedToConnectToPSMoveService()
        {
            Trace.TraceInformation("CServerDriver_PSMoveService::HandleFailedToConnectToPSMoveService - Called");

            // Immediately attempt to reconnect to the service
            //ReconnectToPSMoveService();
        }

        void HandleDisconnectedFromPSMoveService()
        {
            Trace.TraceInformation("CServerDriver_PSMoveService::HandleDisconnectedFromPSMoveService - Called");

            // Fire off disconnection delegate
            if (DisconnectedFromPSMServiceEvent != null)
            {
                DisconnectedFromPSMServiceEvent();
            }

            // Immediately attempt to reconnect to the service
            //ReconnectToPSMoveService();
        }

        public delegate void ControllerListPreUpdate();
        public event ControllerListPreUpdate ControllerListPreUpdateEvent;

        public delegate void ControllerListPostUpdate(List<ControllerConfig> newControllerConfigList);
        public event ControllerListPostUpdate ControllerListPostUpdateEvent;

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
            if (ControllerListPreUpdateEvent != null) {
                ControllerListPreUpdateEvent();
            }

            // Rebuild the controller list and assigning to the config manager
            List<ControllerConfig> controllerConfigList = new List<ControllerConfig>();
            {
                PSMClientControllerInfo[] controllers = controller_list.controllers;
                for (int list_index = 0; list_index < controllers.Length; ++list_index) {
                    PSMClientControllerInfo controller_info = controllers[list_index];
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
            if (ControllerListPostUpdateEvent != null) {
                ControllerListPostUpdateEvent(controllerConfigList);
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

        public bool ReconnectToPSMoveService()
        {
            PSMoveSteamVRBridgeConfig config = PSMoveSteamVRBridgeConfig.Instance;

            if (PSMoveClient.PSM_GetIsConnected())
            {
                PSMoveClient.PSM_Shutdown();
            }

            bool bSuccess= PSMoveClient.PSM_InitializeAsync(config.ServerAddress, config.ServerPort) != PSMResult.PSMResult_Error;

            return bSuccess;
        }

        public void DisconnectFromPSMoveService()
        {
            if (PSMoveClient.PSM_GetIsConnected())
            {
                PSMoveClient.PSM_Shutdown();
            }
        }

        public bool LaunchPSMoveServiceProcess()
        {
            if (!GetIsPSMoveServiceRunning())
            {
                string psm_path = GetPSMoveServicePath();

                if (psm_path.Length > 0)
                {
                    Process process = Process.Start(psm_path);

                    if (process != null) 
                    {
                        ReconnectToPSMoveService();
                        return true;
                    }
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
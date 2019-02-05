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
    public class ControllerInfo
    {
        private int _controller_id;
        private PSMControllerType _controller_type;
        private PSMControllerHand _controller_hand;
        private PSMTrackingColorType _tracking_color_type;
        private bool _is_bluetooth;
        private bool _has_magnetometer;
        private float _prediction_time;
        private int _gamepad_index;
        private string _device_path;
        private string _orientation_filter;
        private string _position_filter;
        private string _gyro_gain_setting;
        private string _controller_serial;
        private string _assigned_host_serial;
        private string _parent_controller_serial;

        public int controller_id { get { return _controller_id; } }
        public PSMControllerType controller_type { get { return _controller_type; } }
        public PSMControllerHand controller_hand { get { return _controller_hand; } }
        public PSMTrackingColorType tracking_color_type { get { return _tracking_color_type; } }
        public bool is_bluetooth { get { return _is_bluetooth; } }
        public bool has_magnetometer { get { return _has_magnetometer; } }
        public float prediction_time { get { return _prediction_time; } }
        public int gamepad_index { get { return _gamepad_index; } }
        public string device_path { get { return _device_path; } }
        public string orientation_filter { get { return _orientation_filter; } }
        public string position_filter { get { return _position_filter; } }
        public string gyro_gain_setting { get { return _gyro_gain_setting; } }
        public string controller_serial { get { return _controller_serial; } }
        public string assigned_host_serial { get { return _assigned_host_serial; } }
        public string parent_controller_serial { get { return _parent_controller_serial; } }

        public static ControllerInfo[] MakeArray(PSMClientControllerInfo[] controllers)
        {
            ControllerInfo[] array = new ControllerInfo[controllers.Length];

            for (int index= 0; index < array.Length; ++index)
            {
                array[index] = ControllerInfo.MakeInstance(controllers[index]);
            }

            return array;
        }

        public static ControllerInfo MakeInstance(PSMClientControllerInfo controller_info)
        {
            ControllerInfo instance = new ControllerInfo
            {
                _controller_id = controller_info.controller_id,
                _controller_type = controller_info.controller_type,
                _controller_hand= controller_info.controller_hand,
                _tracking_color_type= controller_info.tracking_color_type,
                _is_bluetooth= controller_info.is_bluetooth,
                _has_magnetometer= controller_info.has_magnetometer,
                _prediction_time= controller_info.prediction_time,
                _gamepad_index= controller_info.gamepad_index,
                _device_path= controller_info.device_path,
                _orientation_filter= controller_info.orientation_filter,
                _position_filter= controller_info.position_filter,
                _gyro_gain_setting= controller_info.gyro_gain_setting,
                _controller_serial= controller_info.controller_serial,
                _assigned_host_serial= controller_info.assigned_host_serial,
                _parent_controller_serial= controller_info.parent_controller_serial
            };

            return instance;
        }
    }

    public class HMDInfo
    {
        private int _hmd_id;
        private PSMHmdType _hmd_type;
        private PSMTrackingColorType _tracking_color_type;
        private string _device_path;
        private string _orientation_filter;
        private string _position_filter;
        private float _prediction_time;

        public int hmd_id { get { return _hmd_id; } }
        public PSMHmdType hmd_type { get { return _hmd_type; } }
        public PSMTrackingColorType tracking_color_type { get { return _tracking_color_type; } }
        public string device_path { get { return _device_path; } }
        public string orientation_filter { get { return _orientation_filter; } }
        public string position_filter { get { return _position_filter; } }
        public float prediction_time { get { return _prediction_time; } }

        public static HMDInfo[] MakeArray(PSMClientHMDInfo[] hmds)
        {
            HMDInfo[] array = new HMDInfo[hmds.Length];

            for (int index = 0; index < array.Length; ++index)
            {
                array[index] = HMDInfo.MakeInstance(hmds[index]);
            }

            return array;
        }

        public static HMDInfo MakeInstance(PSMClientHMDInfo hmd_info)
        {
            HMDInfo instance = new HMDInfo
            {
                _hmd_id= hmd_info.hmd_id,
                _hmd_type= hmd_info.hmd_type,
                _tracking_color_type= hmd_info.tracking_color_type,
                _device_path= hmd_info.device_path,
                _orientation_filter= hmd_info.orientation_filter,
                _position_filter= hmd_info.position_filter,
                _prediction_time= hmd_info.prediction_time
            };

            return instance;
        }
    }

    public class TrackerInfo
    {
        // ID of the tracker in the service
        private int _tracker_id;
        // Tracker USB properties
        private PSMTrackerType _tracker_type;
        private PSMTrackerDriver _tracker_driver;
        private string _device_path;
        // Video stream properties
        private string _shared_memory_name;
        // Camera Intrinsic properties
        private PSMVector2f _tracker_focal_lengths; ///< lens focal length in pixels
        private PSMVector2f _tracker_principal_point; ///< lens center point in pixels
        private PSMVector2f _tracker_screen_dimensions; ///< tracker image size in pixels
        private float _tracker_hfov; ///< tracker horizontal FOV in degrees
        private float _tracker_vfov; ///< tracker vertical FOV in degrees
        private float _tracker_znear; ///< tracker z-near plane distance in cm
        private float _tracker_zfar; ///< tracker z-far plane distance in cm
        private float _tracker_k1; ///< lens distortion coefficient k1
        private float _tracker_k2; ///< lens distortion coefficient k2
        private float _tracker_k3; ///< lens distortion coefficient k3
        private float _tracker_p1; ///< lens distortion coefficient p1
        private float _tracker_p2; ///< lens distortion coefficient p2
        // Camera Extrinsic properties
        private PSMPosef _tracker_pose;

        // ID of the tracker in the service
        public int tracker_id { get { return _tracker_id; } }
        // Tracker USB properties
        public PSMTrackerType tracker_type { get { return _tracker_type; } }
        public PSMTrackerDriver tracker_driver { get { return _tracker_driver; } }
        public string device_path { get { return _device_path; } }
        // Video stream properties
        public string shared_memory_name { get { return _shared_memory_name; } }
        // Camera Intrinsic properties
        public PSMVector2f tracker_focal_lengths { get { return _tracker_focal_lengths; } }
        public PSMVector2f tracker_principal_point { get { return _tracker_principal_point; } }
        public PSMVector2f tracker_screen_dimensions { get { return _tracker_screen_dimensions; } }
        public float tracker_hfov { get { return _tracker_hfov; } }
        public float tracker_vfov { get { return _tracker_vfov; } }
        public float tracker_znear { get { return _tracker_znear; } }
        public float tracker_zfar { get { return _tracker_zfar; } }
        public float tracker_k1 { get { return _tracker_k1; } }
        public float tracker_k2 { get { return _tracker_k2; } }
        public float tracker_k3 { get { return _tracker_k3; } }
        public float tracker_p1 { get { return _tracker_p1; } }
        public float tracker_p2 { get { return _tracker_p2; } }
        // Camera Extrinsic properties
        public PSMPosef tracker_pose { get { return _tracker_pose; } }

        public static TrackerInfo[] MakeArray(PSMClientTrackerInfo[] trackers)
        {
            TrackerInfo[] array = new TrackerInfo[trackers.Length];

            for (int index = 0; index < array.Length; ++index)
            {
                array[index] = TrackerInfo.MakeInstance(trackers[index]);
            }

            return array;
        }

        private static TrackerInfo MakeInstance(PSMClientTrackerInfo tracker_info)
        {
            TrackerInfo instance = new TrackerInfo
            {
                _tracker_id = tracker_info.tracker_id,
                _tracker_type = tracker_info.tracker_type,
                _tracker_driver = tracker_info.tracker_driver,
                _device_path = tracker_info.device_path,
                _tracker_focal_lengths = new PSMVector2f()
                {
                    x = tracker_info.tracker_focal_lengths.x,
                    y = tracker_info.tracker_focal_lengths.y
                },
                _tracker_principal_point = new PSMVector2f()
                {
                    x = tracker_info.tracker_principal_point.x,
                    y = tracker_info.tracker_principal_point.y,
                },
                _tracker_screen_dimensions = new PSMVector2f()
                {
                    x = tracker_info.tracker_screen_dimensions.x,
                    y = tracker_info.tracker_screen_dimensions.y
                },
                _tracker_hfov = tracker_info.tracker_hfov,
                _tracker_vfov = tracker_info.tracker_vfov,
                _tracker_znear = tracker_info.tracker_znear,
                _tracker_zfar = tracker_info.tracker_zfar,
                _tracker_k1 = tracker_info.tracker_k1,
                _tracker_k2 = tracker_info.tracker_k2,
                _tracker_k3 = tracker_info.tracker_k3,
                _tracker_p1 = tracker_info.tracker_p1,
                _tracker_p2 = tracker_info.tracker_p2,
                _tracker_pose = new PSMPosef()
                {
                    Orientation = new PSMQuatf()
                    {
                        w = tracker_info.tracker_pose.Orientation.w,
                        x = tracker_info.tracker_pose.Orientation.x,
                        y = tracker_info.tracker_pose.Orientation.y,
                        z = tracker_info.tracker_pose.Orientation.z
                    },
                    Position = new PSMVector3f()
                    {
                        x = tracker_info.tracker_pose.Position.x,
                        y = tracker_info.tracker_pose.Position.y,
                        z = tracker_info.tracker_pose.Position.z
                    }
                }
            };

            return instance;
        }
    }

    public class PSMoveServiceContext : SynchronizedContext
    {
        private static string PSMOVESTEAMVRBRIDE_REGKEY_PATH = @"SOFTWARE\WOW6432Node\PSMoveSteamVRBridge\PSMoveSteamVRBridge";
        private static string PSMOVESERVICE_PROCESS_NAME = "PSMoveService";
        private static double POLL_INTERVAL_5FPS = 1.0 / 5.0; // ms
        private static double POLL_INTERVAL_60FPS = 1.0 / 60.0; // ms

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

        private ControllerInfo[] _controllerInfoList;
        public ControllerInfo[] ControllerInfoList
        {
            get { return _controllerInfoList; }
        }

        private HMDInfo[] _hmdInfoList;
        public HMDInfo[] HmdInfoList
        {
            get { return _hmdInfoList; }
        }

        private TrackerInfo[] _trackerInfoList;
        public TrackerInfo[] TrackerInfoList
        {
            get { return _trackerInfoList; }
        }

        private int _pendingControllerRequestId;
        private int _pendingHmdRequestId;
        private int _pendingTrackerRequestId;

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
            _controllerInfoList = new ControllerInfo[0];
            _hmdInfoList = new HMDInfo[0];
            _trackerInfoList = new TrackerInfo[0];
            _pollTimer = new System.Timers.Timer();
            _bInitialized = false;
            _connectionState = PSMConnectionState.disconnected;
            _pendingControllerRequestId = -1;
            _pendingHmdRequestId = -1;
            _pendingTrackerRequestId = -1;
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

                // Reset all state
                _connectionState = PSMConnectionState.disconnected;
                _controllerInfoList = new ControllerInfo[0];
                _hmdInfoList = new HMDInfo[0];
                _trackerInfoList = new TrackerInfo[0];
                _pendingControllerRequestId = -1;
                _pendingHmdRequestId = -1;
                _pendingTrackerRequestId = -1;
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

            // Make the polling interval run more frequently when there are active controller streams
            _pollTimer.Interval =
                (PSMDevicePool.InitializedDevicePoolCount > 0)
                ? POLL_INTERVAL_60FPS : POLL_INTERVAL_5FPS;

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
                        message.response_data.request_id,
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
                        message.response_data.request_id,
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
                        message.response_data.request_id,
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

                        // Ask the service for a list of connected controllers
                        // Response handled in HandleControllerListReponse()
                        PSMoveClient.PSM_GetControllerListAsync(out _pendingControllerRequestId);

                        // Ask the service for a list of connected HMDs
                        // Response handled in HandleHmdListReponse()
                        PSMoveClient.PSM_GetHmdListAsync(out _pendingHmdRequestId);

                        // Ask the service for a list of connected Trackers
                        // Response handled in HandleTrackerListReponse()
                        PSMoveClient.PSM_GetTrackerListAsync(out _pendingTrackerRequestId);
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
            if (_pendingControllerRequestId == -1)
            {
                PSMoveClient.PSM_GetControllerListAsync(out _pendingControllerRequestId);
            }
        }

        void HandleHmdListChanged()
        {
            Trace.TraceInformation("CServerDriver_PSMoveService::HandleHmdListChanged - Called");

            // Ask the service for a list of connected HMDs
            // Response handled in HandleHmdListReponse()
            if (_pendingHmdRequestId == -1)
            {
                PSMoveClient.PSM_GetHmdListAsync(out _pendingHmdRequestId);
            }
        }

        void HandleTrackerListChanged()
        {
            Trace.TraceInformation("CServerDriver_PSMoveService::HandleTrackerListChanged - Called");

            // Ask the service for a list of connected Trackers
            // Response handled in HandleTrackerListReponse()
            if (_pendingTrackerRequestId == -1)
            {
                PSMoveClient.PSM_GetTrackerListAsync(out _pendingTrackerRequestId);
            }
        }

        void HandleControllerListReponse(int request_id, PSMControllerList controller_list, IntPtr response_handle)
        {
            Trace.TraceInformation(
                string.Format(
                    "CServerDriver_PSMoveService::HandleControllerListReponse - Received {0} controllers", 
                    controller_list.count));

            // See if this was the controller list request we were waiting for
            if (request_id == _pendingControllerRequestId)
            {
                _pendingControllerRequestId = -1;
            }

            // Notify any listeners that the controller list is about to change changed
            if (ControllerConfigListPreUpdateEvent != null) {
                ControllerConfigListPreUpdateEvent();
            }

            // Rebuild the controller list and assigning to the config manager
            List<ControllerConfig> controllerConfigList = new List<ControllerConfig>();
            {
                // Update the controller info list
                _controllerInfoList = ControllerInfo.MakeArray(controller_list.controllers);

                for (int list_index = 0; list_index < _controllerInfoList.Length; ++list_index) {
                    ControllerInfo controller_info = _controllerInfoList[list_index];
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

            // Finalize the PSMService connection if this was the last thing we were waiting for
            if (_connectionState == PSMConnectionState.waitingForConnectionResponse)
            {
                TryFinalizePSMConnection();
            }
        }

        void HandleTrackerListReponse(int request_id, PSMTrackerList tracker_list, IntPtr response_handle)
        {
            Trace.TraceInformation(
                string.Format(
                    "CServerDriver_PSMoveService::HandleTrackerListReponse - Received {0} trackers",
                    tracker_list.count));

            // See if this was the controller list request we were waiting for
            if (request_id == _pendingTrackerRequestId)
            {
                _pendingTrackerRequestId = -1;
            }

            // Update the tracker info list
            _trackerInfoList = TrackerInfo.MakeArray(tracker_list.trackers);

            // Notify any listeners that the tracker list changed
            if (TrackerListUpdatedEvent != null) {
                TrackerListUpdatedEvent();
            }

            if (_connectionState == PSMConnectionState.waitingForConnectionResponse)
            {
                TryFinalizePSMConnection();
            }
        }

        void HandleHmdListReponse(int request_id, PSMHmdList hmd_list, IntPtr response_handle)
        {
            Trace.TraceInformation(
                string.Format(
                    "CServerDriver_PSMoveService::HandleHmdListReponse - Received {0} HMDs",
                    hmd_list.count));

            // See if this was the controller list request we were waiting for
            if (request_id == _pendingHmdRequestId)
            {
                _pendingHmdRequestId = -1;
            }

            // Update the HMD info list
            _hmdInfoList = HMDInfo.MakeArray(hmd_list.hmds);

            // Notify any listeners that the tracker list changed
            if (HmdListUpdatedEvent != null) {
                HmdListUpdatedEvent();
            }

            if (_connectionState == PSMConnectionState.waitingForConnectionResponse)
            {
                TryFinalizePSMConnection();
            }
        }

        void AllocateUniquePSMoveController(string psmControllerSerial)
        {
            PSMoveControllerConfig config = new PSMoveControllerConfig(psmControllerSerial);
        }

        public bool IsConnected
        {
            get { return _connectionState == PSMConnectionState.connected; }
        }

        private bool TryConnectToPSMoveService()
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

        void TryFinalizePSMConnection()
        {
            if (_connectionState == PSMConnectionState.waitingForConnectionResponse &&
                _pendingControllerRequestId == -1 &&
                _pendingHmdRequestId == -1 &&
                _pendingTrackerRequestId == -1)
            {
                // Connection is now ready
                _connectionState = PSMConnectionState.connected;

                // Fire off connection delegate
                ConnectedToPSMServiceEvent();
            }
        }

        private void DisconnectFromPSMoveService()
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
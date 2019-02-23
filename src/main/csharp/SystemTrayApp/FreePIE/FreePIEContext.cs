using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using PSMoveService;

namespace SystemTrayApp
{
    public class FreePIEContext
    {
        private static readonly Lazy<FreePIEContext> _lazyInstance = new Lazy<FreePIEContext>(() => new FreePIEContext());
        public static FreePIEContext Instance { get { return _lazyInstance.Value; } }

        private class FreePIESlotState
        {
            private FreePIEApi.FreepieData _slotData;
            public FreePIEApi.FreepieData SlotData
            {
                get { return _slotData; }
            }

            public FreePIESlotState()
            {
                _slotData = new FreePIEApi.FreepieData();
            }

            public void UpdateSlotData(
                FreePIESlotDefinition slotDefinition, 
                PSMControllerPool psmControllerPool,
                PSMHmdPool psmHmdPool)
            {
                if (slotDefinition is FreePIEControllerSlotDefinition)
                {
                    FreePIEControllerSlotDefinition controllerSlotDef = (FreePIEControllerSlotDefinition)slotDefinition;

                    _slotData.x = FetchControllerSlotData(controllerSlotDef.xProperty, psmControllerPool);
                    _slotData.y = FetchControllerSlotData(controllerSlotDef.yProperty, psmControllerPool);
                    _slotData.z = FetchControllerSlotData(controllerSlotDef.zProperty, psmControllerPool);
                    _slotData.pitch = FetchControllerSlotData(controllerSlotDef.pitchProperty, psmControllerPool);
                    _slotData.roll = FetchControllerSlotData(controllerSlotDef.rollProperty, psmControllerPool);
                    _slotData.yaw = FetchControllerSlotData(controllerSlotDef.yawProperty, psmControllerPool);
                }
                else if (slotDefinition is FreePIEHmdSlotDefinition) 
                {
                    FreePIEHmdSlotDefinition hmdSlotDef = (FreePIEHmdSlotDefinition)slotDefinition;

                    _slotData.x = FetchHmdSlotData(hmdSlotDef.xProperty, psmHmdPool);
                    _slotData.y = FetchHmdSlotData(hmdSlotDef.yProperty, psmHmdPool);
                    _slotData.z = FetchHmdSlotData(hmdSlotDef.zProperty, psmHmdPool);
                    _slotData.pitch = FetchHmdSlotData(hmdSlotDef.pitchProperty, psmHmdPool);
                    _slotData.roll = FetchHmdSlotData(hmdSlotDef.rollProperty, psmHmdPool);
                    _slotData.yaw = FetchHmdSlotData(hmdSlotDef.yawProperty, psmHmdPool);
                }
            }

            private float FetchControllerSlotData(FreePIEControllerProperty property, PSMControllerPool psmControllerPool)
            {
                switch(property.controllerPropertySource) {
                    case eControllerPropertySource.POSITION_X:
                        return psmControllerPool.GetControllerPosition(property.controllerSource).x;
                    case eControllerPropertySource.POSITION_Y:
                        return psmControllerPool.GetControllerPosition(property.controllerSource).y;
                    case eControllerPropertySource.POSITION_Z:
                        return psmControllerPool.GetControllerPosition(property.controllerSource).z;
                    case eControllerPropertySource.ORIENTATION_ROLL:
                        return MathUtility.ExtractRoll(psmControllerPool.GetControllerOrientation(property.controllerSource));
                    case eControllerPropertySource.ORIENTATION_PITCH:
                        return MathUtility.ExtractPitch(psmControllerPool.GetControllerOrientation(property.controllerSource));
                    case eControllerPropertySource.ORIENTATION_YAW:
                        return MathUtility.ExtractYaw(psmControllerPool.GetControllerOrientation(property.controllerSource));
                    case eControllerPropertySource.ACCELEROMETER_X:
                        return psmControllerPool.GetControllerAccelerometer(property.controllerSource).x;
                    case eControllerPropertySource.ACCELEROMETER_Y:
                        return psmControllerPool.GetControllerAccelerometer(property.controllerSource).y;
                    case eControllerPropertySource.ACCELEROMETER_Z:
                        return psmControllerPool.GetControllerAccelerometer(property.controllerSource).z;
                    case eControllerPropertySource.GYROSCOPE_X:
                        return psmControllerPool.GetControllerGyroscope(property.controllerSource).x;
                    case eControllerPropertySource.GYROSCOPE_Y:
                        return psmControllerPool.GetControllerGyroscope(property.controllerSource).y;
                    case eControllerPropertySource.GYROSCOPE_Z:
                        return psmControllerPool.GetControllerGyroscope(property.controllerSource).z;
                    case eControllerPropertySource.MAGNETOMETER_X:
                        return psmControllerPool.GetControllerMagnetometer(property.controllerSource).x;
                    case eControllerPropertySource.MAGNETOMETER_Y:
                        return psmControllerPool.GetControllerMagnetometer(property.controllerSource).y;
                    case eControllerPropertySource.MAGNETOMETER_Z:
                        return psmControllerPool.GetControllerMagnetometer(property.controllerSource).z;
                    case eControllerPropertySource.BUTTONS:
                        return psmControllerPool.GetControllerButtonBitmaskAsFloat(property.controllerSource);
                    case eControllerPropertySource.TRIGGER:
                        return psmControllerPool.GetTriggerValue(property.controllerSource);
                    default:
                        break;

                }
                return 0;
            }

            private float FetchHmdSlotData(FreePIEHmdProperty property, PSMHmdPool psmHmdPool)
            {
                switch (property.hmdPropertySource) {
                    case eHmdPropertySource.POSITION_X:
                        return psmHmdPool.GetHmdPosition(property.hmdSource).x;
                    case eHmdPropertySource.POSITION_Y:
                        return psmHmdPool.GetHmdPosition(property.hmdSource).y;
                    case eHmdPropertySource.POSITION_Z:
                        return psmHmdPool.GetHmdPosition(property.hmdSource).z;
                    case eHmdPropertySource.ORIENTATION_ROLL:
                        return MathUtility.ExtractRoll(psmHmdPool.GetHmdOrientation(property.hmdSource));
                    case eHmdPropertySource.ORIENTATION_PITCH:
                        return MathUtility.ExtractPitch(psmHmdPool.GetHmdOrientation(property.hmdSource));
                    case eHmdPropertySource.ORIENTATION_YAW:
                        return MathUtility.ExtractYaw(psmHmdPool.GetHmdOrientation(property.hmdSource));
                    case eHmdPropertySource.ACCELEROMETER_X:
                        return psmHmdPool.GetHmdAccelerometer(property.hmdSource).x;
                    case eHmdPropertySource.ACCELEROMETER_Y:
                        return psmHmdPool.GetHmdAccelerometer(property.hmdSource).y;
                    case eHmdPropertySource.ACCELEROMETER_Z:
                        return psmHmdPool.GetHmdAccelerometer(property.hmdSource).z;
                    case eHmdPropertySource.GYROSCOPE_X:
                        return psmHmdPool.GetHmdGyroscope(property.hmdSource).x;
                    case eHmdPropertySource.GYROSCOPE_Y:
                        return psmHmdPool.GetHmdGyroscope(property.hmdSource).y;
                    case eHmdPropertySource.GYROSCOPE_Z:
                        return psmHmdPool.GetHmdGyroscope(property.hmdSource).z;
                    default:
                        break;

                }
                return 0;
            }
        }

        private FreePIESlotDefinition[] _freePIESlotDefinitions;
        public FreePIESlotDefinition[] FreePIESlotDefinitions
        {
            get { return _freePIESlotDefinitions; }
        }

        private FreePIESlotState[] _freePIESlotStates;
        private FreePIEApi.FreepieData[] _freePIEOutput;
        private PSMControllerPool _psmControllerPool;
        private PSMHmdPool _psmHmdPool;

        private bool _bIsInitialized;
        public bool IsInitialized
        {
            get { return _bIsInitialized; }
        }

        public enum FreePIEConnectionState
        {
            disconnected,
            waitingForPSMoveService,
            connected,
            failed
        }

        private FreePIEConnectionState _connectionState;
        public FreePIEConnectionState ConnectionState
        {
            get { return _connectionState; }
        }

        public bool IsConnected
        {
            get { return _connectionState == FreePIEConnectionState.connected; }
        }

        private string _freePieRuntimePath;
        public string FreePieRuntimePath
        {
            get { return _freePieRuntimePath; }
        }

        private int _freePIEMaxSlotCount;
        public int FreePIEMaxSlotCount
        {
            get { return _freePIEMaxSlotCount; }
        }

        public delegate void ConnectedToFreePIE();
        public event ConnectedToFreePIE ConnectedToFreePIEEvent;

        public delegate void FreePIEConnectionFailure(string Reason);
        public event FreePIEConnectionFailure FreePIEConnectionFailureEvent;

        public delegate void DisconnectedFromFreePIE();
        public event DisconnectedFromFreePIE DisconnectedFromFreePIEEvent;

        private FreePIEContext()
        {
            _freePIESlotDefinitions = new FreePIESlotDefinition[0];
            _freePIESlotStates = new FreePIESlotState[0];
            _freePIEOutput = new FreePIEApi.FreepieData[0];
            _psmControllerPool = new PSMControllerPool();
            _psmHmdPool = new PSMHmdPool();
            _connectionState = FreePIEConnectionState.disconnected;
            _bIsInitialized = false;
            _freePIEMaxSlotCount = 4;
        }

        public bool Init()
        {
            if (!_bIsInitialized)
            {
                _freePieRuntimePath = GetFreePieDLLPath();

                if (_freePieRuntimePath.Length > 0)
                {
                    // Start listening to PSMoveService device events
                    PSMoveServiceContext.Instance.ConnectedToPSMServiceEvent += OnConnectedToPSMServiceEvent;
                    PSMoveServiceContext.Instance.DisconnectedFromPSMServiceEvent += OnDisconnectedFromPSMServiceEvent;
                    PSMoveServiceContext.Instance.ControllerListUpdatedEvent += OnControllerListUpdatedEvent;
                    PSMoveServiceContext.Instance.HmdListUpdatedEvent += OnHmdListUpdatedEvent;
                    PSMoveServiceContext.Instance.PSMMessagesPolledEvent += OnPSMMessagesPolledEvent;

                    // Add the FreePIE DLL directory to the DLL search list
                    SetDllDirectory(_freePieRuntimePath);

                    // Get the max allowed slot count in free pie
                    try {
                        _freePIEMaxSlotCount = FreePIEApi.freepie_io_6dof_slots();
                        _bIsInitialized = true;
                    }
                    catch(Exception e)
                    {
                        Trace.TraceWarning(string.Format("Failed to access FreePIE DLL: {0}", e.Message));
                    }
                }
            }

            return _bIsInitialized;
        }

        public void Cleanup()
        {
            if (_bIsInitialized)
            {
                DisconnectFromFreePIE();

                // Stop listening to PSMoveService device events
                PSMoveServiceContext.Instance.ConnectedToPSMServiceEvent -= OnConnectedToPSMServiceEvent;
                PSMoveServiceContext.Instance.DisconnectedFromPSMServiceEvent -= OnDisconnectedFromPSMServiceEvent;
                PSMoveServiceContext.Instance.ControllerListUpdatedEvent -= OnControllerListUpdatedEvent;
                PSMoveServiceContext.Instance.HmdListUpdatedEvent -= OnHmdListUpdatedEvent;
                PSMoveServiceContext.Instance.PSMMessagesPolledEvent -= OnPSMMessagesPolledEvent;

                _bIsInitialized = false;
            }
        }

        public void ConnectToFreePIE(FreePIESlotDefinition[] newSlots)
        {
            if (_connectionState == FreePIEConnectionState.disconnected ||
                _connectionState == FreePIEConnectionState.failed)
            {
                InitSlotStates(newSlots);

                if (PSMoveServiceContext.Instance.IsConnected)
                {
                    OnConnectedToFreePIE();
                }
                else 
                {
                    if (PSMoveServiceContext.Instance.LaunchPSMoveServiceProcess())
                    {
                        _connectionState = FreePIEConnectionState.waitingForPSMoveService;
                    }
                    else 
                    {
                        _connectionState = FreePIEConnectionState.failed;
                        if (FreePIEConnectionFailureEvent != null)
                        {
                            FreePIEConnectionFailureEvent("Can't launch PSMoveService");
                        }
                    }
                }
            }
        }

        public void DisconnectFromFreePIE()
        {
            if (_connectionState == FreePIEConnectionState.connected)
            {
                _psmControllerPool.Cleanup();
                _psmHmdPool.Cleanup();
                CleanupSlotStates();

                _connectionState = FreePIEConnectionState.disconnected;
                if (DisconnectedFromFreePIEEvent != null) {
                    DisconnectedFromFreePIEEvent();
                }
            }
        }

        private void OnConnectedToFreePIE()
        {
            _psmControllerPool.Init();
            _psmHmdPool.Init();

            _connectionState = FreePIEConnectionState.connected;
            if (ConnectedToFreePIEEvent != null)
            {
                ConnectedToFreePIEEvent();
            }
        }

        private void OnConnectedToPSMServiceEvent()
        {
            if (_connectionState == FreePIEConnectionState.waitingForPSMoveService)
            {
                OnConnectedToFreePIE();
            }
        }

        private void OnDisconnectedFromPSMServiceEvent()
        {
            if (_connectionState == FreePIEConnectionState.connected)
            {
                DisconnectFromFreePIE();
            }
        }

        private void OnHmdListUpdatedEvent()
        {
            if (_connectionState == FreePIEConnectionState.connected)
            {
                _psmHmdPool.RefreshHmdList();
                UpdateSlotStates();
                PublishSlotStates();
            }
        }

        private void OnControllerListUpdatedEvent()
        {
            if (_connectionState == FreePIEConnectionState.connected)
            {
                _psmControllerPool.RefreshControllerList();
                UpdateSlotStates();
                PublishSlotStates();
            }
        }

        private void OnPSMMessagesPolledEvent()
        {
            if (_connectionState == FreePIEConnectionState.connected)
            {
                UpdateSlotStates();
                PublishSlotStates();
            }
        }

        private void InitSlotStates(FreePIESlotDefinition[] newSlots)
        {
            if (newSlots.Length <= _freePIEMaxSlotCount)
            {
                _freePIESlotDefinitions = newSlots;
            }
            else 
            {
                _freePIESlotDefinitions = newSlots.Take(_freePIEMaxSlotCount).ToArray();
            }

            if (_freePIESlotStates.Length != _freePIESlotDefinitions.Length)
            {
                _freePIESlotStates = new FreePIESlotState[_freePIESlotDefinitions.Length];
                _freePIEOutput = new FreePIEApi.FreepieData[_freePIESlotDefinitions.Length];

                for (int slotIndex= 0; slotIndex < _freePIESlotDefinitions.Length; ++slotIndex)
                {
                    _freePIESlotStates[slotIndex] = new FreePIESlotState();
                }
            }
        }

        private void UpdateSlotStates()
        {
            for (int slotIndex = 0; slotIndex < _freePIESlotStates.Length; ++slotIndex)
            {
                FreePIESlotDefinition slotDefinition = _freePIESlotDefinitions[slotIndex];
                FreePIESlotState slotState = _freePIESlotStates[slotIndex];

                slotState.UpdateSlotData(slotDefinition, _psmControllerPool, _psmHmdPool);
            }
        }

        private void PublishSlotStates()
        {
            // Copy the slot data to the final output array
            for (int slotIndex = 0; slotIndex < _freePIESlotDefinitions.Length; ++slotIndex)
            {
                _freePIEOutput[slotIndex] = _freePIESlotStates[slotIndex].SlotData;
            }

            int result = FreePIEApi.freepie_io_6dof_write(0, _freePIEOutput.Length, _freePIEOutput);
            if (result == FreePIEApi.FREEPIE_IO_ERROR_OUT_OF_BOUNDS) {
                Trace.TraceError("FreePIEContext - Could not write slots to freepie: OUT OF BOUNDS");
            }
            else if (result == FreePIEApi.FREEPIE_IO_ERROR_SHARED_DATA) {
                Trace.TraceError("FreePIEContext - Could not write slots to freepie: SHARED DATA ERROR");
            }
        }

        private void CleanupSlotStates()
        {
            _freePIESlotStates = new FreePIESlotState[0];
            _freePIEOutput = new FreePIEApi.FreepieData[0];
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);

        public static string GetFreePieDLLPath()
        {
            string dll_path = "";

            if (!Environment.Is64BitProcess && //TODO: 64-bit dll for FreePIE not included in FreePIE install at the moment
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dll_path = 
                    Microsoft.Win32.Registry.GetValue(
                        string.Format("{0}\\Software\\{1}", Microsoft.Win32.Registry.CurrentUser, "FreePIE"),
                        "path", 
                        null) as string;
            }

            if (dll_path.Length == 0) {
                dll_path = SystemTrayApp.Program.GetExecutingDirectoryName();
            }

            return dll_path;
        }
    }
}

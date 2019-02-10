using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSMoveService;

namespace SystemTrayApp
{
    public enum eControllerSource
    {
        NONE,
        CONTROLLER_0,
        CONTROLLER_1,
        CONTROLLER_2,
        CONTROLLER_3,
        CONTROLLER_4,

        COUNT
    }

    public enum eControllerPropertySource
    {
        NONE,
        POSITION_X,
        POSITION_Y,
        POSITION_Z,
        ORIENTATION_ROLL,
        ORIENTATION_PITCH,
        ORIENTATION_YAW,
        ACCELEROMETER_X,
        ACCELEROMETER_Y,
        ACCELEROMETER_Z,
        GYROSCOPE_X,
        GYROSCOPE_Y,
        GYROSCOPE_Z,
        MAGNETOMETER_X,
        MAGNETOMETER_Y,
        MAGNETOMETER_Z,
        BUTTONS,
        TRIGGER,

        COUNT
    }

    public class PSMControllerPool
    {
        public static string[] ControllerSourceNames = new string[(int)eControllerSource.COUNT] {
            "None",
            "Controller 0",
            "Controller 1",
            "Controller 2",
            "Controller 3",
            "Controller 4",
        };

        public static Dictionary<string, eControllerSource> MakeControllerSourceDictionary()
        {
            Dictionary<string, eControllerSource> dictionary = new Dictionary<string, eControllerSource>();

            for (int source_index = 0; source_index < (int)eControllerSource.COUNT; ++source_index)
            {
                dictionary.Add(ControllerSourceNames[source_index], (eControllerSource)source_index);
            }

            return dictionary;
        }

        public static string[] ControllerPropertySourceNames = new string[(int)eControllerPropertySource.COUNT] {
            "None",
            "Position X",
            "Position Y",
            "Position Z",
            "Orientation X",
            "Orientation Y",
            "Orientation Z",
            "Accelerometer X",
            "Accelerometer Y",
            "Accelerometer Z",
            "Gyroscope X",
            "Gyroscope Y",
            "Gyroscope Z",
            "Magnetometer X",
            "Magnetometer Y",
            "Magnetometer Z",
            "Buttons",
            "Trigger",
        };

        public static Dictionary<string, eControllerPropertySource> MakeControllerPropertyDictionary()
        {
            Dictionary<string, eControllerPropertySource> dictionary = new Dictionary<string, eControllerPropertySource>();

            for (int source_index = 0; source_index < (int)eControllerPropertySource.COUNT; ++source_index)
            {
                dictionary.Add(ControllerPropertySourceNames[source_index], (eControllerPropertySource)source_index);
            }

            return dictionary;
        }

        struct PSMControllerState
        {
            public ControllerInfo ControllerInfo;
            public PSMController Controller;
            public bool IsStreaming;
        }

        private static int _initializedControllerPoolCount = 0;
        public static int InitializedControllerPoolCount
        {
            get { return _initializedControllerPoolCount; }
        }

        bool _isInitialized;
        PSMControllerState[] _controllers;

        public PSMControllerPool()
        {
            _isInitialized = false;
            _controllers = new PSMControllerState[PSMoveClient.PSMOVESERVICE_MAX_CONTROLLER_COUNT];
        }

        public void Init()
        {
            if (_isInitialized)
                return;

            for (int controllerID = 0; controllerID < _controllers.Length; ++controllerID)
            {
                if (PSMoveClient.PSM_AllocateControllerListener(controllerID) == PSMResult.PSMResult_Success)
                {
                    _controllers[controllerID].Controller = PSMoveClient.PSM_GetController(controllerID);
                }
            }

            _isInitialized = true;
            _initializedControllerPoolCount++;

            // Fetch the most recent list of devices posted
            RefreshControllerList();
        }

        public void Cleanup()
        {
            if (!_isInitialized)
                return;

            for (int controllerID = 0; controllerID < _controllers.Length; ++controllerID)
            {
                if (_controllers[controllerID].IsStreaming)
                {
                    int request_id = -1;
                    PSMoveClient.PSM_StopControllerDataStreamAsync(controllerID, out request_id);
                    PSMoveClient.PSM_EatResponse(request_id);

                    _controllers[controllerID].IsStreaming = false;
                }

                PSMoveClient.PSM_FreeControllerListener(controllerID);
                _controllers[controllerID].Controller = null;
            }

            _isInitialized = false;
            _initializedControllerPoolCount--;
        }

        public void RefreshControllerList()
        {
            if (!_isInitialized)
                return;

            ControllerInfo[] ControllerInfoList = PSMoveServiceContext.Instance.ControllerInfoList;

            // Strip off old controller info from every controller state entry
            for (int controllerID = 0; controllerID < _controllers.Length; ++controllerID)
            {
                _controllers[controllerID].ControllerInfo = null;
            }

            // Update the controller state list with the new controller list
            foreach (ControllerInfo controllerInfo in ControllerInfoList)
            {
                int controllerId = controllerInfo.controller_id;

                // Assign the latest controller info to the controller state
                _controllers[controllerId].ControllerInfo = controllerInfo;

                // Start streaming controller data if we aren't already
                if (!_controllers[controllerId].IsStreaming)
                {
                    uint data_stream_flags =
                        (uint)PSMControllerDataStreamFlags.PSMStreamFlags_includePositionData |
                        (uint)PSMControllerDataStreamFlags.PSMStreamFlags_includeCalibratedSensorData;

                    int request_id = -1;
                    PSMoveClient.PSM_StartControllerDataStreamAsync(controllerInfo.controller_id, data_stream_flags, out request_id);
                    PSMoveClient.PSM_EatResponse(request_id);

                    _controllers[controllerId].IsStreaming = true;
                }
            }

            // For any controller state entry that didn't get update
            // make sure to turn off streaming if it was streaming previously
            for (int controllerID = 0; controllerID < _controllers.Length; ++controllerID)
            {
                if (_controllers[controllerID].ControllerInfo == null && _controllers[controllerID].IsStreaming)
                {
                    int request_id = -1;
                    PSMoveClient.PSM_StopControllerDataStreamAsync(controllerID, out request_id);
                    PSMoveClient.PSM_EatResponse(request_id);

                    _controllers[controllerID].IsStreaming = false;
                }
            }
        }

        public bool GetController(eControllerSource source, out PSMController controller)
        {
            int controllerIndex = (int)source - 1;
            if (controllerIndex >= 0 && controllerIndex < _controllers.Length && _controllers[controllerIndex].Controller != null)
            {
                controller = _controllers[controllerIndex].Controller;
                return true;
            }
            else
            {
                controller = null;
                return false;
            }
        }

        public PSMVector3f GetControllerPosition(eControllerSource source)
        {
            PSMController controller = null;
            if (GetController(source, out controller))
            {
                switch (controller.ControllerType)
                {
                    case PSMControllerType.PSMController_Move:
                        return controller.ControllerState.PSMoveState.Pose.Position;
                    case PSMControllerType.PSMController_Navi:
                        return new PSMVector3f();
                    case PSMControllerType.PSMController_DualShock4:
                        return controller.ControllerState.PSDS4State.Pose.Position;
                    case PSMControllerType.PSMController_Virtual:
                        return controller.ControllerState.VirtualController.Pose.Position;
                }
            }
            return PSMoveClient.k_psm_float_vector3_zero;
        }

        public PSMQuatf GetControllerOrientation(eControllerSource source)
        {
            PSMController controller = null;
            if (GetController(source, out controller))
            {
                switch (controller.ControllerType)
                {
                    case PSMControllerType.PSMController_Move:
                        return controller.ControllerState.PSMoveState.Pose.Orientation;
                    case PSMControllerType.PSMController_Navi:
                        return PSMoveClient.k_psm_quaternion_identity;
                    case PSMControllerType.PSMController_DualShock4:
                        return controller.ControllerState.PSDS4State.Pose.Orientation;
                    case PSMControllerType.PSMController_Virtual:
                        return controller.ControllerState.VirtualController.Pose.Orientation;
                }
            }
            return PSMoveClient.k_psm_quaternion_identity;
        }

        public PSMVector3f GetControllerAccelerometer(eControllerSource source)
        {
            PSMController controller = null;
            if (GetController(source, out controller))
            {
                switch (controller.ControllerType)
                {
                    case PSMControllerType.PSMController_Move:
                        return controller.ControllerState.PSMoveState.CalibratedSensorData.Accelerometer;
                    case PSMControllerType.PSMController_Navi:
                        return PSMoveClient.k_psm_float_vector3_zero;
                    case PSMControllerType.PSMController_DualShock4:
                        return controller.ControllerState.PSDS4State.CalibratedSensorData.Accelerometer;
                    case PSMControllerType.PSMController_Virtual:
                        return PSMoveClient.k_psm_float_vector3_zero;
                }
            }
            return PSMoveClient.k_psm_float_vector3_zero;
        }

        public PSMVector3f GetControllerGyroscope(eControllerSource source)
        {
            PSMController controller = null;
            if (GetController(source, out controller))
            {
                switch (controller.ControllerType)
                {
                    case PSMControllerType.PSMController_Move:
                        return controller.ControllerState.PSMoveState.CalibratedSensorData.Gyroscope;
                    case PSMControllerType.PSMController_Navi:
                        return PSMoveClient.k_psm_float_vector3_zero;
                    case PSMControllerType.PSMController_DualShock4:
                        return controller.ControllerState.PSDS4State.CalibratedSensorData.Gyroscope;
                    case PSMControllerType.PSMController_Virtual:
                        return PSMoveClient.k_psm_float_vector3_zero;
                }
            }
            return PSMoveClient.k_psm_float_vector3_zero;
        }

        public PSMVector3f GetControllerMagnetometer(eControllerSource source)
        {
            PSMController controller = null;
            if (GetController(source, out controller))
            {
                if (controller.ControllerType == PSMControllerType.PSMController_Move)
                {
                    return controller.ControllerState.PSMoveState.CalibratedSensorData.Magnetometer;
                }
            }
            return PSMoveClient.k_psm_float_vector3_zero;
        }

        private uint GetButtonBitmask(PSMButtonState state, int bit_index)
        {
            return (state == PSMButtonState.PSMButtonState_DOWN || state == PSMButtonState.PSMButtonState_PRESSED) ? (1u << bit_index) : 0;
        }

        public float GetControllerButtonBitmaskAsFloat(eControllerSource source)
        {
            PSMController controller = null;
            if (GetController(source, out controller))
            {
                switch (controller.ControllerType)
                {
                    case PSMControllerType.PSMController_Move:
                        {
                            PSMPSMove moveView = controller.ControllerState.PSMoveState;
                            uint buttonsPressed = 0;

                            buttonsPressed |= GetButtonBitmask(moveView.SquareButton, 0);
                            buttonsPressed |= GetButtonBitmask(moveView.TriangleButton, 1);
                            buttonsPressed |= GetButtonBitmask(moveView.CrossButton, 2);
                            buttonsPressed |= GetButtonBitmask(moveView.CircleButton, 3);
                            buttonsPressed |= GetButtonBitmask(moveView.MoveButton, 4);
                            buttonsPressed |= GetButtonBitmask(moveView.PSButton, 5);
                            buttonsPressed |= GetButtonBitmask(moveView.StartButton, 6);
                            buttonsPressed |= GetButtonBitmask(moveView.SelectButton, 7);

                            return (float)buttonsPressed;
                        }
                    case PSMControllerType.PSMController_Navi:
                        {
                            PSMPSNavi naviView = controller.ControllerState.PSNaviState;
                            uint buttonsPressed = 0;

                            buttonsPressed |= GetButtonBitmask(naviView.CrossButton, 2);
                            buttonsPressed |= GetButtonBitmask(naviView.CircleButton, 3);
                            buttonsPressed |= GetButtonBitmask(naviView.PSButton, 5);
                            buttonsPressed |= GetButtonBitmask(naviView.DPadDownButton, 8);
                            buttonsPressed |= GetButtonBitmask(naviView.DPadUpButton, 9);
                            buttonsPressed |= GetButtonBitmask(naviView.DPadLeftButton, 10);
                            buttonsPressed |= GetButtonBitmask(naviView.DPadRightButton, 11);
                            buttonsPressed |= GetButtonBitmask(naviView.L2Button, 12);
                            buttonsPressed |= GetButtonBitmask(naviView.L3Button, 14);

                            return (float)buttonsPressed;
                        }
                    case PSMControllerType.PSMController_DualShock4:
                        {
                            PSMDualShock4 ds4View = controller.ControllerState.PSDS4State;
                            uint buttonsPressed = 0;

                            buttonsPressed |= GetButtonBitmask(ds4View.SquareButton, 0);
                            buttonsPressed |= GetButtonBitmask(ds4View.TriangleButton, 1);
                            buttonsPressed |= GetButtonBitmask(ds4View.CrossButton, 2);
                            buttonsPressed |= GetButtonBitmask(ds4View.CircleButton, 3);
                            buttonsPressed |= GetButtonBitmask(ds4View.TrackPadButton, 4);
                            buttonsPressed |= GetButtonBitmask(ds4View.PSButton, 5);
                            buttonsPressed |= GetButtonBitmask(ds4View.OptionsButton, 6);
                            buttonsPressed |= GetButtonBitmask(ds4View.ShareButton, 7);
                            buttonsPressed |= GetButtonBitmask(ds4View.DPadDownButton, 8);
                            buttonsPressed |= GetButtonBitmask(ds4View.DPadUpButton, 9);
                            buttonsPressed |= GetButtonBitmask(ds4View.DPadLeftButton, 10);
                            buttonsPressed |= GetButtonBitmask(ds4View.DPadRightButton, 11);
                            buttonsPressed |= GetButtonBitmask(ds4View.L2Button, 12);
                            buttonsPressed |= GetButtonBitmask(ds4View.R2Button, 13);
                            buttonsPressed |= GetButtonBitmask(ds4View.L3Button, 14);
                            buttonsPressed |= GetButtonBitmask(ds4View.R3Button, 15);

                            return (float)buttonsPressed;
                        }
                    case PSMControllerType.PSMController_Virtual:
                        {
                            PSMVirtualController controllerView = controller.ControllerState.VirtualController;
                            uint buttonsPressed = 0;

                            int buttonCount = (controllerView.numButtons < 16) ? controllerView.numButtons : 16;
                            for (int buttonIndex = 0; buttonIndex < buttonCount; ++buttonIndex)
                            {
                                buttonsPressed |= GetButtonBitmask(controllerView.buttonStates[buttonIndex], 7);
                            }

                            return (float)buttonsPressed;
                        }
                }
            }
            return 0;
        }

        public float GetTriggerValue(eControllerSource source)
        {
            PSMController controller = null;
            if (GetController(source, out controller))
            {
                switch (controller.ControllerType)
                {
                    case PSMControllerType.PSMController_Move:
                        return (float)controller.ControllerState.PSMoveState.TriggerValue / 255.0f;
                    case PSMControllerType.PSMController_Navi:
                        return (float)controller.ControllerState.PSNaviState.TriggerValue / 255.0f;
                    case PSMControllerType.PSMController_DualShock4:
                        return Math.Max(controller.ControllerState.PSDS4State.LeftTriggerValue, controller.ControllerState.PSDS4State.RightTriggerValue);
                    case PSMControllerType.PSMController_Virtual:
                        {
                            PSMVirtualController controllerView = controller.ControllerState.VirtualController;
                            int axis_index = FreePIEConfig.Instance.VirtualControllerTriggerAxisIndex;

                            return
                                axis_index > 0 && axis_index < controllerView.numAxes
                                ? (float)controllerView.axisStates[axis_index] / 255.0f
                                : 0.0f;
                        }
                }
            }
            return 0.0f;
        }
    }
}

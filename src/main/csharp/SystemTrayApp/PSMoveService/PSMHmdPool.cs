using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSMoveService;

namespace SystemTrayApp
{
    public enum eHmdSource
    {
        NONE,
        HMD_0,
        HMD_1,
        HMD_2,
        HMD_3,

        COUNT
    }

    public enum eHmdPropertySource
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

        COUNT
    }

    public class PSMHmdPool
    {
        public static string[] HmdSourceNames = new string[(int)eHmdSource.COUNT] {
            "None",
            "HMD 0",
            "HMD 1",
            "HMD 2",
            "HMD 3"
        };

        public static Dictionary<string, eHmdSource> MakeHmdSourceDictionary()
        {
            Dictionary<string, eHmdSource> dictionary = new Dictionary<string, eHmdSource>();

            for (int source_index = 0; source_index < (int)eHmdSource.COUNT; ++source_index)
            {
                dictionary.Add(HmdSourceNames[source_index], (eHmdSource)source_index);
            }

            return dictionary;
        }

        public static string[] HmdPropertySourceNames = new string[(int)eHmdPropertySource.COUNT] {
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
        };

        public static Dictionary<string, eHmdPropertySource> MakeHmdPropertyDictionary()
        {
            Dictionary<string, eHmdPropertySource> dictionary = new Dictionary<string, eHmdPropertySource>();

            for (int source_index = 0; source_index < (int)eHmdPropertySource.COUNT; ++source_index)
            {
                dictionary.Add(HmdPropertySourceNames[source_index], (eHmdPropertySource)source_index);
            }

            return dictionary;
        }

        struct PSMHmdState
        {
            public HMDInfo HmdInfo;
            public PSMHeadMountedDisplay Hmd;
            public bool IsStreaming;
        }

        private static int _initializedHmdPoolCount = 0;
        public static int InitializedHmdPoolCount
        {
            get { return _initializedHmdPoolCount; }
        }

        bool _isInitialized;
        PSMHmdState[] _hmds;

        public PSMHmdPool()
        {
            _isInitialized = false;
            _hmds = new PSMHmdState[PSMoveClient.PSMOVESERVICE_MAX_HMD_COUNT];
        }

        public void Init()
        {
            if (_isInitialized)
                return;

            for (int hmdID = 0; hmdID < _hmds.Length; ++hmdID)
            {
                if (PSMoveClient.PSM_AllocateHmdListener(hmdID) == PSMResult.PSMResult_Success)
                {
                    _hmds[hmdID].Hmd = PSMoveClient.PSM_GetHmd(hmdID);
                }
            }

            _isInitialized = true;
            _initializedHmdPoolCount++;

            // Fetch the most recent list of devices posted
            RefreshHmdList();
        }

        public void Cleanup()
        {
            if (!_isInitialized)
                return;

            for (int hmdID = 0; hmdID < _hmds.Length; ++hmdID)
            {
                if (_hmds[hmdID].IsStreaming)
                {
                    int request_id = -1;
                    PSMoveClient.PSM_StopHmdDataStreamAsync(hmdID, out request_id);
                    PSMoveClient.PSM_EatResponse(request_id);

                    _hmds[hmdID].IsStreaming = false;
                }

                PSMoveClient.PSM_FreeHmdListener(hmdID);
                _hmds[hmdID].Hmd = null;
            }

            _isInitialized = false;
            _initializedHmdPoolCount--;
        }

        public void RefreshHmdList()
        {
            if (!_isInitialized)
                return;

            HMDInfo[] HmdInfoList = PSMoveServiceContext.Instance.HmdInfoList;

            // Strip off old hmd info from every hmd state entry
            for (int hmdID = 0; hmdID < _hmds.Length; ++hmdID)
            {
                _hmds[hmdID].HmdInfo = null;
            }

            // Update the hmd state list with the new hmd list
            foreach (HMDInfo hmdInfo in HmdInfoList)
            {
                int hmdId = hmdInfo.hmd_id;

                // Assign the latest hmd info to the hmd state
                _hmds[hmdId].HmdInfo = hmdInfo;

                // Start streaming hmd data if we aren't already
                if (!_hmds[hmdId].IsStreaming)
                {
                    uint data_stream_flags =
                        (uint)PSMControllerDataStreamFlags.PSMStreamFlags_includePositionData |
                        (uint)PSMControllerDataStreamFlags.PSMStreamFlags_includeCalibratedSensorData;

                    int request_id = -1;
                    PSMoveClient.PSM_StartHmdDataStreamAsync(hmdInfo.hmd_id, data_stream_flags, out request_id);
                    PSMoveClient.PSM_EatResponse(request_id);

                    _hmds[hmdId].IsStreaming = true;
                }
            }

            // For any hmd state entry that didn't get update
            // make sure to turn off streaming if it was streaming previously
            for (int hmdID = 0; hmdID < _hmds.Length; ++hmdID)
            {
                if (_hmds[hmdID].HmdInfo == null && _hmds[hmdID].IsStreaming)
                {
                    int request_id = -1;
                    PSMoveClient.PSM_StopHmdDataStreamAsync(hmdID, out request_id);
                    PSMoveClient.PSM_EatResponse(request_id);

                    _hmds[hmdID].IsStreaming = false;
                }
            }
        }

        public bool GetHMD(eHmdSource source, out PSMHeadMountedDisplay hmd)
        {
            int hmdIndex = (int)source - 1;
            if (hmdIndex >= 0 && hmdIndex < _hmds.Length && _hmds[hmdIndex].Hmd != null)
            {
                hmd = _hmds[hmdIndex].Hmd;
                return true;
            }
            else
            {
                hmd = null;
                return false;
            }
        }

        public PSMVector3f GetHmdPosition(eHmdSource source)
        {
            PSMHeadMountedDisplay hmd = null;
            if (GetHMD(source, out hmd))
            {
                switch (hmd.HmdType)
                {
                    case PSMHmdType.PSMHmd_Morpheus:
                        return hmd.HmdState.MorpheusState.Pose.Position;
                    case PSMHmdType.PSMHmd_Virtual:
                        return hmd.HmdState.VirtualHMDState.Pose.Position;
                }
            }
            return PSMoveClient.k_psm_float_vector3_zero;
        }

        public PSMQuatf GetHmdOrientation(eHmdSource source)
        {
            PSMHeadMountedDisplay hmd = null;
            if (GetHMD(source, out hmd))
            {
                switch (hmd.HmdType)
                {
                    case PSMHmdType.PSMHmd_Morpheus:
                        return hmd.HmdState.MorpheusState.Pose.Orientation;
                    case PSMHmdType.PSMHmd_Virtual:
                        return hmd.HmdState.VirtualHMDState.Pose.Orientation;
                }
            }
            return PSMoveClient.k_psm_quaternion_identity;
        }

        public PSMVector3f GetHmdAccelerometer(eHmdSource source)
        {
            PSMHeadMountedDisplay hmd = null;
            if (GetHMD(source, out hmd))
            {
                switch (hmd.HmdType)
                {
                    case PSMHmdType.PSMHmd_Morpheus:
                        return hmd.HmdState.MorpheusState.CalibratedSensorData.Accelerometer;
                    case PSMHmdType.PSMHmd_Virtual:
                        return PSMoveClient.k_psm_float_vector3_zero;
                }
            }
            return PSMoveClient.k_psm_float_vector3_zero;
        }

        public PSMVector3f GetHmdGyroscope(eHmdSource source)
        {
            PSMHeadMountedDisplay hmd = null;
            if (GetHMD(source, out hmd))
            {
                switch (hmd.HmdType)
                {
                    case PSMHmdType.PSMHmd_Morpheus:
                        return hmd.HmdState.MorpheusState.CalibratedSensorData.Gyroscope;
                    case PSMHmdType.PSMHmd_Virtual:
                        return PSMoveClient.k_psm_float_vector3_zero;
                }
            }
            return PSMoveClient.k_psm_float_vector3_zero;
        }
    }
}

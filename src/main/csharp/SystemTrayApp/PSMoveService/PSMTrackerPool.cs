using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSMoveService;

namespace SystemTrayApp
{
    public enum eTrackerSource
    {
        NONE,

        TRACKER_0,
        TRACKER_1,
        TRACKER_2,
        TRACKER_3,
        TRACKER_4,
        TRACKER_5,
        TRACKER_6,
        TRACKER_7,

        COUNT
    }

    public class PSMTrackerPool
    {
        class PSMTrackerState
        {
            private TrackerInfo _trackerInfo;
            public TrackerInfo TrackerInfo
            {
                get { return _trackerInfo; }
            }

            private PSMTracker _tracker;
            public PSMTracker Tracker
            {
                get { return _tracker; }
            }

            private bool _isDataStreamActive;
            public bool IsDataStreamActive
            {
                get { return _isDataStreamActive; }
            }

            private bool _isVideoStreamActive;
            public bool IsVideoStreamActive
            {
                get { return _isVideoStreamActive; }
            }

            private int _pendingStartStreamRequest;
            public int PendingStartStreamRequest
            {
                get { return _pendingStartStreamRequest; }
            }

            public bool IsValid
            {
                get { return _trackerInfo != null && _tracker != null; }
            }

            public bool MarkedForCleanup;

            public PSMTrackerState()
            {
                _trackerInfo= null;
                _tracker= null;
                _isDataStreamActive= false;
                _pendingStartStreamRequest= -1;
                _isVideoStreamActive = false;
                MarkedForCleanup = false;
            }

            public bool Init(TrackerInfo trackerInfo)
            {
                if (_trackerInfo != null &&
                    (_trackerInfo.device_path != trackerInfo.device_path ||
                     _trackerInfo.shared_memory_name != trackerInfo.shared_memory_name ||
                     _trackerInfo.tracker_screen_dimensions.x != trackerInfo.tracker_screen_dimensions.x ||
                     _trackerInfo.tracker_screen_dimensions.y != trackerInfo.tracker_screen_dimensions.y))
                {
                    Cleanup();
                }

                _trackerInfo = trackerInfo;

                // Allocate a listener for the tracker
                if (_tracker == null)
                {
                    if (PSMoveClient.PSM_AllocateTrackerListener(TrackerInfo.tracker_id) == PSMResult.PSMResult_Success)
                    {
                        _tracker = PSMoveClient.PSM_GetTracker(TrackerInfo.tracker_id);
                    }
                }

                // Start streaming Tracker data if we aren't already
                if (_tracker != null && !_isDataStreamActive)
                {
                    int request_id = -1;
                    if (PSMoveClient.PSM_StartTrackerDataStreamAsync(TrackerInfo.tracker_id, out request_id) == PSMResult.PSMResult_RequestSent)
                    {
                        PSMoveClient.PSM_RegisterDelegate(request_id, this.OnTrackerStreamStarted);

                        _isDataStreamActive = true;
                    }
                }

                return IsDataStreamActive;
            }

            private void OnTrackerStreamStarted(PSMResponseMessage message)
            {
                if (message.result_code == PSMResult.PSMResult_Success)
                {
                    if (PSMoveClient.PSM_OpenTrackerVideoStream(_trackerInfo.tracker_id) != PSMResult.PSMResult_Error)
                    {
                        _isVideoStreamActive = true;
                    }
                }

                _pendingStartStreamRequest = -1;
            }

            public void Cleanup()
            {
                MarkedForCleanup = false;

                if (_trackerInfo != null)
                {
                    if (_pendingStartStreamRequest != -1)
                    {
                        PSMoveClient.PSM_CancelCallback(_pendingStartStreamRequest);
                    }

                    if (_isVideoStreamActive)
                    {
                        PSMoveClient.PSM_CloseTrackerVideoStream(_trackerInfo.tracker_id);
                    }

                    if (_isDataStreamActive)
                    {
                        int request_id = -1;
                        PSMoveClient.PSM_StopTrackerDataStreamAsync(_trackerInfo.tracker_id, out request_id);
                        PSMoveClient.PSM_EatResponse(request_id);

                    }

                    if (_tracker == null)
                    {
                        PSMoveClient.PSM_FreeTrackerListener(_trackerInfo.tracker_id);
                    }
                }

                _tracker = null;
                _trackerInfo = null;
                _pendingStartStreamRequest = -1;
                _isDataStreamActive = false;
                _isVideoStreamActive = false;
            }
        }

        private static int _initializedTrackerPoolCount = 0;
        public static int InitializedTrackerPoolCount
        {
            get { return _initializedTrackerPoolCount; }
        }

        bool _isInitialized;
        PSMTrackerState[] _trackers;

        public PSMTrackerPool()
        {
            _isInitialized = false;
            _trackers = new PSMTrackerState[PSMoveClient.PSMOVESERVICE_MAX_TRACKER_COUNT];
            for (int trackerId = 0; trackerId < _trackers.Length; ++trackerId)
            {
                _trackers[trackerId] = new PSMTrackerState();
            }
        }

        public void Init(TrackerInfo[] TrackerInfoList)
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            _initializedTrackerPoolCount++;

            // Fetch the most recent list of devices posted
            RefreshTrackerList(TrackerInfoList);
        }

        public void Cleanup()
        {
            if (!_isInitialized)
                return;

            for (int TrackerID = 0; TrackerID < _trackers.Length; ++TrackerID)
            {
                _trackers[TrackerID].Cleanup();
            }

            _isInitialized = false;
            _initializedTrackerPoolCount--;
        }

        public void RefreshTrackerList(TrackerInfo[] TrackerInfoList)
        {
            if (!_isInitialized)
                return;

            // Mark all trackers for clean-up initially
            for (int TrackerID = 0; TrackerID < _trackers.Length; ++TrackerID)
            {
                if (_trackers[TrackerID].IsValid)
                {
                    _trackers[TrackerID].MarkedForCleanup = true;
                }
            }

            // Update the Tracker state list with the new Tracker list
            foreach (TrackerInfo TrackerInfo in TrackerInfoList)
            {
                int TrackerId = TrackerInfo.tracker_id;
                PSMTrackerState trackerState= _trackers[TrackerId];

                if (trackerState.Init(TrackerInfo))
                {
                    trackerState.MarkedForCleanup = false;
                }
            }

            // For any Tracker state entry that didn't get update
            // make sure to turn off streaming if it was streaming previously
            for (int TrackerID = 0; TrackerID < _trackers.Length; ++TrackerID)
            {
                PSMTrackerState trackerState = _trackers[TrackerID];

                if (trackerState.MarkedForCleanup)
                {
                    trackerState.Cleanup();
                }
            }
        }

        public int TrackerSourceToID(eTrackerSource source)
        {
            return (int)source - 1;
        }

        public eTrackerSource TrackerIdToSource(int trackerId)
        {
            return (eTrackerSource)(trackerId + 1);
        }

        public eTrackerSource GetFirstValidTrackerSource(eTrackerSource defaultSource)
        {
            int defaultTrackerId = TrackerSourceToID(defaultSource);

            if (defaultTrackerId >= 0 && defaultTrackerId < _trackers.Length &&
                _trackers[defaultTrackerId].IsValid)
            {
                return defaultSource;
            }

            for (int trackerId = 0; trackerId < _trackers.Length; ++trackerId)
            {
                if (_trackers[trackerId].IsValid)
                {
                    return TrackerIdToSource(trackerId);
                }
            }

            return eTrackerSource.NONE;
        }

        public eTrackerSource GetNextValidTrackerSource(eTrackerSource currentSource)
        {
            int nextTrackerId = TrackerSourceToID(currentSource);

            for (int attemptCount = 0; attemptCount < _trackers.Length; ++attemptCount)
            {
                nextTrackerId++;

                if (nextTrackerId >= _trackers.Length)
                {
                    nextTrackerId = 0;
                }

                if (_trackers[nextTrackerId].IsValid)
                {
                    return TrackerIdToSource(nextTrackerId);
                }
            }

            return currentSource;
        }

        public eTrackerSource GetPrevValidTrackerSource(eTrackerSource currentSource)
        {
            int prevTrackerId = TrackerSourceToID(currentSource);

            for (int attemptCount = 0; attemptCount < _trackers.Length; ++attemptCount)
            {
                prevTrackerId--;

                if (prevTrackerId < 0)
                {
                    prevTrackerId = _trackers.Length - 1;
                }

                if (_trackers[prevTrackerId].IsValid)
                {
                    return TrackerIdToSource(prevTrackerId);
                }
            }

            return currentSource;
        }

        public bool GetTracker(eTrackerSource source, out PSMTracker Tracker)
        {
            int trackerId = TrackerSourceToID(source);
            if (trackerId >= 0 && trackerId < _trackers.Length && _trackers[trackerId].Tracker != null)
            {
                Tracker = _trackers[trackerId].Tracker;
                return true;
            }
            else
            {
                Tracker = null;
                return false;
            }
        }

        public PSMVector3f GetTrackerPosition(eTrackerSource source)
        {
            PSMTracker Tracker = null;
            if (GetTracker(source, out Tracker))
            {
                return Tracker.tracker_info.tracker_pose.Position;
            }
            return PSMoveClient.k_psm_float_vector3_zero;
        }

        public PSMQuatf GetTrackerOrientation(eTrackerSource source)
        {
            PSMTracker Tracker = null;
            if (GetTracker(source, out Tracker))
            {
                return Tracker.tracker_info.tracker_pose.Orientation;
            }
            return PSMoveClient.k_psm_quaternion_identity;
        }

        public bool GetVideoFrameBuffer(eTrackerSource source, PSMVideoFrameBuffer videoFrame)
        {
            int trackerId = TrackerSourceToID(source);

            if (trackerId >= 0 && trackerId < _trackers.Length && _trackers[trackerId].IsVideoStreamActive)
            {
                if (PSMoveClient.PSM_PollTrackerVideoStream(trackerId) == PSMResult.PSMResult_Success)
                {
                    uint lastFrameIndex = videoFrame.frame_index;

                    if (PSMoveClient.PSM_GetTrackerVideoFrameBuffer(trackerId, videoFrame) == PSMResult.PSMResult_Success)
                    {
                        return lastFrameIndex != videoFrame.frame_index;
                    }
                }
            }

            return false;
        }

        public int GetTrackerRefreshRateMilliseconds(eTrackerSource source)
        {
            PSMTracker Tracker = null;
            if (GetTracker(source, out Tracker))
            {
                // TODO: Hard code to 60FPS until tracker supports variable framerates
                return 16;
            }
            return 0;
        }
    }
}

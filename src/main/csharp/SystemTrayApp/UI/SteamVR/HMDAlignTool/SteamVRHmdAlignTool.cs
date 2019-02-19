using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemTrayApp
{
    public partial class SteamVRHmdAlignTool : UserControl, IAppPanel
    {
        class PositionCircularBuffer
        {
            protected static int k_sample_window = 100;
            protected static float k_max_position_variance_meters = 0.01f; // 1cm

            protected int _writeIndex;
            protected OpenGL.Vertex3f[] _positionSamples;

            protected float _maxDistFromAverage;

            protected OpenGL.Vertex3f _avgPositionSample;
            public OpenGL.Vertex3f AveragePosition
            {
                get { return _avgPositionSample; }
            }

            protected int _stableCount;
            public int StableCount
            {
                get { return _stableCount; }
            }

            protected int _sampleCount;
            public int SampleCount
            {
                get { return _sampleCount; }
            }

            public int SamplePercentComplete
            {
                get { return Math.Min(100, 100 * _stableCount / k_sample_window); }
            }

            public bool IsSamplingFinished
            {
                get { return _stableCount > k_sample_window; }
            }

            public bool IsPositionStable
            {
                get { return SampleCount >= k_sample_window && _maxDistFromAverage < k_max_position_variance_meters; }
            }

            public PositionCircularBuffer()
            {
                _positionSamples = new OpenGL.Vertex3f[k_sample_window];
                ClearSamples();
            }

            public void ClearSamples()
            {
                _writeIndex = 0;
                _sampleCount = 0;
                _stableCount = 0;
                _avgPositionSample = new OpenGL.Vertex3f();
            }

            public virtual void RecordSample(OpenGL.ModelMatrix sample)
            {
                _positionSamples[_writeIndex] = sample.Position;
                _writeIndex = (_writeIndex + 1) % k_sample_window;
                _sampleCount = Math.Min(_sampleCount + 1, k_sample_window);

                RecomputeStatistics();

                if (this.IsPositionStable)
                {
                    _stableCount = Math.Min(_stableCount+1, k_sample_window);
                }
                else
                {
                    _stableCount = 0;
                }
            }

            protected virtual void RecomputeStatistics()
            {
                _avgPositionSample = new OpenGL.Vertex3f();
                _maxDistFromAverage = 0.0f;

                if (_sampleCount > 1)
                {
                    // Compute the centroid of the samples
                    for (int sampleIndex = 0; sampleIndex < _sampleCount; ++sampleIndex)
                    {
                        _avgPositionSample+= _positionSamples[sampleIndex];
                    }
                    _avgPositionSample /= (float)_sampleCount;

                    // Compute the max displacement from the centroid
                    for (int sampleIndex = 0; sampleIndex < _sampleCount; ++sampleIndex)
                    {
                        float distSquared = MathUtility.DistanceSquared(_avgPositionSample, _positionSamples[sampleIndex]);

                        _maxDistFromAverage = Math.Max(distSquared, _maxDistFromAverage);
                    }
                    _maxDistFromAverage = MathUtility.sqrtf(_maxDistFromAverage);
                }
                else if (_sampleCount == 1)
                {
                    _avgPositionSample = _positionSamples[0];
                }
            }
        }

        class PoseCircularBuffer : PositionCircularBuffer
        {
            protected OpenGL.Vertex3f[] _forwardSamples;

            protected OpenGL.Vertex3f _avgForwardSample;
            public OpenGL.Vertex3f AverageForward
            {
                get { return _avgForwardSample; }
            }

            public PoseCircularBuffer() : base()
            {
                _forwardSamples = new OpenGL.Vertex3f[k_sample_window];
            }

            public override void RecordSample(OpenGL.ModelMatrix sample)
            {
                _forwardSamples[_writeIndex] = sample.ForwardVector;

                base.RecordSample(sample);
            }

            protected override void RecomputeStatistics()
            {
                _avgForwardSample = new OpenGL.Vertex3f();

                if (_sampleCount > 1)
                {
                    // Compute the average forward vector.
                    // If this position samples are stable then these samples should be pretty close together
                    for (int sampleIndex = 0; sampleIndex < _sampleCount; ++sampleIndex)
                    {
                        _avgForwardSample+= _forwardSamples[sampleIndex];
                    }
                    _avgForwardSample /= (float)_sampleCount;
                    _avgForwardSample.Normalize();
                }
                else if (_sampleCount == 1)
                {
                    _avgForwardSample = _forwardSamples[0];
                }

                base.RecomputeStatistics();
            }
        }

        class DeviceSampleSet
        {
            private PositionCircularBuffer _controllerLocationSamples;
            private PoseCircularBuffer _hmdPoseSamples;

            private uint _hmdDeviceId;
            public uint HmdDeviceId
            {
                get { return _hmdDeviceId; }
            }

            private uint _controlerDeviceId;
            public uint ControllerDeviceId
            {
                get { return _controlerDeviceId; }
            }

            public bool AreDevicesStable
            {
                get { return _controllerLocationSamples.IsPositionStable && _hmdPoseSamples.IsPositionStable; }
            }

            public bool IsSamplingFinished
            {
                get { return _controllerLocationSamples.IsSamplingFinished && _hmdPoseSamples.IsSamplingFinished; }
            }

            public int SamplePercentComplete
            {
                get { return Math.Min(_controllerLocationSamples.SamplePercentComplete, _hmdPoseSamples.SamplePercentComplete); }
            }

            public OpenGL.Vertex3f AverageControllerPosition
            {
                get { return _controllerLocationSamples.AveragePosition; }
            }

            public OpenGL.Vertex3f AverageHMDPosition
            {
                get { return _hmdPoseSamples.AveragePosition; }
            }

            public OpenGL.Vertex3f AverageHMDForward
            {
                get { return _hmdPoseSamples.AverageForward; }
            }

            public DeviceSampleSet(uint hmdDeviceId, uint controllerDeviceId)
            {
                _hmdDeviceId = hmdDeviceId;
                _controlerDeviceId = controllerDeviceId;
                _controllerLocationSamples = new PositionCircularBuffer();
                _hmdPoseSamples = new PoseCircularBuffer();
            }

            public void ClearSamples()
            {
                _controllerLocationSamples.ClearSamples();
                _hmdPoseSamples.ClearSamples();
            }

            public void RecordControllerSample(OpenGL.ModelMatrix sample)
            {
                _controllerLocationSamples.RecordSample(sample);
            }

            public void RecordHmdSample(OpenGL.ModelMatrix sample)
            {
                _hmdPoseSamples.RecordSample(sample);
            }
        }

        private DeviceSampleSet _deviceSampleSet;

        enum ePanelState
        {
            init,
            verifyTracking,
            placeDevices,
            waitForStableDevices,
            recordDevices,
            testAlignment,
            error
        }
        ePanelState _panelState;

        private static float k_psmoveBulbHeightMeters = 0.18f; // 18 cm from base of PSMove to middle of bulb
        private static float k_hmdCenterHeightMeters = 0.04f; // 4 cm from base to middle of HMD (when resting on table)
        private static float k_calibrationMatForwardLengthMeters = 0.11f; // 11 cm from center of calibration mat to edge of paper

        private static OpenGL.ColorRGBA _highlightColor = new OpenGL.ColorRGBA(0.667f, 0.886f, 0.145f, 1.0f);

        private OpenGL.Vertex3f _worldFromDriverPos;
        private OpenGL.Quaternion _worldFromDriverQuat;

        public SteamVRHmdAlignTool()
        {
            InitializeComponent();

            _panelState = ePanelState.init;
        }

        public void OnTrackedDevicesPoseUpdate(Dictionary<uint, OpenGL.ModelMatrix> poses)
        {
            SynchronizedInvoke.Invoke(this, () => HandleTrackedDevicesPoseUpdate(poses));
        }

        private void HandleTrackedDevicesPoseUpdate(Dictionary<uint, OpenGL.ModelMatrix> poses)
        {
            if (_panelState == ePanelState.waitForStableDevices || _panelState == ePanelState.recordDevices)
            {
                ePanelState nextPanelState = _panelState;

                if (poses.ContainsKey(_deviceSampleSet.HmdDeviceId))
                {
                    OpenGL.ModelMatrix newPose = poses[_deviceSampleSet.HmdDeviceId];

                    _deviceSampleSet.RecordHmdSample(newPose);
                }

                if (poses.ContainsKey(_deviceSampleSet.ControllerDeviceId))
                {
                    OpenGL.ModelMatrix newPose = poses[_deviceSampleSet.ControllerDeviceId];

                    _deviceSampleSet.RecordControllerSample(newPose);
                }

                if (_panelState == ePanelState.waitForStableDevices)
                {
                    if (_deviceSampleSet.AreDevicesStable)
                    {
                        if (_deviceSampleSet.IsSamplingFinished)
                        {
                            SamplingProgressBar.Value = 100;
                            nextPanelState = ePanelState.recordDevices;
                        }
                        else
                        {
                            SamplingProgressBar.Value = _deviceSampleSet.SamplePercentComplete;
                        }
                    }
                }
                else if (_panelState == ePanelState.recordDevices)
                {
                    if (!_deviceSampleSet.AreDevicesStable)
                    {
                        SamplingProgressBar.Value = 0;
                        nextPanelState = ePanelState.waitForStableDevices;
                    }
                    else if (!_deviceSampleSet.IsSamplingFinished)
                    {
                        SamplingProgressBar.Value = _deviceSampleSet.SamplePercentComplete;
                    }
                    else
                    {
                        RecomputeAlignmentTransform();
                        SaveAlignmentTransform();
                        SamplingProgressBar.Value = 100;
                        nextPanelState = ePanelState.testAlignment;
                    }
                }

                SetPanelState(nextPanelState);
            }
        }

        private void IdentifyControllerButton_Click(object sender, EventArgs e)
        {
            uint DeviceId = _deviceSampleSet.ControllerDeviceId;

            SynchronizedInvoke.Invoke(SteamVRContext.Instance, () => SteamVRContext.Instance.TriggerHapticPulse(DeviceId, 1.0f));
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            AppWindow.Instance.SetSteamVRPanel(new SteamVRPanel());
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ePanelState newState = _panelState;

            switch (_panelState)
            {
                case ePanelState.verifyTracking:
                    newState = ePanelState.placeDevices;
                    break;
                case ePanelState.placeDevices:
                    newState = ePanelState.waitForStableDevices;
                    break;
                case ePanelState.waitForStableDevices:
                    newState = ePanelState.recordDevices;
                    break;
                case ePanelState.recordDevices:
                    newState = ePanelState.testAlignment;
                    break;
                case ePanelState.testAlignment:
                    AppWindow.Instance.SetSteamVRPanel(new SteamVRPanel());
                    break;
                case ePanelState.error:
                    AppWindow.Instance.SetSteamVRPanel(new SteamVRPanel());
                    break;
            }

            SetPanelState(newState);
        }

        public void OnPanelEntered()
        {
            // Reset UI
            IdentifyControllerButton.Visible = false;
            InstructionsHeaderLabel.Text = "";
            InstructionsBodyLabel.Text = "";
            OkButton.Text = "Next";
            OkButton.Visible = true;
            SamplingProgressBar.Visible = true;

            FetchAlignmentTransform();

            SteamVRHeadMountedDisplay hmd = SteamVRContext.Instance.FetchFirstHMD();
            SteamVRController controller = SteamVRContext.Instance.FetchFirstControllerOfType(SteamVRController.ControllerType_PSMove);

            if (hmd != null && controller != null)
            {
                SteamVRDeviceInstance hmdModel = SteamVRFrame.FetchSteamVRModelInstance(hmd.DeviceID);
                SteamVRDeviceInstance controllerModel = SteamVRFrame.FetchSteamVRModelInstance(controller.DeviceID);

                if (hmdModel != null && controllerModel != null)
                {
                    hmdModel.SetDiffuseColor(_highlightColor);
                    controllerModel.SetDiffuseColor(_highlightColor);

                    _deviceSampleSet = new DeviceSampleSet(hmd.DeviceID, controller.DeviceID);

                    SetPanelState(ePanelState.verifyTracking);
                }
            }

            if (_panelState != ePanelState.verifyTracking)
            {
                InstructionsBodyLabel.Text = "Please make sure an HMD and a PSMove controller is connected and tracking before attempting alignment.";
                SetPanelState(ePanelState.error);
            }
        }

        public void OnPanelExited()
        {
            SetPanelState(ePanelState.init);
        }

        private void SetPanelState(ePanelState newState)
        {
            if (newState != _panelState)
            {
                OnExitPanelState(_panelState);
                OnEnterPanelState(newState);
                _panelState = newState;
            }
        }

        private void OnExitPanelState(ePanelState newState)
        {
            switch(newState)
            {
                case ePanelState.verifyTracking:
                    IdentifyControllerButton.Visible = false;
                    break;
                case ePanelState.placeDevices:
                    break;
                case ePanelState.waitForStableDevices:
                    SamplingProgressBar.Visible = false;
                    SteamVRContext.Instance.TrackedDevicesPoseUpdateEvent -= OnTrackedDevicesPoseUpdate;
                    break;
                case ePanelState.recordDevices:
                    SamplingProgressBar.Visible = false;
                    SteamVRContext.Instance.TrackedDevicesPoseUpdateEvent -= OnTrackedDevicesPoseUpdate;
                    break;
                case ePanelState.testAlignment:
                    break;
                case ePanelState.error:
                    break;
            }
        }

        private void OnEnterPanelState(ePanelState newState)
        {
            switch (newState)
            {
                case ePanelState.verifyTracking:
                    InstructionsHeaderLabel.Text = "Verify Devices";
                    InstructionsBodyLabel.Text= "Make sure an HMD and a controller are currently tracked in SteamVR.\r\nUse the 'Identify' controller button to verify you are using the highlighted controller.";
                    OkButton.Text = "Next";
                    OkButton.Visible = true;
                    IdentifyControllerButton.Visible = true;
                    break;
                case ePanelState.placeDevices:
                    InstructionsHeaderLabel.Text = "Place HMD and Controller";
                    InstructionsBodyLabel.Text = "Place the HMD and the lit PSMoveController on the locations marked on the calibration mat. Press 'Next' once you are ready to calibrate.";
                    break;
                case ePanelState.waitForStableDevices:
                    InstructionsHeaderLabel.Text = "Waiting for Stability";
                    InstructionsBodyLabel.Text = "Calibration will start once the devices are upright and stable.";
                    SamplingProgressBar.Value = 0;
                    SamplingProgressBar.Visible = true;
                    _deviceSampleSet.ClearSamples();
                    SteamVRContext.Instance.TrackedDevicesPoseUpdateEvent += OnTrackedDevicesPoseUpdate;
                    break;
                case ePanelState.recordDevices:
                    SamplingProgressBar.Value = 0;
                    SamplingProgressBar.Visible = true;
                    _deviceSampleSet.ClearSamples();
                    SteamVRContext.Instance.TrackedDevicesPoseUpdateEvent += OnTrackedDevicesPoseUpdate;
                    break;
                case ePanelState.testAlignment:
                    OkButton.Text = "Ok";
                    break;
                case ePanelState.error:
                    InstructionsHeaderLabel.Text = "Error";
                    OkButton.Visible = false;
                    break;
            }
        }

        private void RecomputeAlignmentTransform()
        {
            OpenGL.Vertex3f actualControllerPosition= _deviceSampleSet.AverageControllerPosition;
            OpenGL.Vertex3f actualHmdPosition = _deviceSampleSet.AverageHMDPosition;
            OpenGL.Vertex3f actualHmdForward = _deviceSampleSet.AverageHMDForward;
            OpenGL.Vertex3f actualHmdForward2D = MathUtility.ProjectUnitVectorXZ(actualHmdForward);

            OpenGL.Vertex3f expectedHmdPosition =
                actualControllerPosition
                - actualHmdForward* k_calibrationMatForwardLengthMeters
                - OpenGL.Vertex3f.UnitY * k_psmoveBulbHeightMeters
                + OpenGL.Vertex3f.UnitY * k_hmdCenterHeightMeters;

            OpenGL.Vertex3f expectedHmdForward = actualControllerPosition - actualHmdPosition;
            OpenGL.Vertex3f expectedHmdForward2D = MathUtility.ProjectUnitVectorXZ(expectedHmdForward);

            _worldFromDriverPos = actualHmdPosition - expectedHmdPosition;
            _worldFromDriverQuat = MathUtility.ShortestRotationBetween(expectedHmdForward2D, actualHmdForward2D);
        }

        private void FetchAlignmentTransform()
        {
            PSMoveSteamVRBridgeConfig config = PSMoveSteamVRBridgeConfig.Instance;

            _worldFromDriverPos= config.WorldFromDriverPos;
            _worldFromDriverQuat= config.WorldFromDriverQuat;
        }

        private void SaveAlignmentTransform()
        {
            PSMoveSteamVRBridgeConfig config = PSMoveSteamVRBridgeConfig.Instance;

            config.WorldFromDriverPos= _worldFromDriverPos;
            config.WorldFromDriverQuat= _worldFromDriverQuat;

            // This will push an update the config file,
            // which the SteamVR driver is watching for changes too.
            config.Save();
        }
    }
}

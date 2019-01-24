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
using System.Drawing;

namespace SystemTrayApp
{
    public class SteamVRTrackedDevice
    {
        protected uint _deviceID;
        public uint DeviceID
        {
            get { return _deviceID; }
        }

        protected ETrackedDeviceClass _deviceType;
        public ETrackedDeviceClass DeviceType
        {
            get { return _deviceType; }
        }

        protected OpenGL.ModelMatrix _transform;
        public OpenGL.ModelMatrix Transform
        {
            get { return _transform; }
        }

        protected bool _isPoseValid;
        public bool IsPoseValid
        {
            get { return _isPoseValid; }
        }

        private string _trackingSystem = "";
        public string TrackingSystem
        {
            get { return _trackingSystem; }
        }

        private string _modelLabel = "";
        public string ModelLabel
        {
            get { return _modelLabel; }
        }

        private string _modelNumber = "";
        public string ModelNumber
        {
            get { return _modelNumber; }
        }

        private string _resourcesPath = "";
        public string ResourcesPath
        {
            get { return _resourcesPath; }
        }

        private string _readyIconPath = "";
        public string ReadyIconPath
        {
            get { return _readyIconPath; }
        }

        private Image _readyIcon = null;
        public Image ReadyIcon
        {
            get { return _readyIcon; }
        }

        private string _renderModelName = "";
        public string RenderModelName
        {
            get { return _renderModelName; }
        }

        private SteamVRRenderModel _renderModel = null;
        public SteamVRRenderModel RenderModel
        {
            get { return _renderModel; }
        }

        public SteamVRTrackedDevice(uint deviceID, ETrackedDeviceClass deviceType)
        {
            _deviceID = deviceID;
            _deviceType = deviceType;
            _transform = new OpenGL.ModelMatrix();
            _transform.SetIdentity();
            _isPoseValid = false;
        }

        public virtual void UpdateProperties(CVRSystem SteamVRSystem)
        {
            _trackingSystem = FetchStringProperty(SteamVRSystem, ETrackedDeviceProperty.Prop_TrackingSystemName_String, "");
            _modelLabel = FetchStringProperty(SteamVRSystem, ETrackedDeviceProperty.Prop_ModeLabel_String, "");
            _modelNumber = FetchStringProperty(SteamVRSystem, ETrackedDeviceProperty.Prop_ModelNumber_String, "");

            UpdateResourcesPath(SteamVRSystem);

            if (UpdateReadyIconPath(SteamVRSystem))
            {
                UpdateReadyIconImage();
            }

            if (UpdateRenderModelName(SteamVRSystem))
            {
                UpdateRenderModel();
            }
        }

        private void UpdateResourcesPath(CVRSystem SteamVRSystem)
        {
            if (_trackingSystem.Length == 0)
                return;

            string steamVRRuntimePath = SteamVRContext.Instance.SteamVRRuntimePath;
            if (steamVRRuntimePath.Length == 0 || !Directory.Exists(steamVRRuntimePath))
                return;

            string resourcesPath =
                Path.Combine(new string[] { steamVRRuntimePath, "drivers", _trackingSystem, "resources" });

            if (Directory.Exists(resourcesPath)) {
                _resourcesPath = resourcesPath;
            }
        }

        private bool UpdateReadyIconPath(CVRSystem SteamVRSystem)
        {
            string newIconPath = "";

            if (_resourcesPath.Length != 0)
            {
                string partialIconPath = FetchStringProperty(SteamVRSystem, ETrackedDeviceProperty.Prop_NamedIconPathDeviceReady_String, "");
                if (partialIconPath.Length != 0)
                {
                    string resourcesToken = string.Format("{{{0}}}", _trackingSystem);
                    string fullIconPath = partialIconPath.Replace(resourcesToken, _resourcesPath);

                    if (File.Exists(fullIconPath)) {
                        newIconPath = fullIconPath;
                    }
                }
            }

            if (_readyIconPath != newIconPath) {
                _readyIconPath = newIconPath;
                return true;
            }

            return false;
        }

        private void UpdateReadyIconImage()
        {
            if (_readyIconPath.Length > 0) {
                _readyIcon = SteamVRResourceManager.Instance.FetchImageResource(_readyIconPath);
            } else {
                _readyIcon = null;
            }
        }

        private bool UpdateRenderModelName(CVRSystem SteamVRSystem)
        {
            string newRenderModelName = "";

            if (_resourcesPath.Length != 0) {
                newRenderModelName = FetchStringProperty(SteamVRSystem, ETrackedDeviceProperty.Prop_RenderModelName_String, "");
            }

            if (_renderModelName != newRenderModelName) {
                _renderModelName = newRenderModelName;
                return true;
            }

            return false;
        }

        private void UpdateRenderModel()
        {
            if (_renderModelName.Length > 0) {
                _renderModel = SteamVRResourceManager.Instance.FetchRenderModelResource(_renderModelName);
            }
            else {
                _readyIcon = null;
            }
        }

        public string FetchStringProperty(CVRSystem SteamVRSystem, ETrackedDeviceProperty property, string default_string)
        {
            StringBuilder stringBuilder = new StringBuilder(512);
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;

            SteamVRSystem.GetStringTrackedDeviceProperty(
                _deviceID,
                property,
                stringBuilder,
                (uint)stringBuilder.Capacity,
                ref error);

            return error == ETrackedPropertyError.TrackedProp_Success ? stringBuilder.ToString() : default_string;
        }

        public void ApplyPoseUpdate(TrackedDevicePose_t pose)
        {
            if (pose.bPoseIsValid) {
                SetTransform(pose.mDeviceToAbsoluteTracking);
                _isPoseValid = true;
            }
            else {
                _transform.SetIdentity();
                _isPoseValid = false;
            }
        }

        public void SetTransform(HmdMatrix34_t mat)
        {
            _transform = SteamVRUtility.ConvertToGlModelMatrix(mat);
        }
    }
}
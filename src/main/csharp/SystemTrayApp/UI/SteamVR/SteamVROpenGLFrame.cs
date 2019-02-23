using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenGL;

namespace SystemTrayApp
{
    public partial class SteamVROpenGLFrame : UserControl
    {
        private readonly List<Keys> _PressedKeys = new List<Keys>();
        private System.Drawing.Point? _Mouse;

        private Dictionary<uint, SteamVRDeviceInstance> _trackedDevices = new Dictionary<uint, SteamVRDeviceInstance>();
        private GlScene _glScene = new GlScene();
        private Timer _updateTimer;

        public GlCamera Camera
        {
            get { return _glScene.Camera; }
        }

        public bool EnableKeyboardPan = true;
        public bool EnabledMousePan = true;
        public bool EnabledMouseZoom = true;

        public SteamVROpenGLFrame()
        {
            InitializeComponent();

            // Add the initial set of devices
            List<SteamVRTrackedDevice> TrackedDevicesList = SteamVRContext.Instance.FetchLoadedTrackedDeviceList();
            foreach (var device in TrackedDevicesList)
            {
                HandleTrackedDeviceActivated(device);
            }

            _updateTimer = new Timer();
            _updateTimer.Tick += OnTick;
            _updateTimer.Interval = 33; // 30 fps
            _updateTimer.Start();
        }

        public SteamVRDeviceInstance FetchSteamVRModelInstance(uint steamVRDeviceId)
        {
            if (_trackedDevices.ContainsKey(steamVRDeviceId))
            {
                return _trackedDevices[steamVRDeviceId];
            }

            return null;
        }

        private void OnTick(object sender, EventArgs e)
        {
            this.Refresh();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            SteamVRContext.Instance.TrackedDeviceActivatedEvent += HandleTrackedDeviceActivated;
            SteamVRContext.Instance.TrackedDeviceDeactivatedEvent += HandleTrackedDeviceDeactivated;
            SteamVRContext.Instance.TrackedDevicesPoseUpdateEvent += HandleTrackedDevicesPoseUpdate;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            SteamVRContext.Instance.TrackedDeviceActivatedEvent -= HandleTrackedDeviceActivated;
            SteamVRContext.Instance.TrackedDeviceDeactivatedEvent -= HandleTrackedDeviceDeactivated;
            SteamVRContext.Instance.TrackedDevicesPoseUpdateEvent -= HandleTrackedDevicesPoseUpdate;
        }

        private void HandleTrackedDeviceActivated(SteamVRTrackedDevice device)
        {
            if (device.RenderModel != null)
            {
                SteamVRDeviceInstance instance = new SteamVRDeviceInstance();

                instance.AddToScene(_glScene, device);
                _trackedDevices.Add(device.DeviceID, instance);
            }
        }

        private void HandleTrackedDeviceDeactivated(SteamVRTrackedDevice device)
        {
            if (_trackedDevices.ContainsKey(device.DeviceID))
            {
                _trackedDevices[device.DeviceID].RemoveFromScene(_glScene);
                _trackedDevices.Remove(device.DeviceID);
            }
        }

        private void HandleTrackedDevicesPoseUpdate(Dictionary<uint, OpenGL.ModelMatrix> poses)
        {
            foreach (var KVPair in poses)
            {
                uint deviceID = KVPair.Key;

                if (_trackedDevices.ContainsKey(deviceID))
                {
                    OpenGL.ModelMatrix newPose = KVPair.Value;
                    SteamVRDeviceInstance model = _trackedDevices[deviceID];

                    model.ModelMatrix.Set(newPose);
                }
            }
        }

        private void glControl_ContextCreated(object sender, GlControlEventArgs e)
        {
            GlControl senderControl = (GlControl)sender;

            _glScene.NotifyGLContextCreated(senderControl.Width, senderControl.Height);
        }

        private void glControl_Render(object sender, GlControlEventArgs e)
        {
            GlControl senderControl = (GlControl)sender;

            _glScene.NotifyGLContextRender(senderControl.Width, senderControl.Height, e.DeviceContext);
        }

        private void glControl_ContextUpdate(object sender, GlControlEventArgs e)
        {
            _glScene.NotifyGLContextUpdated(e.DeviceContext);

            // Update child component transforms
            foreach (var KVPair in _trackedDevices)
            {
                KVPair.Value.PollComponentState();
            }

            if (EnableKeyboardPan)
            {
                foreach (Keys pressedKey in _PressedKeys)
                {
                    switch (pressedKey)
                    {
                        case Keys.A:
                            _glScene.Camera.ZOffset -= 0.1f;
                            break;
                        case Keys.D:
                            _glScene.Camera.ZOffset += 0.1f;
                            break;
                        case Keys.W:
                            _glScene.Camera.YOffset += 0.1f;
                            break;
                        case Keys.S:
                            _glScene.Camera.YOffset -= 0.1f;
                            break;
                    }
                }
            }
        }

        private void glControl_ContextDestroying(object sender, GlControlEventArgs e)
        {
            _glScene.ResourceManager.NotifyGLContextDisposed(e.DeviceContext);
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (EnabledMousePan)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _Mouse = e.Location;
                    _glScene.Camera.onMouseButtonDown();
                }
            }
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (EnabledMousePan)
            {
                if (_Mouse.HasValue)
                {
                    System.Drawing.Point delta = _Mouse.Value - (System.Drawing.Size)e.Location;
                    _Mouse = e.Location;
                    _glScene.Camera.onMouseMotion(delta.X, delta.Y);
                }
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            _Mouse = null;
        }

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (EnabledMouseZoom)
            {
                _glScene.Camera.onMouseWheel(e.Delta);
            }
        }

        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (_PressedKeys.Contains(e.KeyCode) == false)
            {
                _PressedKeys.Add(e.KeyCode);
            }
        }

        private void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            _PressedKeys.Remove(e.KeyCode);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin.Controls;

using OpenGL;

namespace SystemTrayApp
{
    public partial class SteamVRWindow : MaterialForm
    {
        private ColorRGBA k_clear_color = new ColorRGBA(0.447f, 0.565f, 0.604f, 1.0f);

        private float k_camera_vfov = 35.0f;
        private float k_camera_z_near = 0.1f;
        private float k_camera_z_far = 5000.0f;

        private readonly List<Keys> _PressedKeys = new List<Keys>();
        private System.Drawing.Point? _Mouse;

        private Dictionary<uint, GlModelInstance> _trackedDevices = new Dictionary<uint, GlModelInstance>();

        private GlResourceManager _glResourceManager = new GlResourceManager();
        private GlScene _glScene = new GlScene();

        private GlProgramCode _steamVRModelProgramCode = 
            new GlProgramCode( 
		        "render model",    
		        // vertex shader
		        @"#version 410
		        uniform mat4 matrix;
		        layout(location = 0) in vec4 position;
		        layout(location = 1) in vec3 v3NormalIn;
		        layout(location = 2) in vec2 v2TexCoordsIn;
		        out vec2 v2TexCoord;
		        void main()
		        {
		        	v2TexCoord = v2TexCoordsIn;
		        	gl_Position = matrix * vec4(position.xyz, 1);
		        }",    
		        //fragment shader
		        @"#version 410 core
		        uniform sampler2D diffuse;
		        in vec2 v2TexCoord;
		        out vec4 outputColor;
		        void main()
		        {
		           outputColor = texture( diffuse, v2TexCoord);
		        }");

        public SteamVRWindow()
        {
            InitializeComponent();

            // Add the initial set of devices
            List<SteamVRTrackedDevice> TrackedDevicesList = SteamVRContext.Instance.FetchLoadedTrackedDeviceList();
            foreach (var device in TrackedDevicesList)
            {
                HandleTrackedDeviceActivated(device);
            }

            SteamVRContext.Instance.TrackedDeviceActivatedEvent += OnTrackedDeviceActivated;
            SteamVRContext.Instance.TrackedDeviceDeactivatedEvent += OnTrackedDeviceDeactivated;
            SteamVRContext.Instance.TrackedDevicesPoseUpdateEvent += OnTrackedDevicesPoseUpdate;
        }

        public void OnTrackedDeviceActivated(SteamVRTrackedDevice device)
        {
            SynchronizedInvoke.Invoke(this, () => HandleTrackedDeviceActivated(device));
        }

        public void OnTrackedDeviceDeactivated(SteamVRTrackedDevice device)
        {
            SynchronizedInvoke.Invoke(this, () => HandleTrackedDeviceDeactivated(device));
        }

        public void OnTrackedDevicesPoseUpdate(Dictionary<uint, OpenGL.ModelMatrix> poses)
        {
            SynchronizedInvoke.Invoke(this, () => HandleTrackedDevicesPoseUpdate(poses));
        }

        private void HandleTrackedDeviceActivated(SteamVRTrackedDevice device)
        {
            if (device.RenderModel != null)
            {
                string instanceName = string.Format("SteamVRDevice_{0}", device.DeviceID);
                GlModelInstance instance = _glResourceManager.AllocateGlModel(instanceName, device.RenderModel, _steamVRModelProgramCode);

                instance.ModelMatrix.Set(device.Transform);

                _glScene.AddInstance(instance);
                _trackedDevices.Add(device.DeviceID, instance);
            }
        }

        private void HandleTrackedDeviceDeactivated(SteamVRTrackedDevice device)
        {
            if (_trackedDevices.ContainsKey(device.DeviceID))
            {
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
                    OpenGL.ModelMatrix newPose= KVPair.Value;
                    GlModelInstance model = _trackedDevices[deviceID];

                    model.ModelMatrix.Set(newPose);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            SteamVRContext.Instance.TrackedDeviceActivatedEvent -= OnTrackedDeviceActivated;
            SteamVRContext.Instance.TrackedDeviceDeactivatedEvent -= OnTrackedDeviceDeactivated;
            SteamVRContext.Instance.TrackedDevicesPoseUpdateEvent -= OnTrackedDevicesPoseUpdate;
        }

        private void glControl_ContextCreated(object sender, GlControlEventArgs e)
        {
            GlControl senderControl = (GlControl)sender;

            Gl.ClearColor(k_clear_color.Red, k_clear_color.Green, k_clear_color.Blue, k_clear_color.Alpha);
            Gl.Viewport(0, 0, senderControl.Width, senderControl.Height);

            Gl.Enable(EnableCap.Texture2d);
            Gl.Enable(EnableCap.DepthTest);

            _glScene.Camera.ProjectionMatrix.SetPerspective(
                k_camera_vfov,
                senderControl.Width / senderControl.Height, 
                k_camera_z_near, k_camera_z_far);
        }

        private void glControl_Render(object sender, GlControlEventArgs e)
        {
            GlControl senderControl = (GlControl)sender;
            float senderAspectRatio = (float)senderControl.Width / senderControl.Height;

            // Clear
            Gl.Viewport(0, 0, senderControl.Width, senderControl.Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _glScene.Render(e.DeviceContext);
        }

        private void glControl_ContextUpdate(object sender, GlControlEventArgs e)
        {
            _glResourceManager.NotifyGLContextUpdated(e.DeviceContext);

            foreach (Keys pressedKey in _PressedKeys) {
                switch (pressedKey) {
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

        private void glControl_ContextDestroying(object sender, GlControlEventArgs e)
        {
            _glResourceManager.NotifyGLContextDisposed(e.DeviceContext);
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _Mouse = e.Location;
                _glScene.Camera.onMouseButtonDown();
            }
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Mouse.HasValue) {
                System.Drawing.Point delta = _Mouse.Value - (System.Drawing.Size)e.Location;
                _Mouse = e.Location;
                _glScene.Camera.onMouseMotion(delta.X, delta.Y);
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            _Mouse = null;
        }

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            _glScene.Camera.onMouseWheel(e.Delta);
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

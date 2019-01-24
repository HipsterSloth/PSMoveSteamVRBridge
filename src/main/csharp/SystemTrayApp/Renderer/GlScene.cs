using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace SystemTrayApp
{
    public class GlDrawCall
    {
        public List<GlModelInstance> instances;

        public GlDrawCall()
        {
            instances = new List<GlModelInstance>();
        }
    }

    public class GlScene
    {
        private ColorRGBA k_clear_color = new ColorRGBA(0.447f, 0.565f, 0.604f, 1.0f);

        private float k_camera_vfov = 35.0f;
        private float k_camera_z_near = 0.1f;
        private float k_camera_z_far = 5000.0f;

        private GlResourceManager _glResourceManager;
        public GlResourceManager ResourceManager
        {
            get { return _glResourceManager; }
        }

        private GlCamera _glCamera;
        public GlCamera Camera
        {
            get { return _glCamera; }
        }

        private Dictionary<GlMaterial, GlDrawCall> _drawCalls;

        public GlScene()
        {
            _glResourceManager = new GlResourceManager();
            _glCamera = new GlCamera();
            _drawCalls = new Dictionary<GlMaterial, GlDrawCall>();
        }

        public void AddInstance(GlModelInstance instance)
        {
            if (!_drawCalls.ContainsKey(instance.Material))
            {
                _drawCalls.Add(instance.Material, new GlDrawCall());
            }

            _drawCalls[instance.Material].instances.Add(instance);
        }

        public void RemoveInstance(GlModelInstance instance)
        {
            if (_drawCalls.ContainsKey(instance.Material)) 
            {
                GlDrawCall drawCall = _drawCalls[instance.Material];

                drawCall.instances.Remove(instance);

                if (drawCall.instances.Count == 0)
                {
                    _drawCalls.Remove(instance.Material);
                }
            }
        }

        public void NotifyGLContextCreated(int WindowWidth, int WindowHeight)
        {
            Gl.ClearColor(k_clear_color.Red, k_clear_color.Green, k_clear_color.Blue, k_clear_color.Alpha);
            Gl.Viewport(0, 0, WindowWidth, WindowHeight);

            Gl.Enable(EnableCap.Texture2d);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Disable(EnableCap.CullFace);

            _glCamera.ProjectionMatrix.SetPerspective(
                k_camera_vfov,
                (float)WindowWidth / (float)WindowHeight,
                k_camera_z_near, k_camera_z_far);
        }

        public void NotifyGLContextUpdated(DeviceContext GlContext)
        {
            _glResourceManager.NotifyGLContextUpdated(GlContext);
        }

        public void NotifyGLContextDisposed(DeviceContext GlContext)
        {
            _glResourceManager.NotifyGLContextDisposed(GlContext);
        }

        public void NotifyGLContextRender(int WindowWidth, int WindowHeight, DeviceContext GlContext)
        {
            Matrix4x4 VPMatrix = _glCamera.ViewProjectionMatrix;

            Gl.Viewport(0, 0, WindowWidth, WindowHeight);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var element in _drawCalls) 
            {
                if (element.Key.BindMaterial(GlContext))
                {
                    foreach (var instance in element.Value.instances)
                    {
                        if (instance.Visible)
                        {
                            // Set the ModelViewProjection matrix transform on the shader program
                            GlMaterial material= element.Key;
                            Matrix4x4 MVPMatrix = VPMatrix * instance.ModelMatrix;
                            material.Program.SetModelViewProjectionMatrix(MVPMatrix);

                            instance.Render(GlContext);
                        }
                    }
                    element.Key.UnbindMaterial(GlContext);
                }
            }
        }
    }
}

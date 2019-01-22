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
        private GlCamera _glCamera = new GlCamera();
        public GlCamera Camera
        {
            get { return _glCamera; }
        }

        private Dictionary<GlMaterial, GlDrawCall> _drawCalls;

        public GlScene()
        {
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

        public void Render(DeviceContext GlContext)
        {
            Matrix4x4 VPMatrix = _glCamera.ViewProjectionMatrix;

            foreach (var element in _drawCalls) {
                if (element.Key.BindMaterial(GlContext))
                {
                    foreach (var instance in element.Value.instances) {                        
                        // Set the ModelViewProjection matrix transform on the shader program
                        GlMaterial material= element.Key;
                        Matrix4x4 MVPMatrix = VPMatrix * instance.ModelMatrix;
                        material.Program.SetModelViewProjectionMatrix(MVPMatrix);

                        instance.Render(GlContext);
                    }
                    element.Key.UnbindMaterial(GlContext);
                }
            }
        }
    }
}

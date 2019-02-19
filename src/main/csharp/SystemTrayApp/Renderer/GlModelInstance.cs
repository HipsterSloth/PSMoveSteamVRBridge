using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace SystemTrayApp
{
    public class GlModelInstance
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private ModelMatrix _modelMatrix = new ModelMatrix();
        public ModelMatrix ModelMatrix
        {
            get { return _modelMatrix; }
            set { _modelMatrix = value; }
        }

        private GlMaterialInstance _materialInstance;
        public GlMaterialInstance MaterialInstance
        {
            get { return _materialInstance; }
        }

        private GlTriangulatedMesh _mesh;
        public GlTriangulatedMesh Mesh
        {
            get { return _mesh; }
        }

        public GlModelInstance(string name, GlTriangulatedMesh mesh, GlMaterialInstance materialInstance)
        {
            _name = name;
            _visible = true;
            _mesh = mesh;
            _materialInstance = materialInstance;
        }

        public void Render(DeviceContext GlContext)
        {
            _mesh.DrawElements(GlContext);
        }
    }
}

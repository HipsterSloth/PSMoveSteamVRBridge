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
        private string _name = "";
        public string Name
        {
            get { return _name; }
        }

        private ModelMatrix _modelMatrix = new ModelMatrix();
        public ModelMatrix ModelMatrix
        {
            get { return _modelMatrix; }
            set { _modelMatrix = value; }
        }

        private GlMaterial _material;
        public GlMaterial Material
        {
            get { return _material; }
        }

        private GlTriangulatedMesh _mesh;
        public GlTriangulatedMesh Mesh
        {
            get { return _mesh; }
        }

        public GlModelInstance(string name, GlTriangulatedMesh mesh, GlMaterial material)
        {
            _name = name;
            _mesh = mesh;
            _material = material;
        }

        public void Render(DeviceContext GlContext)
        {
            _mesh.DrawElements(GlContext);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace SystemTrayApp
{
    public class GlMaterialInstance
    {
        private GlMaterial _material = null;
        public GlMaterial Material
        {
            get { return _material; }
        }

        private ColorRGBA _diffuseColor = null;
        public ColorRGBA DiffuseColor
        {
            get { return _diffuseColor; }
            set { _diffuseColor = value; }
        }

        public GlMaterialInstance(GlMaterial material)
        {
            _material = material;
            _diffuseColor = new ColorRGBA(1.0f, 1.0f, 1.0f, 1.0f);
        }

        public void ApplyMaterialInstanceParameters()
        {
            _material.Program.SetModelColor(_diffuseColor);
        }
    }
}

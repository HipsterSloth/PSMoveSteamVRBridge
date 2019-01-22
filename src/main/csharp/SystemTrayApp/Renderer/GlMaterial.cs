using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace SystemTrayApp
{
    public class GlMaterial
    {
        private string _name = "";
        public string Name
        {
            get { return _name; }
        }

        private GlTexture _texture = null;
        public GlTexture Texture
        {
            get { return _texture; }
        }

        private GlProgram _program = null;
        public GlProgram Program
        {
            get { return _program; }
        }

        public GlMaterial(string name, GlProgram program, GlTexture texture)
        {
            _name = name;
            _program = program;
            _texture = texture ;
        }

        public bool BindMaterial(DeviceContext GlContext)
        {
            if (_program.UseProgram(GlContext))
            {
                if (_texture.BindTexture(GlContext))
                {
                    return true;
                }
                else 
                {
                    _program.ClearProgram(GlContext);
                }
            }

            return false;
        }

        public void UnbindMaterial(DeviceContext GlContext)
        {
            _texture.ClearTexture(GlContext);
            _program.ClearProgram(GlContext);
        }
    }
}

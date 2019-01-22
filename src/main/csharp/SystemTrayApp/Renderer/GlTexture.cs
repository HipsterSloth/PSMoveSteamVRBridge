using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace SystemTrayApp
{
    public class GlTexture : IDisposable
    {
        private bool disposed = false;

        private DeviceContext _glContext = null;

        private uint _glTextureId = 0;
        public uint GlTextureId
        {
            get { return _glTextureId; }
        }

        private ushort _width;
        public ushort Width
        {
            get { return _width; }
        }

        private ushort _height;
        public ushort Height
        {
            get { return _height; }
        }

        private IntPtr _textureMapData; // const uint8_t *
        public IntPtr TextureMapData
        {
            get { return _textureMapData; }
        }

        public GlTexture(ushort width, ushort height, IntPtr textureMapData)
        {
            _width = width;
            _height = height;
            _textureMapData = textureMapData;
        }

        ~GlTexture()
        {
            Dispose(false);
        }

        public void NotifyGLContextUpdated(DeviceContext GlContext)
        {
            if (_glContext == null) {
                CreateGLResources(GlContext);
            }
        }

        public void NotifyGLContextDisposed(DeviceContext GlContext)
        {
            if (GlContext == _glContext) {
                DisposeGLResources();
            }
        }

        public bool BindTexture(DeviceContext GlContext)
        {
            if (_glTextureId != 0 && GlContext == _glContext)
            {
                Gl.ActiveTexture(TextureUnit.Texture0);
                Gl.BindTexture(TextureTarget.Texture2d, _glTextureId);
                return true;
            }

            return false;
        }

        public void ClearTexture(DeviceContext GlContext)
        {
            if (_glTextureId != 0 && GlContext == _glContext) 
            {
                Gl.BindTexture(TextureTarget.Texture2d, 0);
            }
        }

        private void CreateGLResources(DeviceContext GlContext)
        {
            _glContext = GlContext;

            if (_width > 0 && _height > 0 &&_textureMapData != IntPtr.Zero) {
                _glTextureId = Gl.GenTexture();
                Gl.BindTexture(TextureTarget.Texture2d, _glTextureId);

                Gl.TexImage2D(
                    TextureTarget.Texture2d, 0, InternalFormat.Rgba,
                    _width, _height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, _textureMapData);

                Gl.GenerateMipmap(TextureTarget.Texture2d);

                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

                Gl.BindTexture(TextureTarget.Texture2d, 0);
            }
        }

        private void DisposeGLResources()
        {
            if (_glTextureId != 0)
            {
                Gl.DeleteTextures(_glTextureId);
                _glTextureId = 0;
            }

            _glContext = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed) {
                if (disposing) {
                    DisposeGLResources();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

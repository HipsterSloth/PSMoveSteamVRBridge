using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using Valve.VR;

namespace SystemTrayApp
{
    public class SteamVRRenderModelComponent : IDisposable
    {
        private static GlVertexDefinition _vertexDefinition = null;

        private bool disposed = false;

        private IntPtr _renderModelPtr = IntPtr.Zero;
        private IntPtr _texturePtr = IntPtr.Zero;

        private RenderModel_t _renderModel;
        public RenderModel_t RenderModel
        {
            get { return _renderModel; }
        }

        private RenderModel_TextureMap_t _diffuseTexture;
        public RenderModel_TextureMap_t DiffuseTexture
        {
            get { return _diffuseTexture; }
        }

        public int _diffuseTextureId = -1;
        private uint _vertexCount = 0;
        private uint _triangleCount = 0;

        private string _componentName = "";
        public string ComponentName
        {
            get { return _componentName; }
        }

        private string _renderModelName = "";
        public string RenderModelName
        {
            get { return _renderModelName; }
        }

        public SteamVRRenderModelComponent(string componentName, string renderModelName)
        {
            _componentName = componentName;
            _renderModelName = renderModelName;
        }

        ~SteamVRRenderModelComponent()
        {
            Dispose(false);
        }

        public GlVertexDefinition GetVertexDefinition()
        {
            if (_vertexDefinition == null) {
                int vertexSize = Marshal.SizeOf(typeof(RenderModel_Vertex_t));

                List<GlVertexAttribute> attributes = new List<GlVertexAttribute>();
                attributes.Add(new GlVertexAttribute(0, 3, VertexAttribType.Float, false, vertexSize, Marshal.OffsetOf(typeof(RenderModel_Vertex_t), "vPosition")));
                attributes.Add(new GlVertexAttribute(1, 3, VertexAttribType.Float, false, vertexSize, Marshal.OffsetOf(typeof(RenderModel_Vertex_t), "vNormal")));
                attributes.Add(new GlVertexAttribute(2, 2, VertexAttribType.Float, false, vertexSize, Marshal.OffsetOf(typeof(RenderModel_Vertex_t), "rfTextureCoord0")));

                _vertexDefinition = new GlVertexDefinition(attributes, vertexSize);
            }

            return _vertexDefinition;
        }

        public bool LoadResources()
        {
            if (_renderModelName.Length == 0) {
                return false;
            }

            while(true)
            { 
                EVRRenderModelError result =
                    OpenVR.RenderModels.LoadRenderModel_Async(_renderModelName, ref _renderModelPtr);

                if (result == EVRRenderModelError.None) 
                {
                    if (ProcessRenderModel())
                    {
                        break;
                    }
                    else
                    {
                        return false;
                    }                    
                }
                else if (result != EVRRenderModelError.Loading)
                {
                    return false;
                }

                System.Threading.Thread.Sleep(1);
            }

            if (_diffuseTextureId != -1) 
            {
                while (true)
                {
                    EVRRenderModelError result =
                        OpenVR.RenderModels.LoadTexture_Async(_diffuseTextureId, ref _texturePtr);

                    if (result == EVRRenderModelError.None)
                    {
                        if (ProcessDiffuseTexture())
                        {
                            break;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (result != EVRRenderModelError.Loading) 
                    {
                        return false;
                    }

                    System.Threading.Thread.Sleep(1);
                }
            }

            return true;
        }

        private bool ProcessRenderModel()
        {
            if (_renderModelPtr != IntPtr.Zero) {
                _renderModel = (RenderModel_t)Marshal.PtrToStructure(_renderModelPtr, typeof(RenderModel_t));
                _vertexCount = _renderModel.unVertexCount;
                _triangleCount = _renderModel.unTriangleCount;

                if (_renderModel.diffuseTextureId >= 0) 
                {
                    // Move on to the texture loading
                    _diffuseTextureId = _renderModel.diffuseTextureId;
                }
                else 
                {
                    // Mark the render model as loaded (without a texture)
                    _diffuseTextureId = -1;
                }

                return true;
            }

            return false;
        }

        private bool ProcessDiffuseTexture()
        {
            if (_texturePtr != IntPtr.Zero) {
                _diffuseTexture = (RenderModel_TextureMap_t)Marshal.PtrToStructure(_texturePtr, typeof(RenderModel_TextureMap_t));

                return true;
            }

            return false;
        }

        private void DisposeSteamVRResources()
        {
            if (_texturePtr != IntPtr.Zero) {
                OpenVR.RenderModels.FreeTexture(_texturePtr);
                _texturePtr = IntPtr.Zero;
            }

            if (_renderModelPtr != IntPtr.Zero) {
                OpenVR.RenderModels.FreeRenderModel(_renderModelPtr);
                _renderModelPtr = IntPtr.Zero;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed) {
                if (disposing) {
                    DisposeSteamVRResources();
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

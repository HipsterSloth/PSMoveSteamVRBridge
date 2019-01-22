using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using OpenGL;

namespace SystemTrayApp
{
    public class SteamVRRenderModel : IDisposable
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

        private string _renderModelName = "";
        public string RenderModelName
        {
            get { return _renderModelName; }
        }

        public enum RenderModelLoadingState
        {
            Unloaded,
            LoadingGeometry,
            LoadingTexture,
            Loaded,
            Failed
        }
        private RenderModelLoadingState _loadingState = RenderModelLoadingState.Unloaded;
        public RenderModelLoadingState LoadingState
        {
            get { return _loadingState; }
        }
        public bool IsLoading
        {
            get {
                return
                    _loadingState == RenderModelLoadingState.LoadingGeometry ||
                    _loadingState == RenderModelLoadingState.LoadingTexture;
            }
        }

        public SteamVRRenderModel(string renderModelName)
        {
            _renderModelName = renderModelName;
            _loadingState = RenderModelLoadingState.Unloaded;
        }

        ~SteamVRRenderModel()
        {
            Dispose(false);
        }

        public bool StartAsyncLoad()
        {
            if (_loadingState == RenderModelLoadingState.Unloaded)
            {
                _loadingState = RenderModelLoadingState.LoadingGeometry;
                return PollAsyncLoad();
            }

            return false;
        }

        public bool PollAsyncLoad()
        {
            RenderModelLoadingState nextState = RenderModelLoadingState.Failed;

            while (nextState != _loadingState)
            {
                switch (_loadingState) {
                    case RenderModelLoadingState.LoadingGeometry: {
                            if (_renderModelName.Length > 0) {
                                EVRRenderModelError result =
                                    OpenVR.RenderModels.LoadRenderModel_Async(_renderModelName, ref _renderModelPtr);

                                if (result == EVRRenderModelError.None) {
                                    nextState = ProcessRenderModel();
                                }
                                else if (result == EVRRenderModelError.Loading) {
                                    nextState = RenderModelLoadingState.LoadingGeometry;
                                }
                            }
                        }
                        break;
                    case RenderModelLoadingState.LoadingTexture: {
                            if (_diffuseTextureId != -1) {
                                EVRRenderModelError result =
                                    OpenVR.RenderModels.LoadTexture_Async(_diffuseTextureId, ref _texturePtr);

                                if (result == EVRRenderModelError.None) {
                                    nextState = ProcessDiffuseTexture();
                                }
                                else if (result == EVRRenderModelError.Loading) {
                                    nextState = RenderModelLoadingState.LoadingTexture;
                                }
                            }
                        }
                        break;
                    case RenderModelLoadingState.Loaded:
                    case RenderModelLoadingState.Failed: {
                            // Nothing to do ...
                        }
                        break;
                }

                _loadingState = nextState;
            }

            return _loadingState != RenderModelLoadingState.Failed;
        }

        private RenderModelLoadingState ProcessRenderModel()
        {
            RenderModelLoadingState result = RenderModelLoadingState.Failed;

            if (_renderModelPtr != IntPtr.Zero) {
                _renderModel = (RenderModel_t)Marshal.PtrToStructure(_renderModelPtr, typeof(RenderModel_t));
                _vertexCount = _renderModel.unTriangleCount * 3;

                if (_renderModel.diffuseTextureId >= 0) {
                    // Move on to the texture loading
                    _diffuseTextureId = _renderModel.diffuseTextureId;
                    result = RenderModelLoadingState.LoadingTexture;
                }
                else {
                    // Mark the render model as loaded (without a texture)
                    _diffuseTextureId = -1;
                    result = RenderModelLoadingState.Loaded;
                }
            }

            return result;
        }

        private RenderModelLoadingState ProcessDiffuseTexture()
        {
            RenderModelLoadingState result = RenderModelLoadingState.Failed;

            if (_texturePtr != IntPtr.Zero) {
                _diffuseTexture = (RenderModel_TextureMap_t)Marshal.PtrToStructure(_texturePtr, typeof(RenderModel_TextureMap_t));

                // Mark the render model as loaded
                result = RenderModelLoadingState.Loaded;
            }

            return result;
        }

        public GlVertexDefinition GetVertexDefinition()
        {
            if (_vertexDefinition == null)
            {
                int vertexSize = Marshal.SizeOf(typeof(RenderModel_Vertex_t));

                List<GlVertexAttribute> attributes = new List<GlVertexAttribute>();
                attributes.Add(new GlVertexAttribute(0, 3, VertexAttribType.Float, false, vertexSize, Marshal.OffsetOf(typeof(RenderModel_Vertex_t), "vPosition")));
                attributes.Add(new GlVertexAttribute(1, 3, VertexAttribType.Float, false, vertexSize, Marshal.OffsetOf(typeof(RenderModel_Vertex_t), "vNormal")));
                attributes.Add(new GlVertexAttribute(2, 2, VertexAttribType.Float, false, vertexSize, Marshal.OffsetOf(typeof(RenderModel_Vertex_t), "rfTextureCoord0")));

                _vertexDefinition = new GlVertexDefinition(attributes, vertexSize);
            }

            return _vertexDefinition;
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
            if (!disposed)
            {
                if (disposing) 
                {
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

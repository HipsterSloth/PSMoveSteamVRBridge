using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using Valve.VR;

namespace SystemTrayApp
{
    public class GlResourceManager
    {
        private Dictionary<string, GlTriangulatedMesh> MeshCache = new Dictionary<string, GlTriangulatedMesh>();
        private Dictionary<string, GlTexture> TextureCache = new Dictionary<string, GlTexture>();
        private Dictionary<string, GlProgram> ProgramCache = new Dictionary<string, GlProgram>();
        private Dictionary<string, GlMaterial> MaterialCache = new Dictionary<string, GlMaterial>();

        public GlResourceManager()
        {

        }

        public void Init()
        {
        }

        public void NotifyGLContextUpdated(DeviceContext GlContext)
        {
            foreach (var element in MeshCache) {
                element.Value.NotifyGLContextUpdated(GlContext);
            }
            foreach (var element in TextureCache) {
                element.Value.NotifyGLContextUpdated(GlContext);
            }
            foreach (var element in ProgramCache) {
                element.Value.NotifyGLContextUpdated(GlContext);
            }
        }

        public void NotifyGLContextDisposed(DeviceContext GlContext)
        {
            foreach (var element in MeshCache) {
                element.Value.NotifyGLContextDisposed(GlContext);
            }
            foreach (var element in TextureCache) {
                element.Value.NotifyGLContextDisposed(GlContext);
            }
            foreach (var element in ProgramCache) {
                element.Value.NotifyGLContextDisposed(GlContext);
            }
        }

        public void Cleanup()
        {
            MaterialCache.Clear();

            foreach (var element in MeshCache) {
                element.Value.Dispose();
            }
            MeshCache.Clear();

            foreach (var element in TextureCache) {
                element.Value.Dispose();
            }
            TextureCache.Clear();

            foreach (var element in ProgramCache) {
                element.Value.Dispose();
            }
            ProgramCache.Clear();
        }

        public GlModelInstance AllocateGlModel(string name, SteamVRRenderModelComponent renderModel, GlProgramCode code)
        {
            GlMaterial material = FetchMaterial(renderModel, code);
            GlTriangulatedMesh mesh= FetchTriangulatedMeshResource(renderModel);

            return new GlModelInstance(name, mesh, material);
        }


        public GlProgram FetchProgramResource(GlProgramCode code)
        {
            if (ProgramCache.ContainsKey(code.shaderName)) {
                return ProgramCache[code.shaderName];
            }
            else {
                GlProgram program = new GlProgram(code);

                ProgramCache[code.shaderName] = program;
                return program;
            }
        }

        public GlMaterial FetchMaterial(SteamVRRenderModelComponent renderModel, GlProgramCode code)
        {
            string materialName = string.Format("{0}_{1}", renderModel.RenderModelName, code.shaderName);

            if (MaterialCache.ContainsKey(materialName)) {
                return MaterialCache[materialName];
            }
            else {
                GlProgram program = FetchProgramResource(code);
                GlTexture texture = FetchTextureResource(renderModel);
                GlMaterial material = new GlMaterial(materialName, program, texture);

                MaterialCache[renderModel.RenderModelName] = material;
                return material;
            }
        }

        public GlTexture FetchTextureResource(SteamVRRenderModelComponent renderModel)
        {
            if (TextureCache.ContainsKey(renderModel.RenderModelName)) {
                return TextureCache[renderModel.RenderModelName];
            }
            else {
                RenderModel_TextureMap_t vrModel = renderModel.DiffuseTexture;
                GlTexture texture = new GlTexture(vrModel.unWidth, vrModel.unHeight, vrModel.rubTextureMapData);

                TextureCache[renderModel.RenderModelName] = texture;
                return texture;
            }
        }

        public GlTriangulatedMesh FetchTriangulatedMeshResource(SteamVRRenderModelComponent renderModel)
        {
            if (MeshCache.ContainsKey(renderModel.RenderModelName)) {
                return MeshCache[renderModel.RenderModelName];
            }
            else 
            {
                RenderModel_t vrModel= renderModel.RenderModel;
                GlTriangulatedMesh mesh = 
                    new GlTriangulatedMesh(
                        renderModel.RenderModelName,
                        renderModel.GetVertexDefinition(),
                        vrModel.rVertexData, vrModel.unVertexCount, vrModel.rIndexData, vrModel.unTriangleCount);

                MeshCache[renderModel.RenderModelName] = mesh;
                return mesh;
            }
        }
    }
}

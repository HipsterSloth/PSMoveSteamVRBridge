using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace SystemTrayApp
{
    public class SteamVRRenderModel : IDisposable
    {
        private bool disposed = false;

        private SteamVRRenderModelComponent[] _components;
        public SteamVRRenderModelComponent[] Components
        {
            get { return _components; }
        }

        private string _renderModelName = "";
        public string RenderModelName
        {
            get { return _renderModelName; }
        }

        public SteamVRRenderModel(string renderModelName)
        {
            _components = new SteamVRRenderModelComponent[0];
            _renderModelName = renderModelName;
        }

        ~SteamVRRenderModel()
        {
            Dispose(false);
        }

        public bool LoadResources()
        {
            if (_renderModelName.Length <= 0)
                return false;

            if (OpenVR.RenderModels == null)
                return false;

            uint componentCount = OpenVR.RenderModels.GetComponentCount(_renderModelName);
            if (componentCount > 0) {
                List<SteamVRRenderModelComponent> componentList = new List<SteamVRRenderModelComponent>();

                for (uint componentIndex = 0; componentIndex < componentCount; ++componentIndex)
                {
                    uint componentNameLen = OpenVR.RenderModels.GetComponentName(_renderModelName, componentIndex, null, 0);
                    if (componentNameLen == 0)
                        continue;

                    StringBuilder componentName = new StringBuilder((int)componentNameLen);
                    if (OpenVR.RenderModels.GetComponentName(_renderModelName, componentIndex, componentName, componentNameLen) == 0)
                        continue;

                    // NOTE: Some components are dynamic and don't have meshes
                    uint componentRenderModelNameLen =
                        OpenVR.RenderModels.GetComponentRenderModelName(_renderModelName, componentName.ToString(), null, 0);
                    if (componentRenderModelNameLen == 0)
                        continue;

                    StringBuilder componentRenderModelName = new StringBuilder((int)componentRenderModelNameLen);
                    if (OpenVR.RenderModels.GetComponentRenderModelName(_renderModelName, componentName.ToString(), componentRenderModelName, componentRenderModelNameLen) == 0)
                        continue;

                    SteamVRRenderModelComponent ModelComponent =
                        SteamVRResourceManager.Instance.FetchRenderModelComponentResource(
                                componentName.ToString(),
                                componentRenderModelName.ToString());

                    if (ModelComponent != null)
                    {
                        componentList.Add(ModelComponent);
                    }
                    else 
                    {
                        return false;
                    }
                }

                _components = componentList.ToArray();
            }
            else
            {
                SteamVRRenderModelComponent ModelComponent =
                    SteamVRResourceManager.Instance.FetchRenderModelComponentResource("", _renderModelName);

                if (ModelComponent != null) 
                {
                    _components = new SteamVRRenderModelComponent[1];
                    _components[0] = ModelComponent;
                }
                else 
                {
                    return false;
                }
            }

            return true;
        }

        private void DisposeSteamVRResources()
        {
            foreach (SteamVRRenderModelComponent component in _components)
            {
                component.Dispose();
            }
            _components = null;
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

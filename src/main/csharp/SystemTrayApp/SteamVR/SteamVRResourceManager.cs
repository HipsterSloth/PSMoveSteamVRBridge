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
    public class SteamVRResourceManager
    {
        private Dictionary<string, Image> ImageCache = new Dictionary<string, Image>();
        private Dictionary<string, SteamVRRenderModel> RenderModelCache = new Dictionary<string, SteamVRRenderModel>();
        private Dictionary<string, SteamVRRenderModelComponent> RenderModelComponentCache = new Dictionary<string, SteamVRRenderModelComponent>();

        public static SteamVRResourceManager Instance
        {
            get { return SteamVRContext.Instance.ResourceManager; }
        }

        public SteamVRResourceManager()
        {

        }

        public void Init()
        {
        }

        public void Cleanup()
        {
            foreach (var element in ImageCache)
            {
                element.Value.Dispose();
            }
            ImageCache.Clear();

            foreach (var element in RenderModelCache)
            {
                element.Value.Dispose();
            }
            RenderModelCache.Clear();
        }

        public Image FetchImageResource(string imagePath)
        {
            if (imagePath.Length > 0) 
            {
                if (ImageCache.ContainsKey(imagePath))
                {
                    return ImageCache[imagePath];
                }
                else 
                {
                    try
                    {
                        Image image = Image.FromFile(imagePath);

                        if (image != null)
                        {
                            ImageCache.Add(imagePath, image);
                        }

                        return image;
                    }
                    catch(Exception)
                    {
                        return null;
                    }
                }                
            }

            return null;
        }

        public SteamVRRenderModel FetchRenderModelResource(string renderModelName)
        {
            if (renderModelName.Length > 0)
            {
                if (RenderModelCache.ContainsKey(renderModelName))
                {
                    return RenderModelCache[renderModelName];
                }
                else
                {
                    SteamVRRenderModel renderModel = new SteamVRRenderModel(renderModelName);

                    if (renderModel.LoadResources())
                    {
                        RenderModelCache.Add(renderModelName, renderModel);

                        return renderModel;
                    }
                }
            }

            return null;
        }

        public SteamVRRenderModelComponent FetchRenderModelComponentResource(string componentName, string renderModelName)
        {
            if (renderModelName.Length > 0)
            {
                if (RenderModelComponentCache.ContainsKey(renderModelName))
                {
                    return RenderModelComponentCache[renderModelName];
                }
                else 
                {
                    SteamVRRenderModelComponent renderModelComponent = 
                        new SteamVRRenderModelComponent(componentName, renderModelName);

                    if (renderModelComponent.LoadResources())
                    {
                        RenderModelComponentCache.Add(renderModelName, renderModelComponent);

                        return renderModelComponent;
                    }
                }
            }

            return null;
        }
    }
}

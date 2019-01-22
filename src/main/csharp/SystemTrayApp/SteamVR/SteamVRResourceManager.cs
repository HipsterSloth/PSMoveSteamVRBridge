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
        private bool HasPendingAsyncRenderModelLoads = false;

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

        public void PollAsyncLoadRequests()
        {
            if (HasPendingAsyncRenderModelLoads)
            {
                HasPendingAsyncRenderModelLoads = false;

                foreach (var element in RenderModelCache)
                {
                    SteamVRRenderModel renderModel = element.Value;

                    if (renderModel.IsLoading)
                    {
                        renderModel.PollAsyncLoad();
                        HasPendingAsyncRenderModelLoads |= renderModel.IsLoading;
                    }
                }
            }
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
                    try
                    {
                        SteamVRRenderModel renderModel = new SteamVRRenderModel(renderModelName);

                        if (renderModel != null)
                        {
                            if (renderModel.StartAsyncLoad())
                            {
                                RenderModelCache.Add(renderModelName, renderModel);

                                if (renderModel.LoadingState != SteamVRRenderModel.RenderModelLoadingState.Loaded)
                                {
                                    HasPendingAsyncRenderModelLoads= true;
                                }

                                return renderModel;
                            }
                        }

                        return null;
                    }
                    catch (Exception) 
                    {
                        return null;
                    }
                }
            }

            return null;
        }
    }
}

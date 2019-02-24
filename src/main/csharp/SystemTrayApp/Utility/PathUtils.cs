using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public static class PathUtility
    {
        public static string GetAppDataFolderPath()
        {
            string HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataPath = Path.Combine(HomeDir, "PSMoveSteamVRBridge");

            return AppDataPath;
        }

        public static string GetTrayAppExecutablePath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static string GetDriverResourcePath()
        {
            string appPath = GetTrayAppExecutablePath();
            string resourcesPath = Path.Combine(appPath, "drivers", "psmove", "resources");

            return resourcesPath;
        }

        public static string GetInputResourcesPath()
        {
            return Path.Combine(GetDriverResourcePath(), "input");
        }

        public static string GetActionManifestPathPath()
        {
            return Path.Combine(GetInputResourcesPath(), "psmovesteamvrbridge_trayapp_actions.json");
        }

        public static string GetPSMoveSteamVRBridgeInstallPath()
        {
            string install_path = "";

            if (PSMoveSteamVRBridgeConfig.Instance.UseInstallationPath)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(PSMoveServiceContext.PSMOVESTEAMVRBRIDE_REGKEY_PATH))
                    {
                        if (key != null)
                        {
                            object value = key.GetValue("Location");

                            if (value is string)
                            {
                                install_path = (string)value;
                            }
                        }
                    }
                }
            }

            if (install_path.Length == 0)
            {
                install_path = GetTrayAppExecutablePath();
            }

            return install_path;
        }

        public static string GetPSMoveServicePath()
        {
            string install_path = GetPSMoveSteamVRBridgeInstallPath();
            string psm_path = "";

            if (install_path.Length > 0)
            {
                psm_path = Path.Combine(install_path, PSMoveServiceContext.PSMOVESERVICE_PROCESS_NAME);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    psm_path = psm_path + ".exe";
                }

                if (!File.Exists(psm_path))
                {
                    psm_path = "";
                }
            }

            return psm_path;
        }
    }
}

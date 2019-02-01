using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Json;

namespace SystemTrayApp
{
    public sealed class ConfigManager
    {
        private List<ControllerConfig> _controllerConfigList;
        private static readonly Lazy<ConfigManager> _lazySingleton =
            new Lazy<ConfigManager>(() => new ConfigManager());

        public static ConfigManager Instance { get { return _lazySingleton.Value; } }
        
        private ConfigManager()
        {
            _psmSteamVRBBridgeConfig = new PSMoveSteamVRBridgeConfig();
            _freePIEConfig = new FreePIEConfig();
            _controllerConfigList = new List<ControllerConfig>();
        }

        public void LoadAll()
        {
            _psmSteamVRBBridgeConfig.Load();
            _freePIEConfig.Load();
            LoadAllControllerConfigs();
        }

        public void SaveAll()
        {
            _psmSteamVRBBridgeConfig.Save();
            _freePIEConfig.Save();
            SaveAllControllerConfigs();
        }

        public void LoadAllControllerConfigs()
        {
            foreach (ControllerConfig config in _controllerConfigList) {
                config.Load();
            }
        }

        public void SaveAllControllerConfigs()
        {
            foreach (ControllerConfig config in _controllerConfigList) {
                config.Save();
            }
        }

        public PSMoveSteamVRBridgeConfig _psmSteamVRBBridgeConfig;
        public PSMoveSteamVRBridgeConfig PSMSteamVRBridgeConfig
        {
            get { return _psmSteamVRBBridgeConfig; }
        }

        public FreePIEConfig _freePIEConfig;
        public FreePIEConfig FreePIEConfig
        {
            get { return _freePIEConfig; }
        }

        public void SetControllerConfigList(List<ControllerConfig> configList)
        {
            _controllerConfigList = configList;
        }

        public int GetControllerCount()
        {
            return _controllerConfigList.Count;
        }

        public ControllerConfig GetControllerConfig(int ControllerIndex)
        {
            return _controllerConfigList[ControllerIndex];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Json;

namespace SystemTrayApp
{
    public sealed class ConfigManager
    {
        private List<ControllerConfig> controllerConfigList;
        private static readonly Lazy<ConfigManager> lazy =
            new Lazy<ConfigManager>(() => new ConfigManager());

        public static ConfigManager Instance { get { return lazy.Value; } }
        
        private ConfigManager()
        {
            psm_steamvr_bridge_config = new PSMoveSteamVRBridgeConfig();
            controllerConfigList = new List<ControllerConfig>();
        }

        public void LoadAll()
        {
            psm_steamvr_bridge_config.Load();
            LoadAllControllerConfigs();
        }

        public void SaveAll()
        {
            psm_steamvr_bridge_config.Save();
            SaveAllControllerConfigs();
        }

        public void LoadAllControllerConfigs()
        {
            foreach (ControllerConfig config in controllerConfigList) {
                config.Load();
            }
        }

        public void SaveAllControllerConfigs()
        {
            foreach (ControllerConfig config in controllerConfigList) {
                config.Save();
            }
        }

        public PSMoveSteamVRBridgeConfig psm_steamvr_bridge_config;
        public PSMoveSteamVRBridgeConfig PSMSteamVRBridgeConfig
        {
            get { return psm_steamvr_bridge_config; }
        }

        public void SetControllerConfigList(List<ControllerConfig> configList)
        {
            controllerConfigList = configList;
        }

        public int GetControllerCount()
        {
            return controllerConfigList.Count;
        }

        public ControllerConfig GetControllerConfig(int ControllerIndex)
        {
            return controllerConfigList[ControllerIndex];
        }
    }
}
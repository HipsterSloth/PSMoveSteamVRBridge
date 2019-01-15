using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemTrayApp
{
    public partial class ControllerPanel : UserControl
    {
        private List<IControllerPanel> ControllerPanels;
        private int SelectedControllerIndex;

        public ControllerPanel()
        {
            InitializeComponent();

            // Rebuild the panels when the controller list changes
            // These are fired from the 
            ControllerPanels = new List<IControllerPanel>();
            PSMoveServiceContext.Instance.ControllerListPreUpdateEvent += OnControllerListPreChanged;
            PSMoveServiceContext.Instance.ControllerListPostUpdateEvent += OnControllerListPostChanged;
            PSMoveServiceContext.Instance.DisconnectedFromPSMServiceEvent += OnDisconnectedFromPSMServiceEvent;
        }

        public void OnControllerListPreChanged()
        {
            if (this.InvokeRequired) 
            {
                Invoke(
                    new MethodInvoker(delegate ()
                    {
                        SaveAllPanels();
                    }));
            }
            else 
            {
                SaveAllPanels();
            }
        }

        public void OnControllerListPostChanged(List<ControllerConfig> newControllerConfigList)
        {
            if (this.InvokeRequired)
            {
                Invoke(
                    new MethodInvoker(delegate ()
                    {
                        RebuildAllPanels(newControllerConfigList);
                    }));
            }
            else
            {
                RebuildAllPanels(newControllerConfigList);
            }
        }

        private void OnDisconnectedFromPSMServiceEvent()
        {
            if (this.InvokeRequired) {
                Invoke(
                    new MethodInvoker(delegate ()
                    {
                        SaveAllPanels();
                        RebuildAllPanels(new List<ControllerConfig>());
                    }));
            }
            else {
                SaveAllPanels();
                RebuildAllPanels(new List<ControllerConfig>());
            }
        }

        public void SaveAllPanels()
        {
            // Save all panels back to their respective config
            foreach (IControllerPanel panel in ControllerPanels)
            {
                panel.SaveToConfig();
            }

            // Save all controller configs back to disk
            ConfigManager.Instance.SaveAllControllerConfigs();
        }

        public void RebuildAllPanels(List<ControllerConfig> newControllerConfigList)
        {
            // Assign the new controller config list
            ConfigManager.Instance.SetControllerConfigList(newControllerConfigList);

            // Load all controller config back from disk
            ConfigManager.Instance.LoadAllControllerConfigs();

            DetachAllPanels();

            ControllerPanels.Clear();
            for (int ControllerIndex = 0; ControllerIndex < ControllerConfig.GetControllerCount(); ++ControllerIndex) {
                ControllerConfig Config = ControllerConfig.GetControllerConfigByIndex(ControllerIndex);

                if (Config.GetType() == typeof(DS4ControllerConfig)) {
                    ControllerPanels.Add(new Dualshock4ControllerPanel((DS4ControllerConfig)Config));
                }
                else if (Config.GetType() == typeof(PSMoveControllerConfig)) {
                    ControllerPanels.Add(new PSMoveControllerPanel((PSMoveControllerConfig)Config));
                }
                else if (Config.GetType() == typeof(PSNaviControllerConfig)) {
                    ControllerPanels.Add(new PSNaviControllerPanel((PSNaviControllerConfig)Config));
                }
                else if (Config.GetType() == typeof(VirtualControllerConfig)) {
                    ControllerPanels.Add(new VirtualControllerPanel((VirtualControllerConfig)Config));
                }
            }

            if (SelectedControllerIndex < 0 && ControllerPanels.Count > 0) {
                SelectedControllerIndex = 0;
            }
            if (SelectedControllerIndex >= ControllerPanels.Count) {
                SelectedControllerIndex = ControllerPanels.Count - 1;
            }

            SetSelectedControllerIndex(SelectedControllerIndex);
        }

        public void SetSelectedControllerIndex(int ControllerIndex)
        {
            DetachAllPanels();

            if (ControllerIndex >= 0 && ControllerIndex < ControllerPanels.Count)
            {
                ControllerConfig Config = ControllerConfig.GetControllerConfigByIndex(ControllerIndex);

                ControllerLabel.Text = Config.ControllerName;
                ContentPanel.Controls.Add((UserControl)ControllerPanels[ControllerIndex]);
            }
            else
            {
                ControllerLabel.Text = "";
            }

            if (ControllerIndex > 0)
            {
                PreviousControllerButton.Show();
            }
            else 
            {
                PreviousControllerButton.Hide();
            }

            if (ControllerIndex + 1 < ControllerPanels.Count) 
            {
                NextControllerButton.Show();
            }
            else 
            {
                NextControllerButton.Hide();
            }

            SelectedControllerIndex = ControllerIndex;
        }

        private void DetachAllPanels()
        {
            while (ContentPanel.Controls.Count > 0)
            {
                ContentPanel.Controls.RemoveAt(0);
            }
        }

        public void OnTabEntered()
        {
            SetSelectedControllerIndex(SelectedControllerIndex);
        }

        public void OnTabExited()
        {

        }

        private void PreviousControllerButton_Click(object sender, EventArgs e)
        {
            SetSelectedControllerIndex(SelectedControllerIndex - 1);
        }

        private void NextControllerButton_Click(object sender, EventArgs e)
        {
            SetSelectedControllerIndex(SelectedControllerIndex + 1);
        }

        private void ReloadControllerSettingsButton_Click(object sender, EventArgs e)
        {
            // Load all controller config back from disk
            ConfigManager.Instance.LoadAllControllerConfigs();

            if (SelectedControllerIndex >= 0 && SelectedControllerIndex < ControllerPanels.Count)
            {
                ControllerPanels[SelectedControllerIndex].ReloadFromConfig();
            }
        }

        private void SaveControllerSettingsButton_Click(object sender, EventArgs e)
        {
            if (SelectedControllerIndex >= 0 && SelectedControllerIndex < ControllerPanels.Count) 
            {
                ControllerPanels[SelectedControllerIndex].SaveToConfig();
            }

            // Save all dirty controller configs to disk
            ConfigManager.Instance.SaveAllControllerConfigs();
        }
    }
}

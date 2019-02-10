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
    public partial class FreePIEPanel : UserControl, IAppPanel
    {
        private bool bNeedsSlotRebuild;

        public FreePIEPanel()
        {
            InitializeComponent();

            FreePIEContext.Instance.ConnectedToFreePIEEvent += OnConnectedToFreePIEEvent;
            FreePIEContext.Instance.FreePIEConnectionFailureEvent += OnFreePIEConnectionFailureEvent;
            FreePIEContext.Instance.DisconnectedFromFreePIEEvent += OnDisconnectedFromFreePIEEvent;

            FreePIECurrentStatus.Text = "DISCONNECTED";
            FreePIEConnectBtn.Visible = true;
            FreePIEDisconnectBtn.Visible = false;
            bNeedsSlotRebuild = true;

            TriggerAxisComboBox.SelectedIndexChanged += HandleTriggerAxisChanged;
        }

        private void OnConnectedToFreePIEEvent()
        {
            SynchronizedInvoke.Invoke(this, () => HandleConnectedToFreePIEEvent());
        }

        private void OnDisconnectedFromFreePIEEvent()
        {
            SynchronizedInvoke.Invoke(this, () => HandleDisconnectedFromFreePIEEvent());
        }

        private void OnFreePIEConnectionFailureEvent(string Reason)
        {
            SynchronizedInvoke.Invoke(this, () => HandleFreePIEConnectionFailureEvent(Reason));
        }

        private void HandleConnectedToFreePIEEvent()
        {
            SetFreePieUIEnabled(false);
            FreePIECurrentStatus.Text = "CONNECTED";
            FreePIEConnectBtn.Visible = false;
            FreePIEDisconnectBtn.Visible = true;
            AddControllerBindingButton.Visible = false;
            AddHMDBindingButton.Visible = false;
        }

        private void HandleFreePIEConnectionFailureEvent(string Reason)
        {
            SetFreePieUIEnabled(true);
            FreePIECurrentStatus.Text = "FAILED: "+ Reason;
            FreePIEConnectBtn.Visible = true;
            FreePIEDisconnectBtn.Visible = false;

            if (BindingsLayoutPanel.Controls.Count < FreePIEContext.Instance.FreePIEMaxSlotCount) {
                AddControllerBindingButton.Visible = false;
                AddHMDBindingButton.Visible = false;
            }
        }

        private void HandleDisconnectedFromFreePIEEvent()
        {
            SetFreePieUIEnabled(true);
            FreePIECurrentStatus.Text = "DISCONNECTED";
            FreePIEConnectBtn.Visible = true;
            FreePIEDisconnectBtn.Visible = false;

            if (BindingsLayoutPanel.Controls.Count < FreePIEContext.Instance.FreePIEMaxSlotCount)
            {
                AddControllerBindingButton.Visible = true;
                AddHMDBindingButton.Visible = true;
            }
        }

        public void HandleSlotMappingChanged()
        {
            FreePIEConfig.Instance.SlotDefinitions= BuildSlotDefintions();
            FreePIEConfig.Instance.Save();
        }

        public void HandleSlotMappingDeleted()
        {
            FreePIEConfig.Instance.SlotDefinitions = BuildSlotDefintions();
            FreePIEConfig.Instance.Save();

            AddControllerBindingButton.Visible = true;
            AddHMDBindingButton.Visible = true;
        }

        private void HandleTriggerAxisChanged(object sender, EventArgs e)
        {
            FreePIEConfig.Instance.VirtualControllerTriggerAxisIndex = TriggerAxisComboBox.SelectedIndex - 1;
            FreePIEConfig.Instance.Save();
        }

        public void OnPanelEntered()
        {
            ReloadFromConfig();
        }

        public void OnPanelExited()
        {

        }

        private void ReloadFromConfig()
        {
            if (!bNeedsSlotRebuild)
                return;

            FreePIEConfig freePIEConfig= FreePIEConfig.Instance;

            BindingsLayoutPanel.Controls.Clear();

            foreach (FreePIESlotDefinition slotDefinition in freePIEConfig.SlotDefinitions)
            {
                if (slotDefinition is FreePIEControllerSlotDefinition)
                {
                    AddControllerSlotMapping((FreePIEControllerSlotDefinition)slotDefinition);
                }
                else if (slotDefinition is FreePIEHmdSlotDefinition)
                {
                    AddHmdSlotMapping((FreePIEHmdSlotDefinition)slotDefinition);
                }
            }

            TriggerAxisComboBox.SelectedIndex = freePIEConfig.VirtualControllerTriggerAxisIndex + 1;

            bNeedsSlotRebuild = false;
        }

        private void AddControllerSlotMapping(FreePIEControllerSlotDefinition slotDefinition)
        {
            FreePIEControllerSlotMapping slotMapping= new FreePIEControllerSlotMapping(slotDefinition);
            slotMapping.SlotMappingChangedEvent += HandleSlotMappingChanged;
            slotMapping.SlotMappingDeletedEvent += HandleSlotMappingDeleted;
            BindingsLayoutPanel.Controls.Add(slotMapping);
        }

        private void AddHmdSlotMapping(FreePIEHmdSlotDefinition slotDefinition)
        {
            FreePIEHmdSlotMapping slotMapping = new FreePIEHmdSlotMapping(slotDefinition);
            slotMapping.SlotMappingChangedEvent += HandleSlotMappingChanged;
            slotMapping.SlotMappingDeletedEvent += HandleSlotMappingDeleted;
            BindingsLayoutPanel.Controls.Add(slotMapping);
        }

        private void SetFreePieUIEnabled(bool bEnabled)
        {
            foreach (var BindingPanel in BindingsLayoutPanel.Controls)
            {
                if (BindingPanel is FreePIEControllerSlotMapping)
                {
                    ((FreePIEControllerSlotMapping)BindingPanel).SetEnabled(bEnabled);
                }
                else if (BindingPanel is FreePIEHmdSlotMapping)
                {
                    ((FreePIEHmdSlotMapping)BindingPanel).SetEnabled(bEnabled);
                }
            }

            TriggerAxisComboBox.Enabled = bEnabled;
        }

        private void AddControllerBindingButton_Click(object sender, EventArgs e)
        {
            FreePIEControllerSlotDefinition slotDefinition = new FreePIEControllerSlotDefinition(BindingsLayoutPanel.Controls.Count - 1);
            
            AddControllerSlotMapping((FreePIEControllerSlotDefinition)slotDefinition);
            if (BindingsLayoutPanel.Controls.Count >= FreePIEContext.Instance.FreePIEMaxSlotCount)
            {
                AddControllerBindingButton.Visible = false;
                AddHMDBindingButton.Visible = false;
            }

            FreePIEConfig.Instance.SlotDefinitions = BuildSlotDefintions();
            FreePIEConfig.Instance.Save();
        }

        private void AddHMDBindingButton_Click(object sender, EventArgs e)
        {
            FreePIEHmdSlotDefinition slotDefinition = new FreePIEHmdSlotDefinition(BindingsLayoutPanel.Controls.Count - 1);

            AddHmdSlotMapping((FreePIEHmdSlotDefinition)slotDefinition);
            if (BindingsLayoutPanel.Controls.Count >= FreePIEContext.Instance.FreePIEMaxSlotCount)
            {
                AddControllerBindingButton.Visible = false;
                AddHMDBindingButton.Visible = false;
            }

            FreePIEConfig.Instance.SlotDefinitions = BuildSlotDefintions();
            FreePIEConfig.Instance.Save();
        }

        private FreePIESlotDefinition[] BuildSlotDefintions()
        {
            List<FreePIESlotDefinition> slotDefintions = new List<FreePIESlotDefinition>();

            int slotIndex = 0;
            foreach (var BindingPanel in BindingsLayoutPanel.Controls)
            {
                if (BindingPanel is FreePIEControllerSlotMapping)
                {
                    FreePIEControllerSlotDefinition slotDefinition = new FreePIEControllerSlotDefinition(slotIndex);
                    slotIndex++;

                    ((FreePIEControllerSlotMapping)BindingPanel).FetchSlotDefinition(slotDefinition);
                    slotDefintions.Add(slotDefinition);
                }
                else if (BindingPanel is FreePIEHmdSlotMapping)
                {
                    FreePIEHmdSlotDefinition slotDefinition = new FreePIEHmdSlotDefinition(slotIndex);
                    slotIndex++;

                    ((FreePIEHmdSlotMapping)BindingPanel).FetchSlotDefinition(slotDefinition);
                    slotDefintions.Add(slotDefinition);
                }
            }

            return slotDefintions.ToArray();
        }

        private void FreePIEConnectBtn_Click(object sender, EventArgs e)
        {
            FreePIESlotDefinition[] slotDefinitions= BuildSlotDefintions();

            // Connect to FreePIE
            FreePIECurrentStatus.Text = "CONNECTING";
            FreePIEConnectBtn.Visible = false;
            FreePIEDisconnectBtn.Visible = false;
            SynchronizedInvoke.Invoke(FreePIEContext.Instance, () => FreePIEContext.Instance.ConnectToFreePIE(slotDefinitions));
        }

        private void FreePIEDisconnectBtn_Click(object sender, EventArgs e)
        {
            SynchronizedInvoke.Invoke(FreePIEContext.Instance, () => FreePIEContext.Instance.DisconnectFromFreePIE());
        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}

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
    public partial class FreePIEPanel : UserControl
    {
        private bool bNeedsSlotRebuild;

        public FreePIEPanel()
        {
            InitializeComponent();

            FreePIEContext.Instance.ConnectedToFreePIEEvent += OnConnectedToFreePIEEvent;
            FreePIEContext.Instance.DisconnectedFromFreePIEEvent += OnDisconnectedFromFreePIEEvent;

            FreePIECurrentStatus.Text = "DISCONNECTED";
            FreePIEConnectBtn.Visible = true;
            FreePIEDisconnectBtn.Visible = false;
            bNeedsSlotRebuild = true;
        }

        private void OnConnectedToFreePIEEvent()
        {
            SynchronizedInvoke.Invoke(this, () => HandleConnectedToFreePIEEvent());
        }

        private void OnDisconnectedFromFreePIEEvent()
        {
            SynchronizedInvoke.Invoke(this, () => HandleDisconnectedFromFreePIEEvent());
        }

        private void HandleConnectedToFreePIEEvent()
        {
            SetBindingsEnabled(false);
            FreePIECurrentStatus.Text = "CONNECTED";
            FreePIEConnectBtn.Visible = false;
            FreePIEDisconnectBtn.Visible = true;
            AddControllerBindingButton.Visible = false;
            AddHMDBindingButton.Visible = false;
        }

        private void HandleDisconnectedFromFreePIEEvent()
        {
            SetBindingsEnabled(true);
            FreePIECurrentStatus.Text = "DISCONNECTED";
            FreePIEConnectBtn.Visible = true;
            FreePIEDisconnectBtn.Visible = false;

            if (BindingsLayoutPanel.Controls.Count < FreePIEContext.Instance.FreePIEMaxSlotCount)
            {
                AddControllerBindingButton.Visible = false;
                AddHMDBindingButton.Visible = false;
            }
        }

        public void HandleSlotMappingDeleted()
        {
            AddControllerBindingButton.Visible = true;
            AddHMDBindingButton.Visible = true;
        }

        public void OnTabEntered()
        {
            ReloadFromConfig();
        }

        public void OnTabExited()
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
                    BindingsLayoutPanel.Controls.Add(
                        new FreePIEControllerSlotMapping(
                            (FreePIEControllerSlotDefinition)slotDefinition));
                }
                else if (slotDefinition is FreePIEHmdSlotDefinition)
                {
                    BindingsLayoutPanel.Controls.Add(
                        new FreePIEHmdSlotMapping(
                            (FreePIEHmdSlotDefinition)slotDefinition));
                }
            }

            bNeedsSlotRebuild = false;
        }

        private void SetBindingsEnabled(bool bEnabled)
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
        }

        private void AddControllerBindingButton_Click(object sender, EventArgs e)
        {
            FreePIEControllerSlotDefinition slotDefinition = new FreePIEControllerSlotDefinition(BindingsLayoutPanel.Controls.Count - 1);
            FreePIEControllerSlotMapping slotMapping= new FreePIEControllerSlotMapping(slotDefinition);
            slotMapping.SlotMappingDeletedEvent += HandleSlotMappingDeleted;

            BindingsLayoutPanel.Controls.Add(slotMapping);
            if (BindingsLayoutPanel.Controls.Count >= FreePIEContext.Instance.FreePIEMaxSlotCount)
            {
                AddControllerBindingButton.Visible = false;
                AddHMDBindingButton.Visible = false;
            }
        }

        private void AddHMDBindingButton_Click(object sender, EventArgs e)
        {
            FreePIEHmdSlotDefinition slotDefinition = new FreePIEHmdSlotDefinition(BindingsLayoutPanel.Controls.Count - 1);
            FreePIEHmdSlotMapping slotMapping = new FreePIEHmdSlotMapping(slotDefinition);
            slotMapping.SlotMappingDeletedEvent += HandleSlotMappingDeleted;

            BindingsLayoutPanel.Controls.Add(slotMapping);

            if (BindingsLayoutPanel.Controls.Count >= FreePIEContext.Instance.FreePIEMaxSlotCount)
            {
                AddControllerBindingButton.Visible = false;
                AddHMDBindingButton.Visible = false;
            }
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
                }
                else if (BindingPanel is FreePIEHmdSlotMapping)
                {
                    FreePIEHmdSlotDefinition slotDefinition = new FreePIEHmdSlotDefinition(slotIndex);
                    slotIndex++;

                    ((FreePIEHmdSlotMapping)BindingPanel).FetchSlotDefinition(slotDefinition);
                }
            }

            return slotDefintions.ToArray();
        }

        private void FreePIEConnectBtn_Click(object sender, EventArgs e)
        {
            FreePIESlotDefinition[] slotDefinitions= BuildSlotDefintions();

            // Save the new slot definitions to the config
            FreePIEConfig.Instance.SlotDefinitions = slotDefinitions;

            // Connect to FreePIE
            SynchronizedInvoke.Invoke(FreePIEContext.Instance, () => FreePIEContext.Instance.ConnectToFreePIE(slotDefinitions));
        }

        private void FreePIEDisconnectBtn_Click(object sender, EventArgs e)
        {
            SynchronizedInvoke.Invoke(FreePIEContext.Instance, () => FreePIEContext.Instance.DisconnectFromFreePIE());
        }
    }
}

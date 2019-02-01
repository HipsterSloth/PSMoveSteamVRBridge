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
            FreePIEControllerSlotDefinition slotDefinition = new FreePIEControllerSlotDefinition();
            slotDefinition.SlotIndex = BindingsLayoutPanel.Controls.Count - 1;
            slotDefinition.xProperty.controllerSource = eControllerSource.CONTROLLER_0;
            slotDefinition.xProperty.controllerPropertySource = eControllerPropertySource.POSITION_X;
            slotDefinition.yProperty.controllerSource = eControllerSource.CONTROLLER_0;
            slotDefinition.yProperty.controllerPropertySource = eControllerPropertySource.POSITION_Y;
            slotDefinition.zProperty.controllerSource = eControllerSource.CONTROLLER_0;
            slotDefinition.zProperty.controllerPropertySource = eControllerPropertySource.POSITION_Z;

            slotDefinition.pitchProperty.controllerSource = eControllerSource.CONTROLLER_0;
            slotDefinition.pitchProperty.controllerPropertySource = eControllerPropertySource.ORIENTATION_PITCH;
            slotDefinition.rollProperty.controllerSource = eControllerSource.CONTROLLER_0;
            slotDefinition.rollProperty.controllerPropertySource = eControllerPropertySource.ORIENTATION_ROLL;
            slotDefinition.yawProperty.controllerSource = eControllerSource.CONTROLLER_0;
            slotDefinition.yawProperty.controllerPropertySource = eControllerPropertySource.ORIENTATION_YAW;

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
            FreePIEHmdSlotDefinition slotDefinition = new FreePIEHmdSlotDefinition();
            slotDefinition.SlotIndex = BindingsLayoutPanel.Controls.Count - 1;
            slotDefinition.xProperty.hmdSource = eHmdSource.HMD_0;
            slotDefinition.xProperty.hmdPropertySource = eHmdPropertySource.POSITION_X;
            slotDefinition.yProperty.hmdSource = eHmdSource.HMD_0;
            slotDefinition.yProperty.hmdPropertySource = eHmdPropertySource.POSITION_Y;
            slotDefinition.zProperty.hmdSource = eHmdSource.HMD_0;
            slotDefinition.zProperty.hmdPropertySource = eHmdPropertySource.POSITION_Z;

            slotDefinition.pitchProperty.hmdSource = eHmdSource.HMD_0;
            slotDefinition.pitchProperty.hmdPropertySource = eHmdPropertySource.ORIENTATION_PITCH;
            slotDefinition.rollProperty.hmdSource = eHmdSource.HMD_0;
            slotDefinition.rollProperty.hmdPropertySource = eHmdPropertySource.ORIENTATION_ROLL;
            slotDefinition.yawProperty.hmdSource = eHmdSource.HMD_0;
            slotDefinition.yawProperty.hmdPropertySource = eHmdPropertySource.ORIENTATION_YAW;

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
                    FreePIEControllerSlotDefinition slotDefinition = new FreePIEControllerSlotDefinition();
                    slotDefinition.SlotIndex = slotIndex;
                    slotIndex++;

                    ((FreePIEControllerSlotMapping)BindingPanel).FetchSlotDefinition(slotDefinition);
                }
                else if (BindingPanel is FreePIEHmdSlotMapping)
                {
                    FreePIEHmdSlotDefinition slotDefinition = new FreePIEHmdSlotDefinition();
                    slotDefinition.SlotIndex = slotIndex;
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

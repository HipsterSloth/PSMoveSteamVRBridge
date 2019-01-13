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
    public partial class PSMoveServicePanel : UserControl
    {
        public PSMoveServicePanel()
        {
            InitializeComponent();

            PSMoveServiceContext.Instance.ConnectedToPSMServiceEvent += Instance_ConnectedToPSMServiceEvent;
            PSMoveServiceContext.Instance.DisconnectedFromPSMServiceEvent += Instance_DisconnectedFromPSMServiceEvent;
        }

        private void Instance_ConnectedToPSMServiceEvent()
        {
            if (this.InvokeRequired) {
                Invoke(
                    new MethodInvoker(delegate ()
                    {
                        PSMCurrentStatus.Text = "CONNECTED";
                    }));
            }
            else {
                PSMCurrentStatus.Text = "CONNECTED";
            }
        }

        private void Instance_DisconnectedFromPSMServiceEvent()
        {
            if (this.InvokeRequired) {
                Invoke(
                    new MethodInvoker(delegate ()
                    {
                        PSMCurrentStatus.Text = "DISCONNECTED";
                    }));
            }
            else {
                PSMCurrentStatus.Text = "DISCONNECTED";
            }
        }

        public void OnTabEntered()
        {
            PSMoveSteamVRBridgeConfig config = PSMoveSteamVRBridgeConfig.Instance;

            ServerAddressTextField.Text = config.ServerAddress;
            ServerPortTextField.Text = config.ServerPort;
            AutoLaunchPSMCheckBox.Checked = config.AutoLaunchPSMoveService;

            AutoLaunchPSMCheckBox.CheckedChanged += new System.EventHandler(this.AutoLaunchPSMCheckBox_CheckedChanged);
            ServerAddressTextField.Enter += new System.EventHandler(this.ServerAddressTextField_Changed);
            ServerPortTextField.Enter += new System.EventHandler(this.ServerPortTextField_Changed);
        }

        public void OnTabExited()
        {
            AutoLaunchPSMCheckBox.CheckedChanged -= new System.EventHandler(this.AutoLaunchPSMCheckBox_CheckedChanged);
            ServerAddressTextField.Enter -= new System.EventHandler(this.ServerAddressTextField_Changed);
            ServerPortTextField.Enter -= new System.EventHandler(this.ServerPortTextField_Changed);
        }

        private void AutoLaunchPSMCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PSMoveSteamVRBridgeConfig.Instance.AutoLaunchPSMoveService = AutoLaunchPSMCheckBox.Checked;
        }

        private void SavePSMSettingsButton_Click(object sender, EventArgs e)
        {
            PSMoveSteamVRBridgeConfig.Instance.Save();
        }

        private void ReloadPSMSettingsButton_Click(object sender, EventArgs e)
        {
            PSMoveSteamVRBridgeConfig.Instance.Load();
        }

        private void ServerAddressTextField_Changed(object sender, EventArgs e)
        {
            PSMoveSteamVRBridgeConfig.Instance.ServerAddress = ServerAddressTextField.Text;
        }

        private void ServerPortTextField_Changed(object sender, EventArgs e)
        {
            PSMoveSteamVRBridgeConfig.Instance.ServerPort = ServerPortTextField.Text;
        }


        private void StartPSMButton_Click(object sender, EventArgs e)
        {
            PSMoveServiceContext.Instance.LaunchPSMoveServiceProcess();
        }

        private void StopPSMButton_Click(object sender, EventArgs e)
        {
            PSMoveServiceContext.Instance.TerminatePSMoveServiceProcess();
        }
    }
}

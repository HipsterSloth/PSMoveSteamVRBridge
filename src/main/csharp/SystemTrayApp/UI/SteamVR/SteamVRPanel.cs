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
    public partial class SteamVRPanel : UserControl, IAppPanel
    {
        public SteamVRPanel()
        {
            InitializeComponent();

            SteamVRContext.Instance.ConnectedToSteamVREvent += OnConnectedToSteamVREvent;
            SteamVRContext.Instance.DisconnectedFromSteamVREvent += OnDisconnectedFromSteamVREvent;
            SteamVRContext.Instance.TrackedDeviceActivatedEvent += OnTrackedDeviceActivated;
            SteamVRContext.Instance.TrackedDeviceDeactivatedEvent += OnTrackedDeviceDeactivated;
        }

        private void OnConnectedToSteamVREvent()
        {
            SynchronizedInvoke.Invoke(this, () => HandleSteamVRConnect());
        }

        private void OnDisconnectedFromSteamVREvent()
        {
            SynchronizedInvoke.Invoke(this, () => HandleSteamVRDisconnect());
        }

        public void OnTrackedDeviceActivated(SteamVRTrackedDevice device)
        {
            SynchronizedInvoke.Invoke(this, () => HandleTrackedDeviceActivated(device));
        }

        public void OnTrackedDeviceDeactivated(SteamVRTrackedDevice device)
        {
            SynchronizedInvoke.Invoke(this, () => HandleTrackedDeviceDeactivated(device));
        }

        public void OnPanelEntered()
        {
            bool bIsSteamVRConnected = SteamVRContext.Instance.IsConnected;

            SteamVRConnectButton.Visible = !bIsSteamVRConnected;
            SteamVRDisconnectButton.Visible = bIsSteamVRConnected;
            HMDAlignButton.Visible = bIsSteamVRConnected;
            TrackingTestToolButton.Visible = bIsSteamVRConnected;
        }

        public void OnPanelExited()
        {
        }

        private void SteamVRConnectButton_Click(object sender, EventArgs e)
        {
            SynchronizedInvoke.Invoke(SteamVRContext.Instance, () => SteamVRContext.Instance.Connect());
        }

        private void SteamVRDisconnectButton_Click(object sender, EventArgs e)
        {
            SynchronizedInvoke.Invoke(SteamVRContext.Instance, () => SteamVRContext.Instance.Disconnect());
        }

        private void HandleSteamVRConnect()
        {
            SteamVRCurrentStatus.Text = "CONNECTED";
            SteamVRConnectButton.Visible = false;
            SteamVRDisconnectButton.Visible = true;
            HMDAlignButton.Visible = true;
            TrackingTestToolButton.Visible = true;
        }

        private void HandleSteamVRDisconnect()
        {
            SteamVRCurrentStatus.Text = "DISCONNECTED";
            SteamVRConnectButton.Visible = true;
            SteamVRDisconnectButton.Visible = false;
            HMDAlignButton.Visible = false;
            TrackingTestToolButton.Visible = false;
        }

        private void HandleTrackedDeviceActivated(SteamVRTrackedDevice device)
        {
            switch (device.DeviceType) {
                case Valve.VR.ETrackedDeviceClass.HMD:
                    //SteamVRHeadMountedDisplay hmd = (SteamVRHeadMountedDisplay)device;
                    //if (hmd.ReadyIcon != null) {
                    //    HMD_icon.Image = hmd.ReadyIcon;
                    //}
                    //HMD_icon.Visible = true;
                    break;
                case Valve.VR.ETrackedDeviceClass.Controller:
                    break;
                case Valve.VR.ETrackedDeviceClass.TrackingReference:
                    break;
            }
        }

        private void HandleTrackedDeviceDeactivated(SteamVRTrackedDevice device)
        {
            switch (device.DeviceType) {
                case Valve.VR.ETrackedDeviceClass.HMD:
                    //HMD_icon.Visible = false;
                    break;
                case Valve.VR.ETrackedDeviceClass.Controller:
                    break;
                case Valve.VR.ETrackedDeviceClass.TrackingReference:
                    break;
            }
        }

        private void HMDAlignButton_Click(object sender, EventArgs e)
        {
            AppWindow.Instance.SetSteamVRPanel(new TestVideoFeed());
        }
    }
}

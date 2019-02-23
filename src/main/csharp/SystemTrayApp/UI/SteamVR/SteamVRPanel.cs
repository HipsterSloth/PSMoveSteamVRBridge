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
            SteamVRCurrentStatus.Text = "CONNECTED";
            SteamVRConnectButton.Visible = false;
            SteamVRDisconnectButton.Visible = true;
            HMDAlignButton.Visible = true;
            TrackingTestToolButton.Visible = true;
        }

        private void OnDisconnectedFromSteamVREvent()
        {
            SteamVRCurrentStatus.Text = "DISCONNECTED";
            SteamVRConnectButton.Visible = true;
            SteamVRDisconnectButton.Visible = false;
            HMDAlignButton.Visible = false;
            TrackingTestToolButton.Visible = false;
        }

        public void OnTrackedDeviceActivated(SteamVRTrackedDevice device)
        {
            switch (device.DeviceType)
            {
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

        public void OnTrackedDeviceDeactivated(SteamVRTrackedDevice device)
        {
            switch (device.DeviceType)
            {
                case Valve.VR.ETrackedDeviceClass.HMD:
                    //HMD_icon.Visible = false;
                    break;
                case Valve.VR.ETrackedDeviceClass.Controller:
                    break;
                case Valve.VR.ETrackedDeviceClass.TrackingReference:
                    break;
            }
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
            SteamVRContext.Instance.Connect();
        }

        private void SteamVRDisconnectButton_Click(object sender, EventArgs e)
        {
            SteamVRContext.Instance.Disconnect();
        }

        private void HMDAlignButton_Click(object sender, EventArgs e)
        {
            AppWindow.Instance.SetSteamVRPanel(new TestVideoFeed());
        }
    }
}

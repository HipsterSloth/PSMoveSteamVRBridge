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
    public partial class SteamVRPanel : UserControl
    {
        private SteamVRWindow steamVRWindow;

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

        public void OnTabEntered()
        {
            if (SteamVRContext.Instance.IsConnected) {
                SteamVRConnectButton.Visible = false;
                SteamVRDisconnectButton.Visible = true;
            }
            else {
                SteamVRConnectButton.Visible = true;
                SteamVRDisconnectButton.Visible = false;
            }
        }

        public void OnTabExited()
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

            if (steamVRWindow == null) {
                steamVRWindow = new SteamVRWindow();
                steamVRWindow.Show();
            }
        }

        private void HandleSteamVRDisconnect()
        {
            SteamVRCurrentStatus.Text = "DISCONNECTED";
            SteamVRConnectButton.Visible = true;
            SteamVRDisconnectButton.Visible = false;

            if (steamVRWindow != null) {
                steamVRWindow.Hide();
                steamVRWindow.Dispose();
                steamVRWindow = null;
            }
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
    }
}

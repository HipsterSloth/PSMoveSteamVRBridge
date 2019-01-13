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
    public partial class HMDPanel : UserControl
    {
        private SteamVRWindow steamVRWindow;

        public HMDPanel()
        {
            InitializeComponent();

            SteamVRConnectButton.Visible = true;
            SteamVRDisconnectButton.Visible = false;
        }

        public void OnTabEntered()
        {
        }

        public void OnTabExited()
        {
        }

        private void SteamVRConnectButton_Click(object sender, EventArgs e)
        {
            SteamVRConnectButton.Visible = false;
            SteamVRDisconnectButton.Visible = true;

            if (steamVRWindow == null) {
                steamVRWindow = new SteamVRWindow();
                steamVRWindow.Show();
            }
        }

        private void SteamVRDisconnectButton_Click(object sender, EventArgs e)
        {
            SteamVRConnectButton.Visible = true;
            SteamVRDisconnectButton.Visible = false;

            if (steamVRWindow != null) {
                steamVRWindow.Hide();
                steamVRWindow.Dispose();
                steamVRWindow = null;
            }
        }

        private void ReloadHmdSettingsButton_Click(object sender, EventArgs e)
        {

        }

        private void SaveHMDSettingsButton_Click(object sender, EventArgs e)
        {

        }
    }
}

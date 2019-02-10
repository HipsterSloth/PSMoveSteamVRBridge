using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin.Controls;
using MaterialSkin;

namespace SystemTrayApp
{
    public partial class AppWindow : MaterialForm
    {
        private static AppWindow _instance;
        public static AppWindow Instance
        {
            get { return _instance; }
        }

        private TabPage currentTabPage;

        private UserControl psmServicePanel;
        private UserControl steamVRPanel;
        private UserControl freePiePanel;

        private readonly MaterialSkinManager materialSkinManager;

        public AppWindow()
        {
            _instance = this;

            InitializeComponent();

            // Add user-control panels to each tab
            psmServicePanel = new PSMoveServicePanel();
            this.psmoveServiceTabPage.Controls.Add(psmServicePanel);
            steamVRPanel = new SteamVRPanel();
            this.steamVRTabPage.Controls.Add(steamVRPanel);
            freePiePanel = new FreePIEPanel();
            this.freePIETabPage.Controls.Add(freePiePanel);

            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            // Center this window
            this.CenterToScreen();

            // To provide your own custom icon image, go to:
            //   1. Project > Properties... > Resources
            //   2. Change the resource filter to icons
            //   3. Remove the Default resource and add your own
            //   4. Modify the next line to Properties.Resources.<YourResource>
            this.Icon = Properties.Resources.Default;
            this.SystemTrayIcon.Icon = Properties.Resources.Default;

            // Change the Text property to the name of your application
            this.SystemTrayIcon.Text = "PSMoveSteamVR Bridge Tray App";
            this.SystemTrayIcon.Visible = true;

            // Modify the right-click menu of your system tray icon here
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Exit", ContextMenuExit);
            this.SystemTrayIcon.ContextMenu = menu;

            this.Resize += WindowResize;
            this.FormClosing += WindowClosing;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Text = "PSMoveSteamVRBridge Config Tool";

            // Load up all of the device configuration files
            ConfigManager.Instance.LoadAll();

            // Go to the PSMoveService tab first
            SetCurrentTab(this.psmoveServiceTabPage);
        }

        protected void SetCurrentTab(TabPage NewTabPage)
        {
            if (NewTabPage != currentTabPage) 
            {
                // Exit the previous tab
                if (currentTabPage == this.psmoveServiceTabPage)
                {
                    ((IAppPanel)psmServicePanel).OnPanelExited();
                }
                else if (currentTabPage == this.steamVRTabPage)
                {
                    ((IAppPanel)steamVRPanel).OnPanelExited();
                }
                else if (currentTabPage == this.freePIETabPage)
                {
                    ((IAppPanel)freePiePanel).OnPanelExited();
                }

                // Enter the new tab
                if (NewTabPage == this.psmoveServiceTabPage) 
                {
                    ((IAppPanel)psmServicePanel).OnPanelEntered();
                }
                else if (NewTabPage == this.steamVRTabPage)
                {
                    ((IAppPanel)steamVRPanel).OnPanelEntered();
                }
                else if (NewTabPage == this.freePIETabPage)
                {
                    ((IAppPanel)freePiePanel).OnPanelEntered();
                }

                currentTabPage = NewTabPage;
            }
        }

        public void SetPSMServicePanel(UserControl newControl)
        {
            if (psmServicePanel != null)
            {
                if (currentTabPage == this.psmoveServiceTabPage)
                    ((IAppPanel)psmServicePanel).OnPanelExited();

                this.psmoveServiceTabPage.Controls.Remove(psmServicePanel);
                psmServicePanel.Dispose();
                psmServicePanel = null;
            }

            if (newControl != null)
            {
                this.psmoveServiceTabPage.Controls.Add(newControl);

                if (currentTabPage == this.psmoveServiceTabPage)
                    ((IAppPanel)newControl).OnPanelEntered();

                psmServicePanel = newControl;
            }
        }

        public void SetSteamVRPanel(UserControl newControl)
        {
            if (steamVRPanel != null)
            {
                if (currentTabPage == this.steamVRTabPage)
                    ((IAppPanel)steamVRPanel).OnPanelExited();

                this.steamVRTabPage.Controls.Remove(steamVRPanel);
                steamVRPanel.Dispose();
                steamVRPanel = null;
            }

            if (newControl != null)
            {
                this.steamVRTabPage.Controls.Add(newControl);

                if (currentTabPage == this.steamVRTabPage)
                    ((IAppPanel)newControl).OnPanelEntered();

                steamVRPanel = newControl;
            }
        }

        public void SetFreePiePanel(UserControl newControl)
        {
            if (freePiePanel != null)
            {
                if (currentTabPage == this.freePIETabPage)
                    ((IAppPanel)freePiePanel).OnPanelExited();

                this.freePIETabPage.Controls.Remove(freePiePanel);
                freePiePanel.Dispose();
                freePiePanel = null;
            }

            if (newControl != null)
            {
                this.freePIETabPage.Controls.Add(newControl);

                if (currentTabPage == this.freePIETabPage)
                    ((IAppPanel)newControl).OnPanelEntered();

                freePiePanel = newControl;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _instance = null;
        }

        private void SystemTrayIconDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ContextMenuExit(object sender, EventArgs e)
        {
            this.SystemTrayIcon.Visible = false;
            Application.Exit();
            Environment.Exit(0);
        }

        private void WindowResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void WindowClosing(object sender, FormClosingEventArgs e)
        {
            SetCurrentTab(null);
            e.Cancel = true;
            this.Hide();
        }

        private void MaterialTabControl1_Selected(object sender, System.Windows.Forms.TabControlEventArgs e)
        {
            SetCurrentTab(e.TabPage);
        }
    }
}

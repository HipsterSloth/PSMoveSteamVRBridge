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
        private TabPage currentTabPage;

        private PSMoveServicePanel psmServicePanel;
        private ControllerPanel controllerPanel;
        private HMDPanel hmdPanel;

        private readonly MaterialSkinManager materialSkinManager;

        public AppWindow()
        {
            InitializeComponent();

            // Add user-control panels to each tab
            psmServicePanel = new PSMoveServicePanel();
            this.tabPage1.Controls.Add(psmServicePanel);
            controllerPanel = new ControllerPanel();
            this.tabPage2.Controls.Add(controllerPanel);
            hmdPanel = new HMDPanel();
            this.tabPage3.Controls.Add(hmdPanel);

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
            SetCurrentTab(this.tabPage1);
        }

        protected void SetCurrentTab(TabPage NewTabPage)
        {
            if (NewTabPage != currentTabPage) 
            {
                // Exit the previous tab
                if (currentTabPage == this.tabPage1)
                {
                    psmServicePanel.OnTabExited();
                }
                else if (currentTabPage == this.tabPage2)
                {
                    controllerPanel.OnTabExited();
                }
                else if (currentTabPage == this.tabPage3)
                {
                    hmdPanel.OnTabExited();
                }

                // Enter the new tab
                if (NewTabPage == this.tabPage1) 
                {
                    psmServicePanel.OnTabEntered();
                }
                else if (NewTabPage == this.tabPage2)
                {
                    controllerPanel.OnTabEntered();
                }
                else if (NewTabPage == this.tabPage3)
                {
                    hmdPanel.OnTabEntered();
                }

                currentTabPage = NewTabPage;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
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

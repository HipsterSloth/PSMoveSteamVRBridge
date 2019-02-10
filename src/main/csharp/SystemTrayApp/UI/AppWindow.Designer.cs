namespace SystemTrayApp
{
    partial class AppWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SystemTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.materialTabSelector1 = new MaterialSkin.Controls.MaterialTabSelector();
            this.materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            this.psmoveServiceTabPage = new System.Windows.Forms.TabPage();
            this.steamVRTabPage = new System.Windows.Forms.TabPage();
            this.freePIETabPage = new System.Windows.Forms.TabPage();
            this.materialTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SystemTrayIcon
            // 
            this.SystemTrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.SystemTrayIcon.Visible = true;
            this.SystemTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.SystemTrayIconDoubleClick);
            // 
            // materialTabSelector1
            // 
            this.materialTabSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialTabSelector1.BaseTabControl = this.materialTabControl1;
            this.materialTabSelector1.Depth = 0;
            this.materialTabSelector1.Location = new System.Drawing.Point(0, 64);
            this.materialTabSelector1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialTabSelector1.Name = "materialTabSelector1";
            this.materialTabSelector1.Size = new System.Drawing.Size(782, 48);
            this.materialTabSelector1.TabIndex = 17;
            this.materialTabSelector1.Text = "materialTabSelector1";
            // 
            // materialTabControl1
            // 
            this.materialTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialTabControl1.Controls.Add(this.psmoveServiceTabPage);
            this.materialTabControl1.Controls.Add(this.steamVRTabPage);
            this.materialTabControl1.Controls.Add(this.freePIETabPage);
            this.materialTabControl1.Depth = 0;
            this.materialTabControl1.Location = new System.Drawing.Point(14, 118);
            this.materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialTabControl1.Name = "materialTabControl1";
            this.materialTabControl1.SelectedIndex = 0;
            this.materialTabControl1.Size = new System.Drawing.Size(755, 441);
            this.materialTabControl1.TabIndex = 6;
            this.materialTabControl1.Selected += MaterialTabControl1_Selected;
            // 
            // tabPage1
            // 
            this.psmoveServiceTabPage.BackColor = System.Drawing.Color.White;
            this.psmoveServiceTabPage.Location = new System.Drawing.Point(4, 22);
            this.psmoveServiceTabPage.Name = "psmoveServiceTabPage";
            this.psmoveServiceTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.psmoveServiceTabPage.Size = new System.Drawing.Size(747, 415);
            this.psmoveServiceTabPage.TabIndex = 0;
            this.psmoveServiceTabPage.Text = "PSMoveService";
            // 
            // tabPage2
            // 
            this.steamVRTabPage.BackColor = System.Drawing.Color.White;
            this.steamVRTabPage.Location = new System.Drawing.Point(4, 22);
            this.steamVRTabPage.Name = "steamVRTabPage";
            this.steamVRTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.steamVRTabPage.Size = new System.Drawing.Size(747, 415);
            this.steamVRTabPage.TabIndex = 0;
            this.steamVRTabPage.Text = "SteamVR";
            // 
            // tabPage3
            // 
            this.freePIETabPage.BackColor = System.Drawing.Color.White;
            this.freePIETabPage.Location = new System.Drawing.Point(4, 22);
            this.freePIETabPage.Name = "freePIETabPage";
            this.freePIETabPage.Padding = new System.Windows.Forms.Padding(3);
            this.freePIETabPage.Size = new System.Drawing.Size(747, 415);
            this.freePIETabPage.TabIndex = 1;
            this.freePIETabPage.Text = "FreePIE";
            this.freePIETabPage.UseVisualStyleBackColor = true;
            // 
            // AppWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(781, 564);
            this.Controls.Add(this.materialTabControl1);
            this.Controls.Add(this.materialTabSelector1);
            this.Name = "AppWindow";
            this.materialTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon SystemTrayIcon;
        private MaterialSkin.Controls.MaterialTabSelector materialTabSelector1;
        private MaterialSkin.Controls.MaterialTabControl materialTabControl1;
        private System.Windows.Forms.TabPage psmoveServiceTabPage;
        private System.Windows.Forms.TabPage steamVRTabPage;
        private System.Windows.Forms.TabPage freePIETabPage;
    }
}


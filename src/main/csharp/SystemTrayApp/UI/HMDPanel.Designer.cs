namespace SystemTrayApp
{
    partial class HMDPanel
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
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ReloadHmdSettingsButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SaveHMDSettingsButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SteamVRDisconnectButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SteamVRConnectButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.VirtualHMDFilterTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VirtualHMDFilterLabel = new MaterialSkin.Controls.MaterialLabel();
            this.SteamVRCurrentStatus = new MaterialSkin.Controls.MaterialLabel();
            this.SteamVRStatusLabel = new MaterialSkin.Controls.MaterialLabel();
            this.HMD_icon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.HMD_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // ReloadHmdSettingsButton
            // 
            this.ReloadHmdSettingsButton.AutoSize = true;
            this.ReloadHmdSettingsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ReloadHmdSettingsButton.Depth = 0;
            this.ReloadHmdSettingsButton.Icon = null;
            this.ReloadHmdSettingsButton.Location = new System.Drawing.Point(607, 372);
            this.ReloadHmdSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.ReloadHmdSettingsButton.Name = "ReloadHmdSettingsButton";
            this.ReloadHmdSettingsButton.Primary = true;
            this.ReloadHmdSettingsButton.Size = new System.Drawing.Size(72, 36);
            this.ReloadHmdSettingsButton.TabIndex = 16;
            this.ReloadHmdSettingsButton.Text = "Reload";
            this.ReloadHmdSettingsButton.UseVisualStyleBackColor = true;
            this.ReloadHmdSettingsButton.Click += new System.EventHandler(this.ReloadHmdSettingsButton_Click);
            // 
            // SaveHMDSettingsButton
            // 
            this.SaveHMDSettingsButton.AutoSize = true;
            this.SaveHMDSettingsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SaveHMDSettingsButton.Depth = 0;
            this.SaveHMDSettingsButton.Icon = null;
            this.SaveHMDSettingsButton.Location = new System.Drawing.Point(685, 372);
            this.SaveHMDSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SaveHMDSettingsButton.Name = "SaveHMDSettingsButton";
            this.SaveHMDSettingsButton.Primary = true;
            this.SaveHMDSettingsButton.Size = new System.Drawing.Size(55, 36);
            this.SaveHMDSettingsButton.TabIndex = 15;
            this.SaveHMDSettingsButton.Text = "Save";
            this.SaveHMDSettingsButton.UseVisualStyleBackColor = true;
            this.SaveHMDSettingsButton.Click += new System.EventHandler(this.SaveHMDSettingsButton_Click);
            // 
            // SteamVRDisconnectButton
            // 
            this.SteamVRDisconnectButton.AutoSize = true;
            this.SteamVRDisconnectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SteamVRDisconnectButton.Depth = 0;
            this.SteamVRDisconnectButton.Icon = null;
            this.SteamVRDisconnectButton.Location = new System.Drawing.Point(163, 6);
            this.SteamVRDisconnectButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRDisconnectButton.Name = "SteamVRDisconnectButton";
            this.SteamVRDisconnectButton.Primary = true;
            this.SteamVRDisconnectButton.Size = new System.Drawing.Size(172, 36);
            this.SteamVRDisconnectButton.TabIndex = 14;
            this.SteamVRDisconnectButton.Text = "SteamVR Disconnect";
            this.SteamVRDisconnectButton.UseVisualStyleBackColor = true;
            this.SteamVRDisconnectButton.Click += new System.EventHandler(this.SteamVRDisconnectButton_Click);
            // 
            // SteamVRConnectButton
            // 
            this.SteamVRConnectButton.AutoSize = true;
            this.SteamVRConnectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SteamVRConnectButton.Depth = 0;
            this.SteamVRConnectButton.Icon = null;
            this.SteamVRConnectButton.Location = new System.Drawing.Point(6, 6);
            this.SteamVRConnectButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRConnectButton.Name = "SteamVRConnectButton";
            this.SteamVRConnectButton.Primary = true;
            this.SteamVRConnectButton.Size = new System.Drawing.Size(150, 36);
            this.SteamVRConnectButton.TabIndex = 13;
            this.SteamVRConnectButton.Text = "SteamVR Connect";
            this.SteamVRConnectButton.UseVisualStyleBackColor = true;
            this.SteamVRConnectButton.Click += new System.EventHandler(this.SteamVRConnectButton_Click);
            // 
            // VirtualHMDFilterTextField
            // 
            this.VirtualHMDFilterTextField.Depth = 0;
            this.VirtualHMDFilterTextField.Hint = "";
            this.VirtualHMDFilterTextField.Location = new System.Drawing.Point(153, 75);
            this.VirtualHMDFilterTextField.MaxLength = 32767;
            this.VirtualHMDFilterTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VirtualHMDFilterTextField.Name = "VirtualHMDFilterTextField";
            this.VirtualHMDFilterTextField.PasswordChar = '\0';
            this.VirtualHMDFilterTextField.SelectedText = "";
            this.VirtualHMDFilterTextField.SelectionLength = 0;
            this.VirtualHMDFilterTextField.SelectionStart = 0;
            this.VirtualHMDFilterTextField.Size = new System.Drawing.Size(179, 23);
            this.VirtualHMDFilterTextField.TabIndex = 12;
            this.VirtualHMDFilterTextField.TabStop = false;
            this.VirtualHMDFilterTextField.Text = "00:00:00:00:00:00";
            this.VirtualHMDFilterTextField.UseSystemPasswordChar = false;
            // 
            // VirtualHMDFilterLabel
            // 
            this.VirtualHMDFilterLabel.AutoSize = true;
            this.VirtualHMDFilterLabel.Depth = 0;
            this.VirtualHMDFilterLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.VirtualHMDFilterLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VirtualHMDFilterLabel.Location = new System.Drawing.Point(18, 75);
            this.VirtualHMDFilterLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VirtualHMDFilterLabel.Name = "VirtualHMDFilterLabel";
            this.VirtualHMDFilterLabel.Size = new System.Drawing.Size(129, 19);
            this.VirtualHMDFilterLabel.TabIndex = 11;
            this.VirtualHMDFilterLabel.Text = "Virtual HMD Filter";
            // 
            // SteamVRCurrentStatus
            // 
            this.SteamVRCurrentStatus.AutoSize = true;
            this.SteamVRCurrentStatus.Depth = 0;
            this.SteamVRCurrentStatus.Font = new System.Drawing.Font("Roboto", 11F);
            this.SteamVRCurrentStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SteamVRCurrentStatus.Location = new System.Drawing.Point(149, 45);
            this.SteamVRCurrentStatus.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRCurrentStatus.Name = "SteamVRCurrentStatus";
            this.SteamVRCurrentStatus.Size = new System.Drawing.Size(121, 19);
            this.SteamVRCurrentStatus.TabIndex = 21;
            this.SteamVRCurrentStatus.Text = "DISCONNECTED";
            // 
            // SteamVRStatusLabel
            // 
            this.SteamVRStatusLabel.AutoSize = true;
            this.SteamVRStatusLabel.Depth = 0;
            this.SteamVRStatusLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.SteamVRStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SteamVRStatusLabel.Location = new System.Drawing.Point(18, 45);
            this.SteamVRStatusLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRStatusLabel.Name = "SteamVRStatusLabel";
            this.SteamVRStatusLabel.Size = new System.Drawing.Size(75, 19);
            this.SteamVRStatusLabel.TabIndex = 20;
            this.SteamVRStatusLabel.Text = "Steam VR";
            // 
            // HMD_icon
            // 
            this.HMD_icon.Location = new System.Drawing.Point(22, 149);
            this.HMD_icon.Name = "HMD_icon";
            this.HMD_icon.Size = new System.Drawing.Size(64, 64);
            this.HMD_icon.TabIndex = 22;
            this.HMD_icon.TabStop = false;
            // 
            // HMDPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HMD_icon);
            this.Controls.Add(this.SteamVRCurrentStatus);
            this.Controls.Add(this.SteamVRStatusLabel);
            this.Controls.Add(this.ReloadHmdSettingsButton);
            this.Controls.Add(this.SaveHMDSettingsButton);
            this.Controls.Add(this.SteamVRDisconnectButton);
            this.Controls.Add(this.SteamVRConnectButton);
            this.Controls.Add(this.VirtualHMDFilterTextField);
            this.Controls.Add(this.VirtualHMDFilterLabel);
            this.Name = "HMDPanel";
            this.Size = new System.Drawing.Size(747, 415);
            ((System.ComponentModel.ISupportInitialize)(this.HMD_icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialRaisedButton ReloadHmdSettingsButton;
        private MaterialSkin.Controls.MaterialRaisedButton SaveHMDSettingsButton;
        private MaterialSkin.Controls.MaterialRaisedButton SteamVRDisconnectButton;
        private MaterialSkin.Controls.MaterialRaisedButton SteamVRConnectButton;
        private MaterialSkin.Controls.MaterialSingleLineTextField VirtualHMDFilterTextField;
        private MaterialSkin.Controls.MaterialLabel VirtualHMDFilterLabel;
        private MaterialSkin.Controls.MaterialLabel SteamVRCurrentStatus;
        private MaterialSkin.Controls.MaterialLabel SteamVRStatusLabel;
        private System.Windows.Forms.PictureBox HMD_icon;
    }
}

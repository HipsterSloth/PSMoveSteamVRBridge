namespace SystemTrayApp
{
    partial class SteamVRPanel
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
            this.SteamVRCurrentStatus = new MaterialSkin.Controls.MaterialLabel();
            this.SteamVRStatusLabel = new MaterialSkin.Controls.MaterialLabel();
            this.SteamVRDisconnectButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SteamVRConnectButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SteamVRDevicesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SteamVRDevicesLabel = new MaterialSkin.Controls.MaterialLabel();
            this.HMDAlignButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.TrackingTestToolButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SuspendLayout();
            // 
            // SteamVRCurrentStatus
            // 
            this.SteamVRCurrentStatus.AutoSize = true;
            this.SteamVRCurrentStatus.Depth = 0;
            this.SteamVRCurrentStatus.Font = new System.Drawing.Font("Roboto", 11F);
            this.SteamVRCurrentStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SteamVRCurrentStatus.Location = new System.Drawing.Point(146, 42);
            this.SteamVRCurrentStatus.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRCurrentStatus.Name = "SteamVRCurrentStatus";
            this.SteamVRCurrentStatus.Size = new System.Drawing.Size(121, 19);
            this.SteamVRCurrentStatus.TabIndex = 26;
            this.SteamVRCurrentStatus.Text = "DISCONNECTED";
            // 
            // SteamVRStatusLabel
            // 
            this.SteamVRStatusLabel.AutoSize = true;
            this.SteamVRStatusLabel.Depth = 0;
            this.SteamVRStatusLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.SteamVRStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SteamVRStatusLabel.Location = new System.Drawing.Point(15, 42);
            this.SteamVRStatusLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRStatusLabel.Name = "SteamVRStatusLabel";
            this.SteamVRStatusLabel.Size = new System.Drawing.Size(75, 19);
            this.SteamVRStatusLabel.TabIndex = 25;
            this.SteamVRStatusLabel.Text = "Steam VR";
            // 
            // SteamVRDisconnectButton
            // 
            this.SteamVRDisconnectButton.AutoSize = true;
            this.SteamVRDisconnectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SteamVRDisconnectButton.Depth = 0;
            this.SteamVRDisconnectButton.Icon = null;
            this.SteamVRDisconnectButton.Location = new System.Drawing.Point(160, 3);
            this.SteamVRDisconnectButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRDisconnectButton.Name = "SteamVRDisconnectButton";
            this.SteamVRDisconnectButton.Primary = true;
            this.SteamVRDisconnectButton.Size = new System.Drawing.Size(172, 36);
            this.SteamVRDisconnectButton.TabIndex = 24;
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
            this.SteamVRConnectButton.Location = new System.Drawing.Point(3, 3);
            this.SteamVRConnectButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRConnectButton.Name = "SteamVRConnectButton";
            this.SteamVRConnectButton.Primary = true;
            this.SteamVRConnectButton.Size = new System.Drawing.Size(150, 36);
            this.SteamVRConnectButton.TabIndex = 23;
            this.SteamVRConnectButton.Text = "SteamVR Connect";
            this.SteamVRConnectButton.UseVisualStyleBackColor = true;
            this.SteamVRConnectButton.Click += new System.EventHandler(this.SteamVRConnectButton_Click);
            // 
            // SteamVRDevicesPanel
            // 
            this.SteamVRDevicesPanel.AutoScroll = true;
            this.SteamVRDevicesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SteamVRDevicesPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SteamVRDevicesPanel.Location = new System.Drawing.Point(393, 72);
            this.SteamVRDevicesPanel.Name = "SteamVRDevicesPanel";
            this.SteamVRDevicesPanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.SteamVRDevicesPanel.Size = new System.Drawing.Size(302, 292);
            this.SteamVRDevicesPanel.TabIndex = 68;
            this.SteamVRDevicesPanel.WrapContents = false;
            // 
            // SteamVRDevicesLabel
            // 
            this.SteamVRDevicesLabel.AutoSize = true;
            this.SteamVRDevicesLabel.Depth = 0;
            this.SteamVRDevicesLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.SteamVRDevicesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SteamVRDevicesLabel.Location = new System.Drawing.Point(389, 42);
            this.SteamVRDevicesLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.SteamVRDevicesLabel.Name = "SteamVRDevicesLabel";
            this.SteamVRDevicesLabel.Size = new System.Drawing.Size(128, 19);
            this.SteamVRDevicesLabel.TabIndex = 67;
            this.SteamVRDevicesLabel.Text = "SteamVR Devices";
            // 
            // HMDAlignButton
            // 
            this.HMDAlignButton.AutoSize = true;
            this.HMDAlignButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HMDAlignButton.Depth = 0;
            this.HMDAlignButton.Icon = null;
            this.HMDAlignButton.Location = new System.Drawing.Point(81, 123);
            this.HMDAlignButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.HMDAlignButton.Name = "HMDAlignButton";
            this.HMDAlignButton.Primary = true;
            this.HMDAlignButton.Size = new System.Drawing.Size(171, 36);
            this.HMDAlignButton.TabIndex = 69;
            this.HMDAlignButton.Text = "HMD Alignment Tool";
            this.HMDAlignButton.UseVisualStyleBackColor = true;
            // 
            // TrackingTestToolButton
            // 
            this.TrackingTestToolButton.AutoSize = true;
            this.TrackingTestToolButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TrackingTestToolButton.Depth = 0;
            this.TrackingTestToolButton.Icon = null;
            this.TrackingTestToolButton.Location = new System.Drawing.Point(81, 165);
            this.TrackingTestToolButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.TrackingTestToolButton.Name = "TrackingTestToolButton";
            this.TrackingTestToolButton.Primary = true;
            this.TrackingTestToolButton.Size = new System.Drawing.Size(124, 36);
            this.TrackingTestToolButton.TabIndex = 70;
            this.TrackingTestToolButton.Text = "Tracking Test";
            this.TrackingTestToolButton.UseVisualStyleBackColor = true;
            // 
            // SteamVRPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TrackingTestToolButton);
            this.Controls.Add(this.HMDAlignButton);
            this.Controls.Add(this.SteamVRDevicesPanel);
            this.Controls.Add(this.SteamVRDevicesLabel);
            this.Controls.Add(this.SteamVRCurrentStatus);
            this.Controls.Add(this.SteamVRStatusLabel);
            this.Controls.Add(this.SteamVRDisconnectButton);
            this.Controls.Add(this.SteamVRConnectButton);
            this.Name = "SteamVRPanel";
            this.Size = new System.Drawing.Size(747, 415);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MaterialSkin.Controls.MaterialLabel SteamVRCurrentStatus;
        private MaterialSkin.Controls.MaterialLabel SteamVRStatusLabel;
        private MaterialSkin.Controls.MaterialRaisedButton SteamVRDisconnectButton;
        private MaterialSkin.Controls.MaterialRaisedButton SteamVRConnectButton;
        private System.Windows.Forms.FlowLayoutPanel SteamVRDevicesPanel;
        private MaterialSkin.Controls.MaterialLabel SteamVRDevicesLabel;
        private MaterialSkin.Controls.MaterialRaisedButton HMDAlignButton;
        private MaterialSkin.Controls.MaterialRaisedButton TrackingTestToolButton;
    }
}

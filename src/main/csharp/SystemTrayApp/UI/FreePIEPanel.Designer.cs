namespace SystemTrayApp
{
    partial class FreePIEPanel
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
            this.FreePIEDisconnectBtn = new MaterialSkin.Controls.MaterialRaisedButton();
            this.FreePIEConnectBtn = new MaterialSkin.Controls.MaterialRaisedButton();
            this.FreePIECurrentStatus = new MaterialSkin.Controls.MaterialLabel();
            this.FreePIEStatusLabel = new MaterialSkin.Controls.MaterialLabel();
            this.BindingsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AddControllerBindingButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.AddHMDBindingButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SuspendLayout();
            // 
            // FreePIEDisconnectBtn
            // 
            this.FreePIEDisconnectBtn.AutoSize = true;
            this.FreePIEDisconnectBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FreePIEDisconnectBtn.Depth = 0;
            this.FreePIEDisconnectBtn.Icon = null;
            this.FreePIEDisconnectBtn.Location = new System.Drawing.Point(148, 3);
            this.FreePIEDisconnectBtn.MouseState = MaterialSkin.MouseState.HOVER;
            this.FreePIEDisconnectBtn.Name = "FreePIEDisconnectBtn";
            this.FreePIEDisconnectBtn.Primary = true;
            this.FreePIEDisconnectBtn.Size = new System.Drawing.Size(161, 36);
            this.FreePIEDisconnectBtn.TabIndex = 25;
            this.FreePIEDisconnectBtn.Text = "FreePIE Disconnect";
            this.FreePIEDisconnectBtn.UseVisualStyleBackColor = true;
            this.FreePIEDisconnectBtn.Click += new System.EventHandler(this.FreePIEDisconnectBtn_Click);
            // 
            // FreePIEConnectBtn
            // 
            this.FreePIEConnectBtn.AutoSize = true;
            this.FreePIEConnectBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FreePIEConnectBtn.Depth = 0;
            this.FreePIEConnectBtn.Icon = null;
            this.FreePIEConnectBtn.Location = new System.Drawing.Point(3, 3);
            this.FreePIEConnectBtn.MouseState = MaterialSkin.MouseState.HOVER;
            this.FreePIEConnectBtn.Name = "FreePIEConnectBtn";
            this.FreePIEConnectBtn.Primary = true;
            this.FreePIEConnectBtn.Size = new System.Drawing.Size(139, 36);
            this.FreePIEConnectBtn.TabIndex = 24;
            this.FreePIEConnectBtn.Text = "FreePIE Connect";
            this.FreePIEConnectBtn.UseVisualStyleBackColor = true;
            this.FreePIEConnectBtn.Click += new System.EventHandler(this.FreePIEConnectBtn_Click);
            // 
            // FreePIECurrentStatus
            // 
            this.FreePIECurrentStatus.AutoSize = true;
            this.FreePIECurrentStatus.Depth = 0;
            this.FreePIECurrentStatus.Font = new System.Drawing.Font("Roboto", 11F);
            this.FreePIECurrentStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FreePIECurrentStatus.Location = new System.Drawing.Point(126, 46);
            this.FreePIECurrentStatus.MouseState = MaterialSkin.MouseState.HOVER;
            this.FreePIECurrentStatus.Name = "FreePIECurrentStatus";
            this.FreePIECurrentStatus.Size = new System.Drawing.Size(121, 19);
            this.FreePIECurrentStatus.TabIndex = 23;
            this.FreePIECurrentStatus.Text = "DISCONNECTED";
            // 
            // FreePIEStatusLabel
            // 
            this.FreePIEStatusLabel.AutoSize = true;
            this.FreePIEStatusLabel.Depth = 0;
            this.FreePIEStatusLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.FreePIEStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FreePIEStatusLabel.Location = new System.Drawing.Point(14, 46);
            this.FreePIEStatusLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.FreePIEStatusLabel.Name = "FreePIEStatusLabel";
            this.FreePIEStatusLabel.Size = new System.Drawing.Size(60, 19);
            this.FreePIEStatusLabel.TabIndex = 22;
            this.FreePIEStatusLabel.Text = "FreePIE";
            // 
            // BindingsLayoutPanel
            // 
            this.BindingsLayoutPanel.Location = new System.Drawing.Point(3, 69);
            this.BindingsLayoutPanel.Name = "BindingsLayoutPanel";
            this.BindingsLayoutPanel.Size = new System.Drawing.Size(741, 343);
            this.BindingsLayoutPanel.TabIndex = 26;
            // 
            // AddControllerBindingButton
            // 
            this.AddControllerBindingButton.AutoSize = true;
            this.AddControllerBindingButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddControllerBindingButton.Depth = 0;
            this.AddControllerBindingButton.Icon = null;
            this.AddControllerBindingButton.Location = new System.Drawing.Point(18, 366);
            this.AddControllerBindingButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.AddControllerBindingButton.Name = "AddControllerBindingButton";
            this.AddControllerBindingButton.Primary = true;
            this.AddControllerBindingButton.Size = new System.Drawing.Size(195, 36);
            this.AddControllerBindingButton.TabIndex = 27;
            this.AddControllerBindingButton.Text = "Add Controller Binding";
            this.AddControllerBindingButton.UseVisualStyleBackColor = true;
            this.AddControllerBindingButton.Click += new System.EventHandler(this.AddControllerBindingButton_Click);
            // 
            // AddHMDBindingButton
            // 
            this.AddHMDBindingButton.AutoSize = true;
            this.AddHMDBindingButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddHMDBindingButton.Depth = 0;
            this.AddHMDBindingButton.Icon = null;
            this.AddHMDBindingButton.Location = new System.Drawing.Point(219, 366);
            this.AddHMDBindingButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.AddHMDBindingButton.Name = "AddHMDBindingButton";
            this.AddHMDBindingButton.Primary = true;
            this.AddHMDBindingButton.Size = new System.Drawing.Size(140, 36);
            this.AddHMDBindingButton.TabIndex = 28;
            this.AddHMDBindingButton.Text = "Add HMD Binding";
            this.AddHMDBindingButton.UseVisualStyleBackColor = true;
            this.AddHMDBindingButton.Click += new System.EventHandler(this.AddHMDBindingButton_Click);
            // 
            // FreePIEPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AddHMDBindingButton);
            this.Controls.Add(this.AddControllerBindingButton);
            this.Controls.Add(this.FreePIEDisconnectBtn);
            this.Controls.Add(this.FreePIEConnectBtn);
            this.Controls.Add(this.FreePIECurrentStatus);
            this.Controls.Add(this.FreePIEStatusLabel);
            this.Controls.Add(this.BindingsLayoutPanel);
            this.Name = "FreePIEPanel";
            this.Size = new System.Drawing.Size(747, 415);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialRaisedButton FreePIEDisconnectBtn;
        private MaterialSkin.Controls.MaterialRaisedButton FreePIEConnectBtn;
        private MaterialSkin.Controls.MaterialLabel FreePIECurrentStatus;
        private MaterialSkin.Controls.MaterialLabel FreePIEStatusLabel;
        private System.Windows.Forms.FlowLayoutPanel BindingsLayoutPanel;
        private MaterialSkin.Controls.MaterialRaisedButton AddControllerBindingButton;
        private MaterialSkin.Controls.MaterialRaisedButton AddHMDBindingButton;
    }
}

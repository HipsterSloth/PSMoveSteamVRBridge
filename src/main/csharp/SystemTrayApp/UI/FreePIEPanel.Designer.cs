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
            this.TriggerAxisLabel = new MaterialSkin.Controls.MaterialLabel();
            this.TriggerAxisComboBox = new System.Windows.Forms.ComboBox();
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
            this.FreePIECurrentStatus.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.FreePIECurrentStatus.Depth = 0;
            this.FreePIECurrentStatus.Font = new System.Drawing.Font("Roboto", 11F);
            this.FreePIECurrentStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.FreePIECurrentStatus.Location = new System.Drawing.Point(80, 47);
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
            this.BindingsLayoutPanel.AutoScroll = true;
            this.BindingsLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BindingsLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.BindingsLayoutPanel.Location = new System.Drawing.Point(3, 69);
            this.BindingsLayoutPanel.Name = "BindingsLayoutPanel";
            this.BindingsLayoutPanel.Size = new System.Drawing.Size(741, 291);
            this.BindingsLayoutPanel.TabIndex = 26;
            this.BindingsLayoutPanel.WrapContents = false;
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
            // TriggerAxisLabel
            // 
            this.TriggerAxisLabel.AutoSize = true;
            this.TriggerAxisLabel.Depth = 0;
            this.TriggerAxisLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.TriggerAxisLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TriggerAxisLabel.Location = new System.Drawing.Point(403, 378);
            this.TriggerAxisLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.TriggerAxisLabel.Name = "TriggerAxisLabel";
            this.TriggerAxisLabel.Size = new System.Drawing.Size(208, 19);
            this.TriggerAxisLabel.TabIndex = 29;
            this.TriggerAxisLabel.Text = "Virtual Controller Trigger Axis";
            // 
            // TriggerAxisComboBox
            // 
            this.TriggerAxisComboBox.FormattingEnabled = true;
            this.TriggerAxisComboBox.Items.AddRange(new object[] {
            "NONE",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31"});
            this.TriggerAxisComboBox.Location = new System.Drawing.Point(618, 375);
            this.TriggerAxisComboBox.Name = "TriggerAxisComboBox";
            this.TriggerAxisComboBox.Size = new System.Drawing.Size(54, 21);
            this.TriggerAxisComboBox.TabIndex = 30;
            // 
            // FreePIEPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TriggerAxisComboBox);
            this.Controls.Add(this.TriggerAxisLabel);
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
        private MaterialSkin.Controls.MaterialLabel TriggerAxisLabel;
        private System.Windows.Forms.ComboBox TriggerAxisComboBox;
    }
}

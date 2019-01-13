namespace SystemTrayApp
{
    partial class PSMoveServicePanel
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
            this.StopPSMButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.StartPSMButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.PSMCurrentStatus = new MaterialSkin.Controls.MaterialLabel();
            this.PSMStatusLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ReloadPSMSettingsButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SavePSMSettingsButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.AutoLaunchPSMCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.ServerPortTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.serverPortLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ServerAddressTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.serverAddressLabel = new MaterialSkin.Controls.MaterialLabel();
            this.SuspendLayout();
            // 
            // StopPSMButton
            // 
            this.StopPSMButton.AutoSize = true;
            this.StopPSMButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.StopPSMButton.Depth = 0;
            this.StopPSMButton.Icon = null;
            this.StopPSMButton.Location = new System.Drawing.Point(188, 6);
            this.StopPSMButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.StopPSMButton.Name = "StopPSMButton";
            this.StopPSMButton.Primary = true;
            this.StopPSMButton.Size = new System.Drawing.Size(169, 36);
            this.StopPSMButton.TabIndex = 21;
            this.StopPSMButton.Text = "Stop PSMoveService";
            this.StopPSMButton.UseVisualStyleBackColor = true;
            this.StopPSMButton.Click += new System.EventHandler(this.StopPSMButton_Click);
            // 
            // StartPSMButton
            // 
            this.StartPSMButton.AutoSize = true;
            this.StartPSMButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.StartPSMButton.Depth = 0;
            this.StartPSMButton.Icon = null;
            this.StartPSMButton.Location = new System.Drawing.Point(6, 6);
            this.StartPSMButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.StartPSMButton.Name = "StartPSMButton";
            this.StartPSMButton.Primary = true;
            this.StartPSMButton.Size = new System.Drawing.Size(176, 36);
            this.StartPSMButton.TabIndex = 20;
            this.StartPSMButton.Text = "Start PSMoveService";
            this.StartPSMButton.UseVisualStyleBackColor = true;
            this.StartPSMButton.Click += new System.EventHandler(this.StartPSMButton_Click);
            // 
            // PSMCurrentStatus
            // 
            this.PSMCurrentStatus.AutoSize = true;
            this.PSMCurrentStatus.Depth = 0;
            this.PSMCurrentStatus.Font = new System.Drawing.Font("Roboto", 11F);
            this.PSMCurrentStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PSMCurrentStatus.Location = new System.Drawing.Point(129, 49);
            this.PSMCurrentStatus.MouseState = MaterialSkin.MouseState.HOVER;
            this.PSMCurrentStatus.Name = "PSMCurrentStatus";
            this.PSMCurrentStatus.Size = new System.Drawing.Size(121, 19);
            this.PSMCurrentStatus.TabIndex = 19;
            this.PSMCurrentStatus.Text = "DISCONNECTED";
            // 
            // PSMStatusLabel
            // 
            this.PSMStatusLabel.AutoSize = true;
            this.PSMStatusLabel.Depth = 0;
            this.PSMStatusLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.PSMStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PSMStatusLabel.Location = new System.Drawing.Point(17, 49);
            this.PSMStatusLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.PSMStatusLabel.Name = "PSMStatusLabel";
            this.PSMStatusLabel.Size = new System.Drawing.Size(113, 19);
            this.PSMStatusLabel.TabIndex = 18;
            this.PSMStatusLabel.Text = "PSMoveService";
            // 
            // ReloadPSMSettingsButton
            // 
            this.ReloadPSMSettingsButton.AutoSize = true;
            this.ReloadPSMSettingsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ReloadPSMSettingsButton.Depth = 0;
            this.ReloadPSMSettingsButton.Icon = null;
            this.ReloadPSMSettingsButton.Location = new System.Drawing.Point(608, 373);
            this.ReloadPSMSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.ReloadPSMSettingsButton.Name = "ReloadPSMSettingsButton";
            this.ReloadPSMSettingsButton.Primary = true;
            this.ReloadPSMSettingsButton.Size = new System.Drawing.Size(72, 36);
            this.ReloadPSMSettingsButton.TabIndex = 17;
            this.ReloadPSMSettingsButton.Text = "Reload";
            this.ReloadPSMSettingsButton.UseVisualStyleBackColor = true;
            this.ReloadPSMSettingsButton.Click += new System.EventHandler(this.ReloadPSMSettingsButton_Click);
            // 
            // SavePSMSettingsButton
            // 
            this.SavePSMSettingsButton.AutoSize = true;
            this.SavePSMSettingsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SavePSMSettingsButton.Depth = 0;
            this.SavePSMSettingsButton.Icon = null;
            this.SavePSMSettingsButton.Location = new System.Drawing.Point(686, 373);
            this.SavePSMSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SavePSMSettingsButton.Name = "SavePSMSettingsButton";
            this.SavePSMSettingsButton.Primary = true;
            this.SavePSMSettingsButton.Size = new System.Drawing.Size(55, 36);
            this.SavePSMSettingsButton.TabIndex = 16;
            this.SavePSMSettingsButton.Text = "Save";
            this.SavePSMSettingsButton.UseVisualStyleBackColor = true;
            this.SavePSMSettingsButton.Click += new System.EventHandler(this.SavePSMSettingsButton_Click);
            // 
            // AutoLaunchPSMCheckBox
            // 
            this.AutoLaunchPSMCheckBox.AutoSize = true;
            this.AutoLaunchPSMCheckBox.Depth = 0;
            this.AutoLaunchPSMCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.AutoLaunchPSMCheckBox.Location = new System.Drawing.Point(21, 135);
            this.AutoLaunchPSMCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.AutoLaunchPSMCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.AutoLaunchPSMCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.AutoLaunchPSMCheckBox.Name = "AutoLaunchPSMCheckBox";
            this.AutoLaunchPSMCheckBox.Ripple = true;
            this.AutoLaunchPSMCheckBox.Size = new System.Drawing.Size(335, 30);
            this.AutoLaunchPSMCheckBox.TabIndex = 15;
            this.AutoLaunchPSMCheckBox.Text = "Auto Launch PSMoveService on SteamVR Launch";
            this.AutoLaunchPSMCheckBox.UseVisualStyleBackColor = true;
            // 
            // ServerPortTextField
            // 
            this.ServerPortTextField.BackColor = System.Drawing.Color.Silver;
            this.ServerPortTextField.Depth = 0;
            this.ServerPortTextField.Hint = "";
            this.ServerPortTextField.Location = new System.Drawing.Point(133, 109);
            this.ServerPortTextField.MaxLength = 256;
            this.ServerPortTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ServerPortTextField.Name = "ServerPortTextField";
            this.ServerPortTextField.PasswordChar = '\0';
            this.ServerPortTextField.SelectedText = "";
            this.ServerPortTextField.SelectionLength = 0;
            this.ServerPortTextField.SelectionStart = 0;
            this.ServerPortTextField.Size = new System.Drawing.Size(178, 23);
            this.ServerPortTextField.TabIndex = 14;
            this.ServerPortTextField.TabStop = false;
            this.ServerPortTextField.Text = "9512";
            this.ServerPortTextField.UseSystemPasswordChar = false;
            // 
            // serverPortLabel
            // 
            this.serverPortLabel.AutoSize = true;
            this.serverPortLabel.Depth = 0;
            this.serverPortLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.serverPortLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.serverPortLabel.Location = new System.Drawing.Point(17, 109);
            this.serverPortLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.serverPortLabel.Name = "serverPortLabel";
            this.serverPortLabel.Size = new System.Drawing.Size(83, 19);
            this.serverPortLabel.TabIndex = 13;
            this.serverPortLabel.Text = "Server Port";
            // 
            // ServerAddressTextField
            // 
            this.ServerAddressTextField.BackColor = System.Drawing.Color.Silver;
            this.ServerAddressTextField.Depth = 0;
            this.ServerAddressTextField.Hint = "";
            this.ServerAddressTextField.Location = new System.Drawing.Point(133, 80);
            this.ServerAddressTextField.MaxLength = 256;
            this.ServerAddressTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ServerAddressTextField.Name = "ServerAddressTextField";
            this.ServerAddressTextField.PasswordChar = '\0';
            this.ServerAddressTextField.SelectedText = "";
            this.ServerAddressTextField.SelectionLength = 0;
            this.ServerAddressTextField.SelectionStart = 0;
            this.ServerAddressTextField.Size = new System.Drawing.Size(178, 23);
            this.ServerAddressTextField.TabIndex = 12;
            this.ServerAddressTextField.TabStop = false;
            this.ServerAddressTextField.Text = "localhost";
            this.ServerAddressTextField.UseSystemPasswordChar = false;
            // 
            // serverAddressLabel
            // 
            this.serverAddressLabel.AutoSize = true;
            this.serverAddressLabel.Depth = 0;
            this.serverAddressLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.serverAddressLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.serverAddressLabel.Location = new System.Drawing.Point(17, 80);
            this.serverAddressLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.serverAddressLabel.Name = "serverAddressLabel";
            this.serverAddressLabel.Size = new System.Drawing.Size(110, 19);
            this.serverAddressLabel.TabIndex = 11;
            this.serverAddressLabel.Text = "Server Address";
            // 
            // PSMoveServicePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StopPSMButton);
            this.Controls.Add(this.StartPSMButton);
            this.Controls.Add(this.PSMCurrentStatus);
            this.Controls.Add(this.PSMStatusLabel);
            this.Controls.Add(this.ReloadPSMSettingsButton);
            this.Controls.Add(this.SavePSMSettingsButton);
            this.Controls.Add(this.AutoLaunchPSMCheckBox);
            this.Controls.Add(this.ServerPortTextField);
            this.Controls.Add(this.serverPortLabel);
            this.Controls.Add(this.ServerAddressTextField);
            this.Controls.Add(this.serverAddressLabel);
            this.Name = "PSMoveServicePanel";
            this.Size = new System.Drawing.Size(747, 415);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialRaisedButton StopPSMButton;
        private MaterialSkin.Controls.MaterialRaisedButton StartPSMButton;
        private MaterialSkin.Controls.MaterialLabel PSMCurrentStatus;
        private MaterialSkin.Controls.MaterialLabel PSMStatusLabel;
        private MaterialSkin.Controls.MaterialRaisedButton ReloadPSMSettingsButton;
        private MaterialSkin.Controls.MaterialRaisedButton SavePSMSettingsButton;
        private MaterialSkin.Controls.MaterialCheckBox AutoLaunchPSMCheckBox;
        private MaterialSkin.Controls.MaterialSingleLineTextField ServerPortTextField;
        private MaterialSkin.Controls.MaterialLabel serverPortLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField ServerAddressTextField;
        private MaterialSkin.Controls.MaterialLabel serverAddressLabel;
    }
}

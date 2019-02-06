namespace SystemTrayApp
{
    partial class Dualshock4ControllerPanel
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
            this.TouchpadMappingLabel = new MaterialSkin.Controls.MaterialLabel();
            this.VelocityExponentTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VelocityExponentLabel = new MaterialSkin.Controls.MaterialLabel();
            this.VelocityMultiplierTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VelocityMultiplierLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ThumbstickDeadzoneTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ThumbsticklDeadzoneLabel = new MaterialSkin.Controls.MaterialLabel();
            this.UseOrientationInHMDAlignmentCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.DisableAlignmentGestureCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.ZRotate90CheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.ExtendZTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ExtendZLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ExtendYTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ExtendYLabel = new MaterialSkin.Controls.MaterialLabel();
            this.DS4ControllerLabel = new MaterialSkin.Controls.MaterialLabel();
            this.RumbleSuppressedCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.TouchpadMappingsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.AddNewMappingButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SuspendLayout();
            // 
            // TouchpadMappingLabel
            // 
            this.TouchpadMappingLabel.AutoSize = true;
            this.TouchpadMappingLabel.Depth = 0;
            this.TouchpadMappingLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.TouchpadMappingLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TouchpadMappingLabel.Location = new System.Drawing.Point(429, 43);
            this.TouchpadMappingLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.TouchpadMappingLabel.Name = "TouchpadMappingLabel";
            this.TouchpadMappingLabel.Size = new System.Drawing.Size(258, 19);
            this.TouchpadMappingLabel.TabIndex = 65;
            this.TouchpadMappingLabel.Text = "Button to Virtual Touchpad Mappings";
            // 
            // VelocityExponentTextField
            // 
            this.VelocityExponentTextField.Depth = 0;
            this.VelocityExponentTextField.Hint = "";
            this.VelocityExponentTextField.Location = new System.Drawing.Point(266, 244);
            this.VelocityExponentTextField.MaxLength = 32767;
            this.VelocityExponentTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityExponentTextField.Name = "VelocityExponentTextField";
            this.VelocityExponentTextField.PasswordChar = '\0';
            this.VelocityExponentTextField.SelectedText = "";
            this.VelocityExponentTextField.SelectionLength = 0;
            this.VelocityExponentTextField.SelectionStart = 0;
            this.VelocityExponentTextField.Size = new System.Drawing.Size(75, 23);
            this.VelocityExponentTextField.TabIndex = 48;
            this.VelocityExponentTextField.TabStop = false;
            this.VelocityExponentTextField.Text = "0";
            this.VelocityExponentTextField.UseSystemPasswordChar = false;
            this.VelocityExponentTextField.Validated += VelocityExponentTextField_Validated;
            // 
            // VelocityExponentLabel
            // 
            this.VelocityExponentLabel.AutoSize = true;
            this.VelocityExponentLabel.Depth = 0;
            this.VelocityExponentLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.VelocityExponentLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VelocityExponentLabel.Location = new System.Drawing.Point(48, 248);
            this.VelocityExponentLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityExponentLabel.Name = "VelocityExponentLabel";
            this.VelocityExponentLabel.Size = new System.Drawing.Size(130, 19);
            this.VelocityExponentLabel.TabIndex = 47;
            this.VelocityExponentLabel.Text = "Velocity Exponent";
            // 
            // VelocityMultiplierTextField
            // 
            this.VelocityMultiplierTextField.Depth = 0;
            this.VelocityMultiplierTextField.Hint = "";
            this.VelocityMultiplierTextField.Location = new System.Drawing.Point(266, 225);
            this.VelocityMultiplierTextField.MaxLength = 32767;
            this.VelocityMultiplierTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityMultiplierTextField.Name = "VelocityMultiplierTextField";
            this.VelocityMultiplierTextField.PasswordChar = '\0';
            this.VelocityMultiplierTextField.SelectedText = "";
            this.VelocityMultiplierTextField.SelectionLength = 0;
            this.VelocityMultiplierTextField.SelectionStart = 0;
            this.VelocityMultiplierTextField.Size = new System.Drawing.Size(75, 23);
            this.VelocityMultiplierTextField.TabIndex = 46;
            this.VelocityMultiplierTextField.TabStop = false;
            this.VelocityMultiplierTextField.Text = "1";
            this.VelocityMultiplierTextField.UseSystemPasswordChar = false;
            this.VelocityMultiplierTextField.Validated += VelocityMultiplierTextField_Validated;
            // 
            // VelocityMultiplierLabel
            // 
            this.VelocityMultiplierLabel.AutoSize = true;
            this.VelocityMultiplierLabel.Depth = 0;
            this.VelocityMultiplierLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.VelocityMultiplierLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VelocityMultiplierLabel.Location = new System.Drawing.Point(48, 229);
            this.VelocityMultiplierLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityMultiplierLabel.Name = "VelocityMultiplierLabel";
            this.VelocityMultiplierLabel.Size = new System.Drawing.Size(131, 19);
            this.VelocityMultiplierLabel.TabIndex = 45;
            this.VelocityMultiplierLabel.Text = "Velocity Multiplier";
            // 
            // ThumbstickDeadzoneTextField
            // 
            this.ThumbstickDeadzoneTextField.Depth = 0;
            this.ThumbstickDeadzoneTextField.Hint = "";
            this.ThumbstickDeadzoneTextField.Location = new System.Drawing.Point(266, 206);
            this.ThumbstickDeadzoneTextField.MaxLength = 32767;
            this.ThumbstickDeadzoneTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ThumbstickDeadzoneTextField.Name = "ThumbstickDeadzoneTextField";
            this.ThumbstickDeadzoneTextField.PasswordChar = '\0';
            this.ThumbstickDeadzoneTextField.SelectedText = "";
            this.ThumbstickDeadzoneTextField.SelectionLength = 0;
            this.ThumbstickDeadzoneTextField.SelectionStart = 0;
            this.ThumbstickDeadzoneTextField.Size = new System.Drawing.Size(75, 23);
            this.ThumbstickDeadzoneTextField.TabIndex = 44;
            this.ThumbstickDeadzoneTextField.TabStop = false;
            this.ThumbstickDeadzoneTextField.Text = "0";
            this.ThumbstickDeadzoneTextField.UseSystemPasswordChar = false;
            this.ThumbstickDeadzoneTextField.Validated += ThumbstickDeadzoneTextField_Validated;
            // 
            // ThumbsticklDeadzoneLabel
            // 
            this.ThumbsticklDeadzoneLabel.AutoSize = true;
            this.ThumbsticklDeadzoneLabel.Depth = 0;
            this.ThumbsticklDeadzoneLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.ThumbsticklDeadzoneLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ThumbsticklDeadzoneLabel.Location = new System.Drawing.Point(48, 210);
            this.ThumbsticklDeadzoneLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ThumbsticklDeadzoneLabel.Name = "ThumbsticklDeadzoneLabel";
            this.ThumbsticklDeadzoneLabel.Size = new System.Drawing.Size(158, 19);
            this.ThumbsticklDeadzoneLabel.TabIndex = 43;
            this.ThumbsticklDeadzoneLabel.Text = "Thumbstick Deadzone";
            // 
            // UseOrientationInHMDAlignmentCheckBox
            // 
            this.UseOrientationInHMDAlignmentCheckBox.AutoSize = true;
            this.UseOrientationInHMDAlignmentCheckBox.Depth = 0;
            this.UseOrientationInHMDAlignmentCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.UseOrientationInHMDAlignmentCheckBox.Location = new System.Drawing.Point(52, 127);
            this.UseOrientationInHMDAlignmentCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.UseOrientationInHMDAlignmentCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.UseOrientationInHMDAlignmentCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.UseOrientationInHMDAlignmentCheckBox.Name = "UseOrientationInHMDAlignmentCheckBox";
            this.UseOrientationInHMDAlignmentCheckBox.Ripple = true;
            this.UseOrientationInHMDAlignmentCheckBox.Size = new System.Drawing.Size(240, 30);
            this.UseOrientationInHMDAlignmentCheckBox.TabIndex = 42;
            this.UseOrientationInHMDAlignmentCheckBox.Text = "Use Orientation in HMD Alignment";
            this.UseOrientationInHMDAlignmentCheckBox.UseVisualStyleBackColor = true;
            this.UseOrientationInHMDAlignmentCheckBox.CheckedChanged += new System.EventHandler(this.UseOrientationInHMDAlignmentCheckBox_CheckedChanged);
            // 
            // DisableAlignmentGestureCheckBox
            // 
            this.DisableAlignmentGestureCheckBox.AutoSize = true;
            this.DisableAlignmentGestureCheckBox.Depth = 0;
            this.DisableAlignmentGestureCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.DisableAlignmentGestureCheckBox.Location = new System.Drawing.Point(52, 98);
            this.DisableAlignmentGestureCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.DisableAlignmentGestureCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.DisableAlignmentGestureCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.DisableAlignmentGestureCheckBox.Name = "DisableAlignmentGestureCheckBox";
            this.DisableAlignmentGestureCheckBox.Ripple = true;
            this.DisableAlignmentGestureCheckBox.Size = new System.Drawing.Size(194, 30);
            this.DisableAlignmentGestureCheckBox.TabIndex = 41;
            this.DisableAlignmentGestureCheckBox.Text = "Disable Alignment Gesture";
            this.DisableAlignmentGestureCheckBox.UseVisualStyleBackColor = true;
            this.DisableAlignmentGestureCheckBox.CheckedChanged += new System.EventHandler(this.DisableAlignmentGestureCheckBox_CheckedChanged);
            // 
            // ZRotate90CheckBox
            // 
            this.ZRotate90CheckBox.AutoSize = true;
            this.ZRotate90CheckBox.Depth = 0;
            this.ZRotate90CheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.ZRotate90CheckBox.Location = new System.Drawing.Point(52, 73);
            this.ZRotate90CheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.ZRotate90CheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.ZRotate90CheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.ZRotate90CheckBox.Name = "ZRotate90CheckBox";
            this.ZRotate90CheckBox.Ripple = true;
            this.ZRotate90CheckBox.Size = new System.Drawing.Size(124, 30);
            this.ZRotate90CheckBox.TabIndex = 39;
            this.ZRotate90CheckBox.Text = "Rotate Z by 90°";
            this.ZRotate90CheckBox.UseVisualStyleBackColor = true;
            this.ZRotate90CheckBox.CheckedChanged += new System.EventHandler(this.ZRotate90CheckBox_CheckedChanged);
            // 
            // ExtendZTextField
            // 
            this.ExtendZTextField.Depth = 0;
            this.ExtendZTextField.Hint = "";
            this.ExtendZTextField.Location = new System.Drawing.Point(266, 187);
            this.ExtendZTextField.MaxLength = 32767;
            this.ExtendZTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendZTextField.Name = "ExtendZTextField";
            this.ExtendZTextField.PasswordChar = '\0';
            this.ExtendZTextField.SelectedText = "";
            this.ExtendZTextField.SelectionLength = 0;
            this.ExtendZTextField.SelectionStart = 0;
            this.ExtendZTextField.Size = new System.Drawing.Size(75, 23);
            this.ExtendZTextField.TabIndex = 38;
            this.ExtendZTextField.TabStop = false;
            this.ExtendZTextField.Text = "0";
            this.ExtendZTextField.UseSystemPasswordChar = false;
            this.ExtendZTextField.Validated += ExtendZTextField_Validated;
            // 
            // ExtendZLabel
            // 
            this.ExtendZLabel.AutoSize = true;
            this.ExtendZLabel.Depth = 0;
            this.ExtendZLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.ExtendZLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ExtendZLabel.Location = new System.Drawing.Point(48, 191);
            this.ExtendZLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendZLabel.Name = "ExtendZLabel";
            this.ExtendZLabel.Size = new System.Drawing.Size(102, 19);
            this.ExtendZLabel.TabIndex = 37;
            this.ExtendZLabel.Text = "Extend Z (cm)";
            // 
            // ExtendYTextField
            // 
            this.ExtendYTextField.Depth = 0;
            this.ExtendYTextField.Hint = "";
            this.ExtendYTextField.Location = new System.Drawing.Point(266, 168);
            this.ExtendYTextField.MaxLength = 32767;
            this.ExtendYTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendYTextField.Name = "ExtendYTextField";
            this.ExtendYTextField.PasswordChar = '\0';
            this.ExtendYTextField.SelectedText = "";
            this.ExtendYTextField.SelectionLength = 0;
            this.ExtendYTextField.SelectionStart = 0;
            this.ExtendYTextField.Size = new System.Drawing.Size(75, 23);
            this.ExtendYTextField.TabIndex = 36;
            this.ExtendYTextField.TabStop = false;
            this.ExtendYTextField.Text = "0";
            this.ExtendYTextField.UseSystemPasswordChar = false;
            this.ExtendYTextField.Validated += ExtendYTextField_Validated;
            // 
            // ExtendYLabel
            // 
            this.ExtendYLabel.AutoSize = true;
            this.ExtendYLabel.Depth = 0;
            this.ExtendYLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.ExtendYLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ExtendYLabel.Location = new System.Drawing.Point(48, 172);
            this.ExtendYLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendYLabel.Name = "ExtendYLabel";
            this.ExtendYLabel.Size = new System.Drawing.Size(102, 19);
            this.ExtendYLabel.TabIndex = 35;
            this.ExtendYLabel.Text = "Extend Y (cm)";
            // 
            // DS4ControllerLabel
            // 
            this.DS4ControllerLabel.AutoSize = true;
            this.DS4ControllerLabel.Depth = 0;
            this.DS4ControllerLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.DS4ControllerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.DS4ControllerLabel.Location = new System.Drawing.Point(48, 19);
            this.DS4ControllerLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.DS4ControllerLabel.Name = "DS4ControllerLabel";
            this.DS4ControllerLabel.Size = new System.Drawing.Size(222, 19);
            this.DS4ControllerLabel.TabIndex = 34;
            this.DS4ControllerLabel.Text = "Dualshock 4 Controller Settings";
            // 
            // RumbleSuppressedCheckBox
            // 
            this.RumbleSuppressedCheckBox.AutoSize = true;
            this.RumbleSuppressedCheckBox.Depth = 0;
            this.RumbleSuppressedCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.RumbleSuppressedCheckBox.Location = new System.Drawing.Point(52, 43);
            this.RumbleSuppressedCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.RumbleSuppressedCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.RumbleSuppressedCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.RumbleSuppressedCheckBox.Name = "RumbleSuppressedCheckBox";
            this.RumbleSuppressedCheckBox.Ripple = true;
            this.RumbleSuppressedCheckBox.Size = new System.Drawing.Size(145, 30);
            this.RumbleSuppressedCheckBox.TabIndex = 33;
            this.RumbleSuppressedCheckBox.Text = "Rumble Supressed";
            this.RumbleSuppressedCheckBox.UseVisualStyleBackColor = true;
            this.RumbleSuppressedCheckBox.CheckedChanged += new System.EventHandler(this.RumbleSuppressedCheckBox_CheckedChanged);
            // 
            // TouchpadMappingsLayoutPanel
            // 
            this.TouchpadMappingsLayoutPanel.AutoScroll = true;
            this.TouchpadMappingsLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TouchpadMappingsLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.TouchpadMappingsLayoutPanel.Location = new System.Drawing.Point(433, 73);
            this.TouchpadMappingsLayoutPanel.Name = "TouchpadMappingsLayoutPanel";
            this.TouchpadMappingsLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.TouchpadMappingsLayoutPanel.Size = new System.Drawing.Size(262, 194);
            this.TouchpadMappingsLayoutPanel.TabIndex = 66;
            this.TouchpadMappingsLayoutPanel.WrapContents = false;
            // 
            // AddNewMappingButton
            // 
            this.AddNewMappingButton.AutoSize = true;
            this.AddNewMappingButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddNewMappingButton.Depth = 0;
            this.AddNewMappingButton.Icon = null;
            this.AddNewMappingButton.Location = new System.Drawing.Point(493, 273);
            this.AddNewMappingButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.AddNewMappingButton.Name = "AddNewMappingButton";
            this.AddNewMappingButton.Primary = true;
            this.AddNewMappingButton.Size = new System.Drawing.Size(146, 36);
            this.AddNewMappingButton.TabIndex = 67;
            this.AddNewMappingButton.Text = "Add New Mapping";
            this.AddNewMappingButton.UseVisualStyleBackColor = true;
            this.AddNewMappingButton.Click += new System.EventHandler(this.AddNewMappingButton_Click);
            // 
            // Dualshock4ControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.AddNewMappingButton);
            this.Controls.Add(this.TouchpadMappingsLayoutPanel);
            this.Controls.Add(this.TouchpadMappingLabel);
            this.Controls.Add(this.VelocityExponentTextField);
            this.Controls.Add(this.VelocityExponentLabel);
            this.Controls.Add(this.VelocityMultiplierTextField);
            this.Controls.Add(this.VelocityMultiplierLabel);
            this.Controls.Add(this.ThumbstickDeadzoneTextField);
            this.Controls.Add(this.ThumbsticklDeadzoneLabel);
            this.Controls.Add(this.UseOrientationInHMDAlignmentCheckBox);
            this.Controls.Add(this.DisableAlignmentGestureCheckBox);
            this.Controls.Add(this.ZRotate90CheckBox);
            this.Controls.Add(this.ExtendZTextField);
            this.Controls.Add(this.ExtendZLabel);
            this.Controls.Add(this.ExtendYTextField);
            this.Controls.Add(this.ExtendYLabel);
            this.Controls.Add(this.DS4ControllerLabel);
            this.Controls.Add(this.RumbleSuppressedCheckBox);
            this.Name = "Dualshock4ControllerPanel";
            this.Size = new System.Drawing.Size(734, 320);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialLabel TouchpadMappingLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField VelocityExponentTextField;
        private MaterialSkin.Controls.MaterialLabel VelocityExponentLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField VelocityMultiplierTextField;
        private MaterialSkin.Controls.MaterialLabel VelocityMultiplierLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField ThumbstickDeadzoneTextField;
        private MaterialSkin.Controls.MaterialLabel ThumbsticklDeadzoneLabel;
        private MaterialSkin.Controls.MaterialCheckBox UseOrientationInHMDAlignmentCheckBox;
        private MaterialSkin.Controls.MaterialCheckBox DisableAlignmentGestureCheckBox;
        private MaterialSkin.Controls.MaterialCheckBox ZRotate90CheckBox;
        private MaterialSkin.Controls.MaterialSingleLineTextField ExtendZTextField;
        private MaterialSkin.Controls.MaterialLabel ExtendZLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField ExtendYTextField;
        private MaterialSkin.Controls.MaterialLabel ExtendYLabel;
        private MaterialSkin.Controls.MaterialLabel DS4ControllerLabel;
        private MaterialSkin.Controls.MaterialCheckBox RumbleSuppressedCheckBox;
        private System.Windows.Forms.FlowLayoutPanel TouchpadMappingsLayoutPanel;
        private MaterialSkin.Controls.MaterialRaisedButton AddNewMappingButton;
    }
}

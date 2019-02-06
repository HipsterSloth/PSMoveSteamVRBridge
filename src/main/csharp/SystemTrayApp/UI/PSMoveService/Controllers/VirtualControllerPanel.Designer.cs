namespace SystemTrayApp
{
    partial class VirtualControllerPanel
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
            this.AddNewMappingButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.TouchpadMappingsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.TouchpadMappingLabel = new MaterialSkin.Controls.MaterialLabel();
            this.VelocityExponentTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VelocityExponentLabel = new MaterialSkin.Controls.MaterialLabel();
            this.VelocityMultiplierTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VelocityMultiplierLabel = new MaterialSkin.Controls.MaterialLabel();
            this.DisableAlignmentGestureCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.ZRotate90CheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.ExtendZTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ExtendZLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ExtendYTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ExtendYLabel = new MaterialSkin.Controls.MaterialLabel();
            this.VirtualControllerLabel = new MaterialSkin.Controls.MaterialLabel();
            this.TouchpadPressDelayCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.VirtualTumbstickScaleTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VirtualThumbstickScaleLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ThumbstickDeadzoneTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ThumbsticklDeadzoneLabel = new MaterialSkin.Controls.MaterialLabel();
            this.TouchpadXAxisIndexComboBox = new System.Windows.Forms.ComboBox();
            this.TouchpadXAxisIndexLabel = new MaterialSkin.Controls.MaterialLabel();
            this.TouchpadYAxisIndexComboBox = new System.Windows.Forms.ComboBox();
            this.TouchpadYAxisIndexLabel = new MaterialSkin.Controls.MaterialLabel();
            this.TriggerAxisIndexComboBox = new System.Windows.Forms.ComboBox();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.VirtualAxisMappingsLabel = new MaterialSkin.Controls.MaterialLabel();
            this.HMDAlignButtonComboBox = new System.Windows.Forms.ComboBox();
            this.HMDAlignButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.SystemButtonComboBox = new System.Windows.Forms.ComboBox();
            this.SystemButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.SuspendLayout();
            // 
            // AddNewMappingButton
            // 
            this.AddNewMappingButton.AutoSize = true;
            this.AddNewMappingButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddNewMappingButton.Depth = 0;
            this.AddNewMappingButton.Icon = null;
            this.AddNewMappingButton.Location = new System.Drawing.Point(489, 269);
            this.AddNewMappingButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.AddNewMappingButton.Name = "AddNewMappingButton";
            this.AddNewMappingButton.Primary = true;
            this.AddNewMappingButton.Size = new System.Drawing.Size(146, 36);
            this.AddNewMappingButton.TabIndex = 85;
            this.AddNewMappingButton.Text = "Add New Mapping";
            this.AddNewMappingButton.UseVisualStyleBackColor = true;
            this.AddNewMappingButton.Click += new System.EventHandler(this.AddNewMappingButton_Click);
            // 
            // TouchpadMappingsLayoutPanel
            // 
            this.TouchpadMappingsLayoutPanel.AutoScroll = true;
            this.TouchpadMappingsLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TouchpadMappingsLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.TouchpadMappingsLayoutPanel.Location = new System.Drawing.Point(429, 143);
            this.TouchpadMappingsLayoutPanel.Name = "TouchpadMappingsLayoutPanel";
            this.TouchpadMappingsLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.TouchpadMappingsLayoutPanel.Size = new System.Drawing.Size(262, 120);
            this.TouchpadMappingsLayoutPanel.TabIndex = 84;
            this.TouchpadMappingsLayoutPanel.WrapContents = false;
            // 
            // TouchpadMappingLabel
            // 
            this.TouchpadMappingLabel.AutoSize = true;
            this.TouchpadMappingLabel.Depth = 0;
            this.TouchpadMappingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.TouchpadMappingLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TouchpadMappingLabel.Location = new System.Drawing.Point(425, 121);
            this.TouchpadMappingLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.TouchpadMappingLabel.Name = "TouchpadMappingLabel";
            this.TouchpadMappingLabel.Size = new System.Drawing.Size(285, 18);
            this.TouchpadMappingLabel.TabIndex = 83;
            this.TouchpadMappingLabel.Text = "Button to Virtual Touchpad Mappings";
            // 
            // VelocityExponentTextField
            // 
            this.VelocityExponentTextField.Depth = 0;
            this.VelocityExponentTextField.Hint = "";
            this.VelocityExponentTextField.Location = new System.Drawing.Point(240, 286);
            this.VelocityExponentTextField.MaxLength = 32767;
            this.VelocityExponentTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityExponentTextField.Name = "VelocityExponentTextField";
            this.VelocityExponentTextField.PasswordChar = '\0';
            this.VelocityExponentTextField.SelectedText = "";
            this.VelocityExponentTextField.SelectionLength = 0;
            this.VelocityExponentTextField.SelectionStart = 0;
            this.VelocityExponentTextField.Size = new System.Drawing.Size(75, 23);
            this.VelocityExponentTextField.TabIndex = 82;
            this.VelocityExponentTextField.TabStop = false;
            this.VelocityExponentTextField.Text = "0";
            this.VelocityExponentTextField.UseSystemPasswordChar = false;
            // 
            // VelocityExponentLabel
            // 
            this.VelocityExponentLabel.AutoSize = true;
            this.VelocityExponentLabel.Depth = 0;
            this.VelocityExponentLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.VelocityExponentLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VelocityExponentLabel.Location = new System.Drawing.Point(22, 290);
            this.VelocityExponentLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityExponentLabel.Name = "VelocityExponentLabel";
            this.VelocityExponentLabel.Size = new System.Drawing.Size(130, 19);
            this.VelocityExponentLabel.TabIndex = 81;
            this.VelocityExponentLabel.Text = "Velocity Exponent";
            // 
            // VelocityMultiplierTextField
            // 
            this.VelocityMultiplierTextField.Depth = 0;
            this.VelocityMultiplierTextField.Hint = "";
            this.VelocityMultiplierTextField.Location = new System.Drawing.Point(240, 267);
            this.VelocityMultiplierTextField.MaxLength = 32767;
            this.VelocityMultiplierTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityMultiplierTextField.Name = "VelocityMultiplierTextField";
            this.VelocityMultiplierTextField.PasswordChar = '\0';
            this.VelocityMultiplierTextField.SelectedText = "";
            this.VelocityMultiplierTextField.SelectionLength = 0;
            this.VelocityMultiplierTextField.SelectionStart = 0;
            this.VelocityMultiplierTextField.Size = new System.Drawing.Size(75, 23);
            this.VelocityMultiplierTextField.TabIndex = 80;
            this.VelocityMultiplierTextField.TabStop = false;
            this.VelocityMultiplierTextField.Text = "1";
            this.VelocityMultiplierTextField.UseSystemPasswordChar = false;
            // 
            // VelocityMultiplierLabel
            // 
            this.VelocityMultiplierLabel.AutoSize = true;
            this.VelocityMultiplierLabel.Depth = 0;
            this.VelocityMultiplierLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.VelocityMultiplierLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VelocityMultiplierLabel.Location = new System.Drawing.Point(22, 271);
            this.VelocityMultiplierLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityMultiplierLabel.Name = "VelocityMultiplierLabel";
            this.VelocityMultiplierLabel.Size = new System.Drawing.Size(131, 19);
            this.VelocityMultiplierLabel.TabIndex = 79;
            this.VelocityMultiplierLabel.Text = "Velocity Multiplier";
            // 
            // DisableAlignmentGestureCheckBox
            // 
            this.DisableAlignmentGestureCheckBox.AutoSize = true;
            this.DisableAlignmentGestureCheckBox.Depth = 0;
            this.DisableAlignmentGestureCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.DisableAlignmentGestureCheckBox.Location = new System.Drawing.Point(23, 95);
            this.DisableAlignmentGestureCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.DisableAlignmentGestureCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.DisableAlignmentGestureCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.DisableAlignmentGestureCheckBox.Name = "DisableAlignmentGestureCheckBox";
            this.DisableAlignmentGestureCheckBox.Ripple = true;
            this.DisableAlignmentGestureCheckBox.Size = new System.Drawing.Size(194, 30);
            this.DisableAlignmentGestureCheckBox.TabIndex = 75;
            this.DisableAlignmentGestureCheckBox.Text = "Disable Alignment Gesture";
            this.DisableAlignmentGestureCheckBox.UseVisualStyleBackColor = true;
            this.DisableAlignmentGestureCheckBox.CheckedChanged += new System.EventHandler(this.DisableAlignmentGestureCheckBox_CheckedChanged);
            // 
            // ZRotate90CheckBox
            // 
            this.ZRotate90CheckBox.AutoSize = true;
            this.ZRotate90CheckBox.Depth = 0;
            this.ZRotate90CheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.ZRotate90CheckBox.Location = new System.Drawing.Point(23, 151);
            this.ZRotate90CheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.ZRotate90CheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.ZRotate90CheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.ZRotate90CheckBox.Name = "ZRotate90CheckBox";
            this.ZRotate90CheckBox.Ripple = true;
            this.ZRotate90CheckBox.Size = new System.Drawing.Size(124, 30);
            this.ZRotate90CheckBox.TabIndex = 74;
            this.ZRotate90CheckBox.Text = "Rotate Z by 90°";
            this.ZRotate90CheckBox.UseVisualStyleBackColor = true;
            this.ZRotate90CheckBox.CheckedChanged += new System.EventHandler(this.ZRotate90CheckBox_CheckedChanged);
            // 
            // ExtendZTextField
            // 
            this.ExtendZTextField.Depth = 0;
            this.ExtendZTextField.Hint = "";
            this.ExtendZTextField.Location = new System.Drawing.Point(240, 208);
            this.ExtendZTextField.MaxLength = 32767;
            this.ExtendZTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendZTextField.Name = "ExtendZTextField";
            this.ExtendZTextField.PasswordChar = '\0';
            this.ExtendZTextField.SelectedText = "";
            this.ExtendZTextField.SelectionLength = 0;
            this.ExtendZTextField.SelectionStart = 0;
            this.ExtendZTextField.Size = new System.Drawing.Size(75, 23);
            this.ExtendZTextField.TabIndex = 73;
            this.ExtendZTextField.TabStop = false;
            this.ExtendZTextField.Text = "0";
            this.ExtendZTextField.UseSystemPasswordChar = false;
            // 
            // ExtendZLabel
            // 
            this.ExtendZLabel.AutoSize = true;
            this.ExtendZLabel.Depth = 0;
            this.ExtendZLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.ExtendZLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ExtendZLabel.Location = new System.Drawing.Point(22, 212);
            this.ExtendZLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendZLabel.Name = "ExtendZLabel";
            this.ExtendZLabel.Size = new System.Drawing.Size(102, 19);
            this.ExtendZLabel.TabIndex = 72;
            this.ExtendZLabel.Text = "Extend Z (cm)";
            // 
            // ExtendYTextField
            // 
            this.ExtendYTextField.Depth = 0;
            this.ExtendYTextField.Hint = "";
            this.ExtendYTextField.Location = new System.Drawing.Point(240, 189);
            this.ExtendYTextField.MaxLength = 32767;
            this.ExtendYTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendYTextField.Name = "ExtendYTextField";
            this.ExtendYTextField.PasswordChar = '\0';
            this.ExtendYTextField.SelectedText = "";
            this.ExtendYTextField.SelectionLength = 0;
            this.ExtendYTextField.SelectionStart = 0;
            this.ExtendYTextField.Size = new System.Drawing.Size(75, 23);
            this.ExtendYTextField.TabIndex = 71;
            this.ExtendYTextField.TabStop = false;
            this.ExtendYTextField.Text = "0";
            this.ExtendYTextField.UseSystemPasswordChar = false;
            // 
            // ExtendYLabel
            // 
            this.ExtendYLabel.AutoSize = true;
            this.ExtendYLabel.Depth = 0;
            this.ExtendYLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.ExtendYLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ExtendYLabel.Location = new System.Drawing.Point(22, 193);
            this.ExtendYLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendYLabel.Name = "ExtendYLabel";
            this.ExtendYLabel.Size = new System.Drawing.Size(102, 19);
            this.ExtendYLabel.TabIndex = 70;
            this.ExtendYLabel.Text = "Extend Y (cm)";
            // 
            // VirtualControllerLabel
            // 
            this.VirtualControllerLabel.AutoSize = true;
            this.VirtualControllerLabel.Depth = 0;
            this.VirtualControllerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.VirtualControllerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VirtualControllerLabel.Location = new System.Drawing.Point(19, 14);
            this.VirtualControllerLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VirtualControllerLabel.Name = "VirtualControllerLabel";
            this.VirtualControllerLabel.Size = new System.Drawing.Size(201, 18);
            this.VirtualControllerLabel.TabIndex = 69;
            this.VirtualControllerLabel.Text = "Virtual Controller Settings";
            // 
            // TouchpadPressDelayCheckBox
            // 
            this.TouchpadPressDelayCheckBox.AutoSize = true;
            this.TouchpadPressDelayCheckBox.Depth = 0;
            this.TouchpadPressDelayCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.TouchpadPressDelayCheckBox.Location = new System.Drawing.Point(23, 121);
            this.TouchpadPressDelayCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.TouchpadPressDelayCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.TouchpadPressDelayCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.TouchpadPressDelayCheckBox.Name = "TouchpadPressDelayCheckBox";
            this.TouchpadPressDelayCheckBox.Ripple = true;
            this.TouchpadPressDelayCheckBox.Size = new System.Drawing.Size(202, 30);
            this.TouchpadPressDelayCheckBox.TabIndex = 86;
            this.TouchpadPressDelayCheckBox.Text = "Delay After Touchpad Press";
            this.TouchpadPressDelayCheckBox.UseVisualStyleBackColor = true;
            this.TouchpadPressDelayCheckBox.CheckedChanged += new System.EventHandler(this.TouchpadPressDelayCheckBox_CheckedChanged);
            // 
            // VirtualTumbstickScaleTextField
            // 
            this.VirtualTumbstickScaleTextField.Depth = 0;
            this.VirtualTumbstickScaleTextField.Hint = "";
            this.VirtualTumbstickScaleTextField.Location = new System.Drawing.Point(240, 227);
            this.VirtualTumbstickScaleTextField.MaxLength = 32767;
            this.VirtualTumbstickScaleTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VirtualTumbstickScaleTextField.Name = "VirtualTumbstickScaleTextField";
            this.VirtualTumbstickScaleTextField.PasswordChar = '\0';
            this.VirtualTumbstickScaleTextField.SelectedText = "";
            this.VirtualTumbstickScaleTextField.SelectionLength = 0;
            this.VirtualTumbstickScaleTextField.SelectionStart = 0;
            this.VirtualTumbstickScaleTextField.Size = new System.Drawing.Size(75, 23);
            this.VirtualTumbstickScaleTextField.TabIndex = 88;
            this.VirtualTumbstickScaleTextField.TabStop = false;
            this.VirtualTumbstickScaleTextField.Text = "0";
            this.VirtualTumbstickScaleTextField.UseSystemPasswordChar = false;
            // 
            // VirtualThumbstickScaleLabel
            // 
            this.VirtualThumbstickScaleLabel.AutoSize = true;
            this.VirtualThumbstickScaleLabel.Depth = 0;
            this.VirtualThumbstickScaleLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.VirtualThumbstickScaleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VirtualThumbstickScaleLabel.Location = new System.Drawing.Point(22, 231);
            this.VirtualThumbstickScaleLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VirtualThumbstickScaleLabel.Name = "VirtualThumbstickScaleLabel";
            this.VirtualThumbstickScaleLabel.Size = new System.Drawing.Size(212, 19);
            this.VirtualThumbstickScaleLabel.TabIndex = 87;
            this.VirtualThumbstickScaleLabel.Text = "Virtual Thumbstick Scale (cm)";
            // 
            // ThumbstickDeadzoneTextField
            // 
            this.ThumbstickDeadzoneTextField.Depth = 0;
            this.ThumbstickDeadzoneTextField.Hint = "";
            this.ThumbstickDeadzoneTextField.Location = new System.Drawing.Point(240, 246);
            this.ThumbstickDeadzoneTextField.MaxLength = 32767;
            this.ThumbstickDeadzoneTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ThumbstickDeadzoneTextField.Name = "ThumbstickDeadzoneTextField";
            this.ThumbstickDeadzoneTextField.PasswordChar = '\0';
            this.ThumbstickDeadzoneTextField.SelectedText = "";
            this.ThumbstickDeadzoneTextField.SelectionLength = 0;
            this.ThumbstickDeadzoneTextField.SelectionStart = 0;
            this.ThumbstickDeadzoneTextField.Size = new System.Drawing.Size(75, 23);
            this.ThumbstickDeadzoneTextField.TabIndex = 90;
            this.ThumbstickDeadzoneTextField.TabStop = false;
            this.ThumbstickDeadzoneTextField.Text = "0";
            this.ThumbstickDeadzoneTextField.UseSystemPasswordChar = false;
            // 
            // ThumbsticklDeadzoneLabel
            // 
            this.ThumbsticklDeadzoneLabel.AutoSize = true;
            this.ThumbsticklDeadzoneLabel.Depth = 0;
            this.ThumbsticklDeadzoneLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.ThumbsticklDeadzoneLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ThumbsticklDeadzoneLabel.Location = new System.Drawing.Point(22, 250);
            this.ThumbsticklDeadzoneLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ThumbsticklDeadzoneLabel.Name = "ThumbsticklDeadzoneLabel";
            this.ThumbsticklDeadzoneLabel.Size = new System.Drawing.Size(158, 19);
            this.ThumbsticklDeadzoneLabel.TabIndex = 89;
            this.ThumbsticklDeadzoneLabel.Text = "Thumbstick Deadzone";
            // 
            // TouchpadXAxisIndexComboBox
            // 
            this.TouchpadXAxisIndexComboBox.FormattingEnabled = true;
            this.TouchpadXAxisIndexComboBox.Items.AddRange(new object[] {
            "None",
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
            this.TouchpadXAxisIndexComboBox.Location = new System.Drawing.Point(643, 40);
            this.TouchpadXAxisIndexComboBox.Name = "TouchpadXAxisIndexComboBox";
            this.TouchpadXAxisIndexComboBox.Size = new System.Drawing.Size(67, 21);
            this.TouchpadXAxisIndexComboBox.TabIndex = 92;
            this.TouchpadXAxisIndexComboBox.SelectedIndexChanged += TouchpadXAxisIndexComboBox_SelectedIndexChanged;
            // 
            // TouchpadXAxisIndexLabel
            // 
            this.TouchpadXAxisIndexLabel.AutoSize = true;
            this.TouchpadXAxisIndexLabel.Depth = 0;
            this.TouchpadXAxisIndexLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.TouchpadXAxisIndexLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TouchpadXAxisIndexLabel.Location = new System.Drawing.Point(425, 42);
            this.TouchpadXAxisIndexLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.TouchpadXAxisIndexLabel.Name = "TouchpadXAxisIndexLabel";
            this.TouchpadXAxisIndexLabel.Size = new System.Drawing.Size(208, 19);
            this.TouchpadXAxisIndexLabel.TabIndex = 91;
            this.TouchpadXAxisIndexLabel.Text = "Virtual Touchpad X-Axis Index";
            // 
            // TouchpadYAxisIndexComboBox
            // 
            this.TouchpadYAxisIndexComboBox.FormattingEnabled = true;
            this.TouchpadYAxisIndexComboBox.Items.AddRange(new object[] {
            "None",
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
            this.TouchpadYAxisIndexComboBox.Location = new System.Drawing.Point(643, 63);
            this.TouchpadYAxisIndexComboBox.Name = "TouchpadYAxisIndexComboBox";
            this.TouchpadYAxisIndexComboBox.Size = new System.Drawing.Size(67, 21);
            this.TouchpadYAxisIndexComboBox.TabIndex = 94;
            this.TouchpadYAxisIndexComboBox.SelectedIndexChanged += TouchpadYAxisIndexComboBox_SelectedIndexChanged;
            // 
            // TouchpadYAxisIndexLabel
            // 
            this.TouchpadYAxisIndexLabel.AutoSize = true;
            this.TouchpadYAxisIndexLabel.Depth = 0;
            this.TouchpadYAxisIndexLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.TouchpadYAxisIndexLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TouchpadYAxisIndexLabel.Location = new System.Drawing.Point(425, 65);
            this.TouchpadYAxisIndexLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.TouchpadYAxisIndexLabel.Name = "TouchpadYAxisIndexLabel";
            this.TouchpadYAxisIndexLabel.Size = new System.Drawing.Size(208, 19);
            this.TouchpadYAxisIndexLabel.TabIndex = 93;
            this.TouchpadYAxisIndexLabel.Text = "Virtual Touchpad Y-Axis Index";
            // 
            // TriggerAxisIndexComboBox
            // 
            this.TriggerAxisIndexComboBox.FormattingEnabled = true;
            this.TriggerAxisIndexComboBox.Items.AddRange(new object[] {
            "None",
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
            this.TriggerAxisIndexComboBox.Location = new System.Drawing.Point(643, 86);
            this.TriggerAxisIndexComboBox.Name = "TriggerAxisIndexComboBox";
            this.TriggerAxisIndexComboBox.Size = new System.Drawing.Size(67, 21);
            this.TriggerAxisIndexComboBox.TabIndex = 96;
            this.TriggerAxisIndexComboBox.SelectedIndexChanged += TriggerAxisIndexComboBox_SelectedIndexChanged;
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(425, 88);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(176, 19);
            this.materialLabel1.TabIndex = 95;
            this.materialLabel1.Text = "Virtual Trigger Axis Index";
            // 
            // VirtualAxisMappingsLabel
            // 
            this.VirtualAxisMappingsLabel.AutoSize = true;
            this.VirtualAxisMappingsLabel.Depth = 0;
            this.VirtualAxisMappingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.VirtualAxisMappingsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VirtualAxisMappingsLabel.Location = new System.Drawing.Point(425, 14);
            this.VirtualAxisMappingsLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VirtualAxisMappingsLabel.Name = "VirtualAxisMappingsLabel";
            this.VirtualAxisMappingsLabel.Size = new System.Drawing.Size(168, 18);
            this.VirtualAxisMappingsLabel.TabIndex = 97;
            this.VirtualAxisMappingsLabel.Text = "Virtual Axis Mappings";
            // 
            // HMDAlignButtonComboBox
            // 
            this.HMDAlignButtonComboBox.FormattingEnabled = true;
            this.HMDAlignButtonComboBox.Location = new System.Drawing.Point(191, 67);
            this.HMDAlignButtonComboBox.Name = "HMDAlignButtonComboBox";
            this.HMDAlignButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.HMDAlignButtonComboBox.TabIndex = 101;
            this.HMDAlignButtonComboBox.SelectedIndexChanged += HMDAlignButtonComboBox_SelectedIndexChanged;
            // 
            // HMDAlignButtonLabel
            // 
            this.HMDAlignButtonLabel.AutoSize = true;
            this.HMDAlignButtonLabel.Depth = 0;
            this.HMDAlignButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.HMDAlignButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.HMDAlignButtonLabel.Location = new System.Drawing.Point(22, 69);
            this.HMDAlignButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.HMDAlignButtonLabel.Name = "HMDAlignButtonLabel";
            this.HMDAlignButtonLabel.Size = new System.Drawing.Size(163, 19);
            this.HMDAlignButtonLabel.TabIndex = 100;
            this.HMDAlignButtonLabel.Text = "HMD Alignment Button";
            // 
            // SystemButtonComboBox
            // 
            this.SystemButtonComboBox.FormattingEnabled = true;
            this.SystemButtonComboBox.Location = new System.Drawing.Point(191, 40);
            this.SystemButtonComboBox.Name = "SystemButtonComboBox";
            this.SystemButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.SystemButtonComboBox.TabIndex = 99;
            this.SystemButtonComboBox.SelectedIndexChanged += SystemButtonComboBox_SelectedIndexChanged;
            // 
            // SystemButtonLabel
            // 
            this.SystemButtonLabel.AutoSize = true;
            this.SystemButtonLabel.Depth = 0;
            this.SystemButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.SystemButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SystemButtonLabel.Location = new System.Drawing.Point(22, 42);
            this.SystemButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.SystemButtonLabel.Name = "SystemButtonLabel";
            this.SystemButtonLabel.Size = new System.Drawing.Size(155, 19);
            this.SystemButtonLabel.TabIndex = 98;
            this.SystemButtonLabel.Text = "Virtual System Button";
            // 
            // VirtualControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HMDAlignButtonComboBox);
            this.Controls.Add(this.HMDAlignButtonLabel);
            this.Controls.Add(this.SystemButtonComboBox);
            this.Controls.Add(this.SystemButtonLabel);
            this.Controls.Add(this.VirtualAxisMappingsLabel);
            this.Controls.Add(this.TriggerAxisIndexComboBox);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.TouchpadYAxisIndexComboBox);
            this.Controls.Add(this.TouchpadYAxisIndexLabel);
            this.Controls.Add(this.TouchpadXAxisIndexComboBox);
            this.Controls.Add(this.TouchpadXAxisIndexLabel);
            this.Controls.Add(this.ThumbstickDeadzoneTextField);
            this.Controls.Add(this.ThumbsticklDeadzoneLabel);
            this.Controls.Add(this.VirtualTumbstickScaleTextField);
            this.Controls.Add(this.VirtualThumbstickScaleLabel);
            this.Controls.Add(this.TouchpadPressDelayCheckBox);
            this.Controls.Add(this.AddNewMappingButton);
            this.Controls.Add(this.TouchpadMappingsLayoutPanel);
            this.Controls.Add(this.TouchpadMappingLabel);
            this.Controls.Add(this.VelocityExponentTextField);
            this.Controls.Add(this.VelocityExponentLabel);
            this.Controls.Add(this.VelocityMultiplierTextField);
            this.Controls.Add(this.VelocityMultiplierLabel);
            this.Controls.Add(this.DisableAlignmentGestureCheckBox);
            this.Controls.Add(this.ZRotate90CheckBox);
            this.Controls.Add(this.ExtendZTextField);
            this.Controls.Add(this.ExtendZLabel);
            this.Controls.Add(this.ExtendYTextField);
            this.Controls.Add(this.ExtendYLabel);
            this.Controls.Add(this.VirtualControllerLabel);
            this.Name = "VirtualControllerPanel";
            this.Size = new System.Drawing.Size(734, 320);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private MaterialSkin.Controls.MaterialRaisedButton AddNewMappingButton;
        private System.Windows.Forms.FlowLayoutPanel TouchpadMappingsLayoutPanel;
        private MaterialSkin.Controls.MaterialLabel TouchpadMappingLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField VelocityExponentTextField;
        private MaterialSkin.Controls.MaterialLabel VelocityExponentLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField VelocityMultiplierTextField;
        private MaterialSkin.Controls.MaterialLabel VelocityMultiplierLabel;
        private MaterialSkin.Controls.MaterialCheckBox DisableAlignmentGestureCheckBox;
        private MaterialSkin.Controls.MaterialCheckBox ZRotate90CheckBox;
        private MaterialSkin.Controls.MaterialSingleLineTextField ExtendZTextField;
        private MaterialSkin.Controls.MaterialLabel ExtendZLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField ExtendYTextField;
        private MaterialSkin.Controls.MaterialLabel ExtendYLabel;
        private MaterialSkin.Controls.MaterialLabel VirtualControllerLabel;
        private MaterialSkin.Controls.MaterialCheckBox TouchpadPressDelayCheckBox;
        private MaterialSkin.Controls.MaterialSingleLineTextField VirtualTumbstickScaleTextField;
        private MaterialSkin.Controls.MaterialLabel VirtualThumbstickScaleLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField ThumbstickDeadzoneTextField;
        private MaterialSkin.Controls.MaterialLabel ThumbsticklDeadzoneLabel;
        private System.Windows.Forms.ComboBox TouchpadXAxisIndexComboBox;
        private MaterialSkin.Controls.MaterialLabel TouchpadXAxisIndexLabel;
        private System.Windows.Forms.ComboBox TouchpadYAxisIndexComboBox;
        private MaterialSkin.Controls.MaterialLabel TouchpadYAxisIndexLabel;
        private System.Windows.Forms.ComboBox TriggerAxisIndexComboBox;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel VirtualAxisMappingsLabel;
        private System.Windows.Forms.ComboBox HMDAlignButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel HMDAlignButtonLabel;
        private System.Windows.Forms.ComboBox SystemButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel SystemButtonLabel;
    }
}

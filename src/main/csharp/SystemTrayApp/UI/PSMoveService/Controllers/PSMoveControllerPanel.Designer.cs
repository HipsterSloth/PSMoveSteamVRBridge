namespace SystemTrayApp
{
    partial class PSMoveControllerPanel
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
            this.RumbleSuppressedCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.PSMoveControllerLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ExtendYLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ExtendYTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ExtendZTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.ExtendZLabel = new MaterialSkin.Controls.MaterialLabel();
            this.ZRotate90CheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.TouchpadPressDelayCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.VirtualTumbstickScaleTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VirtualThumbstickScaleLabel = new MaterialSkin.Controls.MaterialLabel();
            this.VelocityMultiplierTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VelocityMultiplierLabel = new MaterialSkin.Controls.MaterialLabel();
            this.VelocityExponentTextField = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.VelocityExponentLabel = new MaterialSkin.Controls.MaterialLabel();
            this.PSButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.PSButtonComboBox = new System.Windows.Forms.ComboBox();
            this.MoveButtonComboBox = new System.Windows.Forms.ComboBox();
            this.MoveButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.TriangleButtonComboBox = new System.Windows.Forms.ComboBox();
            this.TriangleButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.SquareButtonComboBox = new System.Windows.Forms.ComboBox();
            this.SquareButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.CircleButtonComboBox = new System.Windows.Forms.ComboBox();
            this.CircleButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.CrossButtonComboBox = new System.Windows.Forms.ComboBox();
            this.CrossButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.SelectButtonComboBox = new System.Windows.Forms.ComboBox();
            this.SelectButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.StartButtonComboBox = new System.Windows.Forms.ComboBox();
            this.StartButtonLabel = new MaterialSkin.Controls.MaterialLabel();
            this.TouchpadMappingLabel = new MaterialSkin.Controls.MaterialLabel();
            this.DisableControllerCheckBox = new MaterialSkin.Controls.MaterialCheckBox();
            this.SuspendLayout();
            // 
            // RumbleSuppressedCheckBox
            // 
            this.RumbleSuppressedCheckBox.AutoSize = true;
            this.RumbleSuppressedCheckBox.Depth = 0;
            this.RumbleSuppressedCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.RumbleSuppressedCheckBox.Location = new System.Drawing.Point(20, 62);
            this.RumbleSuppressedCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.RumbleSuppressedCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.RumbleSuppressedCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.RumbleSuppressedCheckBox.Name = "RumbleSuppressedCheckBox";
            this.RumbleSuppressedCheckBox.Ripple = true;
            this.RumbleSuppressedCheckBox.Size = new System.Drawing.Size(145, 30);
            this.RumbleSuppressedCheckBox.TabIndex = 0;
            this.RumbleSuppressedCheckBox.Text = "Rumble Supressed";
            this.RumbleSuppressedCheckBox.UseVisualStyleBackColor = true;
            this.RumbleSuppressedCheckBox.CheckedChanged += new System.EventHandler(this.RumbleSuppressedCheckBox_CheckedChanged);
            // 
            // PSMoveControllerLabel
            // 
            this.PSMoveControllerLabel.AutoSize = true;
            this.PSMoveControllerLabel.Depth = 0;
            this.PSMoveControllerLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.PSMoveControllerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PSMoveControllerLabel.Location = new System.Drawing.Point(16, 13);
            this.PSMoveControllerLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.PSMoveControllerLabel.Name = "PSMoveControllerLabel";
            this.PSMoveControllerLabel.Size = new System.Drawing.Size(194, 19);
            this.PSMoveControllerLabel.TabIndex = 1;
            this.PSMoveControllerLabel.Text = "PSMove Controller Settings";
            // 
            // ExtendYLabel
            // 
            this.ExtendYLabel.AutoSize = true;
            this.ExtendYLabel.Depth = 0;
            this.ExtendYLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.ExtendYLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ExtendYLabel.Location = new System.Drawing.Point(16, 156);
            this.ExtendYLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendYLabel.Name = "ExtendYLabel";
            this.ExtendYLabel.Size = new System.Drawing.Size(102, 19);
            this.ExtendYLabel.TabIndex = 2;
            this.ExtendYLabel.Text = "Extend Y (cm)";
            // 
            // ExtendYTextField
            // 
            this.ExtendYTextField.Depth = 0;
            this.ExtendYTextField.Hint = "";
            this.ExtendYTextField.Location = new System.Drawing.Point(234, 152);
            this.ExtendYTextField.MaxLength = 32767;
            this.ExtendYTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendYTextField.Name = "ExtendYTextField";
            this.ExtendYTextField.PasswordChar = '\0';
            this.ExtendYTextField.SelectedText = "";
            this.ExtendYTextField.SelectionLength = 0;
            this.ExtendYTextField.SelectionStart = 0;
            this.ExtendYTextField.Size = new System.Drawing.Size(75, 23);
            this.ExtendYTextField.TabIndex = 3;
            this.ExtendYTextField.TabStop = false;
            this.ExtendYTextField.Text = "0";
            this.ExtendYTextField.UseSystemPasswordChar = false;
            // 
            // ExtendZTextField
            // 
            this.ExtendZTextField.Depth = 0;
            this.ExtendZTextField.Hint = "";
            this.ExtendZTextField.Location = new System.Drawing.Point(234, 171);
            this.ExtendZTextField.MaxLength = 32767;
            this.ExtendZTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendZTextField.Name = "ExtendZTextField";
            this.ExtendZTextField.PasswordChar = '\0';
            this.ExtendZTextField.SelectedText = "";
            this.ExtendZTextField.SelectionLength = 0;
            this.ExtendZTextField.SelectionStart = 0;
            this.ExtendZTextField.Size = new System.Drawing.Size(75, 23);
            this.ExtendZTextField.TabIndex = 5;
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
            this.ExtendZLabel.Location = new System.Drawing.Point(16, 175);
            this.ExtendZLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ExtendZLabel.Name = "ExtendZLabel";
            this.ExtendZLabel.Size = new System.Drawing.Size(102, 19);
            this.ExtendZLabel.TabIndex = 4;
            this.ExtendZLabel.Text = "Extend Z (cm)";
            // 
            // ZRotate90CheckBox
            // 
            this.ZRotate90CheckBox.AutoSize = true;
            this.ZRotate90CheckBox.Depth = 0;
            this.ZRotate90CheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.ZRotate90CheckBox.Location = new System.Drawing.Point(20, 88);
            this.ZRotate90CheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.ZRotate90CheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.ZRotate90CheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.ZRotate90CheckBox.Name = "ZRotate90CheckBox";
            this.ZRotate90CheckBox.Ripple = true;
            this.ZRotate90CheckBox.Size = new System.Drawing.Size(124, 30);
            this.ZRotate90CheckBox.TabIndex = 6;
            this.ZRotate90CheckBox.Text = "Rotate Z by 90°";
            this.ZRotate90CheckBox.UseVisualStyleBackColor = true;
            this.ZRotate90CheckBox.CheckedChanged += new System.EventHandler(this.ZRotate90CheckBox_CheckedChanged);
            // 
            // TouchpadPressDelayCheckBox
            // 
            this.TouchpadPressDelayCheckBox.AutoSize = true;
            this.TouchpadPressDelayCheckBox.Depth = 0;
            this.TouchpadPressDelayCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.TouchpadPressDelayCheckBox.Location = new System.Drawing.Point(20, 114);
            this.TouchpadPressDelayCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.TouchpadPressDelayCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.TouchpadPressDelayCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.TouchpadPressDelayCheckBox.Name = "TouchpadPressDelayCheckBox";
            this.TouchpadPressDelayCheckBox.Ripple = true;
            this.TouchpadPressDelayCheckBox.Size = new System.Drawing.Size(202, 30);
            this.TouchpadPressDelayCheckBox.TabIndex = 7;
            this.TouchpadPressDelayCheckBox.Text = "Delay After Touchpad Press";
            this.TouchpadPressDelayCheckBox.UseVisualStyleBackColor = true;
            this.TouchpadPressDelayCheckBox.CheckedChanged += new System.EventHandler(this.TouchpadPressDelayCheckBox_CheckedChanged);
            // 
            // VirtualTumbstickScaleTextField
            // 
            this.VirtualTumbstickScaleTextField.Depth = 0;
            this.VirtualTumbstickScaleTextField.Hint = "";
            this.VirtualTumbstickScaleTextField.Location = new System.Drawing.Point(234, 190);
            this.VirtualTumbstickScaleTextField.MaxLength = 32767;
            this.VirtualTumbstickScaleTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VirtualTumbstickScaleTextField.Name = "VirtualTumbstickScaleTextField";
            this.VirtualTumbstickScaleTextField.PasswordChar = '\0';
            this.VirtualTumbstickScaleTextField.SelectedText = "";
            this.VirtualTumbstickScaleTextField.SelectionLength = 0;
            this.VirtualTumbstickScaleTextField.SelectionStart = 0;
            this.VirtualTumbstickScaleTextField.Size = new System.Drawing.Size(75, 23);
            this.VirtualTumbstickScaleTextField.TabIndex = 11;
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
            this.VirtualThumbstickScaleLabel.Location = new System.Drawing.Point(16, 194);
            this.VirtualThumbstickScaleLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VirtualThumbstickScaleLabel.Name = "VirtualThumbstickScaleLabel";
            this.VirtualThumbstickScaleLabel.Size = new System.Drawing.Size(212, 19);
            this.VirtualThumbstickScaleLabel.TabIndex = 10;
            this.VirtualThumbstickScaleLabel.Text = "Virtual Thumbstick Scale (cm)";
            // 
            // VelocityMultiplierTextField
            // 
            this.VelocityMultiplierTextField.Depth = 0;
            this.VelocityMultiplierTextField.Hint = "";
            this.VelocityMultiplierTextField.Location = new System.Drawing.Point(234, 209);
            this.VelocityMultiplierTextField.MaxLength = 32767;
            this.VelocityMultiplierTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityMultiplierTextField.Name = "VelocityMultiplierTextField";
            this.VelocityMultiplierTextField.PasswordChar = '\0';
            this.VelocityMultiplierTextField.SelectedText = "";
            this.VelocityMultiplierTextField.SelectionLength = 0;
            this.VelocityMultiplierTextField.SelectionStart = 0;
            this.VelocityMultiplierTextField.Size = new System.Drawing.Size(75, 23);
            this.VelocityMultiplierTextField.TabIndex = 13;
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
            this.VelocityMultiplierLabel.Location = new System.Drawing.Point(16, 213);
            this.VelocityMultiplierLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityMultiplierLabel.Name = "VelocityMultiplierLabel";
            this.VelocityMultiplierLabel.Size = new System.Drawing.Size(131, 19);
            this.VelocityMultiplierLabel.TabIndex = 12;
            this.VelocityMultiplierLabel.Text = "Velocity Multiplier";
            // 
            // VelocityExponentTextField
            // 
            this.VelocityExponentTextField.Depth = 0;
            this.VelocityExponentTextField.Hint = "";
            this.VelocityExponentTextField.Location = new System.Drawing.Point(234, 228);
            this.VelocityExponentTextField.MaxLength = 32767;
            this.VelocityExponentTextField.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityExponentTextField.Name = "VelocityExponentTextField";
            this.VelocityExponentTextField.PasswordChar = '\0';
            this.VelocityExponentTextField.SelectedText = "";
            this.VelocityExponentTextField.SelectionLength = 0;
            this.VelocityExponentTextField.SelectionStart = 0;
            this.VelocityExponentTextField.Size = new System.Drawing.Size(75, 23);
            this.VelocityExponentTextField.TabIndex = 15;
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
            this.VelocityExponentLabel.Location = new System.Drawing.Point(16, 232);
            this.VelocityExponentLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VelocityExponentLabel.Name = "VelocityExponentLabel";
            this.VelocityExponentLabel.Size = new System.Drawing.Size(130, 19);
            this.VelocityExponentLabel.TabIndex = 14;
            this.VelocityExponentLabel.Text = "Velocity Exponent";
            // 
            // PSButtonLabel
            // 
            this.PSButtonLabel.AutoSize = true;
            this.PSButtonLabel.Depth = 0;
            this.PSButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.PSButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PSButtonLabel.Location = new System.Drawing.Point(439, 43);
            this.PSButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.PSButtonLabel.Name = "PSButtonLabel";
            this.PSButtonLabel.Size = new System.Drawing.Size(27, 19);
            this.PSButtonLabel.TabIndex = 16;
            this.PSButtonLabel.Text = "PS";
            // 
            // PSButtonComboBox
            // 
            this.PSButtonComboBox.FormattingEnabled = true;
            this.PSButtonComboBox.Location = new System.Drawing.Point(472, 41);
            this.PSButtonComboBox.Name = "PSButtonComboBox";
            this.PSButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.PSButtonComboBox.TabIndex = 17;
            // 
            // MoveButtonComboBox
            // 
            this.MoveButtonComboBox.FormattingEnabled = true;
            this.MoveButtonComboBox.Location = new System.Drawing.Point(472, 68);
            this.MoveButtonComboBox.Name = "MoveButtonComboBox";
            this.MoveButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.MoveButtonComboBox.TabIndex = 19;
            // 
            // MoveButtonLabel
            // 
            this.MoveButtonLabel.AutoSize = true;
            this.MoveButtonLabel.Depth = 0;
            this.MoveButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.MoveButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.MoveButtonLabel.Location = new System.Drawing.Point(420, 70);
            this.MoveButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.MoveButtonLabel.Name = "MoveButtonLabel";
            this.MoveButtonLabel.Size = new System.Drawing.Size(46, 19);
            this.MoveButtonLabel.TabIndex = 18;
            this.MoveButtonLabel.Text = "Move";
            // 
            // TriangleButtonComboBox
            // 
            this.TriangleButtonComboBox.FormattingEnabled = true;
            this.TriangleButtonComboBox.Location = new System.Drawing.Point(472, 95);
            this.TriangleButtonComboBox.Name = "TriangleButtonComboBox";
            this.TriangleButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.TriangleButtonComboBox.TabIndex = 21;
            // 
            // TriangleButtonLabel
            // 
            this.TriangleButtonLabel.AutoSize = true;
            this.TriangleButtonLabel.Depth = 0;
            this.TriangleButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.TriangleButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TriangleButtonLabel.Location = new System.Drawing.Point(403, 97);
            this.TriangleButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.TriangleButtonLabel.Name = "TriangleButtonLabel";
            this.TriangleButtonLabel.Size = new System.Drawing.Size(63, 19);
            this.TriangleButtonLabel.TabIndex = 20;
            this.TriangleButtonLabel.Text = "Triangle";
            // 
            // SquareButtonComboBox
            // 
            this.SquareButtonComboBox.FormattingEnabled = true;
            this.SquareButtonComboBox.Location = new System.Drawing.Point(472, 122);
            this.SquareButtonComboBox.Name = "SquareButtonComboBox";
            this.SquareButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.SquareButtonComboBox.TabIndex = 23;
            // 
            // SquareButtonLabel
            // 
            this.SquareButtonLabel.AutoSize = true;
            this.SquareButtonLabel.Depth = 0;
            this.SquareButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.SquareButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SquareButtonLabel.Location = new System.Drawing.Point(410, 124);
            this.SquareButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.SquareButtonLabel.Name = "SquareButtonLabel";
            this.SquareButtonLabel.Size = new System.Drawing.Size(56, 19);
            this.SquareButtonLabel.TabIndex = 22;
            this.SquareButtonLabel.Text = "Square";
            // 
            // CircleButtonComboBox
            // 
            this.CircleButtonComboBox.FormattingEnabled = true;
            this.CircleButtonComboBox.Location = new System.Drawing.Point(472, 149);
            this.CircleButtonComboBox.Name = "CircleButtonComboBox";
            this.CircleButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.CircleButtonComboBox.TabIndex = 25;
            // 
            // CircleButtonLabel
            // 
            this.CircleButtonLabel.AutoSize = true;
            this.CircleButtonLabel.Depth = 0;
            this.CircleButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.CircleButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.CircleButtonLabel.Location = new System.Drawing.Point(418, 151);
            this.CircleButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.CircleButtonLabel.Name = "CircleButtonLabel";
            this.CircleButtonLabel.Size = new System.Drawing.Size(48, 19);
            this.CircleButtonLabel.TabIndex = 24;
            this.CircleButtonLabel.Text = "Circle";
            // 
            // CrossButtonComboBox
            // 
            this.CrossButtonComboBox.FormattingEnabled = true;
            this.CrossButtonComboBox.Location = new System.Drawing.Point(472, 176);
            this.CrossButtonComboBox.Name = "CrossButtonComboBox";
            this.CrossButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.CrossButtonComboBox.TabIndex = 27;
            // 
            // CrossButtonLabel
            // 
            this.CrossButtonLabel.AutoSize = true;
            this.CrossButtonLabel.Depth = 0;
            this.CrossButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.CrossButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.CrossButtonLabel.Location = new System.Drawing.Point(417, 178);
            this.CrossButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.CrossButtonLabel.Name = "CrossButtonLabel";
            this.CrossButtonLabel.Size = new System.Drawing.Size(49, 19);
            this.CrossButtonLabel.TabIndex = 26;
            this.CrossButtonLabel.Text = "Cross";
            // 
            // SelectButtonComboBox
            // 
            this.SelectButtonComboBox.FormattingEnabled = true;
            this.SelectButtonComboBox.Location = new System.Drawing.Point(472, 203);
            this.SelectButtonComboBox.Name = "SelectButtonComboBox";
            this.SelectButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.SelectButtonComboBox.TabIndex = 29;
            // 
            // SelectButtonLabel
            // 
            this.SelectButtonLabel.AutoSize = true;
            this.SelectButtonLabel.Depth = 0;
            this.SelectButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.SelectButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SelectButtonLabel.Location = new System.Drawing.Point(415, 206);
            this.SelectButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.SelectButtonLabel.Name = "SelectButtonLabel";
            this.SelectButtonLabel.Size = new System.Drawing.Size(51, 19);
            this.SelectButtonLabel.TabIndex = 28;
            this.SelectButtonLabel.Text = "Select";
            // 
            // StartButtonComboBox
            // 
            this.StartButtonComboBox.FormattingEnabled = true;
            this.StartButtonComboBox.Location = new System.Drawing.Point(472, 230);
            this.StartButtonComboBox.Name = "StartButtonComboBox";
            this.StartButtonComboBox.Size = new System.Drawing.Size(155, 21);
            this.StartButtonComboBox.TabIndex = 31;
            // 
            // StartButtonLabel
            // 
            this.StartButtonLabel.AutoSize = true;
            this.StartButtonLabel.Depth = 0;
            this.StartButtonLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.StartButtonLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.StartButtonLabel.Location = new System.Drawing.Point(425, 232);
            this.StartButtonLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.StartButtonLabel.Name = "StartButtonLabel";
            this.StartButtonLabel.Size = new System.Drawing.Size(41, 19);
            this.StartButtonLabel.TabIndex = 30;
            this.StartButtonLabel.Text = "Start";
            // 
            // TouchpadMappingLabel
            // 
            this.TouchpadMappingLabel.AutoSize = true;
            this.TouchpadMappingLabel.Depth = 0;
            this.TouchpadMappingLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.TouchpadMappingLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TouchpadMappingLabel.Location = new System.Drawing.Point(391, 13);
            this.TouchpadMappingLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.TouchpadMappingLabel.Name = "TouchpadMappingLabel";
            this.TouchpadMappingLabel.Size = new System.Drawing.Size(258, 19);
            this.TouchpadMappingLabel.TabIndex = 32;
            this.TouchpadMappingLabel.Text = "Button to Virtual Touchpad Mappings";
            // 
            // DisableControllerCheckBox
            // 
            this.DisableControllerCheckBox.AutoSize = true;
            this.DisableControllerCheckBox.Depth = 0;
            this.DisableControllerCheckBox.Font = new System.Drawing.Font("Roboto", 10F);
            this.DisableControllerCheckBox.Location = new System.Drawing.Point(20, 36);
            this.DisableControllerCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.DisableControllerCheckBox.MouseLocation = new System.Drawing.Point(-1, -1);
            this.DisableControllerCheckBox.MouseState = MaterialSkin.MouseState.HOVER;
            this.DisableControllerCheckBox.Name = "DisableControllerCheckBox";
            this.DisableControllerCheckBox.Ripple = true;
            this.DisableControllerCheckBox.Size = new System.Drawing.Size(140, 30);
            this.DisableControllerCheckBox.TabIndex = 69;
            this.DisableControllerCheckBox.Text = "Disable Controller";
            this.DisableControllerCheckBox.UseVisualStyleBackColor = true;
            this.DisableControllerCheckBox.CheckedChanged += new System.EventHandler(this.DisableControllerCheckBox_CheckedChanged);
            // 
            // PSMoveControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DisableControllerCheckBox);
            this.Controls.Add(this.TouchpadMappingLabel);
            this.Controls.Add(this.StartButtonComboBox);
            this.Controls.Add(this.StartButtonLabel);
            this.Controls.Add(this.SelectButtonComboBox);
            this.Controls.Add(this.SelectButtonLabel);
            this.Controls.Add(this.CrossButtonComboBox);
            this.Controls.Add(this.CrossButtonLabel);
            this.Controls.Add(this.CircleButtonComboBox);
            this.Controls.Add(this.CircleButtonLabel);
            this.Controls.Add(this.SquareButtonComboBox);
            this.Controls.Add(this.SquareButtonLabel);
            this.Controls.Add(this.TriangleButtonComboBox);
            this.Controls.Add(this.TriangleButtonLabel);
            this.Controls.Add(this.MoveButtonComboBox);
            this.Controls.Add(this.MoveButtonLabel);
            this.Controls.Add(this.PSButtonComboBox);
            this.Controls.Add(this.PSButtonLabel);
            this.Controls.Add(this.VelocityExponentTextField);
            this.Controls.Add(this.VelocityExponentLabel);
            this.Controls.Add(this.VelocityMultiplierTextField);
            this.Controls.Add(this.VelocityMultiplierLabel);
            this.Controls.Add(this.VirtualTumbstickScaleTextField);
            this.Controls.Add(this.VirtualThumbstickScaleLabel);
            this.Controls.Add(this.TouchpadPressDelayCheckBox);
            this.Controls.Add(this.ZRotate90CheckBox);
            this.Controls.Add(this.ExtendZTextField);
            this.Controls.Add(this.ExtendZLabel);
            this.Controls.Add(this.ExtendYTextField);
            this.Controls.Add(this.ExtendYLabel);
            this.Controls.Add(this.PSMoveControllerLabel);
            this.Controls.Add(this.RumbleSuppressedCheckBox);
            this.Name = "PSMoveControllerPanel";
            this.Size = new System.Drawing.Size(734, 320);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialCheckBox RumbleSuppressedCheckBox;
        private MaterialSkin.Controls.MaterialLabel PSMoveControllerLabel;
        private MaterialSkin.Controls.MaterialLabel ExtendYLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField ExtendYTextField;
        private MaterialSkin.Controls.MaterialSingleLineTextField ExtendZTextField;
        private MaterialSkin.Controls.MaterialLabel ExtendZLabel;
        private MaterialSkin.Controls.MaterialCheckBox ZRotate90CheckBox;
        private MaterialSkin.Controls.MaterialCheckBox TouchpadPressDelayCheckBox;
        private MaterialSkin.Controls.MaterialSingleLineTextField VirtualTumbstickScaleTextField;
        private MaterialSkin.Controls.MaterialLabel VirtualThumbstickScaleLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField VelocityMultiplierTextField;
        private MaterialSkin.Controls.MaterialLabel VelocityMultiplierLabel;
        private MaterialSkin.Controls.MaterialSingleLineTextField VelocityExponentTextField;
        private MaterialSkin.Controls.MaterialLabel VelocityExponentLabel;
        private MaterialSkin.Controls.MaterialLabel PSButtonLabel;
        private System.Windows.Forms.ComboBox PSButtonComboBox;
        private System.Windows.Forms.ComboBox MoveButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel MoveButtonLabel;
        private System.Windows.Forms.ComboBox TriangleButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel TriangleButtonLabel;
        private System.Windows.Forms.ComboBox SquareButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel SquareButtonLabel;
        private System.Windows.Forms.ComboBox CircleButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel CircleButtonLabel;
        private System.Windows.Forms.ComboBox CrossButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel CrossButtonLabel;
        private System.Windows.Forms.ComboBox SelectButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel SelectButtonLabel;
        private System.Windows.Forms.ComboBox StartButtonComboBox;
        private MaterialSkin.Controls.MaterialLabel StartButtonLabel;
        private MaterialSkin.Controls.MaterialLabel TouchpadMappingLabel;
        private MaterialSkin.Controls.MaterialCheckBox DisableControllerCheckBox;
    }
}

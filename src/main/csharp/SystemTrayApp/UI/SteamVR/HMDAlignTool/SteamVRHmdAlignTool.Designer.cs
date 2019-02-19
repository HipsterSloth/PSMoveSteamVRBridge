namespace SystemTrayApp
{
    partial class SteamVRHmdAlignTool
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InstructionsBodyLabel = new MaterialSkin.Controls.MaterialLabel();
            this.InstructionsHeaderLabel = new MaterialSkin.Controls.MaterialLabel();
            this.CancelButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.OkButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SamplingProgressBar = new System.Windows.Forms.ProgressBar();
            this.IdentifyControllerButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SuspendLayout();
            // 
            // InstructionsBodyLabel
            // 
            this.InstructionsBodyLabel.AutoSize = true;
            this.InstructionsBodyLabel.Depth = 0;
            this.InstructionsBodyLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.InstructionsBodyLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.InstructionsBodyLabel.Location = new System.Drawing.Point(418, 145);
            this.InstructionsBodyLabel.MaximumSize = new System.Drawing.Size(320, 400);
            this.InstructionsBodyLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.InstructionsBodyLabel.Name = "InstructionsBodyLabel";
            this.InstructionsBodyLabel.Size = new System.Drawing.Size(126, 19);
            this.InstructionsBodyLabel.TabIndex = 11;
            this.InstructionsBodyLabel.Text = "Instructions Body";
            // 
            // InstructionsHeaderLabel
            // 
            this.InstructionsHeaderLabel.AutoSize = true;
            this.InstructionsHeaderLabel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.InstructionsHeaderLabel.Depth = 0;
            this.InstructionsHeaderLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.InstructionsHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.InstructionsHeaderLabel.Location = new System.Drawing.Point(477, 99);
            this.InstructionsHeaderLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.InstructionsHeaderLabel.Name = "InstructionsHeaderLabel";
            this.InstructionsHeaderLabel.Size = new System.Drawing.Size(141, 19);
            this.InstructionsHeaderLabel.TabIndex = 10;
            this.InstructionsHeaderLabel.Text = "Instructions Header";
            // 
            // CancelButton
            // 
            this.CancelButton.AutoSize = true;
            this.CancelButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton.Depth = 0;
            this.CancelButton.Icon = null;
            this.CancelButton.Location = new System.Drawing.Point(470, 360);
            this.CancelButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Primary = true;
            this.CancelButton.Size = new System.Drawing.Size(73, 36);
            this.CancelButton.TabIndex = 9;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.AutoSize = true;
            this.OkButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.OkButton.Depth = 0;
            this.OkButton.Icon = null;
            this.OkButton.Location = new System.Drawing.Point(600, 360);
            this.OkButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.OkButton.Name = "OkButton";
            this.OkButton.Primary = true;
            this.OkButton.Size = new System.Drawing.Size(39, 36);
            this.OkButton.TabIndex = 8;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // SamplingProgressBar
            // 
            this.SamplingProgressBar.Location = new System.Drawing.Point(422, 58);
            this.SamplingProgressBar.Name = "SamplingProgressBar";
            this.SamplingProgressBar.Size = new System.Drawing.Size(267, 23);
            this.SamplingProgressBar.TabIndex = 12;
            this.SamplingProgressBar.Visible = false;
            // 
            // IdentifyControllerButton
            // 
            this.IdentifyControllerButton.AutoSize = true;
            this.IdentifyControllerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.IdentifyControllerButton.Depth = 0;
            this.IdentifyControllerButton.Icon = null;
            this.IdentifyControllerButton.Location = new System.Drawing.Point(470, 256);
            this.IdentifyControllerButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.IdentifyControllerButton.Name = "IdentifyControllerButton";
            this.IdentifyControllerButton.Primary = true;
            this.IdentifyControllerButton.Size = new System.Drawing.Size(169, 36);
            this.IdentifyControllerButton.TabIndex = 13;
            this.IdentifyControllerButton.Text = "Identify Controller";
            this.IdentifyControllerButton.UseVisualStyleBackColor = true;
            this.IdentifyControllerButton.Click += new System.EventHandler(this.IdentifyControllerButton_Click);
            // 
            // SteamVRHmdAlignTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.IdentifyControllerButton);
            this.Controls.Add(this.SamplingProgressBar);
            this.Controls.Add(this.InstructionsBodyLabel);
            this.Controls.Add(this.InstructionsHeaderLabel);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.Name = "SteamVRHmdAlignTool";
            this.Size = new System.Drawing.Size(747, 415);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SteamVROpenGLFrame SteamVRFrame;
        private MaterialSkin.Controls.MaterialLabel InstructionsBodyLabel;
        private MaterialSkin.Controls.MaterialLabel InstructionsHeaderLabel;
        private MaterialSkin.Controls.MaterialRaisedButton CancelButton;
        private MaterialSkin.Controls.MaterialRaisedButton OkButton;
        private System.Windows.Forms.ProgressBar SamplingProgressBar;
        private MaterialSkin.Controls.MaterialRaisedButton IdentifyControllerButton;
    }
}

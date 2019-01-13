namespace SystemTrayApp
{
    partial class ControllerPanel
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
            this.ContentPanel = new System.Windows.Forms.Panel();
            this.ReloadControllerSettingsButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SaveControllerSettingsButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.ControllerLabel = new MaterialSkin.Controls.MaterialLabel();
            this.NextControllerButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.PreviousControllerButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SuspendLayout();
            // 
            // ContentPanel
            // 
            this.ContentPanel.Location = new System.Drawing.Point(7, 48);
            this.ContentPanel.Name = "ContentPanel";
            this.ContentPanel.Size = new System.Drawing.Size(734, 320);
            this.ContentPanel.TabIndex = 16;
            // 
            // ReloadControllerSettingsButton
            // 
            this.ReloadControllerSettingsButton.AutoSize = true;
            this.ReloadControllerSettingsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ReloadControllerSettingsButton.Depth = 0;
            this.ReloadControllerSettingsButton.Icon = null;
            this.ReloadControllerSettingsButton.Location = new System.Drawing.Point(608, 374);
            this.ReloadControllerSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.ReloadControllerSettingsButton.Name = "ReloadControllerSettingsButton";
            this.ReloadControllerSettingsButton.Primary = true;
            this.ReloadControllerSettingsButton.Size = new System.Drawing.Size(72, 36);
            this.ReloadControllerSettingsButton.TabIndex = 15;
            this.ReloadControllerSettingsButton.Text = "Reload";
            this.ReloadControllerSettingsButton.UseVisualStyleBackColor = true;
            this.ReloadControllerSettingsButton.Click += new System.EventHandler(this.ReloadControllerSettingsButton_Click);
            // 
            // SaveControllerSettingsButton
            // 
            this.SaveControllerSettingsButton.AutoSize = true;
            this.SaveControllerSettingsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SaveControllerSettingsButton.Depth = 0;
            this.SaveControllerSettingsButton.Icon = null;
            this.SaveControllerSettingsButton.Location = new System.Drawing.Point(686, 374);
            this.SaveControllerSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SaveControllerSettingsButton.Name = "SaveControllerSettingsButton";
            this.SaveControllerSettingsButton.Primary = true;
            this.SaveControllerSettingsButton.Size = new System.Drawing.Size(55, 36);
            this.SaveControllerSettingsButton.TabIndex = 14;
            this.SaveControllerSettingsButton.Text = "Save";
            this.SaveControllerSettingsButton.UseVisualStyleBackColor = true;
            this.SaveControllerSettingsButton.Click += new System.EventHandler(this.SaveControllerSettingsButton_Click);
            // 
            // ControllerLabel
            // 
            this.ControllerLabel.AutoSize = true;
            this.ControllerLabel.Depth = 0;
            this.ControllerLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.ControllerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ControllerLabel.Location = new System.Drawing.Point(62, 13);
            this.ControllerLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.ControllerLabel.Name = "ControllerLabel";
            this.ControllerLabel.Size = new System.Drawing.Size(120, 19);
            this.ControllerLabel.TabIndex = 13;
            this.ControllerLabel.Text = "Controller Name";
            this.ControllerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NextControllerButton
            // 
            this.NextControllerButton.AutoSize = true;
            this.NextControllerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.NextControllerButton.Depth = 0;
            this.NextControllerButton.Icon = null;
            this.NextControllerButton.Location = new System.Drawing.Point(188, 7);
            this.NextControllerButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.NextControllerButton.Name = "NextControllerButton";
            this.NextControllerButton.Primary = true;
            this.NextControllerButton.Size = new System.Drawing.Size(35, 36);
            this.NextControllerButton.TabIndex = 12;
            this.NextControllerButton.Text = ">>";
            this.NextControllerButton.UseVisualStyleBackColor = true;
            this.NextControllerButton.Click += new System.EventHandler(this.NextControllerButton_Click);
            // 
            // PreviousControllerButton
            // 
            this.PreviousControllerButton.AutoSize = true;
            this.PreviousControllerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PreviousControllerButton.Depth = 0;
            this.PreviousControllerButton.Icon = null;
            this.PreviousControllerButton.Location = new System.Drawing.Point(6, 5);
            this.PreviousControllerButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.PreviousControllerButton.Name = "PreviousControllerButton";
            this.PreviousControllerButton.Primary = true;
            this.PreviousControllerButton.Size = new System.Drawing.Size(35, 36);
            this.PreviousControllerButton.TabIndex = 11;
            this.PreviousControllerButton.Text = "<<";
            this.PreviousControllerButton.UseVisualStyleBackColor = true;
            this.PreviousControllerButton.Click += new System.EventHandler(this.PreviousControllerButton_Click);
            // 
            // ControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ContentPanel);
            this.Controls.Add(this.ReloadControllerSettingsButton);
            this.Controls.Add(this.SaveControllerSettingsButton);
            this.Controls.Add(this.ControllerLabel);
            this.Controls.Add(this.NextControllerButton);
            this.Controls.Add(this.PreviousControllerButton);
            this.Name = "ControllerPanel";
            this.Size = new System.Drawing.Size(747, 415);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel ContentPanel;
        private MaterialSkin.Controls.MaterialRaisedButton ReloadControllerSettingsButton;
        private MaterialSkin.Controls.MaterialRaisedButton SaveControllerSettingsButton;
        private MaterialSkin.Controls.MaterialLabel ControllerLabel;
        private MaterialSkin.Controls.MaterialRaisedButton NextControllerButton;
        private MaterialSkin.Controls.MaterialRaisedButton PreviousControllerButton;
    }
}

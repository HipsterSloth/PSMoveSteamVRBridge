namespace SystemTrayApp
{
    partial class SteamVRDeviceEntry
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
            this.HMD_icon = new System.Windows.Forms.PictureBox();
            this.DeviceNameLabel = new MaterialSkin.Controls.MaterialLabel();
            ((System.ComponentModel.ISupportInitialize)(this.HMD_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // HMD_icon
            // 
            this.HMD_icon.Location = new System.Drawing.Point(3, 3);
            this.HMD_icon.Name = "HMD_icon";
            this.HMD_icon.Size = new System.Drawing.Size(64, 64);
            this.HMD_icon.TabIndex = 28;
            this.HMD_icon.TabStop = false;
            // 
            // DeviceNameLabel
            // 
            this.DeviceNameLabel.AutoSize = true;
            this.DeviceNameLabel.Depth = 0;
            this.DeviceNameLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.DeviceNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.DeviceNameLabel.Location = new System.Drawing.Point(73, 3);
            this.DeviceNameLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.DeviceNameLabel.Name = "DeviceNameLabel";
            this.DeviceNameLabel.Size = new System.Drawing.Size(98, 19);
            this.DeviceNameLabel.TabIndex = 66;
            this.DeviceNameLabel.Text = "Device Name";
            // 
            // SteamVRDeviceEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DeviceNameLabel);
            this.Controls.Add(this.HMD_icon);
            this.Name = "SteamVRDeviceEntry";
            this.Size = new System.Drawing.Size(363, 75);
            ((System.ComponentModel.ISupportInitialize)(this.HMD_icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox HMD_icon;
        private MaterialSkin.Controls.MaterialLabel DeviceNameLabel;
    }
}

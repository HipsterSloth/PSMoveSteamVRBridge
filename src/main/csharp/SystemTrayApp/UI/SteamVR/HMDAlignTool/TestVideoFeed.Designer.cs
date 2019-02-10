namespace SystemTrayApp
{
    partial class TestVideoFeed
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
            this.VideoFrame = new System.Windows.Forms.Panel();
            this.PrevCameraButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.NextCameraButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.VideoFrameLabel = new MaterialSkin.Controls.MaterialLabel();
            this.OkButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.CancelButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SuspendLayout();
            // 
            // VideoFrame
            // 
            this.VideoFrame.Location = new System.Drawing.Point(22, 58);
            this.VideoFrame.Name = "VideoFrame";
            this.VideoFrame.Size = new System.Drawing.Size(320, 240);
            this.VideoFrame.TabIndex = 0;
            // 
            // PrevCameraButton
            // 
            this.PrevCameraButton.AutoSize = true;
            this.PrevCameraButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PrevCameraButton.Depth = 0;
            this.PrevCameraButton.Icon = null;
            this.PrevCameraButton.Location = new System.Drawing.Point(22, 304);
            this.PrevCameraButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.PrevCameraButton.Name = "PrevCameraButton";
            this.PrevCameraButton.Primary = true;
            this.PrevCameraButton.Size = new System.Drawing.Size(145, 36);
            this.PrevCameraButton.TabIndex = 1;
            this.PrevCameraButton.Text = "Previous Camera";
            this.PrevCameraButton.UseVisualStyleBackColor = true;
            this.PrevCameraButton.Click += new System.EventHandler(this.PrevCameraButton_Click);
            // 
            // NextCameraButton
            // 
            this.NextCameraButton.AutoSize = true;
            this.NextCameraButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.NextCameraButton.Depth = 0;
            this.NextCameraButton.Icon = null;
            this.NextCameraButton.Location = new System.Drawing.Point(227, 304);
            this.NextCameraButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.NextCameraButton.Name = "NextCameraButton";
            this.NextCameraButton.Primary = true;
            this.NextCameraButton.Size = new System.Drawing.Size(115, 36);
            this.NextCameraButton.TabIndex = 2;
            this.NextCameraButton.Text = "Next Camera";
            this.NextCameraButton.UseVisualStyleBackColor = true;
            this.NextCameraButton.Click += new System.EventHandler(this.NextCameraButton_Click);
            // 
            // VideoFrameLabel
            // 
            this.VideoFrameLabel.AutoSize = true;
            this.VideoFrameLabel.Depth = 0;
            this.VideoFrameLabel.Font = new System.Drawing.Font("Roboto", 11F);
            this.VideoFrameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.VideoFrameLabel.Location = new System.Drawing.Point(144, 36);
            this.VideoFrameLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.VideoFrameLabel.Name = "VideoFrameLabel";
            this.VideoFrameLabel.Size = new System.Drawing.Size(82, 19);
            this.VideoFrameLabel.TabIndex = 3;
            this.VideoFrameLabel.Text = "Camera #1";
            // 
            // OkButton
            // 
            this.OkButton.AutoSize = true;
            this.OkButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.OkButton.Depth = 0;
            this.OkButton.Icon = null;
            this.OkButton.Location = new System.Drawing.Point(692, 364);
            this.OkButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.OkButton.Name = "OkButton";
            this.OkButton.Primary = true;
            this.OkButton.Size = new System.Drawing.Size(39, 36);
            this.OkButton.TabIndex = 4;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.AutoSize = true;
            this.CancelButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton.Depth = 0;
            this.CancelButton.Icon = null;
            this.CancelButton.Location = new System.Drawing.Point(613, 364);
            this.CancelButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Primary = true;
            this.CancelButton.Size = new System.Drawing.Size(73, 36);
            this.CancelButton.TabIndex = 5;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // TestVideoFeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.VideoFrameLabel);
            this.Controls.Add(this.NextCameraButton);
            this.Controls.Add(this.PrevCameraButton);
            this.Controls.Add(this.VideoFrame);
            this.Name = "TestVideoFeed";
            this.Size = new System.Drawing.Size(747, 415);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel VideoFrame;
        private MaterialSkin.Controls.MaterialRaisedButton PrevCameraButton;
        private MaterialSkin.Controls.MaterialRaisedButton NextCameraButton;
        private MaterialSkin.Controls.MaterialLabel VideoFrameLabel;
        private MaterialSkin.Controls.MaterialRaisedButton OkButton;
        private MaterialSkin.Controls.MaterialRaisedButton CancelButton;
    }
}

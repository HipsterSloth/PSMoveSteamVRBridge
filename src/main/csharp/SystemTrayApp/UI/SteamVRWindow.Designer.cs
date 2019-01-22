using OpenGL;

namespace SystemTrayApp
{
    partial class SteamVRWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControl = new OpenGL.GlControl();
            this.glPanel = new System.Windows.Forms.Panel();
            this.glPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.Animation = true;
            this.glControl.AnimationTimer = false;
            this.glControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.glControl.ColorBits = ((uint)(24u));
            this.glControl.DepthBits = ((uint)(24u));
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.MultisampleBits = ((uint)(0u));
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(781, 532);
            this.glControl.StencilBits = ((uint)(0u));
            this.glControl.TabIndex = 0;
            this.glControl.ContextCreated += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl_ContextCreated);
            this.glControl.ContextDestroying += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl_ContextDestroying);
            this.glControl.Render += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl_Render);
            this.glControl.ContextUpdate += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl_ContextUpdate);
            this.glControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl_KeyDown);
            this.glControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl_KeyUp);
            this.glControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseWheel);
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseUp);

            // 
            // glPanel
            // 
            this.glPanel.Controls.Add(this.glControl);
            this.glPanel.Location = new System.Drawing.Point(0, 64);
            this.glPanel.Name = "glPanel";
            this.glPanel.Size = new System.Drawing.Size(781, 500);
            this.glPanel.TabIndex = 1;
            // 
            // SteamVRWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 564);
            this.Controls.Add(this.glPanel);
            this.Name = "SteamVRWindow";
            this.Text = "SteamVR Window";
            this.glPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal GlControl glControl;
        private System.Windows.Forms.Panel glPanel;
    }
}
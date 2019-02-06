namespace SystemTrayApp
{
    partial class ButtonMapping
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
            this.TrackpadActionComboBox = new System.Windows.Forms.ComboBox();
            this.PSButtonComboBox = new System.Windows.Forms.ComboBox();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TrackpadActionComboBox
            // 
            this.TrackpadActionComboBox.FormattingEnabled = true;
            this.TrackpadActionComboBox.Location = new System.Drawing.Point(107, 2);
            this.TrackpadActionComboBox.Name = "TrackpadActionComboBox";
            this.TrackpadActionComboBox.Size = new System.Drawing.Size(94, 21);
            this.TrackpadActionComboBox.TabIndex = 0;
            // 
            // PSButtonComboBox
            // 
            this.PSButtonComboBox.FormattingEnabled = true;
            this.PSButtonComboBox.Location = new System.Drawing.Point(7, 2);
            this.PSButtonComboBox.Name = "PSButtonComboBox";
            this.PSButtonComboBox.Size = new System.Drawing.Size(94, 21);
            this.PSButtonComboBox.TabIndex = 1;
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(206, 1);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(23, 23);
            this.DeleteButton.TabIndex = 2;
            this.DeleteButton.Text = "X";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ButtonMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.TrackpadActionComboBox);
            this.Controls.Add(this.PSButtonComboBox);
            this.Name = "ButtonMapping";
            this.Size = new System.Drawing.Size(233, 27);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox TrackpadActionComboBox;
        private System.Windows.Forms.ComboBox PSButtonComboBox;
        private System.Windows.Forms.Button DeleteButton;
    }
}

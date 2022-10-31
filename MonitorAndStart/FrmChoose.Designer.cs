namespace MonitorAndStart
{
    partial class FrmChoose
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnFile = new System.Windows.Forms.Button();
            this.BtnConnection = new System.Windows.Forms.Button();
            this.BtnStuck = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnFile
            // 
            this.BtnFile.Location = new System.Drawing.Point(12, 12);
            this.BtnFile.Name = "BtnFile";
            this.BtnFile.Size = new System.Drawing.Size(75, 23);
            this.BtnFile.TabIndex = 0;
            this.BtnFile.Text = "File";
            this.BtnFile.UseVisualStyleBackColor = true;
            this.BtnFile.Click += new System.EventHandler(this.BtnFile_Click);
            // 
            // BtnConnection
            // 
            this.BtnConnection.Location = new System.Drawing.Point(247, 12);
            this.BtnConnection.Name = "BtnConnection";
            this.BtnConnection.Size = new System.Drawing.Size(75, 23);
            this.BtnConnection.TabIndex = 1;
            this.BtnConnection.Text = "Connection";
            this.BtnConnection.UseVisualStyleBackColor = true;
            this.BtnConnection.Click += new System.EventHandler(this.BtnConnection_Click);
            // 
            // BtnStuck
            // 
            this.BtnStuck.Location = new System.Drawing.Point(129, 12);
            this.BtnStuck.Name = "BtnStuck";
            this.BtnStuck.Size = new System.Drawing.Size(75, 23);
            this.BtnStuck.TabIndex = 2;
            this.BtnStuck.Text = "Stuck File";
            this.BtnStuck.UseVisualStyleBackColor = true;
            this.BtnStuck.Click += new System.EventHandler(this.BtnStuck_Click);
            // 
            // FrmChoose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 53);
            this.ControlBox = false;
            this.Controls.Add(this.BtnStuck);
            this.Controls.Add(this.BtnConnection);
            this.Controls.Add(this.BtnFile);
            this.Name = "FrmChoose";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "What are we adding?";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnFile;
        private System.Windows.Forms.Button BtnConnection;
        private System.Windows.Forms.Button BtnStuck;
    }
}
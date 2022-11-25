namespace MonitorAndStart
{
    partial class FrmStuckFile
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
            this.BtnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CmbDuration = new System.Windows.Forms.ComboBox();
            this.NUD = new System.Windows.Forms.NumericUpDown();
            this.BtnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NUD)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(369, 12);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 0;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "File";
            // 
            // TxtFile
            // 
            this.TxtFile.Location = new System.Drawing.Point(41, 12);
            this.TxtFile.Name = "TxtFile";
            this.TxtFile.Size = new System.Drawing.Size(289, 20);
            this.TxtFile.TabIndex = 2;
            this.TxtFile.Click += new System.EventHandler(this.TxtFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Stuck Longer Than";
            // 
            // CmbDuration
            // 
            this.CmbDuration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbDuration.FormattingEnabled = true;
            this.CmbDuration.Items.AddRange(new object[] {
            "Minutes",
            "Hours",
            "Days"});
            this.CmbDuration.Location = new System.Drawing.Point(170, 40);
            this.CmbDuration.Name = "CmbDuration";
            this.CmbDuration.Size = new System.Drawing.Size(160, 21);
            this.CmbDuration.TabIndex = 4;
            // 
            // NUD
            // 
            this.NUD.Location = new System.Drawing.Point(117, 41);
            this.NUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD.Name = "NUD";
            this.NUD.Size = new System.Drawing.Size(47, 20);
            this.NUD.TabIndex = 5;
            this.NUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(369, 38);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 6;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // FrmStuckFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 78);
            this.ControlBox = false;
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.NUD);
            this.Controls.Add(this.CmbDuration);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmStuckFile";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Stuck File";
            ((System.ComponentModel.ISupportInitialize)(this.NUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnCancel;
        public System.Windows.Forms.ComboBox CmbDuration;
        public System.Windows.Forms.NumericUpDown NUD;
        public System.Windows.Forms.TextBox TxtFile;
    }
}
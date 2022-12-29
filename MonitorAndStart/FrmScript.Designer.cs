
namespace MonitorAndStart
{
    partial class FrmScript
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
            this.TxtApplication = new System.Windows.Forms.TextBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.TxtParameters = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ChkHidden = new System.Windows.Forms.CheckBox();
            this.NUD = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpStartFrom = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.NUD)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(371, 13);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 0;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // TxtApplication
            // 
            this.TxtApplication.Location = new System.Drawing.Point(78, 15);
            this.TxtApplication.Name = "TxtApplication";
            this.TxtApplication.Size = new System.Drawing.Size(276, 20);
            this.TxtApplication.TabIndex = 1;
            this.TxtApplication.Click += new System.EventHandler(this.TxtApplicatoin_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(371, 42);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 2;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // TxtParameters
            // 
            this.TxtParameters.Location = new System.Drawing.Point(78, 44);
            this.TxtParameters.Name = "TxtParameters";
            this.TxtParameters.Size = new System.Drawing.Size(276, 20);
            this.TxtParameters.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Application";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Paremeters";
            // 
            // ChkHidden
            // 
            this.ChkHidden.AutoSize = true;
            this.ChkHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ChkHidden.Location = new System.Drawing.Point(352, 72);
            this.ChkHidden.Name = "ChkHidden";
            this.ChkHidden.Size = new System.Drawing.Size(95, 19);
            this.ChkHidden.TabIndex = 7;
            this.ChkHidden.Text = "Run Hidden";
            this.ChkHidden.UseVisualStyleBackColor = true;
            // 
            // NUD
            // 
            this.NUD.Location = new System.Drawing.Point(80, 70);
            this.NUD.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.NUD.Name = "NUD";
            this.NUD.Size = new System.Drawing.Size(38, 20);
            this.NUD.TabIndex = 8;
            this.NUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Run every";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(122, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "hours, starting from";
            // 
            // dtpStartFrom
            // 
            this.dtpStartFrom.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpStartFrom.Location = new System.Drawing.Point(237, 71);
            this.dtpStartFrom.Name = "dtpStartFrom";
            this.dtpStartFrom.Size = new System.Drawing.Size(110, 20);
            this.dtpStartFrom.TabIndex = 12;
            // 
            // FrmScript
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(458, 103);
            this.ControlBox = false;
            this.Controls.Add(this.dtpStartFrom);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NUD);
            this.Controls.Add(this.ChkHidden);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtParameters);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.TxtApplication);
            this.Controls.Add(this.BtnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmScript";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Parameters";
            ((System.ComponentModel.ISupportInitialize)(this.NUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox TxtApplication;
        public System.Windows.Forms.TextBox TxtParameters;
        public System.Windows.Forms.CheckBox ChkHidden;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown NUD;
        public System.Windows.Forms.DateTimePicker dtpStartFrom;
    }
}
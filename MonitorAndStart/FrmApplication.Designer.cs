
namespace MonitorAndStart
{
    partial class FrmApplication
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
            this.TxtApplicatoin = new System.Windows.Forms.TextBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.TxtParameters = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ChkRestart = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(279, 12);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 0;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // TxtApplicatoin
            // 
            this.TxtApplicatoin.Location = new System.Drawing.Point(78, 15);
            this.TxtApplicatoin.Name = "TxtApplicatoin";
            this.TxtApplicatoin.Size = new System.Drawing.Size(195, 20);
            this.TxtApplicatoin.TabIndex = 1;
            this.TxtApplicatoin.Click += new System.EventHandler(this.TxtApplicatoin_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(279, 41);
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
            this.TxtParameters.Size = new System.Drawing.Size(195, 20);
            this.TxtParameters.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Application";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Paremeters";
            // 
            // ChkRestart
            // 
            this.ChkRestart.AutoSize = true;
            this.ChkRestart.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ChkRestart.Location = new System.Drawing.Point(27, 70);
            this.ChkRestart.Name = "ChkRestart";
            this.ChkRestart.Size = new System.Drawing.Size(66, 17);
            this.ChkRestart.TabIndex = 6;
            this.ChkRestart.Text = "Restart?";
            this.ChkRestart.UseVisualStyleBackColor = true;
            // 
            // FrmApplication
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(366, 108);
            this.ControlBox = false;
            this.Controls.Add(this.ChkRestart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtParameters);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.TxtApplicatoin);
            this.Controls.Add(this.BtnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmApplication";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Parameters";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox TxtApplicatoin;
        public System.Windows.Forms.TextBox TxtParameters;
        public System.Windows.Forms.CheckBox ChkRestart;
    }
}
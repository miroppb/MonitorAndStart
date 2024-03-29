﻿using System;

namespace MonitorAndStart
{
	partial class FrmTime
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
			this.BtnCancel = new System.Windows.Forms.Button();
			this.BtnOK = new System.Windows.Forms.Button();
			this.Dtp = new System.Windows.Forms.DateTimePicker();
			this.SuspendLayout();
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(185, 41);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 5;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
			// 
			// BtnOK
			// 
			this.BtnOK.Location = new System.Drawing.Point(185, 12);
			this.BtnOK.Name = "BtnOK";
			this.BtnOK.Size = new System.Drawing.Size(75, 23);
			this.BtnOK.TabIndex = 4;
			this.BtnOK.Text = "OK";
			this.BtnOK.UseVisualStyleBackColor = true;
			this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
			// 
			// Dtp
			// 
			this.Dtp.CustomFormat = "hh:mm tt";
			this.Dtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
			this.Dtp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.Dtp.Location = new System.Drawing.Point(12, 24);
			this.Dtp.Name = "Dtp";
			this.Dtp.ShowUpDown = true;
			this.Dtp.Size = new System.Drawing.Size(131, 24);
			this.Dtp.TabIndex = 3;
			// 
			// FrmTime
			// 
			this.AcceptButton = this.BtnOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(272, 74);
			this.ControlBox = false;
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOK);
			this.Controls.Add(this.Dtp);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "FrmTime";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Time...";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOK;
		public System.Windows.Forms.DateTimePicker Dtp;
	}
}
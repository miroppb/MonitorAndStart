﻿namespace MonitorAndStart
{
    partial class FrmMain
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
			this.BtnStartStop = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.BtnAdd = new System.Windows.Forms.Button();
			this.LstItems = new System.Windows.Forms.ListBox();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BtnRem = new System.Windows.Forms.Button();
			this.ChkAdmin = new System.Windows.Forms.CheckBox();
			this.ChkCheckOnStart = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.TxtTimeStart = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.TxtTimeEnd = new System.Windows.Forms.TextBox();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnStartStop
			// 
			this.BtnStartStop.Location = new System.Drawing.Point(215, 12);
			this.BtnStartStop.Name = "BtnStartStop";
			this.BtnStartStop.Size = new System.Drawing.Size(75, 23);
			this.BtnStartStop.TabIndex = 1;
			this.BtnStartStop.Text = "Stop";
			this.BtnStartStop.UseVisualStyleBackColor = true;
			this.BtnStartStop.Click += new System.EventHandler(this.BtnStartStop_Click);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 3600000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// BtnAdd
			// 
			this.BtnAdd.Location = new System.Drawing.Point(332, 12);
			this.BtnAdd.Name = "BtnAdd";
			this.BtnAdd.Size = new System.Drawing.Size(34, 23);
			this.BtnAdd.TabIndex = 2;
			this.BtnAdd.Text = "Add";
			this.BtnAdd.UseVisualStyleBackColor = true;
			this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
			// 
			// LstItems
			// 
			this.LstItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.LstItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LstItems.FormattingEnabled = true;
			this.LstItems.ItemHeight = 18;
			this.LstItems.Location = new System.Drawing.Point(12, 41);
			this.LstItems.Name = "LstItems";
			this.LstItems.Size = new System.Drawing.Size(404, 184);
			this.LstItems.TabIndex = 3;
			this.LstItems.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LstItems_MouseDoubleClick);
			this.LstItems.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LstItems_MouseMove);
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "MonitorAndStart";
			this.notifyIcon1.Visible = true;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.startToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(104, 70);
			// 
			// showToolStripMenuItem
			// 
			this.showToolStripMenuItem.Name = "showToolStripMenuItem";
			this.showToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.showToolStripMenuItem.Text = "Show";
			this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
			// 
			// startToolStripMenuItem
			// 
			this.startToolStripMenuItem.Name = "startToolStripMenuItem";
			this.startToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.startToolStripMenuItem.Text = "Start";
			this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// BtnRem
			// 
			this.BtnRem.Location = new System.Drawing.Point(372, 12);
			this.BtnRem.Name = "BtnRem";
			this.BtnRem.Size = new System.Drawing.Size(43, 23);
			this.BtnRem.TabIndex = 4;
			this.BtnRem.Text = "Rem";
			this.BtnRem.UseVisualStyleBackColor = true;
			this.BtnRem.Click += new System.EventHandler(this.BtnRem_Click);
			// 
			// ChkAdmin
			// 
			this.ChkAdmin.AutoSize = true;
			this.ChkAdmin.Location = new System.Drawing.Point(12, 16);
			this.ChkAdmin.Name = "ChkAdmin";
			this.ChkAdmin.Size = new System.Drawing.Size(92, 17);
			this.ChkAdmin.TabIndex = 5;
			this.ChkAdmin.Text = "Run as Admin";
			this.ChkAdmin.UseVisualStyleBackColor = true;
			this.ChkAdmin.CheckedChanged += new System.EventHandler(this.ChkAdmin_CheckedChanged);
			// 
			// ChkCheckOnStart
			// 
			this.ChkCheckOnStart.AutoSize = true;
			this.ChkCheckOnStart.Location = new System.Drawing.Point(110, 16);
			this.ChkCheckOnStart.Name = "ChkCheckOnStart";
			this.ChkCheckOnStart.Size = new System.Drawing.Size(99, 17);
			this.ChkCheckOnStart.TabIndex = 6;
			this.ChkCheckOnStart.Text = "Check On Start";
			this.ChkCheckOnStart.UseVisualStyleBackColor = true;
			this.ChkCheckOnStart.CheckedChanged += new System.EventHandler(this.ChkCheckOnStart_CheckedChanged);
			// 
			// TxtTimeStart
			// 
			this.TxtTimeStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.TxtTimeStart.Location = new System.Drawing.Point(110, 232);
			this.TxtTimeStart.Name = "TxtTimeStart";
			this.TxtTimeStart.ReadOnly = true;
			this.TxtTimeStart.Size = new System.Drawing.Size(68, 20);
			this.TxtTimeStart.TabIndex = 7;
			this.TxtTimeStart.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TxtTimeStart_MouseDoubleClick);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 235);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Don\'t run between:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(184, 235);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(25, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "and";
			// 
			// TxtTimeEnd
			// 
			this.TxtTimeEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.TxtTimeEnd.Location = new System.Drawing.Point(215, 232);
			this.TxtTimeEnd.Name = "TxtTimeEnd";
			this.TxtTimeEnd.ReadOnly = true;
			this.TxtTimeEnd.Size = new System.Drawing.Size(68, 20);
			this.TxtTimeEnd.TabIndex = 10;
			this.TxtTimeEnd.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TxtTimeEnd_MouseDoubleClick);
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(428, 273);
			this.Controls.Add(this.TxtTimeEnd);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TxtTimeStart);
			this.Controls.Add(this.ChkCheckOnStart);
			this.Controls.Add(this.ChkAdmin);
			this.Controls.Add(this.BtnRem);
			this.Controls.Add(this.LstItems);
			this.Controls.Add(this.BtnAdd);
			this.Controls.Add(this.BtnStartStop);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FrmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Monitor and Start";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
			this.Load += new System.EventHandler(this.FrmMain_Load);
			this.Resize += new System.EventHandler(this.FrmMain_Resize);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnStartStop;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.ListBox LstItems;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button BtnRem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        public System.Windows.Forms.CheckBox ChkAdmin;
        private System.Windows.Forms.CheckBox ChkCheckOnStart;
        private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TextBox TxtTimeStart;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TxtTimeEnd;
	}
}


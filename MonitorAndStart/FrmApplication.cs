using System;
using System.Windows.Forms;

namespace MonitorAndStart
{
    public partial class FrmApplication : Form
    {
        public FrmApplication()
        {
            InitializeComponent();
        }

        private void TxtApplicatoin_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Application|*.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TxtApplication.Text = ofd.FileName;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

using System;
using System.Windows.Forms;

namespace MonitorAndStart
{
    public partial class FrmChoose : Form
    {
        public FrmChoose()
        {
            InitializeComponent();
        }

        private void BtnFile_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes; //File
            Close();
        }

        private void BtnStuck_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No; //Stuck
            Close();
        }

        private void BtnConnection_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel; //Connection
            Close();
        }

        private void BtnService_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort; //Service
            Close();
        }

        private void BtnScript_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

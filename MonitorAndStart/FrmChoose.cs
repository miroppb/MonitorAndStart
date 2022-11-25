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
            this.Close();
        }

        private void BtnStuck_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No; //Stuck
            this.Close();
        }

        private void BtnConnection_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel; //Connection
            this.Close();
        }

        private void BtnService_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort; //Service
            this.Close();
        }
    }
}

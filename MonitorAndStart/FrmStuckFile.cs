using System;
using System.Windows.Forms;

namespace MonitorAndStart
{
    public partial class FrmStuckFile : Form
    {
        public FrmStuckFile()
        {
            InitializeComponent();
        }

        private void TxtFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Any File|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                TxtFile.Text = openFileDialog.FileName;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

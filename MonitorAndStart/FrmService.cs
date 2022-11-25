using System;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;

namespace MonitorAndStart
{
    public partial class FrmService : Form
    {
        public string CurrentService = String.Empty;
        public int CurrentHours = 0;

        public FrmService(string SelectedService, int hours)
        {
            InitializeComponent();
            CmbServices.Items.Clear();
            CmbServices.Items.AddRange(ServiceController.GetServices().Select(x => x.ServiceName).ToArray());

            if (SelectedService != null)
                try
                {
                    CmbServices.SelectedIndex = CmbServices.Items.IndexOf(SelectedService);
                }
                catch { MessageBox.Show($"{SelectedService} isn't present on the list of Services"); }
            if (hours > 0)
                NUD.Value = hours;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            CurrentService = CmbServices.SelectedItem.ToString();
            CurrentHours = (int)NUD.Value;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

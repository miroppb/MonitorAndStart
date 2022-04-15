using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitorAndStart
{
    public partial class FrmMain : Form
    {
        bool die = false;
        bool starting = false;
        public FrmMain()
        {
            InitializeComponent();
            starting = true;
            if (Properties.Settings.Default.Admin)
            {
                CheckAdminStart();
            }
            Log("Hello. Monitor And Start has started!");
            try
            {
                string[] a = Properties.Settings.Default.Items.Split(';');
                foreach (string b in a)
                {
                    if (b != "")
                    {
                        LstItems.Items.Add(b);
                        Log("Added '" + b + "' to list");
                    }
                }
            }
            catch { }
            //timer1_Tick(null, null);
        }

        private void CheckAdminStart()
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("Please escalate Monitor and Start...");
                ProcessStartInfo pp = new ProcessStartInfo();
                pp.FileName = Application.ExecutablePath;
                pp.Verb = "runas";
                Process process = new Process();
                process.StartInfo = pp;
                process.Start();
                die = true;
            }
        }

        public bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Application|*.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FrmApplication f = new FrmApplication();
                f.TxtApplicatoin.Text = ofd.FileName;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LstItems.Items.Add(f.TxtApplicatoin.Text + "," + f.TxtParameters.Text + "," + f.ChkRestart.Checked.ToString());

                    //save items
                    string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
                    Properties.Settings.Default.Items = a;
                    Properties.Settings.Default.Save();

                    Log("Added '" + ofd.FileName + "' to list, and saved");
                }
            }
        }
        private void BtnRem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove '" + LstItems.SelectedItems[0].ToString().Split(',')[0] + "'?") == DialogResult.OK)
            {
                Log("Removing '" + LstItems.SelectedItems[0].ToString().Split(',')[0] + "'");

                int b = LstItems.Items.IndexOf(LstItems.SelectedItems[0].ToString());
                LstItems.Items.RemoveAt(b);

                string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
                Properties.Settings.Default.Items = a;
                Properties.Settings.Default.Save();

                Log("Removed");
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            foreach (string a in LstItems.Items)
            {
                string b = a.Split(',')[0];
                string par = a.Split(',')[1];
                bool r = Convert.ToBoolean(a.Split(',')[2]);
                Log("Checking if '" + b + "' is running...");
                if (!ProgramIsRunning(a))
                {
                    Log("'" + b + "' is not running. Trying to start");
                    try
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = b;
                        p.StartInfo.Arguments = par;
                        p.StartInfo.WorkingDirectory = Path.GetDirectoryName(b);
                        p.Start();
                        Log("'" + b + "' has been started");
                    }
                    catch (Exception ex)
                    {
                        Log("Error starting '" + b + "'. Message: " + ex.Message);
                    }
                }
                else if (r)
                {
                    Process[] runningProcesses = Process.GetProcesses();
                    foreach (Process process in runningProcesses)
                    {
                        if (process.ProcessName == Path.GetFileNameWithoutExtension(b))
                        {
                            process.CloseMainWindow();
                            await Task.Delay(5000);

                            try
                            {
                                Process p = new Process();
                                p.StartInfo.FileName = b;
                                p.StartInfo.Arguments = par;
                                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(b);
                                p.Start();
                                Log("'" + b + "' has been started");
                            }
                            catch (Exception ex)
                            {
                                Log("Error starting '" + b + "'. Message: " + ex.Message);
                            }
                        }
                    }
                }
            }
        }

        private bool ProgramIsRunning(string FullPath)
        {
            string FilePath = Path.GetDirectoryName(FullPath);
            string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            bool isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);
            try
            {

                foreach (Process p in pList)
                {
                    if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        isRunning = true;
                        break;
                    }
                }
            }
            catch (Exception ex) { Log("Error: " + ex.Message); }

            return isRunning;
        }

        private void Log(string t)
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MonitorAndWatch\\"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MonitorAndWatch\\");

            try
            {
                try
                {
                    long n = new System.IO.FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MonitorAndWatch\\log.log").Length;
                    if (n > 1024 * 1024)
                        File.Move(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MonitorAndWatch\\log.log", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MonitorAndWatch\\log" + n + ".log");
                }
                catch { }
                StreamWriter w = File.AppendText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MonitorAndWatch\\log.log");
                w.WriteLine("[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + t);
                w.Close();
            }
            catch { }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!die)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            die = true;
            Application.Exit();
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
                BtnStartStop.Text = "Start";
                Log("Monitoring stopped");
            }
            else
            {
                timer1.Start();
                BtnStartStop.Text = "Stop";
                Log("Monitoring started");
            }
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (die)
                Application.Exit();
            if (LstItems.Items.Count > 0)
                this.WindowState = FormWindowState.Minimized;

            ChkAdmin.Checked = Properties.Settings.Default.Admin;

            starting = false;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("Manually checking...");
            timer1_Tick(null, null);
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Dispose();
        }

        private void LstItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = LstItems.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                FrmApplication f = new FrmApplication();
                f.TxtApplicatoin.Text = LstItems.Items[index].ToString().Split(',')[0];
                f.TxtParameters.Text = LstItems.Items[index].ToString().Split(',')[1];
                f.ChkRestart.Checked = Convert.ToBoolean(LstItems.Items[index].ToString().Split(',')[2].ToString());
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LstItems.Items[index] = f.TxtApplicatoin.Text + "," + f.TxtParameters.Text + "," + f.ChkRestart.Checked.ToString();

                    //save items
                    string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
                    Properties.Settings.Default.Items = a;
                    Properties.Settings.Default.Save();

                    Log("Changed '" + f.TxtApplicatoin.Text + "', and saved");
                }
            }
        }

        private void ChkAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (!starting)
            {
                Properties.Settings.Default.Admin = ChkAdmin.Checked;
                Properties.Settings.Default.Save();

                if (ChkAdmin.Checked)
                    CheckAdminStart();
            }
        }

        private void FrmMain_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}

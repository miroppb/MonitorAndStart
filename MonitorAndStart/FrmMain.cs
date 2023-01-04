using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitorAndStart
{
    public partial class FrmMain : Form
    {
        bool die = false;
        bool starting = false;
        Dictionary<string, DateTime> services = new Dictionary<string, DateTime>();
        Dictionary<string, DateTime> scripts = new Dictionary<string, DateTime>();

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
            if (Properties.Settings.Default.CheckOnStart)
                timer1_Tick(null, null);
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
            FrmChoose frm = new FrmChoose();
            DialogResult res = frm.ShowDialog(); //Yes = File, No = StuckFile, Cancel = Connection, OK = Script
            if (res == DialogResult.Yes)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Application|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FrmApplication f = new FrmApplication();
                    f.TxtApplication.Text = ofd.FileName;
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LstItems.Items.Add("File:" + f.TxtApplication.Text + "," + f.TxtParameters.Text + "," + f.ChkRestart.Checked.ToString());

                        //save items
                        string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
                        Properties.Settings.Default.Items = a;
                        Properties.Settings.Default.Save();

                        Log("Added 'File:" + ofd.FileName + "' to list, and saved");
                    }
                }
            }
            else if (res == DialogResult.No)
            {
                FrmStuckFile f = new FrmStuckFile();
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LstItems.Items.Add("Stuck:" + f.TxtFile.Text + "," + f.NUD.Value + "," + f.CmbDuration.SelectedItem.ToString());

                    //save items
                    string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
                    Properties.Settings.Default.Items = a;
                    Properties.Settings.Default.Save();

                    Log("Added 'Stuck:" + f.TxtFile.Text + "," + f.NUD.Value + "," + f.CmbDuration.SelectedItem.ToString() + "' to list, and saved");
                }
            }
            else if (res == DialogResult.Cancel)
            {
                FrmConnection f = new FrmConnection();
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LstItems.Items.Add("Connection:" + f.TxtFirst.Text + "," + f.TxtSecond.Text);

                    //save items
                    string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
                    Properties.Settings.Default.Items = a;
                    Properties.Settings.Default.Save();

                    Log("Added 'Connection: " + f.TxtFirst.Text + ", " + f.TxtSecond.Text + "' to list, and saved");
                }
            }
            else if (res == DialogResult.Abort)
            {
                MessageBox.Show("Be aware, that Monitor And Start needs to run as an Admin to restart a Service");
                FrmService f = new FrmService(null, 0);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LstItems.Items.Add("Service:" + f.CurrentService + "," + f.CurrentHours);
                    services.Add(f.CurrentService, DateTime.Now.AddHours(f.CurrentHours));

                    //save items
                    string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
                    Properties.Settings.Default.Items = a;
                    Properties.Settings.Default.Save();

                    Log("Added 'Service: " + f.CurrentService + "," + f.CurrentHours + "' to list, and saved");
                }
            }
            if (res == DialogResult.OK)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Application|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FrmScript f = new FrmScript();
                    f.TxtApplication.Text = ofd.FileName;
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LstItems.Items.Add($"Script:{f.TxtApplication.Text},{f.TxtParameters.Text},{f.NUD.Value.ToString()},{f.ChkHidden.Checked.ToString()},{f.dtpStartFrom.Value.ToString("MM/dd/yyyy hh:mm tt")}");

                        //save items
                        string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
                        Properties.Settings.Default.Items = a;
                        Properties.Settings.Default.Save();

                        Log($"Added 'Script:{ofd.FileName}' to list, and saved");
                    }
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
            List<string> items = LstItems.Items.OfType<string>().ToList();
            foreach (string file in items)
            {
                if (file.StartsWith("File:"))
                {
                    string aa = file.Remove(0, 5);
                    string b = aa.Split(',')[0];
                    string par = aa.Split(',')[1];
                    bool r = Convert.ToBoolean(aa.Split(',')[2]);
                    Log("Checking if '" + b + "' is running...");
                    if (!ProgramIsRunning(aa))
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
                else if (file.StartsWith("Stuck:"))
                {
                    string line = file.Remove(0, 6);
                    string filename = line.Split(',')[0];
                    double number = Convert.ToInt32(line.Split(',')[1]);
                    string duration = line.Split(',')[2];

                    double hours = 0.0;
                    switch (duration)
                    {
                        case "Minutes":
                            hours = number / 60;
                            break;
                        case "Hours":
                            hours = number;
                            break;
                        case "Days":
                            hours = number * 24;
                            break;
                    }

                    Log($"Checking if file exists and is past threshold: {filename} {number} {duration}...");

                    //check if file exists
                    if (File.Exists(filename))
                    {
                        if (IsAboveThreshold(filename, hours))
                        {
                            try
                            {
                                File.Delete(filename);
                                Log($"File {filename} deleted");
                            }
                            catch (Exception ex)
                            {
                                Log($"Error deleting file: {filename}, message: {ex.Message}");
                            }
                        }
                    }
                }
                else if (file.StartsWith("Connection:"))
                {
                    string line = file.Remove(0, 11);
                    string firstConnection = line.Split(',')[0];
                    string connectionToLoad = line.Split(',')[1];

                    Log($"Checking if connection is online: {firstConnection}...");

                    PingReply reply = null;
                    try
                    {
                        Ping ping = new Ping();
                        reply = ping.Send(firstConnection, 2000);

                        if (reply == null || reply.Status == IPStatus.TimedOut)
                        {
                            Log($"First connection is offline: {firstConnection}");
                            // Call asynchronous network methods in a try/catch block to handle exceptions.
                            try
                            {
                                Log($"Trying to load 2nd connection: {connectionToLoad}");
                                HttpClient client = new HttpClient();
                                string responseBody = await client.GetStringAsync(connectionToLoad);

                                Console.WriteLine(responseBody);
                            }
                            catch (HttpRequestException ex)
                            {
                                Log($"Error loading 2nd connection: {connectionToLoad}, message: {ex.Message}");
                            }
                        }
                    }
                    catch { Log($"Error pinging: {firstConnection}"); }
                }
                else if (file.StartsWith("Service:"))
                {
                    string line = file.Remove(0, 8);
                    string service = line.Split(',')[0];
                    int hours = Convert.ToInt32(line.Split(',')[1]);

                    Log($"Checking Service: {service}");

                    if (!services.ContainsKey(service))
                        services.Add(service, DateTime.Now.AddHours(hours));

                    if (services[service].Day == DateTime.Now.Day && services[service].Hour == DateTime.Now.Hour)
                    {
                        try
                        {
                            Log($"Trying to restart {service}.");

                            ServiceController sc = new ServiceController(service);
                            sc.Stop();

                            await Task.Delay(1000);
                            sc.Start();

                            Log($"{service} restarted successfully");

                            services[service] = DateTime.Now.AddHours(hours);
                        }
                        catch
                        {
                            Log($"Restarting {service} Failed");
                            services[service] = DateTime.Now.AddHours(1);
                        }
                    }
                }
                else if (file.StartsWith("Script:"))
                {
                    string line = file.Remove(0, 7);
                    string filename = line.Split(',')[0];
                    string par = line.Split(',')[1];
                    int hours = Convert.ToInt32(line.Split(',')[2]);
                    DateTime dt = Convert.ToDateTime(line.Split(',')[4]);
                    ProcessWindowStyle ws = (line.Split(',')[3] == "True" ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal);

                    Log($"Checking Script: {filename}");

                    if (!scripts.ContainsKey(filename))
                        scripts.Add(filename, dt.AddHours(hours));

                    Log($"Date to run: {scripts[filename]} -- Current date: {DateTime.Now}");
                    if (scripts[filename] <= DateTime.Now)
                    {
                        try
                        {
                            Process p = new Process();
                            p.StartInfo.FileName = filename;
                            p.StartInfo.Arguments = par;
                            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
                            p.StartInfo.WindowStyle = ws;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.UseShellExecute = false;
                            p.OutputDataReceived += P_OutputDataReceived;
                            p.Start();
                            Log("'" + filename + "' has been started");

                            p.BeginOutputReadLine();
                            p.WaitForExit();

                            scripts[filename] = DateTime.Now.AddHours(hours);
                            LstItems.Items[LstItems.Items.IndexOf(file)] = $"Script:{filename},{par},{hours},{line.Split(',')[3]},{DateTime.Now}";
                            Save();
                            Log($"New Time: {scripts[filename]} -- Current time: {DateTime.Now}");
                        }
                        catch (Exception ex)
                        {
                            Log("Error starting '" + filename + "'. Message: " + ex.Message);
                            scripts[filename] = DateTime.Now.AddHours(hours);
                        }
                    }
                }
            }
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log(e.Data);
        }

        public bool IsAboveThreshold(string filename, double hours)
        {
            DateTime threshold = DateTime.Now.AddHours(-hours);
            return File.GetCreationTime(filename) <= threshold;
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
            if (WindowState == FormWindowState.Minimized)
                Close();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (die)
                Application.Exit();
            if (LstItems.Items.Count > 0)
            {
                WindowState = FormWindowState.Minimized;
                HideMe();
            }

            ChkAdmin.Checked = Properties.Settings.Default.Admin;

            starting = false;
        }

        private async void HideMe()
        {
            await Task.Delay(500);
            Close();
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
            if (index != ListBox.NoMatches)
            {
                string txt = LstItems.Items[index].ToString();
                if (txt.StartsWith("File:"))
                {
                    txt = txt.Remove(0, 5);
                    FrmApplication f = new FrmApplication();
                    f.TxtApplication.Text = txt.Split(',')[0];
                    f.TxtParameters.Text = txt.Split(',')[1];
                    f.ChkRestart.Checked = Convert.ToBoolean(txt.Split(',')[2].ToString());
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LstItems.Items[index] = "File:" + f.TxtApplication.Text + "," + f.TxtParameters.Text + "," + f.ChkRestart.Checked.ToString();
                        Save();

                        Log("Changed '" + f.TxtApplication.Text + "', and saved");
                    }
                }
                else if (txt.StartsWith("Stuck:"))
                {
                    txt = txt.Remove(0, 6);
                    FrmStuckFile f = new FrmStuckFile();
                    f.TxtFile.Text = txt.Split(',')[0];
                    f.NUD.Value = Convert.ToInt32(txt.Split(',')[1]);
                    f.CmbDuration.SelectedIndex = f.CmbDuration.Items.IndexOf(txt.Split(',')[2].ToString());
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LstItems.Items[index] = "Stuck:" + f.TxtFile.Text + "," + f.NUD.Value + "," + f.CmbDuration.SelectedItem.ToString();
                        Save();

                        Log("Changed 'Stuck:" + f.TxtFile.Text + "," + f.NUD.Value + "," + f.CmbDuration.SelectedItem.ToString() + "', and saved");
                    }
                }
                else if (txt.StartsWith("Connection:"))
                {
                    txt = txt.Remove(0, 11);
                    FrmConnection f = new FrmConnection();
                    f.TxtFirst.Text = txt.Split(',')[0];
                    f.TxtSecond.Text = txt.Split(',')[1];
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LstItems.Items[index] = "Connection:" + f.TxtFirst.Text + "," + f.TxtSecond.Text;
                        Save();

                        Log("Changed 'Connection:" + f.TxtFirst.Text + "," + f.TxtSecond.Text + "', and saved");
                    }
                }
                else if (txt.StartsWith("Service:"))
                {
                    txt = txt.Remove(0, 11);
                    FrmService f = new FrmService(txt.Split(',')[0], Convert.ToInt32(txt.Split(',')[1]));
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LstItems.Items[index] = "Service:" + f.CurrentHours + "," + f.CurrentService;
                        Save();

                        Log("Changed 'Service:" + f.CurrentHours + "," + f.CurrentService + "', and saved");
                    }
                }
                else if (txt.StartsWith("Script:"))
                {
                    txt = txt.Remove(0, 7);
                    FrmScript f = new FrmScript();
                    f.TxtApplication.Text = txt.Split(',')[0];
                    f.TxtParameters.Text = txt.Split(',')[1];
                    f.NUD.Value = Convert.ToInt32(txt.Split(',')[2]);
                    f.ChkHidden.Checked = Convert.ToBoolean(txt.Split(',')[3]);
                    f.dtpStartFrom.Value = Convert.ToDateTime(txt.Split(',')[4]);
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LstItems.Items[index] = $"Script:{f.TxtApplication.Text},{f.TxtParameters.Text},{f.NUD.Value},{f.ChkHidden.Checked},{f.dtpStartFrom.Value.ToString("MM/dd/yyyy hh:mm tt")}";
                        Save();

                        scripts[f.TxtApplication.Text] = f.dtpStartFrom.Value;

                        Log("Changed '" + f.TxtApplication.Text + "', and saved");
                    }
                }
            }
        }

        private void Save()
        {
            //save items
            string a = String.Join(";", LstItems.Items.Cast<string>().ToArray());
            Properties.Settings.Default.Items = a;
            Properties.Settings.Default.Save();
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

        private void ChkCheckOnStart_CheckedChanged(object sender, EventArgs e)
        {
            if (!starting)
            {
                Properties.Settings.Default.CheckOnStart = ChkCheckOnStart.Checked;
                Properties.Settings.Default.Save();

                if (ChkCheckOnStart.Checked)
                    timer1_Tick(null, null);
            }
        }

        int hoveredIndex = -1;

        private void LstItems_MouseMove(object sender, MouseEventArgs e)
        {
            // See which row is currently under the mouse:
            int newHoveredIndex = LstItems.IndexFromPoint(e.Location);
            Console.WriteLine(newHoveredIndex.ToString());

            // If the row has changed since last moving the mouse:
            if (hoveredIndex != newHoveredIndex)
            {
                // Change the variable for the next time we move the mouse:
                hoveredIndex = newHoveredIndex;

                // If over a row showing data (rather than blank space):
                if (hoveredIndex > -1)
                {
                    string hovered = LstItems.Items[hoveredIndex].ToString();
                    switch (hovered.Split(':')[0])
                    {
                        case "Script":
                            string line = hovered.Remove(0, 7);
                            int hours = Convert.ToInt32(line.Split(',')[2]);
                            DateTime dt = Convert.ToDateTime(line.Split(',')[4]);
                            toolTip1.Active = false;
                            toolTip1.SetToolTip(LstItems, $"Running on {dt.AddHours(hours).ToString("MM/dd/yyyy htt")}");
                            toolTip1.Active = true;
                            break;
                        default:
                            toolTip1.Active = false;
                            toolTip1.SetToolTip(LstItems, "");
                            toolTip1.Active = true;
                            break;
                    }
                }
            }
        }
    }
}

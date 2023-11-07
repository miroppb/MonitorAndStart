using miroppb;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonitorAndStart.v2
{
	class Script : Job
	{
		public string Filename;
		public string Parameters;
		public bool RunAsAdmin;
		public bool RunHidden;

		public Script(string Name, string Filename, string Parameters, bool runAsAdmin, bool runHidden, int IntervalInMinutes, Intervals SelectedInterval, DateTime LastRan, DateTime NextTimeToRun, bool runOnStart)
		{
			base.Name = Name;
			this.Filename = Filename;
			this.Parameters = Parameters;
			RunAsAdmin = runAsAdmin;
			RunHidden = runHidden;
			base.IntervalInMinutes = IntervalInMinutes;
			Interval = SelectedInterval;
			LastRun = LastRan;
			base.NextTimeToRun = NextTimeToRun;
			RunOnStart = runOnStart;
		}
		public override int TypeOfJob => 3;

		public static List<string> Vars => new() { "Filename", "Parameters", "Run as Admin", "Run Hidden" };

		public override void ExecuteJob()
		{
			ProcessWindowStyle ws = (RunHidden ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal);

			try
			{
				Process p = new();
				p.StartInfo.FileName = Filename;
				p.StartInfo.Arguments = Parameters;
				p.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Filename);
				p.StartInfo.WindowStyle = ws;
				if (ws == ProcessWindowStyle.Hidden)
					p.StartInfo.CreateNoWindow = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.OutputDataReceived += P_OutputDataReceived;
				p.Start();
				libmiroppb.Log($"'{Filename}' has been started");

				p.BeginOutputReadLine();
				p.WaitForExit();
				if (p.ExitCode != 0)
					NextTimeToRun = DateTime.Now.AddMinutes(5);
				else
				{
					NextTimeToRun = DateTime.Now.AddMinutes(IntervalInMinutes);
				}
				p.Dispose();

			}
			catch (Exception ex)
			{
				libmiroppb.Log($"Error starting '{Filename}'. Message: {ex.Message}");
				NextTimeToRun = DateTime.Now.AddMinutes(5);
			}
		}

		private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			libmiroppb.Log(e.Data);
		}
	}
}

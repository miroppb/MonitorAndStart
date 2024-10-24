using miroppb;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonitorAndStart.v2
{
	class Script : Job
	{
		public string filename;
		public string parameters;
		public bool runAsAdmin;
		public bool runHidden;
		public bool runOnce;
		bool alreadyRan;

		public Script(string _Name, string _Filename, string _Parameters, bool _RunAsAdmin, bool _RunHidden, bool _RunOnce, int _IntervalInMinutes, Intervals _SelectedInterval, DateTime _LastRan, DateTime _NextTimeToRun, bool _RunOnStart)
		{
			Name = _Name;
			filename = _Filename;
			parameters = _Parameters;
			runAsAdmin = _RunAsAdmin;
			runHidden = _RunHidden;
			runOnce = _RunOnce;
			IntervalInMinutes = _IntervalInMinutes;
			Interval = _SelectedInterval;
			LastRun = _LastRan;
			NextTimeToRun = _NextTimeToRun;
			RunOnStart = _RunOnStart;
		}
		public override int TypeOfJob => 3;

		public static List<string> Vars => new() { "Filename", "Parameters", "Run as Admin", "Run Hidden", "Run Once" };

		public override void ExecuteJob(bool force)
		{
			if (force | (!force & runOnce & !alreadyRan) && Enabled)
			{
				alreadyRan = true;
				ProcessWindowStyle ws = runHidden ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal;

				try
				{
					Process p = new();
					p.StartInfo.FileName = filename;
					p.StartInfo.Arguments = parameters;
					p.StartInfo.Verb = runAsAdmin ? "runas" : ""; //the secret sauce?
					p.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(filename);
					p.StartInfo.WindowStyle = ws;
					if (ws == ProcessWindowStyle.Hidden)
						p.StartInfo.CreateNoWindow = true;
					p.StartInfo.RedirectStandardOutput = true;
					p.StartInfo.UseShellExecute = false;
					p.OutputDataReceived += P_OutputDataReceived;
					p.Start();
					Libmiroppb.Log($"'{filename}' has been started");

					p.BeginOutputReadLine();
					p.WaitForExit();
					if (p.ExitCode != 0)
						NextTimeToRun = DateTime.Now.AddMinutes(5);
					else
					{
						LastRun = DateTime.Now;
						NextTimeToRun = DateTime.Now.AddMinutes(IntervalInMinutes);
					}
					p.Dispose();

				}
				catch (Exception ex)
				{
					Libmiroppb.Log($"Error starting '{filename}'. Message: {ex.Message}");
					NextTimeToRun = DateTime.Now.AddMinutes(5);
				}
			}
		}

		private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e != null && e.Data != null)
				Libmiroppb.Log(e.Data);
		}
	}
}

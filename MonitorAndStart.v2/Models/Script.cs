using miroppb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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
		bool IsRunning = false;

		public Script(string _Name, string _Filename, string _Parameters, bool _RunAsAdmin, bool _RunHidden, bool _RunOnce)
		{
			Name = _Name;
			filename = _Filename;
			parameters = _Parameters;
			runAsAdmin = _RunAsAdmin;
			runHidden = _RunHidden;
			runOnce = _RunOnce;
		}
		public override int TypeOfJob => 3;

		public static List<string> Vars => new() { "Filename", "Parameters", "Run as Admin", "Run Hidden", "Run Once" };

		public override Task ExecuteJob(bool force)
		{
			if (ShouldRun(force))
			{
				IsRunning = true;
				CompletedSuccess = false;
				ProcessWindowStyle ws = runHidden ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Normal;

				try
				{
					Process p = new();
					p.StartInfo.FileName = filename;
					p.StartInfo.Arguments = parameters;
					p.StartInfo.Verb = runAsAdmin ? "runas" : ""; //the secret sauce?
					p.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
					p.StartInfo.WindowStyle = ws;
					if (ws == ProcessWindowStyle.Hidden)
						p.StartInfo.CreateNoWindow = true;
					p.StartInfo.RedirectStandardOutput = true;
					p.StartInfo.RedirectStandardError = true;
					p.StartInfo.UseShellExecute = false;
					p.OutputDataReceived += P_OutputDataReceived;
					p.ErrorDataReceived += P_OutputDataReceived;

					try
					{
						p.Start();
						Libmiroppb.Log($"'{p.StartInfo.FileName}' has been started with arguments '{p.StartInfo.Arguments}'");

						p.BeginOutputReadLine();
						p.BeginErrorReadLine();
						p.WaitForExit();

						if (!p.StartInfo.Arguments.Contains("taskkill") && p.ExitCode != 0) //ignore taskkills
						{
							Libmiroppb.Log($"ExitCode {p.ExitCode}");
							CompletedSuccess = false;
						}
						else
						{
							CompletedSuccess = true;
						}
					}
					catch (Exception ex)
					{
						Libmiroppb.Log($"Exception: {ex.Message}");
						CompletedSuccess = false;
					}
					finally
					{
						p.Dispose();
						alreadyRan = true;
					}
				}
				catch (Exception ex)
				{
					Libmiroppb.Log($"Error starting '{filename}'. Message: {ex.Message}");
					CompletedSuccess = false;
				}
            }
			if (!Enabled)
				CompletedSuccess = true; //mark as completed even if disabled
			IsRunning = false;
            return Task.CompletedTask;
        }

		private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (e != null && e.Data != null)
				Libmiroppb.Log(e.Data);
		}

		private bool ShouldRun(bool ForceRun)
		{
			if (!Enabled) { Libmiroppb.Log("Disabled..."); return false; } //job is disabled
			if (IsRunning) { Libmiroppb.Log("Running..."); return false; } // Job is already running
            if (ForceRun) return true; // Force run even if already
            if (runOnce && alreadyRan) return false; // Job should run only once and has already run
			return true; // Default case: job should run
		}

		public override string ToString => $"Filename: {Path.GetFileName(filename)} Parameters: {parameters.Length > 0} Admin: {runAsAdmin} Once: {runOnce}";
	}
}

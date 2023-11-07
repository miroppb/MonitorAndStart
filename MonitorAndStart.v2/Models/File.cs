using miroppb;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MonitorAndStart.v2
{
	class File : Job
	{
		public string filename;
		public string parameters;
		public bool restart;
		public bool runAsAdmin;

		public File(string Name, string Filename, string Parameters, bool Restart, bool runAsAdmin, int IntervalInMinutes, Intervals SelectedInterval, DateTime LastRan, DateTime NextTimeToRun, bool runOnStart)
		{
			this.Name = Name;
			filename = Filename;
			parameters = Parameters;
			restart = Restart;
			this.IntervalInMinutes = IntervalInMinutes;
			Interval = SelectedInterval;
			LastRun = LastRan;
			this.NextTimeToRun = NextTimeToRun;
			RunOnStart = runOnStart;
			this.runAsAdmin = runAsAdmin;
		}
		public override int TypeOfJob => 0;

		public static List<string> Vars => new() { "Filename", "Parameters", "Restart", "Run as Admin" };

		public async override void ExecuteJob()
		{
			libmiroppb.Log($"Checking if '{Path.GetFileName(filename)}' is running...");
			if (!ProgramIsRunning(filename))
			{
				libmiroppb.Log($"'{Path.GetFileName(filename)}' is not running. Trying to start");
				try
				{
					Process p = new();
					p.StartInfo.FileName = runAsAdmin ? filename : Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\explorer.exe";
					p.StartInfo.Arguments = (runAsAdmin ? "" : filename + " ") + parameters;
					p.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
					p.StartInfo.Verb = runAsAdmin ? "runas" : ""; //the secret sauce?
					p.Start();
					libmiroppb.Log($"'{Path.GetFileName(filename)}' has been started");

					LastRun = DateTime.Now;
					NextTimeToRun = DateTime.Now.AddMinutes(IntervalInMinutes);
				}
				catch (Exception ex)
				{
					libmiroppb.Log($"Error starting '{Path.GetFileName(filename)}'. Message: {ex.Message}");
				}
			}
			else if (restart)
			{
				Process[] runningProcesses = Process.GetProcesses();
				foreach (Process process in runningProcesses)
				{
					if (process.ProcessName == Path.GetFileNameWithoutExtension(filename))
					{
						process.CloseMainWindow();
						await Task.Delay(5000);

						try
						{
							Process p = new();
							p.StartInfo.FileName = (runAsAdmin ? Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "explorer.exe" : filename);
							p.StartInfo.Arguments = (runAsAdmin ? filename + " " : "") + parameters;
							p.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
							p.StartInfo.Verb = (runAsAdmin ? "runas" : ""); //the secret sauce?
							p.Start();
							libmiroppb.Log($"'{Path.GetFileName(filename)}' has been started");

							LastRun = DateTime.Now;
							NextTimeToRun = DateTime.Now.AddMinutes(IntervalInMinutes);
						}
						catch (Exception ex)
						{
							libmiroppb.Log($"Error starting '{Path.GetFileName(filename)}'. Message: {ex.Message}");
						}
					}
				}
			}
		}

		private static bool ProgramIsRunning(string FullPath)
		{
			string FilePath = Path.GetDirectoryName(FullPath)!;
			string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
			bool isRunning = false;

			Process[] pList = Process.GetProcessesByName(FileName);
			try
			{
				foreach (Process p in pList)
				{
					if (p.MainModule!.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
					{
						isRunning = true;
						break;
					}
				}
			}
			catch (Exception ex) { libmiroppb.Log("Error: " + ex.Message); }

			return isRunning;
		}
	}
}

﻿using miroppb;
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
		public bool runOnce;
		bool alreadyRan;

		public File(string _Name, string _Filename, string _Parameters, bool _Restart, bool _RunAsAdmin, bool _RunOnce, int _IntervalInMinutes, Intervals _SelectedInterval, DateTime _LastRan, DateTime _NextTimeToRun, bool _RunOnStart)
		{
			Name = _Name;
			filename = _Filename;
			parameters = _Parameters;
			restart = _Restart;
			IntervalInMinutes = _IntervalInMinutes;
			Interval = _SelectedInterval;
			LastRun = _LastRan;
			NextTimeToRun = _NextTimeToRun;
			RunOnStart = _RunOnStart;
			runAsAdmin = _RunAsAdmin;
			runOnce = _RunOnce;
		}
		public override int TypeOfJob => 0;

		public static List<string> Vars => new() { "Filename", "Parameters", "Restart", "Run as Admin", "Run Once" };

		public override void ExecuteJob(bool force)
		{
			if (force | (!force & runOnce & !alreadyRan) && Enabled)
			{
				CompletedSuccess = false;
				alreadyRan = true;
				Libmiroppb.Log($"Checking if '{Path.GetFileName(filename)}' is running...");
				if (!ProgramIsRunning(filename))
				{
					Libmiroppb.Log($"'{Path.GetFileName(filename)}' is not running. Trying to start");
					try
					{
						if (runAsAdmin)
							ProcessRunner.ExecuteProcess(filename, parameters);
						else
							ProcessRunner.ExecuteProcessUnElevated(filename, parameters);
						Libmiroppb.Log($"'{Path.GetFileName(filename)}' has been restarted");

						CompletedSuccess = true;
						return;
					}
					catch (Exception ex)
					{
						Libmiroppb.Log($"Error starting '{Path.GetFileName(filename)}'. Message: {ex.Message}");
						CompletedSuccess = false;
					}
					return;
				}
				else if (restart)
				{
					Process[] runningProcesses = Process.GetProcesses();
					foreach (Process process in runningProcesses)
					{
						if (process.ProcessName == Path.GetFileNameWithoutExtension(filename))
						{
							try
							{
								process.CloseMainWindow();
								Task.Delay(5000);
							}
							catch { }
						}
					}

					try
					{
						if (runAsAdmin)
							ProcessRunner.ExecuteProcess(filename, parameters);
						else
							ProcessRunner.ExecuteProcessUnElevated(filename, parameters);
						Libmiroppb.Log($"'{Path.GetFileName(filename)}' has been restarted");

						CompletedSuccess = true;
						return;
					}
					catch (Exception ex)
					{
						Libmiroppb.Log($"Error starting '{Path.GetFileName(filename)}'. Message: {ex.Message}");
						CompletedSuccess = false;
						return;
					}
				}
			}
			CompletedSuccess = true;
		}

		private static bool ProgramIsRunning(string FullPath)
		{
			string FilePath = Path.GetDirectoryName(FullPath)!;
			string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
			bool isRunning = false;

			Process[] pList = Process.GetProcessesByName(FileName);
			Libmiroppb.Log(pList.Length + " processes running");
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
			catch (Exception ex) { Libmiroppb.Log("Error: " + ex.Message); }
			Libmiroppb.Log("Returning: " + isRunning.ToString());
			return isRunning;
		}
	}
}

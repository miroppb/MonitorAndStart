using miroppb;
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

        public bool consoleApp { get; }

        bool alreadyRan;

		public File(string _Name, string _Filename, string _Parameters, bool _Restart, bool _RunAsAdmin, bool _RunOnce, bool _ConsoleApp)
		{
			Name = _Name;
			filename = _Filename;
			parameters = _Parameters;
			restart = _Restart;
			runAsAdmin = _RunAsAdmin;
			runOnce = _RunOnce;
			consoleApp = _ConsoleApp;
		}
		public override int TypeOfJob => 0;

		public static List<string> Vars => new() { "Filename", "Parameters", "Restart", "Run as Admin", "Run Once", "Console" };

		public override Task ExecuteJob(bool force)
		{
			if (ShouldRun(force))
			{
				CompletedSuccess = false;
				Libmiroppb.Log($"Checking if '{Path.GetFileName(filename)}' is running...");
				if (!ProgramIsRunning(filename))
                {
                    Libmiroppb.Log($"'{Path.GetFileName(filename)}' is not running. Trying to start");
                    return TryToStartProcess();
                }
                else if (restart)
				{
					Process[] runningProcesses = Process.GetProcesses();
					foreach (Process process in runningProcesses)
					{
						if (process.ProcessName == Path.GetFileNameWithoutExtension(filename))
						{
							TryKill(process);
						}
					}

                    Libmiroppb.Log($"'{Path.GetFileName(filename)}' is running, but we'll restart it");
                    return TryToStartProcess();
				}
			}
			CompletedSuccess = true;
			return Task.CompletedTask;
        }

        private Task TryToStartProcess()
        {
            try
            {
                if (consoleApp)
                {
                    string newParameters = $"-dir \"{Path.GetDirectoryName(filename)}\" -run -new_console:t:\"{Name}\" \"{filename}\" {parameters}";
                    if (runAsAdmin)
                        ProcessRunner.ExecuteProcess("conemu64", newParameters);
                    else
                        ProcessRunner.ExecuteProcessUnElevated("conemu64", newParameters);
                }
                else
                {
                    if (runAsAdmin)
                        ProcessRunner.ExecuteProcess(filename, parameters);
                    else
                        ProcessRunner.ExecuteProcessUnElevated(filename, parameters);
                }
                Libmiroppb.Log($"'{Path.GetFileName(filename)}' has been restarted");

                CompletedSuccess = true;
                alreadyRan = true;
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Libmiroppb.Log($"Error starting '{Path.GetFileName(filename)}'. Message: {ex.Message}");
                CompletedSuccess = false;
            }
            return Task.CompletedTask;
        }

        public override string ToString => $"Filename: {Path.GetFileName(filename)} Parameters: {parameters.Length > 0} Restart: {restart} Admin: {runAsAdmin} Once: {runOnce}, Console: {consoleApp}";

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

        static void TryKill(Process proc)
        {
            try
            {
                if (!proc.HasExited)
                {
                    Console.WriteLine($"Shutting down {proc.ProcessName} process...");
                    proc.Kill(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error killing child process: {ex.Message}");
            }
        }

        private bool ShouldRun(bool ForceRun)
		{
			if (!Enabled) return false; //job is disabled
            if (ForceRun) return true; // Force run even if already
            if (runOnce && alreadyRan) return false; // Job should run only once and has already run
			return true; // Default case: job should run
		}
	}
}

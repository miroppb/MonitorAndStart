using miroppb;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;

namespace MonitorAndStart.v2
{
	class Stuck : Job
	{
		public int StuckLongerThanMinutes;
		public string Filename;

		public Stuck(string _Name, string _Filename, int _StuckLongerThanMinutes, int _IntervalInMinutes, Intervals _SelectedInterval, DateTime _LastRan, DateTime _NextTimeToRun, bool _RunOnStart)
		{
			Name = _Name;
			Filename = _Filename;
			StuckLongerThanMinutes = _StuckLongerThanMinutes;
			IntervalInMinutes = _IntervalInMinutes;
			Interval = _SelectedInterval;
			LastRun = _LastRan;
			NextTimeToRun = _NextTimeToRun;
			RunOnStart = _RunOnStart;
		}
		public override int TypeOfJob => 2;

		public static List<string> Vars => new() { "Filename", "Minutes Stuck" };

		public override void ExecuteJob(bool force = false)
		{
			if (Enabled)
			{
				CompletedSuccess = false;
				Libmiroppb.Log($"Checking if file exists and is past threshold: {Filename} {StuckLongerThanMinutes}...");

				//check if file exists
				if (System.IO.File.Exists(Filename))
				{
					if (IsAboveThreshold(Filename, StuckLongerThanMinutes))
					{
						try
						{
							System.IO.File.Delete(Filename);
							Libmiroppb.Log($"File {Filename} deleted");
							CompletedSuccess = true;
						}
						catch (Exception ex)
						{
							Libmiroppb.Log($"Error deleting file: {Filename}, message: {ex.Message}");
							CompletedSuccess = false;
						}
					}
					else
					{
						CompletedSuccess = true;
						return;
					}
				}
				else
				{
					CompletedSuccess = false;
					return;
				}
			}
			CompletedSuccess = true;
		}

		private static bool IsAboveThreshold(string filename, double minutes)
		{
			DateTime threshold = DateTime.Now.AddMinutes(-minutes);
			return System.IO.File.GetCreationTime(filename) <= threshold;
		}
	}
}

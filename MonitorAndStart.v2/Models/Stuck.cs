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

		public Stuck(string Name, string filename, int StuckLongerThanMinutes, int IntervalInMinutes, Intervals SelectedInterval, DateTime LastRan, DateTime NextTimeToRun, bool RunOnStart)
		{
			base.Name = Name;
			Filename = filename;
			this.StuckLongerThanMinutes = StuckLongerThanMinutes;
			this.IntervalInMinutes = IntervalInMinutes;
			Interval = SelectedInterval;
			LastRun = LastRan;
			base.NextTimeToRun = NextTimeToRun;
			base.RunOnStart = RunOnStart;
		}
		public override int TypeOfJob => 2;

		public static List<string> Vars => new() { "Filename", "Minutes Stuck" };

		public override void ExecuteJob()
		{
			libmiroppb.Log($"Checking if file exists and is past threshold: {Filename} {StuckLongerThanMinutes}...");

			//check if file exists
			if (System.IO.File.Exists(Filename))
			{
				if (IsAboveThreshold(Filename, StuckLongerThanMinutes))
				{
					try
					{
						System.IO.File.Delete(Filename);
						libmiroppb.Log($"File {Filename} deleted");
					}
					catch (Exception ex)
					{
						libmiroppb.Log($"Error deleting file: {Filename}, message: {ex.Message}");
					}
				}
			}
		}

		private static bool IsAboveThreshold(string filename, double minutes)
		{
			DateTime threshold = DateTime.Now.AddMinutes(-minutes);
			return System.IO.File.GetCreationTime(filename) <= threshold;
		}
	}
}

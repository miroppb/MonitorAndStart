using miroppb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MonitorAndStart.v2
{
	class Stuck : Job
	{
		public int StuckLongerThanMinutes;
		public string Filename;

		public Stuck(string _Name, string _Filename, int _StuckLongerThanMinutes)
		{
			Name = _Name;
			Filename = _Filename;
			StuckLongerThanMinutes = _StuckLongerThanMinutes;
		}
		public override int TypeOfJob => 2;

		public static List<string> Vars => new() { "Filename", "Minutes Stuck" };

		public override Task ExecuteJob(bool force = false)
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
							return Task.CompletedTask;
						}
					}
					else
					{
						CompletedSuccess = true;
					}
				}
				else
				{
					CompletedSuccess = false;
                    return Task.CompletedTask;
                }
			}
			CompletedSuccess = true;
            return Task.CompletedTask;
        }

		public override string ToString => $"Filename: {Path.GetFileName(Filename)} Longer than: {StuckLongerThanMinutes}";

		private static bool IsAboveThreshold(string filename, double minutes)
		{
			DateTime threshold = DateTime.Now.AddMinutes(-minutes);
			return System.IO.File.GetCreationTime(filename) <= threshold;
		}
	}
}

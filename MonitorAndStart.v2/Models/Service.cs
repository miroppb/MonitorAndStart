using miroppb;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace MonitorAndStart.v2
{
	public class Service : Job
	{
		public string ServiceName;

		public Service(string _Name, string _ServiceName, int _IntervalInMinutes, Intervals _SelectedInterval, DateTime _LastRan, DateTime _NextTimeToRun, bool _RunOnStart)
		{
			Name = _Name;
			ServiceName = _ServiceName;
			IntervalInMinutes = _IntervalInMinutes;
			Interval = _SelectedInterval;
			LastRun = _LastRan;
			NextTimeToRun = _NextTimeToRun;
			RunOnStart = _RunOnStart;
		}
		public override int TypeOfJob => 1;

		public static List<string> Vars => new() { "Service" };

		public async override void ExecuteJob(bool force = false)
		{
			if (Enabled)
			{
				try
				{
					Libmiroppb.Log($"Trying to restart {ServiceName}.");

					ServiceController sc = new(ServiceName);
					try
					{
						sc.Stop();
					}
					catch { }

					await Task.Delay(1000);
					try
					{
						sc.Start();
						Libmiroppb.Log($"{ServiceName} restarted successfully");

						LastRun = DateTime.Now;
						NextTimeToRun = DateTime.Now.AddMinutes(IntervalInMinutes);
					}
					catch { }
				}
				catch
				{
					Libmiroppb.Log($"Restarting {ServiceName} Failed");
				}
			}
		}

		public static List<string> GetServices() => ServiceController.GetServices().Select(x => x.ServiceName).ToList();
	}
}

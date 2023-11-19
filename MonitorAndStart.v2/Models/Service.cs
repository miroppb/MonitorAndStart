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

		public Service(string Name, string ServiceName, int IntervalInMinutes, Intervals SelectedInterval, DateTime LastRan, DateTime NextTimeToRun, bool RunOnStart)
		{
			base.Name = Name;
			this.ServiceName = ServiceName;
			this.IntervalInMinutes = IntervalInMinutes;
			Interval = SelectedInterval;
			LastRun = LastRan;
			base.NextTimeToRun = NextTimeToRun;
			base.RunOnStart = RunOnStart;
		}
		public override int TypeOfJob => 1;

		public static List<string> Vars => new() { "Service" };

		public async override void ExecuteJob()
		{
			try
			{
				libmiroppb.Log($"Trying to restart {ServiceName}.");

				ServiceController sc = new(ServiceName);
				try
				{
					sc.Stop();
				} catch { }

				await Task.Delay(1000);
				try
				{
					sc.Start();
					libmiroppb.Log($"{ServiceName} restarted successfully");

					LastRun = DateTime.Now;
					NextTimeToRun = DateTime.Now.AddMinutes(IntervalInMinutes);
				}
				catch { }
			}
			catch
			{
				libmiroppb.Log($"Restarting {ServiceName} Failed");
			}
		}

		public static List<string> GetServices() => ServiceController.GetServices().Select(x => x.ServiceName).ToList();
	}
}

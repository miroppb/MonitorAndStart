using miroppb;
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

		public Service(string _Name, string _ServiceName)
		{
			Name = _Name;
			ServiceName = _ServiceName;
		}
		public override int TypeOfJob => 1;

		public static List<string> Vars => new() { "Service" };

		public override Task ExecuteJob(bool force = false)
		{
			if (Enabled)
			{
				CompletedSuccess = false;
				try
				{
					Libmiroppb.Log($"Trying to restart {ServiceName}.");

					ServiceController sc = new(ServiceName);
					try
					{
						sc.Stop();
						sc.WaitForStatus(ServiceControllerStatus.Stopped);
					}
					catch (Exception ex) { Libmiroppb.Log($"Failed to stop: {ex.Message}"); }

					try
					{
						sc.Start();
						sc.WaitForStatus(ServiceControllerStatus.Running);
						Libmiroppb.Log($"{ServiceName} restarted successfully");

						CompletedSuccess = true;
					}
					catch (Exception ex)
					{
						Libmiroppb.Log($"Failed to restart: {ex.Message}");
						CompletedSuccess = false;
						return Task.CompletedTask;
					}
				}
				catch
				{
					Libmiroppb.Log($"Restarting {ServiceName} Failed");
					CompletedSuccess = false;
					return Task.CompletedTask;
				}
			}
			CompletedSuccess = true;
			return Task.CompletedTask;
		}

		public static List<string> GetServices() => ServiceController.GetServices().Select(x => x.ServiceName).ToList();

		public override string ToString => $"Service: {ServiceName}";
	}
}

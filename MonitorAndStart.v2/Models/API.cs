using miroppb;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MonitorAndStart.v2.Models
{
	public class API : Job
	{
		public string url;

		public API(string _Name, string _Url, int _IntervalInMinutes, Intervals _SelectedInterval, DateTime _LastRan, DateTime _NextTimeToRun, bool _RunOnStart)
		{
			Name = _Name;
			url = _Url;
			IntervalInMinutes = _IntervalInMinutes;
			Interval = _SelectedInterval;
			LastRun = _LastRan;
			NextTimeToRun = _NextTimeToRun;
			RunOnStart = _RunOnStart;
		}
		public override int TypeOfJob => 4;

		public static List<string> Vars => new() { "URL" };

		public async override void ExecuteJob(bool force = false)
		{
			if (Enabled)
			{
				Libmiroppb.Log($"Calling API: {url}");
				HttpClient client = new()
				{
					BaseAddress = new(url)
				};
				var response = await client.GetAsync("");
				Libmiroppb.Log($"Response: {response}");
			}
		}
	}
}

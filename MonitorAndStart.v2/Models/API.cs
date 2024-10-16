using miroppb;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MonitorAndStart.v2.Models
{
	public class API : Job
	{
		public string URL;

		public API(string Name, string url, int IntervalInMinutes, Intervals SelectedInterval, DateTime LastRan, DateTime NextTimeToRun, bool runOnStart)
		{
			this.Name = Name;
			URL = url;
			this.IntervalInMinutes = IntervalInMinutes;
			Interval = SelectedInterval;
			LastRun = LastRan;
			this.NextTimeToRun = NextTimeToRun;
			RunOnStart = runOnStart;
		}
		public override int TypeOfJob => 4;

		public static List<string> Vars => new() { "URL" };

		public async override void ExecuteJob()
		{
			libmiroppb.Log($"Calling API: {URL}");
			HttpClient client = new()
			{
				BaseAddress = new(URL)
			};
			var response = await client.GetAsync("");
			libmiroppb.Log($"Response: {response}");
		}
	}
}

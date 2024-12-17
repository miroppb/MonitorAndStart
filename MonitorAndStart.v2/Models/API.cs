using miroppb;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace MonitorAndStart.v2.Models
{
	public class API : Job
	{
		public string url;
		public string cookies;
		public string output;

		public API(string _Name, string _Url, string _Cookies, string _Output, int _IntervalInMinutes, Intervals _SelectedInterval, DateTime _LastRan, DateTime _NextTimeToRun, bool _RunOnStart)
		{
			Name = _Name;
			url = _Url;
			IntervalInMinutes = _IntervalInMinutes;
			Interval = _SelectedInterval;
			LastRun = _LastRan;
			NextTimeToRun = _NextTimeToRun;
			RunOnStart = _RunOnStart;
			cookies = _Cookies;
			output = _Output;
		}
		public override int TypeOfJob => 4;

		public static List<string> Vars => new() { "URL", "Cookie(s)", "Output" };

		public async override void ExecuteJob(bool force = false)
		{
			if (Enabled)
			{
				Libmiroppb.Log($"Calling API: {url}");
				HttpClientHandler httpHandler = new();
				HttpClient client = new(httpHandler)
				{
					BaseAddress = new(url)
				};
				if (cookies.Length > 0)
				{
					CookieContainer cookieContainer = new();
					httpHandler.CookieContainer = cookieContainer;
					var _cookies = cookies.Split([';'], StringSplitOptions.RemoveEmptyEntries);
					foreach (var cookie in _cookies)
					{
						var cookieParts = cookie.Split('=', 2);
						if (cookieParts.Length == 2)
						{
							string name = cookieParts[0].Trim();
							string value = cookieParts[1].Trim();
							cookieContainer.Add(client.BaseAddress, new Cookie(name, value));
						}
					}
				}
				var response = await client.GetAsync("");
				if (response.IsSuccessStatusCode)
				{
					if (output.Length > 0)
					{
						StreamWriter w = new(output);
						w.Write(await response.Content.ReadAsStringAsync());
						w.Close();
					}
					CompletedSuccess = true;
					return;
				}
				return;
			}
			CompletedSuccess = true;
			return; //will always succeed. TODO: Check for errors
		}
	}
}

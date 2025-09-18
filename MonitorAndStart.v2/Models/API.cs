using miroppb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MonitorAndStart.v2.Models
{
	public class API : Job
	{
		public string url;
		public string cookies;
		public string output;

		public API(string _Name, string _Url, string _Cookies, string _Output)
		{
			Name = _Name;
			url = _Url;
			cookies = _Cookies;
			output = _Output;
		}
		public override int TypeOfJob => 4;

		public static List<string> Vars => new() { "URL", "Cookie(s)", "Output" };

		public async override Task ExecuteJob(bool force = false)
		{
			if (Enabled)
			{
				Libmiroppb.Log($"Calling API: {url}");
				try
				{
					HttpClientHandler httpHandler = new();
					HttpClient client = new(httpHandler)
					{
						BaseAddress = new(url)
					};
                    Libmiroppb.Log($"Setting cookies, if exist");
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
                    Libmiroppb.Log($"Making the call");
                    var response = await client.GetAsync("");
					if (response.IsSuccessStatusCode)
					{
                        Libmiroppb.Log($"Success");
                        if (output.Length > 0)
						{
                            Libmiroppb.Log($"Writing output");
                            StreamWriter w = new(output);
							w.Write(await response.Content.ReadAsStringAsync());
							w.Close();
                            Libmiroppb.Log($"Done writing");
                        }
						CompletedSuccess = true;
                        Libmiroppb.Log($"Returning with completed Success");
                        return;
					}
					else
					{
						Console.WriteLine($"Failed calling api. Reason: {response.StatusCode}");
						Libmiroppb.Log($"Failed calling api. Reason: {response.StatusCode}");
					}
					return;
				}
				catch (Exception ex) { Libmiroppb.Log($"Error: {ex.Message}"); CompletedSuccess = false; return; }
			}
			CompletedSuccess = true;
			return;
		}

		public override string ToString => $"URL: {url} Cookies {cookies.Length > 0} Output: {output}";
	}
}

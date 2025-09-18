using RestSharp;
using System.Threading.Tasks;

namespace MonitorAndStart.v2.Data
{
	public interface INotificationProvider
	{
		Task<bool> SendNotification(string engine, string apichannel, string message);
	}

	public class NotificationProvider : INotificationProvider
	{
		public async Task<bool> SendNotification(string engine, string apichannel, string message)
		{
			var client = new RestClient();
			var request = new RestRequest();
			if (engine != null)
			{
				switch (engine)
				{
					case "Prowl":
						client = new RestClient(@"https://api.prowlapp.com/publicapi/add");
						request.AddQueryParameter("apikey", apichannel);
						request.AddQueryParameter("application", "Monitor and Start");
						request.AddQueryParameter("description", message);
						await client.PostAsync(request);
						break;
					case "Ntfy":
						client = new RestClient(apichannel);
						request.AddParameter("message", message);
						request.AddHeader("X-Title", "Monitor and Start");
						await client.PostAsync(request);
						break;
				}
				return true;
			}
			else
				return false;
		}
	}
}

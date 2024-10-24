namespace MonitorAndStart.v2.Models
{
	class FileJson
	{
		public string filename = string.Empty;
		public string parameters = string.Empty;
		public bool restart;
		public bool runasadmin;
		public bool runonce;
	}

	class ServiceJson
	{
		public string servicename = string.Empty;
    }

	class StuckJson
	{
		public string filename = string.Empty;
		public int stucklongerthanminutes;
	}

	class ScriptJson
	{
		public string filename = string.Empty;
		public string parameters = string.Empty;
		public bool runasadmin;
		public bool runhidden;
		public bool runonce;
	}

	class APIJson
	{
		public string url = string.Empty;
	}
}

using Dapper.Contrib.Extensions;

namespace MonitorAndStart.v2.Models
{
	[Table("settings")]
	public class Settings
	{
		[Key]
		public int Id { get; set; }
		public string PcName { get; set; } = string.Empty;
		public string NotificationEngine { get; set; } = string.Empty;
		public string APIChannel { get; set; } = string.Empty;
		public ConsoleEngine Console { get; set; }
		public bool RunServer { get; set; } = false;
		public int ServerPort { get; set; } = 1135;
	}

	public enum ConsoleEngine
	{
		WindowsTerminal,
		ConEmu64
    }
}

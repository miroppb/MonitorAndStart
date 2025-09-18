using Dapper.Contrib.Extensions;

namespace MonitorAndStart.v2.Models
{
	[Table("jobs")]
	public class TempJob
	{
		[Key]
		public int Id { get; set; }
		public bool Enabled { get; set; }
		public string PcName { get; set; } = string.Empty;
		public int Type { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Json { get; set; } = string.Empty;
	}
}

using System;
using Dapper.Contrib.Extensions;
using MonitorAndStart.v2.Enums;

namespace MonitorAndStart.v2.Models
{
	[Table("jobs")]
	public class TempJob
	{
		[Key]
		public int Id { get; set; }
		public string PcName { get; set; } = string.Empty;
		public int Type { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Intervalinminutes { get; set; }
		public Intervals Selectedinterval { get; set; }
		public DateTime Nexttimetorun { get; set; }
		public DateTime Lastrun { get; set; }
		public bool RunOnStart { get; set; }
		public string Json { get; set; } = string.Empty;
	}
}

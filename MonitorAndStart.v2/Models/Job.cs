using MonitorAndStart.v2.Data;
using MonitorAndStart.v2.Enums;
using System;

namespace MonitorAndStart.v2
{
	public abstract class Job
	{
		public int Id { get; set; }
		private bool _Enabled;
		public bool Enabled
		{
			get => _Enabled;
			set
			{
				_Enabled = value;
				MainDataProvider mainData = new();
				mainData.UpdateRecord(this);
			}
		}
		public abstract int TypeOfJob { get; }
		public abstract void ExecuteJob(bool force);

		public DateTime NextTimeToRun { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime LastRun { get; set; } = DateTime.Now;
		public int IntervalInMinutes { get; set; }
		public Intervals Interval { get; set; }
		public bool RunOnStart { get; set; }
	}
}
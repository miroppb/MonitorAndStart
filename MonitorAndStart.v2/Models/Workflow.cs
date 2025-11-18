using Dapper.Contrib.Extensions;
using MonitorAndStart.v2.Data;
using MonitorAndStart.v2.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace MonitorAndStart.v2.Models
{
	[Table("workflows")]
	public class Workflow
	{
		[Key]
		public int Id { get; set; } = -1;
		public string PcName { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		[Computed]
		public List<Job> Jobs { get; set; } = new();
		public string JobIDs { get; set; } = string.Empty;
		public int IntervalInMinutes { get; set; }
		public Intervals SelectedInterval { get; set; }
		public DateTime NextTimeToRun { get; set; }
		public DateTime LastRun { get; set; }
		public bool RunOnStart { get; set; }
		public bool Notify { get; set; }
		private bool _Enabled;
		public bool Enabled
		{
			get => _Enabled;
			set
			{
				_Enabled = value;
				if (!Loading)
				{
					MainDataProvider mainDataProvider = new();
					mainDataProvider.UpdateWorkflow(this);
				}
			}
		}
		public static bool Loading = false;
		public bool CompletedSuccess = false;
		public bool IsRunning = false;
		public void UpdateTimes()
		{
			LastRun = DateTime.Now;
			if (CompletedSuccess)
				while (NextTimeToRun < DateTime.Now)
					NextTimeToRun = NextTimeToRun.AddMinutes(IntervalInMinutes);
		}
	}
}

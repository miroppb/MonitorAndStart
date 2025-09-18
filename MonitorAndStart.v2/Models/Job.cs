using MonitorAndStart.v2.Data;
using System.Threading.Tasks;

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
				mainData.UpdateJob(this);
			}
		}
		public abstract int TypeOfJob { get; }
		public abstract Task ExecuteJob(bool force);
		public string Name { get; set; } = string.Empty;
		public bool CompletedSuccess = false;
		public new abstract string ToString { get; }
	}
}
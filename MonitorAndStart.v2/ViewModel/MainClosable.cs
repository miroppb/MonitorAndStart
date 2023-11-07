using System;

namespace MonitorAndStart.v2.ViewModel
{
	public abstract class ClosableViewModel
	{
		public event EventHandler? ClosingRequest;

		protected void OnClosingRequest()
		{
			ClosingRequest?.Invoke(this, EventArgs.Empty);
		}
	}
}

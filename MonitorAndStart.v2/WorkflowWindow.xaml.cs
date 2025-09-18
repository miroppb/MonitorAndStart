using MonitorAndStart.v2.ViewModel;
using System.Windows;

namespace MonitorAndStart.v2
{
	/// <summary>
	/// Interaction logic for AddWorkflowWindow.xaml
	/// </summary>
	public partial class WorkflowWindow : Window
	{
		public WorkflowViewModel _viewmodel;

		public WorkflowWindow(MainViewModel vm)
		{
			InitializeComponent();
			_viewmodel = new WorkflowViewModel(vm);
			DataContext = _viewmodel;
			_viewmodel.ClosingRequest += (sender, e) => Close();
		}

		private void LstItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (_viewmodel.SelectedJob != null)
			{
				//lets edit the item
				JobWindow EditingWindow = new(_viewmodel)
				{
					Title = "Edit"
				};
				EditingWindow.BtnClose.Content = "Save";
				_viewmodel.Editing = true;
				EditingWindow._addvm.PreviousJobs.Clear();
				MainViewModel.AllJobs.ForEach(EditingWindow._addvm.PreviousJobs.Add);
				EditingWindow._addvm.SelectedJob = _viewmodel.SelectedJob;
				EditingWindow.ShowDialog();
			}
		}
	}
}

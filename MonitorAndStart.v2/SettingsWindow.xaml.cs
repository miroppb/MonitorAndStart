using MonitorAndStart.v2.ViewModel;
using System.Windows;

namespace MonitorAndStart.v2
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		private readonly SettingsViewModel _viewModel;

		public SettingsWindow()
		{
			InitializeComponent();
			_viewModel = new SettingsViewModel();
			DataContext = _viewModel;
			_viewModel.ClosingRequest += (sender, e) => Close();
		}
	}
}

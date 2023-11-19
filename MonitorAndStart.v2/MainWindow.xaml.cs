using Microsoft.Win32;
using miroppb;
using MonitorAndStart.v2.Data;
using MonitorAndStart.v2.ViewModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MonitorAndStart.v2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();
			libmiroppb.Log($"Welcome to Monitor and Start 2. v{Assembly.GetEntryAssembly()!.GetName().Version}");
			_viewModel = new MainViewModel(new MainDataProvider(), this);
			DataContext = _viewModel;
			Loaded += MainWindow_Loaded;
			TxtVar1.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(TxtVar1_MouseLeftButtonDown), true);
			_viewModel.ClosingRequest += (sender, e) => Close();
			StateChanged += MainWindow_StateChanged;
		}

		private void MainWindow_StateChanged(object? sender, System.EventArgs e)
		{
			if (WindowState == WindowState.Minimized)
				Hide();
		}

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			await _viewModel.LoadAsync();
			_viewModel.ExecuteTasks(true);
			Closing += ScanWindow_Closing;
			await Task.Delay(1000);
			Hide();
		}

		private void TxtVar1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_viewModel.SelectedJob is File | _viewModel.SelectedJob is Script)
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = "Application File|*.exe";
				if (ofd.ShowDialog() == true)
				{
					_viewModel.Var1 = ofd.FileName;
				}
			}
			else if (_viewModel.SelectedJob is Stuck)
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = "Any File|*.*";
				if (ofd.ShowDialog() == true)
				{
					_viewModel.Var1 = ofd.FileName;
				}
			}
		}

		private void ScanWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			libmiroppb.Log("Application closing");
			_viewModel.tbi.Dispose();
		}
	}
}

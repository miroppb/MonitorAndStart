using miroppb;
using MonitorAndStart.v2.Data;
using MonitorAndStart.v2.Models;
using MonitorAndStart.v2.ViewModel;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
			Libmiroppb.Log($"Welcome to Monitor and Start 2. v{Assembly.GetEntryAssembly()!.GetName().Version}");
			Title = $"Monitor and Start 2 v.{Assembly.GetEntryAssembly()!.GetName().Version}";
			bool ShowTbi = true;
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				if (Environment.GetCommandLineArgs()[1] == "--silent")
					ShowTbi = false;
			}

			_viewModel = new MainViewModel(new MainDataProvider(), new NotificationProvider(), this, ShowTbi);
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
			Closing += MainWindow_Closing;
			await Task.Delay(1000);
			Hide();
		}

		private void TxtVar1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//if (_viewModel.SelectedJob is File | _viewModel.SelectedJob is Script)
			//{
			//	OpenFileDialog ofd = new()
			//	{
			//		Filter = "Application File|*.exe|Batch File|*.bat"
			//	};
			//	if (ofd.ShowDialog() == true)
			//	{
			//		_viewModel.Var1 = ofd.FileName;
			//	}
			//}
			//else if (_viewModel.SelectedJob is Stuck)
			//{
			//	OpenFileDialog ofd = new()
			//	{
			//		Filter = "Any File|*.*"
			//	};
			//	if (ofd.ShowDialog() == true)
			//	{
			//		_viewModel.Var1 = ofd.FileName;
			//	}
			//}
		}

		private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			Libmiroppb.Log("Application closing");
			_viewModel.tbi.Dispose();
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (_viewModel != null)
			{
				if (e.NewValue is Workflow workflow)
				{
					// Workflow is selected, set it and clear job selection
					_viewModel.SelectedWorkflow = workflow;
					_viewModel.SelectedJob = null;
				}
				else if (e.NewValue is Job job)
				{
					// Job is selected, set the selected job
					_viewModel.SelectedJob = job;

					// Find the parent workflow by looking up the TreeViewItem of the job
					_viewModel.SelectedWorkflow = GetParentWorkflowFromJob(sender as TreeView);
				}
			}
		}

		// Helper method to find the parent Workflow of the selected job in the TreeView
		private Workflow? GetParentWorkflowFromJob(TreeView? treeView)
		{
			// Traverse the visual tree to find the parent TreeViewItem representing the Workflow
			var container = treeView?.ItemContainerGenerator.ContainerFromItem(_viewModel.SelectedJob) as TreeViewItem;

			if (container != null)
			{
				// Now get the parent TreeViewItem, which represents the Workflow
				var parentItem = VisualTreeHelper.GetParent(container) as TreeViewItem;

				// If we find the parent TreeViewItem, get its DataContext, which will be the Workflow
				if (parentItem != null && parentItem.DataContext is Workflow parentWorkflow)
				{
					return parentWorkflow;
				}
			}

			return null;
		}


		private bool isHandled = false;

		private async void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (isHandled)
			{
				// Reset the flag and return to prevent further handling
				isHandled = false;
				return;
			}

			if (sender is TreeViewItem item)
			{
				if (item.DataContext is Workflow workflow)
				{
					// Handle double-click on Workflow
					WorkflowWindow window = new(_viewModel);
					window._viewmodel.CurrentWorkflow = workflow;
					window.ShowDialog();
					await _viewModel.LoadAsync();
				}
				else if (item.DataContext is Job job)
				{
					// Not handling this right now
				}

				// Mark the event as handled to prevent it from bubbling up
				e.Handled = true;
				isHandled = true;
			}
		}
	}
}

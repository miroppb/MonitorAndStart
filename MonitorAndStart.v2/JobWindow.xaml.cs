using Microsoft.Win32;
using MonitorAndStart.v2.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MonitorAndStart.v2
{
	/// <summary>
	/// Interaction logic for JobWindow.xaml
	/// </summary>
	public partial class JobWindow : Window
	{
		public readonly WorkflowViewModel _viewmodel;
		public readonly JobViewModel _addvm;

		public JobWindow(WorkflowViewModel vm)
		{
			InitializeComponent();
			_addvm = new JobViewModel(vm, this);
			_viewmodel = vm;
			DataContext = _addvm;

			TxtVar1.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(TxtVar1_MouseLeftButtonDown), true);
			TxtVar7.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(TxtVar7_MouseLeftButtonDown), true);
			_addvm.ClosingRequest += (sender, e) => Close();
		}

		private void TxtVar1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_addvm.SelectedType == 0 | _addvm.SelectedType == 3)
			{
				OpenFileDialog ofd = new()
				{
					Filter = "Application File|*.exe|Batch File|*.bat",
					InitialDirectory = Path.GetDirectoryName(_addvm.Var1),
					FileName = _addvm.Var1
				};
				if (ofd.ShowDialog() == true)
				{
					_addvm.Var1 = ofd.FileName;
				}
			}
			else if (_addvm.SelectedType == 2)
			{
				OpenFileDialog ofd = new()
				{
					Filter = "Any File|*.*",
                    InitialDirectory = Path.GetDirectoryName(_addvm.Var1),
                    FileName = _addvm.Var1
                };
				if (ofd.ShowDialog() == true)
				{
					_addvm.Var1 = ofd.FileName;
				}
			}
		}

		private void TxtVar7_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_addvm.SelectedType == 4)
			{
				SaveFileDialog ofd = new()
				{
					Filter = "Any File|*.*",
                    InitialDirectory = Path.GetDirectoryName(_addvm.Var7),
                    FileName = _addvm.Var7
                };
				if (ofd.ShowDialog() == true)
				{
					_addvm.Var7 = ofd.FileName;
				}
			}
		}
	}
}

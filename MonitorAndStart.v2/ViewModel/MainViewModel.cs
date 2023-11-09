using Hardcodet.Wpf.TaskbarNotification;
using miroppb;
using MonitorAndStart.v2.Command;
using MonitorAndStart.v2.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MonitorAndStart.v2.ViewModel
{
	public class MainViewModel : ClosableViewModel, INotifyPropertyChanged
	{
		private readonly IMainDataProvider _mainDataProvider;

		public DelegateCommand AddNewJob { get; }
		public DelegateCommand SaveJob { get; }

		public TaskbarIcon tbi;

		public event PropertyChangedEventHandler? PropertyChanged;

		private static Window? MainWindow { get; set; }

		private ContextMenu _contextMenu;

		protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<Job> Jobs { get; } = new();

		private Job? _SelectedJob;

		public Job SelectedJob
		{
			get => _SelectedJob!;
			set
			{
				_SelectedJob = value;
				if (SelectedJob != null)
				{
					if (SelectedJob is File file)
					{
						Var1Text = File.Vars[0];
						Var1 = file.filename;
						Var2Text = File.Vars[1];
						Var2 = file.parameters;
						Var3Text = File.Vars[2];
						Var3 = file.restart;
						Var4Text = File.Vars[3];
						Var4 = file.runAsAdmin;
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Visibility.Visible;
						Var5Visible = Visibility.Hidden;
					}
					else if (SelectedJob is Service service)
					{
						Var5Text = File.Vars[1];
						Var5 = Service.GetServices();
						SelectedVar5 = service.ServiceName;
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Visibility.Hidden;
						Var5Visible = Visibility.Visible;
					}
					else if (SelectedJob is Stuck stuck)
					{
						Var1Text = File.Vars[0];
						Var1 = stuck.Filename;
						Var2Text = Stuck.Vars[1];
						Var2 = stuck.StuckLongerThanMinutes.ToString();
						Var1Visible = Var2Visible = Visibility.Visible;
						Var3Visible = Var4Visible = Var5Visible = Visibility.Hidden;
					}
					else if (SelectedJob is Script script)
					{
						Var1Text = Script.Vars[0];
						Var1 = script.Filename;
						Var2Text = Script.Vars[1];
						Var2 = script.Parameters;
						Var3Text = Script.Vars[2];
						Var3 = script.RunAsAdmin;
						Var4Text = Script.Vars[3];
						Var4 = script.RunHidden;
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Visibility.Visible;
						Var5Visible = Visibility.Hidden;
					}
					SelectedInterval = (int)SelectedJob.Interval;
					switch (SelectedJob.Interval)
					{
						case Enums.Intervals.Minutes:
							Interval = SelectedJob.IntervalInMinutes;
							break;
						case Enums.Intervals.Hours:
							Interval = SelectedJob.IntervalInMinutes / 60;
							break;
						case Enums.Intervals.Days:
							Interval = SelectedJob.IntervalInMinutes / 60 / 24;
							break;
						case Enums.Intervals.Weeks:
							Interval = SelectedJob.IntervalInMinutes / 60 / 24 / 7;
							break;
					}
					RunOnStart = SelectedJob.RunOnStart;
					StartDate = SelectedJob.LastRun;
					ShowLastRun = $"Last Run: {SelectedJob.LastRun}";
					ShowNextRun = $"Next Run: {SelectedJob.NextTimeToRun}";
				}
				RaisePropertyChanged();
			}
		}

		public MainViewModel(IMainDataProvider mainDataProvider, Window mainWindow)
		{
			_mainDataProvider = mainDataProvider;
			AddNewJob = new DelegateCommand(ExecuteAddNewJob, () => true);
			SaveJob = new DelegateCommand(ExecuteSaveJob, () => true);

			MainWindow = mainWindow;
			_contextMenu = new ContextMenu();

			MenuItem menuShowWindow = new MenuItem
			{
				Header = "Show"
			};
			menuShowWindow.Click += MenuShowWindow_Click;

			_contextMenu.Items.Add(menuShowWindow);

			tbi = new TaskbarIcon
			{
				Icon = System.Drawing.Icon.ExtractAssociatedIcon("monitor.ico"),
				ToolTipText = "Monitor And Start",
				ContextMenu = _contextMenu
			};

			SetupTimer();
		}

		private void MenuShowWindow_Click(object sender, RoutedEventArgs e)
		{
			if (MainWindow != null)
			{
				MainWindow.Show();
				MainWindow.WindowState = WindowState.Normal;
			}
		}

		private void ExecuteSaveJob(object obj)
		{
			if (SelectedJob != null)
			{
				_mainDataProvider.UpdateRecord(SelectedJob);
			}
		}

		private void SetupTimer()
		{
			int minutes = 1;
			libmiroppb.Log($"Setting up the Timer for every {minutes} minutes");
			DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(minutes) };
			timer.Tick += delegate
			{
				ExecuteTasks(false);
			};
			timer.Start();
		}

		internal async Task LoadAsync()
		{
			if (Jobs.Any())
				Jobs.Clear();

			IEnumerable<Job>? jobs = await _mainDataProvider.GetJobsAsync();
			jobs?.ToList().ForEach(Jobs.Add);
		}

		internal void ExecuteTasks(bool start)
		{
			foreach (Job job in Jobs)
			{
				try
				{
					if (job.NextTimeToRun <= DateTime.Now)
					{
						job.ExecuteJob();
						job.LastRun = DateTime.Now;
						job.NextTimeToRun = DateTime.Now.AddMinutes(job.IntervalInMinutes);

						_mainDataProvider.UpdateRecord(job);
					}
					else if (start & job.RunOnStart)
					{
						job.ExecuteJob();
						job.LastRun = DateTime.Now;
						job.NextTimeToRun = DateTime.Now.AddMinutes(job.IntervalInMinutes);

						_mainDataProvider.UpdateRecord(job);
					}
				}
				catch { }
			}
		}

		private async void ExecuteAddNewJob(object a)
		{
			AddNewWindow win = new(this);
			win.ShowDialog();
			await LoadAsync();
		}

		internal void InsertNewJob(Job? obj)
		{
			if (obj != null)
				_mainDataProvider.InsertRecord(obj);
		}

		public static ObservableCollection<string> Intervals => new() { "Weeks", "Days", "Hours", "Minutes" };

		private int _SelectedInterval = 3;

		public int SelectedInterval
		{
			get => _SelectedInterval;
			set
			{
				_SelectedInterval = value;
				RaisePropertyChanged();
			}
		}

		private string _Var1Text = string.Empty;

		public string Var1Text
		{
			get => _Var1Text;
			set
			{
				_Var1Text = value;
				RaisePropertyChanged();
			}
		}

		private string _Var1 = string.Empty;

		public string Var1
		{
			get => _Var1;
			set
			{
				_Var1 = value;
				if (SelectedJob is File)
					(SelectedJob as File)!.filename = Var1;
				else if (SelectedJob is Stuck)
					(SelectedJob as Stuck)!.Filename = Var1;
				else if (SelectedJob is Script)
					(SelectedJob as Script)!.Filename = Var1;

				RaisePropertyChanged();
			}
		}

		private string _Var2Text = string.Empty;

		public string Var2Text
		{
			get => _Var2Text;
			set
			{
				_Var2Text = value;
				RaisePropertyChanged();
			}
		}

		private string _Var2 = string.Empty;

		public string Var2
		{
			get => _Var2;
			set
			{
				_Var2 = value;
				if (SelectedJob is File)
					(SelectedJob as File)!.parameters = Var2;
				else if (SelectedJob is Stuck)
					(SelectedJob as Stuck)!.StuckLongerThanMinutes = int.Parse(Var2);
				else if (SelectedJob is Script)
					(SelectedJob as Script)!.Parameters = Var2;

				RaisePropertyChanged();
			}
		}

		private string _Var3Text = string.Empty;

		public string Var3Text
		{
			get => _Var3Text;
			set
			{
				_Var3Text = value;
				RaisePropertyChanged();
			}
		}

		private bool _Var3;

		public bool Var3
		{
			get => _Var3;
			set
			{
				_Var3 = value;
				if (SelectedJob is File)
					(SelectedJob as File)!.restart = Var3;
				else if (SelectedJob is Script)
					(SelectedJob as Script)!.RunAsAdmin = Var3;

				RaisePropertyChanged();
			}
		}


		private string _Var4Text = string.Empty;

		public string Var4Text
		{
			get => _Var4Text;
			set
			{
				_Var4Text = value;
				RaisePropertyChanged();
			}
		}

		private bool _Var4;

		public bool Var4
		{
			get => _Var4;
			set
			{
				_Var4 = value;
				if (SelectedJob is File)
					(SelectedJob as File)!.runAsAdmin = Var4;
				else if (SelectedJob is Script)
					(SelectedJob as Script)!.RunHidden = Var4;

				RaisePropertyChanged();
			}
		}

		private string _Var5Text = string.Empty;

		public string Var5Text
		{
			get => _Var5Text;
			set
			{
				_Var5Text = value;
				RaisePropertyChanged();
			}
		}

		private string _SelectedVar5 = string.Empty;

		public string SelectedVar5
		{
			get => _SelectedVar5;
			set
			{
				_SelectedVar5 = value;
				if (SelectedJob is Service)
					(SelectedJob as Service)!.ServiceName = SelectedVar5;
				RaisePropertyChanged();
			}
		}

		private List<string> _Var5 = new();

		public List<string> Var5
		{
			get => _Var5;
			set
			{
				_Var5 = value;
				RaisePropertyChanged();
			}
		}

		private Visibility _Var1Visible = Visibility.Hidden;

		public Visibility Var1Visible
		{
			get => _Var1Visible;
			set
			{
				_Var1Visible = value;
				RaisePropertyChanged();
			}
		}
		private Visibility _Var2Visible = Visibility.Hidden;

		public Visibility Var2Visible
		{
			get => _Var2Visible;
			set
			{
				_Var2Visible = value;
				RaisePropertyChanged();
			}
		}
		private Visibility _Var3Visible = Visibility.Hidden;

		public Visibility Var3Visible
		{
			get => _Var3Visible;
			set
			{
				_Var3Visible = value;
				RaisePropertyChanged();
			}
		}
		private Visibility _Var4Visible = Visibility.Hidden;

		public Visibility Var4Visible
		{
			get => _Var4Visible;
			set
			{
				_Var4Visible = value;
				RaisePropertyChanged();
			}
		}
		private Visibility _Var5Visible = Visibility.Hidden;

		public Visibility Var5Visible
		{
			get => _Var5Visible;
			set
			{
				_Var5Visible = value;
				RaisePropertyChanged();
			}
		}

		private bool _RunOnStart;

		public bool RunOnStart
		{
			get => _RunOnStart;
			set
			{
				_RunOnStart = value;
				RaisePropertyChanged();
			}
		}

		private int _Interval;

		public int Interval
		{
			get => _Interval;
			set
			{
				_Interval = value;
				RaisePropertyChanged();
			}
		}

		private DateTime? _StartDate = DateTime.Now.AddDays(-2);

		public DateTime? StartDate
		{
			get => _StartDate;
			set
			{
				_StartDate = value;
				RaisePropertyChanged();
			}
		}

		private string _ShowLastRun = string.Empty;

		public string ShowLastRun
		{
			get => _ShowLastRun;
			set
			{
				_ShowLastRun = value;
				RaisePropertyChanged();
			}
		}

		private string _ShowNextRun = string.Empty;

		public string ShowNextRun
		{
			get => _ShowNextRun;
			set
			{
				_ShowNextRun = value;
				RaisePropertyChanged();
			}
		}

	}
}

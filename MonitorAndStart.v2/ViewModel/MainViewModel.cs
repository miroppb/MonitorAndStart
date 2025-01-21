using Hardcodet.Wpf.TaskbarNotification;
using miroppb;
using MonitorAndStart.v2.Command;
using MonitorAndStart.v2.Data;
using MonitorAndStart.v2.Enums;
using MonitorAndStart.v2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace MonitorAndStart.v2.ViewModel
{
	public class MainViewModel : ClosableViewModel, INotifyPropertyChanged
	{
		private readonly IMainDataProvider _mainDataProvider;

		public DelegateCommand AddNewJob { get; }
		public DelegateCommand SaveJob { get; }
		public DelegateCommand DeleteJob { get; }
		public DelegateCommand RunJob { get; }

		public TaskbarIcon tbi;

		public event PropertyChangedEventHandler? PropertyChanged;

		private static Window? MainWindow { get; set; }

		private readonly ContextMenu _contextMenu;
		private Timer? _executionTimer;
		private Timer? _uploadLogsTimer;

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
					IsJobSelected = true;
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
						Var6Text = File.Vars[4];
						Var6 = file.runOnce;
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Visibility.Visible;
						Var5Visible = Var7Visible = Visibility.Hidden;
					}
					else if (SelectedJob is Service service)
					{
						Var5Text = File.Vars[1];
						Var5 = Service.GetServices();
						SelectedVar5 = service.ServiceName;
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Var7Visible = Visibility.Hidden;
						Var5Visible = Visibility.Visible;
					}
					else if (SelectedJob is Stuck stuck)
					{
						Var1Text = File.Vars[0];
						Var1 = stuck.Filename;
						Var2Text = Stuck.Vars[1];
						Var2 = stuck.StuckLongerThanMinutes.ToString();
						Var1Visible = Var2Visible = Visibility.Visible;
						Var3Visible = Var4Visible = Var5Visible = Var6Visible = Var7Visible = Visibility.Hidden;
					}
					else if (SelectedJob is Script script)
					{
						Var1Text = Script.Vars[0];
						Var1 = script.filename;
						Var2Text = Script.Vars[1];
						Var2 = script.parameters;
						Var3Text = Script.Vars[2];
						Var3 = script.runAsAdmin;
						Var4Text = Script.Vars[3];
						Var4 = script.runHidden;
						Var6Text = Script.Vars[4];
						Var6 = script.runOnce;
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Visibility.Visible;
						Var5Visible = Var7Visible = Visibility.Hidden;
					}
					else if (SelectedJob is API api)
					{
						Var1Text = API.Vars[0];
						Var2Text = API.Vars[1];
						Var7Text = API.Vars[2];
						Var1 = api.url;
						Var2 = api.cookies;
						Var7 = api.output;
						Var1Visible = Var2Visible = Var7Visible = Visibility.Visible;
						Var5Visible = Var3Visible = Var4Visible = Var6Visible = Visibility.Hidden;
					}
					SelectedInterval = (int)SelectedJob.Interval;
					switch (SelectedJob.Interval)
					{
						case Enums.Intervals.Minutes:
							IntervalInMinutes = SelectedJob.IntervalInMinutes;
							break;
						case Enums.Intervals.Hours:
							IntervalInMinutes = SelectedJob.IntervalInMinutes / 60;
							break;
						case Enums.Intervals.Days:
							IntervalInMinutes = SelectedJob.IntervalInMinutes / 60 / 24;
							break;
						case Enums.Intervals.Weeks:
							IntervalInMinutes = SelectedJob.IntervalInMinutes / 60 / 24 / 7;
							break;
					}
					RunOnStart = SelectedJob.RunOnStart;
					StartDate = SelectedJob.LastRun;
					ShowLastRun = $"Last Run: {SelectedJob.LastRun}";
					ShowNextRun = $"Next Run: {SelectedJob.NextTimeToRun}";
				}
				else
					IsJobSelected = false;

				RaisePropertyChanged();
			}
		}

		public MainViewModel(IMainDataProvider mainDataProvider, Window mainWindow, bool ShowTbi)
		{
			_mainDataProvider = mainDataProvider;
			AddNewJob = new DelegateCommand(ExecuteAddNewJob, () => true);
			SaveJob = new DelegateCommand(ExecuteSaveJob, () => true);
			DeleteJob = new DelegateCommand(ExecuteDeleteJob, () => true);
			RunJob = new DelegateCommand(ExecuteRunCurrentJob, () => true);

			MainWindow = mainWindow;
			_contextMenu = new ContextMenu();

			MenuItem menuShowWindow = new()
			{
				Header = "Show"
			};
			menuShowWindow.Click += MenuShowWindow_Click;
			MenuItem menuExit = new()
			{
				Header = "Exit"
			};
			menuExit.Click += MenuExit_Click;

			_contextMenu.Items.Add(menuShowWindow);
			_contextMenu.Items.Add(menuExit);

			tbi = new TaskbarIcon
			{
				Icon = System.Drawing.Icon.ExtractAssociatedIcon("monitor.ico"),
				ToolTipText = "Monitor And Start",
				ContextMenu = _contextMenu,
				Visibility = ShowTbi ? Visibility.Visible : Visibility.Collapsed
			};

			SetupTimer();
			SetupUploadLogsTimer();
		}

		private void MenuExit_Click(object sender, RoutedEventArgs e)
		{
			OnClosingRequest();
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
				UpdateIntervalInMinutes();
				SelectedJob.NextTimeToRun = SelectedJob.LastRun.AddMinutes(SelectedJob.IntervalInMinutes);
				if (_mainDataProvider.UpdateRecord(SelectedJob))
					MessageBox.Show("Saved");
			}
		}

		private void ExecuteDeleteJob(object obj)
		{
			if (SelectedJob != null)
			{
				if (MessageBox.Show($"Delete job {SelectedJob.Name}?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					_mainDataProvider.DeleteRecord(SelectedJob);
					Jobs.Remove(SelectedJob);
					SelectedJob = Jobs.LastOrDefault()!;
				}
			}
		}

		private void ExecuteRunCurrentJob(object obj) 
		{
			SelectedJob?.ExecuteJob(true);
		}

		private void UpdateIntervalInMinutes()
		{
			switch (SelectedJob.Interval)
			{
				case Enums.Intervals.Minutes:
					SelectedJob.IntervalInMinutes = IntervalInMinutes;
					break;
				case Enums.Intervals.Hours:
					SelectedJob.IntervalInMinutes = IntervalInMinutes * 60;
					break;
				case Enums.Intervals.Days:
					SelectedJob.IntervalInMinutes = IntervalInMinutes * 60 * 24;
					break;
				case Enums.Intervals.Weeks:
					SelectedJob.IntervalInMinutes = IntervalInMinutes * 60 * 24 * 7;
					break;
			}
		}

		private void SetupTimer()
		{
			int minutes = 1;
			Libmiroppb.Log($"Setting up the Timer for every {minutes} minutes");
			_executionTimer = new Timer(TimeSpan.FromMinutes(minutes));
			_executionTimer.Elapsed += ExecutionTimer_Elapsed;
			_executionTimer.Start();
		}

		private void ExecutionTimer_Elapsed(object? sender, ElapsedEventArgs e) => ExecuteTasks(false);

		private void SetupUploadLogsTimer()
		{
			int minutes = 30;
			Libmiroppb.Log($"Setting up uploadLogs timer for every {minutes} minutes");
			_uploadLogsTimer = new Timer(TimeSpan.FromMinutes(minutes));
			_uploadLogsTimer.Elapsed += UploadLogsTimer_Elapsed; ;
			_uploadLogsTimer.Start();
		}

		private void UploadLogsTimer_Elapsed(object? sender, ElapsedEventArgs e) => UploadLogs(true);

		private async static void UploadLogs(bool deleteAfter) => await Libmiroppb.UploadLogAsync(Secrets.GetConnectionString().ConnectionString, deleteAfter);

		internal async Task LoadAsync()
		{
			if (Jobs.Any())
				Jobs.Clear();

			IEnumerable<Job>? jobs = await TryGetJobsAsync();
			jobs?.ToList().ForEach(Jobs.Add);
		}

		private async Task<IEnumerable<Job>?> TryGetJobsAsync()
		{
			IEnumerable<Job>? jobs = null;
			while (jobs == null)
			{
				jobs = await _mainDataProvider.GetJobsAsync();
				if (jobs == null)
					await Task.Delay(TimeSpan.FromHours(1));
				else
					SelectedJob = jobs.First();
			}
			return jobs;
		}


        internal void ExecuteTasks(bool start)
		{
			foreach (Job job in Jobs)
			{
				try
				{
					if (job.NextTimeToRun <= DateTime.Now)
					{
						job.ExecuteJob(false);
						UpdateTimesBasedOnType(job);

						_mainDataProvider.UpdateRecord(job);
					}
					else if (start & job.RunOnStart)
					{
						job.ExecuteJob(true);
						UpdateTimesBasedOnType(job);

						_mainDataProvider.UpdateRecord(job);
					}
				}
				catch (Exception ex) { Libmiroppb.Log($"Error while executing job: {ex.Message}"); }
			}
		}

		void UpdateTimesBasedOnType(Job job)
		{
			switch (job)
			{
				case File file when !file.runOnce:
				case Script script when !script.runOnce:
					job.UpdateTimes();
					break;
				case Stuck:
				case API:
				case Service:
					job.UpdateTimes(); // Always update times for these types
					break;
				default: // Handle other types if necessary
					break;
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
				SelectedJob.Interval = (Intervals)SelectedInterval;
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
					(SelectedJob as Script)!.filename = Var1;
				else if (SelectedJob is API)
					(SelectedJob as API)!.url = Var1;

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
					(SelectedJob as Script)!.parameters = Var2;
				else if (SelectedJob is API)
					(SelectedJob as API)!.cookies = Var2;

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
					(SelectedJob as Script)!.runAsAdmin = Var3;

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
					(SelectedJob as Script)!.runHidden = Var4;

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

		private bool _Var6;

		public bool Var6
		{
			get => _Var6;
			set
			{
				_Var6 = value;
				RaisePropertyChanged();
			}
		}

		private string _Var6Text = string.Empty;

		public string Var6Text
		{
			get => _Var6Text;
			set
			{
				_Var6Text = value;
				RaisePropertyChanged();
			}
		}

		private string _Var7Text = string.Empty;

		public string Var7Text
		{
			get => _Var7Text;
			set
			{
				_Var7Text = value;
				RaisePropertyChanged();
			}
		}

		private string _Var7 = string.Empty;

		public string Var7
		{
			get => _Var7;
			set
			{
				_Var7 = value;
				if (SelectedJob is API)
					(SelectedJob as API)!.output = Var7;

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

		private Visibility _Var6Visible = Visibility.Hidden;

		public Visibility Var6Visible
		{
			get => _Var6Visible;
			set
			{
				_Var6Visible = value;
				RaisePropertyChanged();
			}
		}

		private Visibility _Var7Visible = Visibility.Hidden;

		public Visibility Var7Visible
		{
			get => _Var7Visible;
			set
			{
				_Var7Visible = value;
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
				SelectedJob.RunOnStart = _RunOnStart;
				RaisePropertyChanged();
			}
		}

		private int _IntervalInMinutes;

		public int IntervalInMinutes
		{
			get => _IntervalInMinutes;
			set
			{
				_IntervalInMinutes = value;
				UpdateIntervalInMinutes();
				RaisePropertyChanged();
			}
		}

		private DateTime? _StartDate = DateTime.Now;

		public DateTime? StartDate
		{
			get => _StartDate;
			set
			{
				_StartDate = value;
				SelectedJob.LastRun = _StartDate!.Value;
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


		private bool _IsJobSelected = false;

		public bool IsJobSelected
		{
			get => _IsJobSelected;
			set
			{
				_IsJobSelected = value;
				RaisePropertyChanged();
			}
		}
			
	}
}

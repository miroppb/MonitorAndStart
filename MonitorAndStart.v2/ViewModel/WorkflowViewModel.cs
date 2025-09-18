using MonitorAndStart.v2.Command;
using MonitorAndStart.v2.Data;
using MonitorAndStart.v2.Helpers;
using MonitorAndStart.v2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace MonitorAndStart.v2.ViewModel
{
	public class WorkflowViewModel : ClosableViewModel, INotifyPropertyChanged
	{
		public DelegateCommand AddCommand { get; }
		public DelegateCommand RemoveCommand { get; }
		public DelegateCommand UpCommand { get; }
		public DelegateCommand DownCommand { get; }
		public DelegateCommand CloseCommand { get; }
		private readonly MainViewModel _vm;

		public bool Editing = false;

		private readonly IMainDataProvider _mainDataProvider;

		public event PropertyChangedEventHandler? PropertyChanged;
		protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public WorkflowViewModel(MainViewModel vm)
		{
			AddCommand = new DelegateCommand(ExecuteAddJob, () => true);
			RemoveCommand = new DelegateCommand(ExecuteRemoveJob, () => SelectedJob != null);
			UpCommand = new DelegateCommand(ExecuteUp, () => SelectedJob != null && Jobs.IndexOf(SelectedJob) > 0);
			DownCommand = new DelegateCommand(ExecuteDown, () => SelectedJob != null && Jobs.IndexOf(SelectedJob) < Jobs.Count - 1);
			CloseCommand = new DelegateCommand(ExecuteClose, () => true);
			_vm = vm;
			_mainDataProvider = new MainDataProvider();
		}

		private void LoadWorkflow()
		{
			if (Jobs.Any())
				Jobs.Clear();

			CurrentWorkflow.Jobs.ForEach(Jobs.Add);
		}

		private void ExecuteAddJob(object obj)
		{
			if (CurrentWorkflow.Name != string.Empty)
			{
				JobWindow AddNew = new(this);
				AddNew._addvm.PreviousJobs.Clear();
				MainViewModel.AllJobs.ForEach(AddNew._addvm.PreviousJobs.Add);
				if (CurrentWorkflow.Id == -1)
				{
					//create a workflow
					int minutes = Interval;
					CurrentWorkflow.SelectedInterval = (Enums.Intervals)SelectedInterval;
					switch (CurrentWorkflow.SelectedInterval)
					{
						case Enums.Intervals.Hours:
							minutes *= 60;
							break;
						case Enums.Intervals.Days:
							minutes *= 60 * 24;
							break;
						case Enums.Intervals.Weeks:
							minutes *= 60 * 24 * 7;
							break;
					}
					CurrentWorkflow.IntervalInMinutes = minutes;
					CurrentWorkflow.LastRun = _StartDate;
					CurrentWorkflow.NextTimeToRun = CurrentWorkflow.LastRun.AddMinutes(minutes);
					CurrentWorkflow.RunOnStart = RunOnStart;
					CurrentWorkflow.Enabled = true;
					CurrentWorkflow.Notify = false;
					CurrentWorkflow.Id = _mainDataProvider.InsertWorkflow(CurrentWorkflow);
				}
				AddNew._addvm.Adding = true;
				AddNew.ShowDialog();
			}
			else
				MessageBox.Show("Please enter in a name for the Workflow");
		}

		private void ExecuteRemoveJob(object obj)
		{
			if (SelectedJob != null)
			{
				if (MessageBox.Show("Are you sure you want to remove this item?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					_mainDataProvider.DeleteJob(SelectedJob, CurrentWorkflow.Id);
                    Jobs.Remove(SelectedJob);
                    CurrentWorkflow.JobIDs = string.Join(',', Jobs);
				}
			}
		}

		private void ExecuteUp(object obj)
		{
			if (SelectedJob != null)
			{
				var index = Jobs.IndexOf(SelectedJob);
				if (index > 0)
				{
					Jobs.Move(index, index - 1);
					CurrentWorkflow.Jobs.MoveUp(SelectedJob);
					CurrentWorkflow.JobIDs = string.Join(',', Jobs);
                }
				_mainDataProvider.UpdateWorkflow(CurrentWorkflow);
			}
		}

		private void ExecuteDown(object obj)
		{
            if (SelectedJob != null)
            {
                var index = Jobs.IndexOf(SelectedJob);
                if (index < CurrentWorkflow.Jobs.Count)
                {
                    Jobs.Move(index, index + 1);
                    CurrentWorkflow.Jobs.MoveDown(SelectedJob);
                    CurrentWorkflow.JobIDs = string.Join(',', Jobs);
                }
                _mainDataProvider.UpdateWorkflow(CurrentWorkflow);
            }
        }

		private void ExecuteClose(object obj)
		{
			if (CurrentWorkflow.Jobs.Count > 0)
			{
				int minutes = Interval;
				CurrentWorkflow.SelectedInterval = (Enums.Intervals)SelectedInterval;
				switch (CurrentWorkflow.SelectedInterval)
				{
					case Enums.Intervals.Hours:
						minutes *= 60;
						break;
					case Enums.Intervals.Days:
						minutes *= 60 * 24;
						break;
					case Enums.Intervals.Weeks:
						minutes *= 60 * 24 * 7;
						break;
				}
				CurrentWorkflow.IntervalInMinutes = minutes;
				CurrentWorkflow.LastRun = _StartDate;
				CurrentWorkflow.NextTimeToRun = CurrentWorkflow.LastRun.AddMinutes(minutes);
				CurrentWorkflow.RunOnStart = RunOnStart;
				CurrentWorkflow.Notify = Notify;
				_mainDataProvider.UpdateWorkflow(CurrentWorkflow);
			}
			OnClosingRequest();
		}

		internal async void InsertNewJob(Job? obj)
		{
			if (obj != null)
			{
				int newJobID = _mainDataProvider.InsertJob(obj);
				var temp = await _mainDataProvider.GetJobsAsync();

				MainViewModel.AllJobs.Clear();
				MainViewModel.AllJobs.AddRange(temp!);

				Job job = MainViewModel.AllJobs.First(x => x.Id == newJobID);
				CurrentWorkflow.Jobs.Add(job);
				_mainDataProvider.UpdateWorkflow(CurrentWorkflow);
				LoadWorkflow();
			}
		}

		internal async void UpdateCurrentJob(Job? obj)
		{
			if (obj != null)
			{
				_mainDataProvider.UpdateJob(obj);
				var temp = await _mainDataProvider.GetJobsAsync();

				MainViewModel.AllJobs.Clear();
				MainViewModel.AllJobs.AddRange(temp!);

				CurrentWorkflow.Jobs.Clear();
				CurrentWorkflow.Jobs.AddRange(temp!.Where(x => CurrentWorkflow.JobIDs.Split(',').Select(int.Parse).Contains(x.Id)));

				LoadWorkflow();
			}
		}
		internal void InsertPreviousJob(Job obj)
		{
			if (obj != null)
			{
				Job job = MainViewModel.AllJobs.First(x => x.Id == obj.Id);
				CurrentWorkflow.Jobs.Add(job);
				_mainDataProvider.UpdateWorkflow(CurrentWorkflow);
				LoadWorkflow();
			}
		}

		private ObservableCollection<Job> _Jobs = new();

		public ObservableCollection<Job> Jobs
		{
			get => _Jobs;
			set
			{
				_Jobs = value;
				RaisePropertyChanged();
			}
		}

		private Job? _SelectedJob = null;

		public Job? SelectedJob
		{
			get => _SelectedJob;
			set
			{
				_SelectedJob = value;
				RemoveCommand.RaiseCanExecuteChanged();
				UpCommand.RaiseCanExecuteChanged();
				DownCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged();
			}
		}

		private Workflow _CurrentWorkflow = new();

		public Workflow CurrentWorkflow
		{
			get => _CurrentWorkflow;
			set
			{
				_CurrentWorkflow = value;

				Jobs.Clear();
				foreach (Job job in CurrentWorkflow.Jobs)
					Jobs.Add(job);
				RunOnStart = CurrentWorkflow.RunOnStart;
				switch (CurrentWorkflow.SelectedInterval)
				{
					case Enums.Intervals.Minutes:
						Interval = CurrentWorkflow.IntervalInMinutes;
						break;
					case Enums.Intervals.Hours:
						Interval = CurrentWorkflow.IntervalInMinutes / 60;
						break;
					case Enums.Intervals.Days:
						Interval = CurrentWorkflow.IntervalInMinutes / 24 / 60;
						break;
					case Enums.Intervals.Weeks:
						Interval = CurrentWorkflow.IntervalInMinutes / 7 / 24 / 60;
						break;
				}
				StartDate = CurrentWorkflow.LastRun;
				Notify = CurrentWorkflow.Notify;
				SelectedInterval = (int)CurrentWorkflow.SelectedInterval;

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
				CurrentWorkflow.RunOnStart = value;
				RaisePropertyChanged();
			}
		}

		private int _Interval = 1;

		public int Interval
		{
			get => _Interval;
			set
			{
				_Interval = value;
				switch (SelectedInterval)
				{
					case 0:
						CurrentWorkflow.IntervalInMinutes = Interval;
						break;
					case 1:
						CurrentWorkflow.IntervalInMinutes = Interval * 60;
						break;
					case 2:
						CurrentWorkflow.IntervalInMinutes = Interval * 60 * 24;
						break;
					case 3:
						CurrentWorkflow.IntervalInMinutes = Interval * 60 * 24 * 7;
						break;
				}
				RaisePropertyChanged();
			}
		}

		private DateTime _StartDate = DateTime.Now;

		public DateTime StartDate
		{
			get => _StartDate;
			set
			{
				if (value != _StartDate)
				{
					_StartDate = value;
                    CurrentWorkflow.LastRun = _StartDate;
				}
				RaisePropertyChanged();
			}
		}

		public static List<string> Intervals => new() { "Weeks", "Days", "Hours", "Minutes" };

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

		private bool _Notify;

		public bool Notify
		{
			get => _Notify;
			set
			{
				_Notify = value;
				RaisePropertyChanged();
			}
		}
	}
}

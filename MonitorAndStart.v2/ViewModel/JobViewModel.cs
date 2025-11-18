using MonitorAndStart.v2.Command;
using MonitorAndStart.v2.Data;
using MonitorAndStart.v2.Models;
using MonitorAndStart.v2.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MonitorAndStart.v2.ViewModel
{
	public class JobViewModel : ClosableViewModel, INotifyPropertyChanged
	{
		public DelegateCommand AddCommand { get; }
		public DelegateCommand ClearPreviousJob { get; }
		private readonly WorkflowViewModel _vm;
		private readonly JobWindow _win;

		public event PropertyChangedEventHandler? PropertyChanged;

		public bool Editing = false, Adding = false;

		protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public JobViewModel(WorkflowViewModel vm, JobWindow win)
		{
			AddCommand = new DelegateCommand(ExecuteAddNewJob, () => true);
			ClearPreviousJob = new DelegateCommand(ExecuteClearPreviousJob, () => PreviousJobSelected);

			_vm = vm;
			_win = win;

			MainDataProvider _mainData = new();

			if (Editing && vm.SelectedJob != null)
			{
				SelectedType = vm.SelectedJob.TypeOfJob;
				Name = vm.SelectedJob.Name;
				switch (SelectedType)
				{
					case 0:
						File? f = vm.SelectedJob as File;
						Var1 = f!.filename;
						Var2 = f!.parameters;
						Var3 = f!.restart;
						Var4 = f!.runAsAdmin;
						Var6 = f!.runOnce;
						Var8 = f!.consoleApp;
						break;
					case 1:
						Service? s = vm.SelectedJob as Service;
						SelectedVar5 = s!.ServiceName;
						break;
					case 2:
						Stuck? st = vm.SelectedJob as Stuck;
						Var1 = st!.Filename;
						Var2 = st!.StuckLongerThanMinutes.ToString();
						break;
					case 3:
						Script? sc = vm.SelectedJob as Script;
						Var1 = sc!.filename;
						Var2 = sc!.parameters;
						Var3 = sc!.runAsAdmin;
						Var4 = sc!.runHidden;
						Var6 = sc!.runOnce;
						break;
					case 4:
						API? a = vm.SelectedJob as API;
						Var1 = a!.url;
						Var2 = a!.cookies;
						Var7 = a!.output;
						break;
				}
				_win.BtnClose.Content = "Save";
			}
			else
			{

				Var1Text = File.Vars[0];
				Var2Text = File.Vars[1];
				Var3Text = File.Vars[2];
				Var4Text = File.Vars[3];
				Var6Text = File.Vars[4];
				Var8Text = File.Vars[5];

				Var7Visible = Visibility.Hidden; //When window opens, it's on File
			}
		}

		public static ObservableCollection<string> Types => new() { "File", "Service", "Stuck", "Script", "API", "Pause" };
		private ObservableCollection<Job> _PreviousJobs = new();

		public ObservableCollection<Job> PreviousJobs
		{
			get => _PreviousJobs;
			set
			{
				_PreviousJobs = value;
				RaisePropertyChanged();
			}
		}

		private int _SelectedType;

		public int SelectedType
		{
			get => _SelectedType;
			set
			{
				_SelectedType = value;
				switch (SelectedType)
				{
					case 0:
						//file
						Var1Text = File.Vars[0];
						Var2Text = File.Vars[1];
						Var3Text = File.Vars[2];
						Var4Text = File.Vars[3];
						Var6Text = File.Vars[4];
						Var8Text = File.Vars[5];
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Var8Visible = Visibility.Visible;
						Var5Visible = Var7Visible = Visibility.Hidden;
						break;
					case 1:
						//service
						Var5Text = Service.Vars[0];
						Var5 = Service.GetServices();
						Var5Visible = Visibility.Visible;
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Var7Visible = Var8Visible = Visibility.Hidden;
						break;
					case 2:
						//stuck
						Var1Text = Stuck.Vars[0];
						Var2Text = Stuck.Vars[1];
						Var1Visible = Var2Visible = Visibility.Visible;
						Var3Visible = Var4Visible = Var5Visible = Var6Visible = Var7Visible = Var8Visible = Visibility.Hidden;
						break;
					case 3:
						//script
						Var1Text = Script.Vars[0];
						Var2Text = Script.Vars[1];
						Var3Text = Script.Vars[2];
						Var4Text = Script.Vars[3];
						Var6Text = Script.Vars[4];
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Visibility.Visible;
						Var5Visible = Var7Visible = Var8Visible = Visibility.Hidden;
						break;
					case 4:
						//api
						Var1Text = API.Vars[0];
						Var2Text = API.Vars[1];
						Var7Text = API.Vars[2];
						Var3Text = API.Vars[3];
                        Var1Visible = Var2Visible = Var7Visible = Var3Visible = Visibility.Visible;
						Var4Visible = Var5Visible = Var6Visible = Var8Visible = Visibility.Hidden;
						break;
					case 5:
						//pause
						Var1Text = Pause.Vars[0];
						Var1Visible = Visibility.Visible;
						Var2Visible = Var3Visible = Var4Visible = Var5Visible = Var6Visible = Var7Visible = Var8Visible = Visibility.Hidden;
						break;
				}
				RaisePropertyChanged();
			}
		}

		private bool UsingPreviousJob = false;

		private Job? _SelectedJob = null;

		public Job? SelectedJob
		{
			get => _SelectedJob;
			set
			{
				_SelectedJob = value;
				if (SelectedJob != null)
				{
					UsingPreviousJob = true;
					//Set type and variables
					SelectedType = SelectedJob.TypeOfJob;
					Name = SelectedJob.Name;
					switch (SelectedType) //dont forget this assignment
                    {
						case 0:
							Var1 = (SelectedJob as File)!.filename;
							Var2 = (SelectedJob as File)!.parameters;
							Var3 = (SelectedJob as File)!.restart;
							Var4 = (SelectedJob as File)!.runAsAdmin;
							Var6 = (SelectedJob as File)!.runOnce;
							Var8 = (SelectedJob as File)!.consoleApp;
							break;
						case 1:
							SelectedVar5 = (SelectedJob as Service)!.ServiceName;
							break;
						case 2:
							Var1 = (SelectedJob as Stuck)!.Filename;
							Var2 = (SelectedJob as Stuck)!.StuckLongerThanMinutes.ToString();
							break;
						case 3:
							Var1 = (SelectedJob as Script)!.filename;
							Var2 = (SelectedJob as Script)!.parameters;
							Var3 = (SelectedJob as Script)!.runAsAdmin;
							Var4 = (SelectedJob as Script)!.runHidden;
							Var6 = (SelectedJob as Script)!.runOnce;
							break;
						case 4:
							Var1 = (SelectedJob as API)!.url;
							Var2 = (SelectedJob as API)!.cookies;
							Var7 = (SelectedJob as API)!.output;
							Var3 = (SelectedJob as API)!.NotifyOnFailureButComplete;
                            break;
						case 5:
							Var1 = (SelectedJob as Pause)!.seconds.ToString();
							break;
					}
				}
				RaisePropertyChanged();
			}
		}

		private void ExecuteAddNewJob(object a)
		{
			if (UsingPreviousJob && SelectedJob != null)
			{
				//compare previous with current values
				bool changed = false;
				try
				{
					switch (SelectedType) //dont forget this comparison
					{
						case 0:
							File job = (SelectedJob as File)!;
							changed = SelectedJob.Name != Name || Var1 != job.filename || Var2 != job.parameters || Var3 != job.restart || Var4 != job.runAsAdmin || Var6 != job.runOnce || Var8 != job.consoleApp;
							break;
						case 1:
							Service serv = (SelectedJob as Service)!;
							changed = SelectedJob.Name != Name || SelectedVar5 != serv.ServiceName;
							break;
						case 2:
							Stuck stuck = (SelectedJob as Stuck)!;
							changed = SelectedJob.Name != Name || Var1 != stuck.Filename || Var2 != stuck.StuckLongerThanMinutes.ToString();
							break;
						case 3:
							Script scr = (SelectedJob as Script)!;
							changed = SelectedJob.Name != Name || Var1 != scr.filename || Var2 != scr.parameters || Var3 != scr.runAsAdmin || Var4 != scr.runHidden || Var6 != scr.runOnce;
							break;
						case 4:
							API api = (SelectedJob as API)!;
							changed = SelectedJob.Name != Name || Var1 != api.url || Var2 != api.cookies || Var7 != api.output || Var3 != api.NotifyOnFailureButComplete;
							break;
						case 5:
							Pause pause = (SelectedJob as Pause)!;
							changed = SelectedJob.Name != Name || Var1 != pause.seconds.ToString();
							break;
					}
				}
				catch { changed = true; } //will happen if user changed types. In this case, save regardless
				if (changed)
				{
					bool Overwrite = MessageBox.Show("The Job was modified. Do you want to overwrite it?", "Overwrite?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
					if (Overwrite)
					{
						Job? obj = null;
						switch (SelectedType)
						{
							case 0:
								obj = new File(Name, Var1, Var2, Var3, Var4, Var6, Var8);
								break;
							case 1:
								obj = new Service(Name, SelectedVar5);
								break;
							case 2:
								obj = new Stuck(Name, Var1, int.Parse(Var2));
								break;
							case 3:
								obj = new Script(Name, Var1, Var2, Var3, Var4, Var6);
								break;
							case 4:
								obj = new API(Name, Var1, Var2, Var7, Var3);
								break;
							case 5:
								obj = new Pause(Name, Convert.ToInt32(Var1));
								break;
						}
						if (obj != null) obj.Enabled = SelectedJob.Enabled;
						(bool, string) valid = obj!.Validate();
						if (valid.Item1)
						{
							obj!.Id = _vm.SelectedJob!.Id;
							_vm.UpdateCurrentJob(obj);

						}
						else
							MessageBox.Show(valid.Item2);
					}
					else //ask about creating a new one
					{
						bool CreateNew = MessageBox.Show("Do you want to create a new Job with these parameters?", "Create new?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
						if (CreateNew)
						{
							Job? obj = null;
							switch (SelectedType)
							{
								case 0:
									obj = new File(Name, Var1, Var2, Var3, Var4, Var6, Var8);
									break;
								case 1:
									obj = new Service(Name, SelectedVar5);
									break;
								case 2:
									obj = new Stuck(Name, Var1, int.Parse(Var2));
									break;
								case 3:
									obj = new Script(Name, Var1, Var2, Var3, Var4, Var6);
									break;
								case 4:
									obj = new API(Name, Var1, Var2, Var7, Var3);
									break;
                                case 5:
                                    obj = new Pause(Name, Convert.ToInt32(Var1));
                                    break;
                            }
							if (obj != null) obj.Enabled = true;
							(bool, string) valid = obj!.Validate();
							if (valid.Item1)
							{
								_vm.InsertNewJob(obj);
								OnClosingRequest();
							}
							else
								MessageBox.Show(valid.Item2);
						}
					}
				}
				if (Adding) //if we're adding a previous job
				{
					Job? obj = SelectedJob;
					_vm.InsertPreviousJob(obj);
					OnClosingRequest();
				}
				OnClosingRequest();
			}
			else
			{
				Job? obj = null;
				switch (SelectedType)
				{
					case 0:
						obj = new File(Name, Var1, Var2, Var3, Var4, Var6, Var8);
						break;
					case 1:
						obj = new Service(Name, SelectedVar5);
						break;
					case 2:
						obj = new Stuck(Name, Var1, int.Parse(Var2));
						break;
					case 3:
						obj = new Script(Name, Var1, Var2, Var3, Var4, Var6);
						break;
					case 4:
						obj = new API(Name, Var1, Var2, Var7, Var3);
						break;
                    case 5:
                        obj = new Pause(Name, Convert.ToInt32(Var1));
                        break;
                }
				if (obj != null) obj.Enabled = true;
				(bool, string) valid = obj!.Validate();
				if (valid.Item1)
				{
					if (Editing)
					{
						obj!.Id = _vm.SelectedJob!.Id;
						_vm.UpdateCurrentJob(obj);
					}
					else
						_vm.InsertNewJob(obj);
					OnClosingRequest();
				}
				else
					MessageBox.Show(valid.Item2);
			}
		}

		private void ExecuteClearPreviousJob(object obj)
		{
			SelectedJob = null;
			UsingPreviousJob = false;
		}

		private string _Name = string.Empty;

		public string Name
		{
			get => _Name;
			set
			{
				_Name = value;
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
				RaisePropertyChanged();
			}
		}

        private string _Var8Text = string.Empty;

        public string Var8Text
        {
            get => _Var8Text;
            set
            {
                _Var8Text = value;
                RaisePropertyChanged();
            }
        }

        private bool _Var8;

        public bool Var8
        {
            get => _Var8;
            set
            {
                _Var8 = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _Var1Visible = Visibility.Visible;

		public Visibility Var1Visible
		{
			get => _Var1Visible;
			set
			{
				_Var1Visible = value;
				RaisePropertyChanged();
			}
		}
		private Visibility _Var2Visible = Visibility.Visible;

		public Visibility Var2Visible
		{
			get => _Var2Visible;
			set
			{
				_Var2Visible = value;
				RaisePropertyChanged();
			}
		}
		private Visibility _Var3Visible = Visibility.Visible;

		public Visibility Var3Visible
		{
			get => _Var3Visible;
			set
			{
				_Var3Visible = value;
				RaisePropertyChanged();
			}
		}
		private Visibility _Var4Visible = Visibility.Visible;

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

		private Visibility _Var6Visible = Visibility.Visible;

		public Visibility Var6Visible
		{
			get => _Var6Visible;
			set
			{
				_Var6Visible = value;
				RaisePropertyChanged();
			}
		}

		private Visibility _Var7Visible = Visibility.Visible;

		public Visibility Var7Visible
		{
			get => _Var7Visible;
			set
			{
				_Var7Visible = value;
				RaisePropertyChanged();
			}
		}

        private Visibility _Var8Visible = Visibility.Visible;

        public Visibility Var8Visible
        {
            get => _Var8Visible;
            set
            {
                _Var8Visible = value;
                RaisePropertyChanged();
            }
        }

        public bool PreviousJobSelected => SelectedJob != null;
	}
}

﻿using MonitorAndStart.v2.Command;
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
	public class AddNewViewModel : ClosableViewModel, INotifyPropertyChanged
	{
		public DelegateCommand AddCommand { get; }
		private readonly MainViewModel _vm;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public AddNewViewModel(MainViewModel vm)
		{
			AddCommand = new DelegateCommand(ExecuteAddNewJob, () => true);
			_vm = vm;

			Var1Text = File.Vars[0];
			Var2Text = File.Vars[1];
			Var3Text = File.Vars[2];
			Var4Text = File.Vars[3];
			Var6Text = File.Vars[4];
		}

		public static ObservableCollection<string> Types => new() { "File", "Service", "Stuck", "Script", "API" };

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
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Visibility.Visible;
						Var5Visible = Var7Visible = Visibility.Hidden;
						break;
					case 1:
						//service
						Var5Text = Service.Vars[0];
						Var5 = Service.GetServices();
						Var5Visible = Visibility.Visible;
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Var7Visible = Visibility.Hidden;
						break;
					case 2:
						//stuck
						Var1Text = Stuck.Vars[0];
						Var2Text = Stuck.Vars[1];
						Var1Visible = Var2Visible = Visibility.Visible;
						Var3Visible = Var4Visible = Var5Visible = Var6Visible = Var7Visible = Visibility.Hidden;
						break;
					case 3:
						//script
						Var1Text = Script.Vars[0];
						Var2Text = Script.Vars[1];
						Var3Text = Script.Vars[2];
						Var4Text = Script.Vars[3];
						Var6Text = Script.Vars[4];
						Var1Visible = Var2Visible = Var3Visible = Var4Visible = Var6Visible = Visibility.Visible;
						Var5Visible = Var7Visible = Visibility.Hidden;
						break;
					case 4:
						//api
						Var1Text = API.Vars[0];
						Var2Text = API.Vars[1];
						Var7Text = API.Vars[2];
						Var1Visible = Var2Visible = Var7Visible = Visibility.Visible;
						Var3Visible = Var4Visible = Var5Visible = Var6Visible = Visibility.Hidden;
						break;
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

		private void ExecuteAddNewJob(object a)
		{
			Job? obj = null;
			int minutes = Interval;
			switch ((Enums.Intervals)SelectedInterval)
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
			switch (SelectedType)
			{
				case 0:
					obj = new File(Name, Var1, Var2, Var3, Var4, Var6, minutes, (Enums.Intervals)SelectedInterval, _StartDate, _StartDate.AddMinutes(minutes), RunOnStart);
					break;
				case 1:
					obj = new Service(Name, SelectedVar5, minutes, (Enums.Intervals)SelectedInterval, _StartDate, _StartDate.AddMinutes(minutes), RunOnStart);
					break;
				case 2:
					obj = new Stuck(Name, Var1, int.Parse(Var2), minutes, (Enums.Intervals)SelectedInterval, _StartDate, _StartDate.AddMinutes(minutes), RunOnStart);
					break;
				case 3:
					obj = new Script(Name, Var1, Var2, Var3, Var4, Var6, minutes, (Enums.Intervals)SelectedInterval, _StartDate, _StartDate.AddMinutes(minutes), RunOnStart);
					break;
				case 4:
					obj = new API(Name, Var1, Var2, Var7, minutes, (Enums.Intervals)SelectedInterval, _StartDate, _StartDate.AddMinutes(minutes), RunOnStart);
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

		private DateTime _StartDate = DateTime.Now;

		public string StartDate
		{
			get => _StartDate.ToString();
			set
			{
				_StartDate = DateTime.Parse(value);
				RaisePropertyChanged();
			}
		}
	}
}

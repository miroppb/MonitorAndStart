using MonitorAndStart.v2.Command;
using MonitorAndStart.v2.Data;
using MonitorAndStart.v2.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MonitorAndStart.v2.ViewModel
{
	public class SettingsViewModel : ClosableViewModel, INotifyPropertyChanged
	{
		public DelegateCommand SaveCommand { get; }

		public IMainDataProvider _mainDataProvider;

		public SettingsViewModel()
		{
			SaveCommand = new DelegateCommand(ExecuteSaveCommand, () => true);
			_mainDataProvider = new MainDataProvider();

			NotificationEngines.Add("Prowl");
			NotificationEngines.Add("Ntfy");

			LoadSettings();
		}

		public async void LoadSettings()
		{
			CurrentSettings = await _mainDataProvider.GetSettings();
			if (CurrentSettings != null)
			{
				SelectedNotificationEngine = CurrentSettings.NotificationEngine;
				APIChannel = CurrentSettings.APIChannel;
			}
			else
				CurrentSettings = new();
		}

		private async void ExecuteSaveCommand(object obj)
		{
			CurrentSettings!.NotificationEngine = SelectedNotificationEngine;
			CurrentSettings.APIChannel = APIChannel;
			CurrentSettings.PcName = Environment.MachineName;
			await _mainDataProvider.SaveSettings(CurrentSettings);
			OnClosingRequest();
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public Settings? CurrentSettings { get; private set; }

		private ObservableCollection<string> _NotificationEngines = new();

		public ObservableCollection<string> NotificationEngines
		{
			get => _NotificationEngines;
			set
			{
				_NotificationEngines = value;
				RaisePropertyChanged();
			}
		}

		private string _SelectedNotificationEngine = string.Empty;

		public string SelectedNotificationEngine
		{
			get => _SelectedNotificationEngine;
			set
			{
				_SelectedNotificationEngine = value;
				RaisePropertyChanged();
			}
		}

		private string _APIChannel = string.Empty;

		public string APIChannel
		{
			get => _APIChannel;
			set
			{
				_APIChannel = value;
				RaisePropertyChanged();
			}
		}
	}
}

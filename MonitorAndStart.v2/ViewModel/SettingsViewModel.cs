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

            ConsoleEngines = new ObservableCollection<ConsoleEngine>((ConsoleEngine[])Enum.GetValues(typeof(ConsoleEngine)));

            LoadSettings();
        }

        public async void LoadSettings()
        {
            CurrentSettings = await _mainDataProvider.GetSettings();
            if (CurrentSettings != null)
            {
                SelectedNotificationEngine = CurrentSettings.NotificationEngine;
                APIChannel = CurrentSettings.APIChannel;
                SelectedConsoleEngine = CurrentSettings.Console;
                RunServer = CurrentSettings.RunServer;
                ServerPort = CurrentSettings.ServerPort;
            }
            else
                CurrentSettings = new();
        }

        private async void ExecuteSaveCommand(object obj)
        {
            CurrentSettings!.NotificationEngine = SelectedNotificationEngine;
            CurrentSettings.APIChannel = APIChannel;
            CurrentSettings.Console = SelectedConsoleEngine;
            CurrentSettings.PcName = Environment.MachineName;
            CurrentSettings.RunServer = RunServer;
            CurrentSettings.ServerPort = ServerPort;
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

        public ObservableCollection<ConsoleEngine> ConsoleEngines { get; }

        private ConsoleEngine _selectedConsoleEngine;
        public ConsoleEngine SelectedConsoleEngine
        {
            get => _selectedConsoleEngine;
            set
            {
                _selectedConsoleEngine = value;
                RaisePropertyChanged();
            }
        }

        private bool _RunServer;

        public bool RunServer
        {
            get => _RunServer;
            set
            {
                _RunServer = value;
                RaisePropertyChanged();
            }
        }

        private int _ServerPort;

        public int ServerPort
        {
            get => _ServerPort;
            set
            {
                _ServerPort = value;
                RaisePropertyChanged();
            }
        }
    }
}

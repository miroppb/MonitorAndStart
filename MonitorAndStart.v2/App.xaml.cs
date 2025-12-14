using MonitorAndStart.v2.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MonitorAndStart.v2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? mutex = null;

        protected override async void OnStartup(StartupEventArgs e)
        {
            const string appName = "MonitorAndStart.v2";
            mutex = new Mutex(true, appName, out bool createdNew);

            if (!createdNew)
            {
                Shutdown();
                return;
            }

            // Wait for Explorer / shell
            await ShellReady.WaitForExplorerAsync();

            base.OnStartup(e);

            // Create window manually
            var window = new MainWindow();
            window.Show();

            // Give WPF + shell time to attach
            await Task.Delay(750);

            // Detect failed startup
            if (!window.StartupConfirmed)
            {
                // Relaunch ourselves
                RestartSelf();
                Shutdown();
                return;
            }

            MainWindow = window;
        }

        private static bool IsWindowReallyVisible(Window w)
        {
            if (!w.IsVisible) return false;
            if (w.WindowState == WindowState.Minimized) return false;
            if (w.ActualWidth < 50 || w.ActualHeight < 50) return false;
            if (!w.IsLoaded) return false;
            return true;
        }

        private static void RestartSelf()
        {
            var exe = Environment.ProcessPath!;
            Process.Start(new ProcessStartInfo
            {
                FileName = exe,
                UseShellExecute = true
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Release the mutex
            mutex?.ReleaseMutex();
            mutex = null;

            base.OnExit(e);
        }
    }
}

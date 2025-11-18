using System.Threading;
using System.Windows;

namespace MonitorAndStart.v2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "MonitorAndStart.v2";

            mutex = new Mutex(true, appName, out bool createdNew);

            if (!createdNew)
            {
                // If another instance is already running, shut down this instance
                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Release the mutex
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex = null;
            }

            base.OnExit(e);
        }
    }
}

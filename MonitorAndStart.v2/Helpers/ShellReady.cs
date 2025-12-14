using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MonitorAndStart.v2.Helpers
{
    public static class ShellReady
    {
        public static async Task WaitForExplorerAsync(int timeoutMs = 15000)
        {
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (IsExplorerReady())
                    return;

                await Task.Delay(250);
            }
        }

        private static bool IsExplorerReady()
        {
            return Process.GetProcessesByName("explorer").Any()
                && GetShellWindow() != IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
    }

}

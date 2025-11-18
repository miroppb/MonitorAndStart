using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitorAndStart.v2.Helpers
{
    public class NonAdminLauncherClient
    {
        private const string PipeName = "MonitorAndStart_NonAdminLauncherPipe_v1";

        public static async Task<bool> LaunchProcessAsync(
            string exe,
            string args,
            string workingDir,
            bool hidden = false,
            Action<string>? logCallback = null)
        {
            using var client = new NamedPipeClientStream(
                ".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            await client.ConnectAsync(2000);

            using var sr = new StreamReader(client, Encoding.UTF8, false, 4096, leaveOpen: true);
            using var sw = new StreamWriter(client, Encoding.UTF8, 4096, leaveOpen: true) { AutoFlush = true };

            // Send launch request
            var request = new
            {
                Executable = exe,
                Arguments = args,
                WorkingDirectory = workingDir,
                Hidden = hidden
            };
            string json = JsonSerializer.Serialize(request);
            await sw.WriteLineAsync(json);

            // Optional: read initial acknowledgment (if you implement one in the helper)
            string? ack = await sr.ReadLineAsync();
            if (ack == null) return false;

            // Track exit code
            int exitCode = -1;

            // Stream output until we get exit message
            while (true)
            {
                string? line = await sr.ReadLineAsync();
                if (line == null) break;

                try
                {
                    var msg = JsonSerializer.Deserialize<StreamMessage>(line);
                    if (msg == null) continue;

                    if (msg.Type == "stdout" || msg.Type == "stderr")
                        logCallback?.Invoke($"{msg.Type}: {msg.Text}");
                    else if (msg.Type == "meta" && msg.Text?.StartsWith("exit:") == true)
                    {
                        if (int.TryParse(msg.Text.Substring(5), out int code))
                            exitCode = code;
                        break; // process finished
                    }
                }
                catch { /* ignore parse errors */ }
            }

            return exitCode == 0;
        }

        private class StreamMessage
        {
            public string Type { get; set; } = "stdout"; // "stdout", "stderr", "meta"
            public string Text { get; set; } = "";
        }
    }
}
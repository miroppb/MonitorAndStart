using System.Diagnostics;
using System.IO.Pipes;
using System.Text.Json;

namespace NonAdminLauncher
{
    internal class Program
    {
        private const string PipeName = "MonitorAndStart_NonAdminLauncherPipe_v1";

        static async Task Main()
        {
            Console.WriteLine("NonAdminLauncher starting...");

            while (true)
            {
                // Create a dedicated pipe for each client
#pragma warning disable CA1416 // Validate platform compatibility
                var server = new NamedPipeServerStream(
                    PipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous
                );
#pragma warning restore CA1416 // Validate platform compatibility

                Console.WriteLine($"Waiting for client on pipe: {PipeName}");
                await server.WaitForConnectionAsync();
                Console.WriteLine("Client connected.");

                // Handle the client in a background task
                _ = Task.Run(() => HandleClientAsync(server));
            }
        }

        private static async Task HandleClientAsync(NamedPipeServerStream server)
        {
            try
            {
                using var sr = new StreamReader(server);
                using var sw = new StreamWriter(server) { AutoFlush = true };

                // Read the launch request
                string? json = await sr.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(json)) return;

                var cmd = JsonSerializer.Deserialize<LaunchRequest>(json);
                if (cmd == null) return;

                var psi = new ProcessStartInfo(cmd.Executable, cmd.Arguments)
                {
                    WorkingDirectory = cmd.WorkingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = cmd.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(psi);
                if (process == null)
                {
                    await sw.WriteLineAsync(JsonSerializer.Serialize(new StreamMessage
                    {
                        Type = "meta",
                        Text = "exit:-1"
                    }));
                    return;
                }

                // Stream stdout
                var stdoutTask = Task.Run(async () =>
                {
                    try
                    {
                        string? line;
                        while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                        {
                            await sw.WriteLineAsync(JsonSerializer.Serialize(new StreamMessage { Type = "stdout", Text = line }));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[stdout pipe broken: {ex.GetType().Name}] {ex.Message}");
                    }
                });

                // Stream stderr
                var stderrTask = Task.Run(async () =>
                {
                    try
                    {
                        string? line;
                        while ((line = await process.StandardError.ReadLineAsync()) != null)
                        {
                            await sw.WriteLineAsync(JsonSerializer.Serialize(new StreamMessage { Type = "stderr", Text = line }));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[stderr pipe broken: {ex.GetType().Name}] {ex.Message}");
                    }
                });

                // Wait for process to exit
                await Task.Run(() => process.WaitForExit());

                await Task.WhenAll(stdoutTask, stderrTask);

                // Send final exit code
                try
                {
                    await sw.WriteLineAsync(JsonSerializer.Serialize(new StreamMessage { Type = "meta", Text = "exit:" + process.ExitCode }));
                    await sw.FlushAsync();
                }
                catch (IOException)
                {
                    // Client disconnected; safe to ignore
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pipe server error: {ex}");
            }
            finally
            {
                server.Dispose();
            }
        }

        private class LaunchRequest
        {
            public string Executable { get; set; } = "";
            public string Arguments { get; set; } = "";
            public string WorkingDirectory { get; set; } = "";
            public bool Hidden { get; set; }
        }

        private class StreamMessage
        {
            public string Type { get; set; } = "stdout"; // "stdout", "stderr", "meta"
            public string Text { get; set; } = "";
        }
    }
}

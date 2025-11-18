// WebServerHost.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MonitorAndStart.v2.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorAndStart.v2.Web
{
    public sealed class WebServerHost : IDisposable
    {
        private IHost? _host;

        public async Task StartAsync(MainViewModel vm, int port = 5000)
        {
            if (_host != null)
                return;

            // read credentials from environment (or fall back to defaults)
            var webUser = Environment.GetEnvironmentVariable("MNS_WEB_USER") ?? Secrets.WebUsername;
            var webPass = Environment.GetEnvironmentVariable("MNS_WEB_PASS") ?? Secrets.WebPassword;

            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://*:{port}");
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddRouting();
                    });

                    webBuilder.Configure(app =>
                    {
                        // Determine web root at runtime
                        string exeBase = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
                        string candidate1 = Path.Combine(exeBase, "wwwroot");
                        string candidate2 = Path.Combine(exeBase, "Web", "wwwroot");
                        string projectCandidate = Path.GetFullPath(Path.Combine(exeBase, "..", "..", "..", "Web", "wwwroot"));

                        string webRoot;
                        if (Directory.Exists(candidate1)) webRoot = candidate1;
                        else if (Directory.Exists(candidate2)) webRoot = candidate2;
                        else if (Directory.Exists(projectCandidate)) webRoot = projectCandidate;
                        else webRoot = candidate1; // fallback

                        var fileProvider = new PhysicalFileProvider(webRoot);

                        // Simple Basic Auth middleware:
                        //
                        // - Bypass authentication for API endpoints (/api/*) and /health
                        // - Protect everything else (static UI)
                        app.Use(async (context, next) =>
                        {
                            var path = context.Request.Path;

                            // allow API and health endpoints through without auth
                            if (path.StartsWithSegments("/api") || path.Equals("/health", StringComparison.OrdinalIgnoreCase))
                            {
                                await next();
                                return;
                            }

                            // Expect "Authorization: Basic base64(user:pass)"
                            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                            {
                                // Ask client for credentials
                                context.Response.StatusCode = 401;
                                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"MonitorAndStart\"";
                                await context.Response.WriteAsync("Authorization required");
                                return;
                            }

                            string encoded = authHeader.Substring("Basic ".Length).Trim();
                            string decoded;
                            try
                            {
                                decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                            }
                            catch
                            {
                                context.Response.StatusCode = 400;
                                await context.Response.WriteAsync("Invalid Authorization header");
                                return;
                            }

                            var parts = decoded.Split(':', 2);
                            if (parts.Length != 2 || parts[0] != webUser || parts[1] != webPass)
                            {
                                context.Response.StatusCode = 401;
                                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"MonitorAndStart\"";
                                await context.Response.WriteAsync("Unauthorized");
                                return;
                            }

                            // authorized
                            await next();
                        });

                        // Serve static UI from determined folder
                        app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
                        app.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });

                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            // simple JSON list of workflows and jobs
                            endpoints.MapGet("/api/workflows", async context =>
                            {
                                var dto = vm.Workflows.Select(w => new
                                {
                                    w.Id,
                                    w.Name,
                                    w.Enabled,
                                    Jobs = w.Jobs.Select(j => new { j.Id, j.Name, j.Enabled })
                                });
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsJsonAsync(dto);
                            });

                            // trigger workflow run by id
                            endpoints.MapPost("/api/workflows/{id}/run", async context =>
                            {
                                if (!int.TryParse((string?)context.Request.RouteValues["id"], out var id))
                                {
                                    context.Response.StatusCode = 400;
                                    await context.Response.WriteAsync("Invalid id");
                                    return;
                                }

                                _ = vm.RunWorkflowById(id); // fire-and-forget
                                context.Response.StatusCode = 202;
                                await context.Response.WriteAsync("Accepted");
                            });

                            // trigger job run by id
                            endpoints.MapPost("/api/jobs/{id}/run", async context =>
                            {
                                if (!int.TryParse((string?)context.Request.RouteValues["id"], out var id))
                                {
                                    context.Response.StatusCode = 400;
                                    await context.Response.WriteAsync("Invalid id");
                                    return;
                                }

                                _ = vm.RunJobById(id);
                                context.Response.StatusCode = 202;
                                await context.Response.WriteAsync("Accepted");
                            });

                            // logs endpoint - returns the whole log file (plain text). Safe if file missing.
                            endpoints.MapGet("/api/logs", async context =>
                            {
                                try
                                {
                                    string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MonitorAndStart.v2", "log.log");
                                    if (!System.IO.File.Exists(logPath))
                                    {
                                        context.Response.ContentType = "text/plain; charset=utf-8";
                                        await context.Response.WriteAsync(string.Empty);
                                        return;
                                    }

                                    string content = await System.IO.File.ReadAllTextAsync(logPath);
                                    context.Response.ContentType = "text/plain; charset=utf-8";
                                    await context.Response.WriteAsync(content);
                                }
                                catch (Exception ex)
                                {
                                    context.Response.StatusCode = 500;
                                    await context.Response.WriteAsync($"Error reading logs: {ex.Message}");
                                }
                            });

                            // simple health check
                            endpoints.MapGet("/health", async ctx => await ctx.Response.WriteAsync("OK"));
                        });
                    });
                });

            _host = builder.Build();
            await _host.StartAsync();
        }

        public async Task StopAsync()
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
                _host = null;
            }
        }

        public void Dispose()
        {
            try
            {
                _host?.Dispose();
                _host = null;
            }
            catch { }
        }
    }
}

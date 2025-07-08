using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PharmaBack.TrayApp.Logging;
using PharmaBack.WebApi;

namespace PharmaBack.TrayApp;

public class WebApiHost(Action<string> log)
{
    private IHost? _host;
    private readonly Action<string> _log = log;

    public async Task StartAsync()
    {
        var configRootPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "WebApi")
        );

        _log($"📁 Using config from: {configRootPath}");

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(
                (ctx, config) =>
                {
                    config.SetBasePath(configRootPath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                }
            )
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddProvider(new GuiLoggerProvider(_log));
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureWebHostDefaults(web =>
            {
                web.UseStartup<Startup>();
                web.UseUrls("http://0.0.0.0:5050");
            })
            .Build();

        _log("🚀 Starting embedded Web API...");
        await _host.StartAsync();
        _log("✅Web API running at http://localhost:5050");
    }

    public async Task StopAsync()
    {
        if (_host != null)
        {
            _log("🛑 Stopping Web API...");
            await _host.StopAsync();
        }
    }
}

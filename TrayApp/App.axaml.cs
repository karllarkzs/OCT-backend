using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace PharmaBack.TrayApp;

public partial class App : Application
{
    private WebApiHost? _webApiHost;
    private MainWindow? _mainWindow;

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainWindow = new MainWindow();
            _webApiHost = new WebApiHost(_mainWindow.Log);

            try
            {
                await _webApiHost.StartAsync();
                _mainWindow.Log("✅Web API started.");
            }
            catch (Exception ex)
            {
                _mainWindow.Log("❌ Failed to start Web API:");
                _mainWindow.Log(ex.ToString());
                Environment.Exit(1);
            }

            desktop.MainWindow = _mainWindow;
            desktop.Exit += async (_, _) => await _webApiHost.StopAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public Task StopWebApiAsync() => _webApiHost?.StopAsync() ?? Task.CompletedTask;
}

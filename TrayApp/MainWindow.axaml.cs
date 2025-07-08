using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace PharmaBack.TrayApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        QuitButton.Click += OnQuitClicked;
        this.Closing += OnWindowClosing;
        ShowLocalIp();
        Log("Application started.");
    }

    private void ShowLocalIp()
    {
        string ip = GetLocalIPAddress();
        LocalIpDisplay.Text = $"Local IP: {ip}";
        Log($"Detected local IP: {ip}");
    }

    private static string GetLocalIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(a =>
                a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a)
            );
            return ip?.ToString() ?? "No local IPv4 address found";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    private async void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;

        var result = await MessageBox.Show(
            this,
            "Do you want to quit the Pharma server?",
            "Exit Confirmation"
        );

        if (result == true)
        {
            await ShutdownApp();
        }
    }

    private async void OnQuitClicked(object? sender, RoutedEventArgs e)
    {
        var result = await MessageBox.Show(
            this,
            "Do you want to quit the Pharma server?",
            "Exit Confirmation"
        );

        if (result == true)
        {
            await ShutdownApp();
        }
    }

    private async Task ShutdownApp()
    {
        Log("Shutting down Web API...");
        if (
            Application.Current?.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime lifetime
        )
        {
            await ((App)Application.Current).StopWebApiAsync();
            Log("Web API stopped.");
            lifetime.Shutdown();
        }
    }

    private readonly int MaxLogLines = 500;
    private readonly List<string> logLines = new();

    public async void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logLine = $"[{timestamp}] {message}";

        logLines.Add(logLine);

        if (logLines.Count > MaxLogLines)
            logLines.RemoveRange(0, logLines.Count - MaxLogLines);

        var joined = string.Join('\n', logLines);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            LogOutput.Text = joined;
        });
    }
}

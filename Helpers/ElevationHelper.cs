using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace PharmaBack.Helpers;

internal static class ElevationHelper
{
    public static void EnsureElevated()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        if (
            !new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(
                WindowsBuiltInRole.Administrator
            )
        )
        {
            var psi = new ProcessStartInfo
            {
                FileName = GetLauncher(),
                Arguments = GetArgs(),
                Verb = "runas",
                UseShellExecute = true,
            };
            Process.Start(psi);
            Environment.Exit(0);
        }
    }

    private static string GetLauncher()
    {
        var host = Process.GetCurrentProcess().MainModule?.FileName;
        return host ?? throw new InvalidOperationException("Cannot determine launcher");
    }

    private static string GetArgs()
    {
        var host = Process.GetCurrentProcess().MainModule?.FileName;
        return Path.GetFileName(host) == "dotnet"
            ? $"\"{Assembly.GetExecutingAssembly().Location}\""
            : "";
    }
}

using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace PharmaBack;

internal static class StaticIpManager
{
    private static string? _interfaceName;

    public static void TryAssignStatic(int topHosts = 5)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var nic = NetworkInterface
            .GetAllNetworkInterfaces()
            .FirstOrDefault(i =>
                i.OperationalStatus == OperationalStatus.Up
                && i.NetworkInterfaceType != NetworkInterfaceType.Loopback
                && i.GetIPProperties()
                    .UnicastAddresses.Any(u =>
                        u.Address.AddressFamily == AddressFamily.InterNetwork
                    )
            );

        if (nic is null)
            return;
        _interfaceName = nic.Name;

        var ipInfo = nic.GetIPProperties()
            .UnicastAddresses.First(u => u.Address.AddressFamily == AddressFamily.InterNetwork);

        var mask = ipInfo.IPv4Mask!;
        var maskInt = ToUInt(mask);
        var network = ToUInt(ipInfo.Address) & maskInt;
        var broadcast = network | ~maskInt;

        using var ping = new Ping();
        IPAddress? free = Enumerable
            .Range(1, topHosts)
            .Select(k => FromUInt(broadcast - (uint)k))
            .FirstOrDefault(addr =>
            {
                try
                {
                    return ping.Send(addr, 300).Status != IPStatus.Success;
                }
                catch
                {
                    return false;
                }
            });

        if (free is null)
            return;

        string gw = $"{string.Join('.', free.ToString().Split('.').Take(3))}.1";
        Run("netsh", $"interface ip set address \"{nic.Name}\" static {free} {mask} {gw} 1");
        Run("netsh", $"interface ip set dnsservers \"{nic.Name}\" static 8.8.8.8 primary");
        Run("netsh", $"interface ip add dnsservers \"{nic.Name}\" 8.8.4.4 index=2");
        Run(
            "netsh",
            $"advfirewall firewall add rule name=\"PharmaBackAllow\" dir=in action=allow protocol=TCP localport=any"
        );
    }

    public static void RestoreDhcp()
    {
        if (_interfaceName is null)
            return;
        Run("netsh", $"interface ip set address name=\"{_interfaceName}\" source=dhcp");
        Run("netsh", $"interface ip set dnsservers name=\"{_interfaceName}\" source=dhcp");
    }

    private static void Run(string file, string args)
    {
        var p = Process.Start(
            new ProcessStartInfo(file, args) { UseShellExecute = false, CreateNoWindow = true }
        );
        p?.WaitForExit();
    }

    private static uint ToUInt(IPAddress ip) =>
        BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);

    private static IPAddress FromUInt(uint v) => new(BitConverter.GetBytes(v).Reverse().ToArray());
}

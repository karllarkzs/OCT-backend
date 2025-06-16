using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace PharmaBack;

internal static class StaticIpManager
{
    private static string? _interfaceName;

    public static void TryAssignStatic(int topHosts = 10)
    {
        Console.WriteLine("[IPMGR] Starting TryAssignStatic...");
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("[IPMGR] Not running on Windows — skipping IP assignment.");
            return;
        }

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
        {
            Console.WriteLine("[IPMGR] No suitable network interface found.");
            return;
        }

        _interfaceName = nic.Name;
        Console.WriteLine($"[IPMGR] Selected interface: {_interfaceName}");

        var ipInfo = nic.GetIPProperties()
            .UnicastAddresses.First(u => u.Address.AddressFamily == AddressFamily.InterNetwork);

        var currentIP = ipInfo.Address;
        var subnetMask = ipInfo.IPv4Mask!;
        Console.WriteLine($"[IPMGR] Current IP: {currentIP}, Subnet Mask: {subnetMask}");

        var maskInt = ToUInt(subnetMask);
        var baseIPInt = ToUInt(currentIP) & maskInt;
        var broadcast = baseIPInt | ~maskInt;

        using var ping = new Ping();
        IPAddress? free = null;

        var maxHost = 253;

        for (int offset = 0; offset < topHosts; offset++)
        {
            var candidate = FromUInt(baseIPInt + (uint)maxHost - (uint)offset);
            Console.WriteLine($"[IPMGR] Pinging candidate {candidate}...");

            try
            {
                var reply = ping.Send(candidate, 300);
                if (reply.Status != IPStatus.Success)
                {
                    Console.WriteLine($"[IPMGR] {candidate} is free.");
                    free = candidate;
                    break;
                }
                else
                {
                    Console.WriteLine($"[IPMGR] {candidate} is in use (Status: {reply.Status}).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IPMGR] Error pinging {candidate}: {ex.Message}");
            }
        }

        if (free is null)
        {
            Console.WriteLine("[IPMGR] No available free IP found.");
            return;
        }

        Console.WriteLine($"[IPMGR] Selected free IP: {free}");

        var gateway = nic.GetIPProperties()
            .GatewayAddresses.FirstOrDefault(g =>
                g.Address.AddressFamily == AddressFamily.InterNetwork
            )
            ?.Address.ToString();

        if (string.IsNullOrWhiteSpace(gateway))
        {
            Console.WriteLine("[IPMGR] No IPv4 gateway found on interface — aborting.");
            return;
        }

        Console.WriteLine($"[IPMGR] Retaining gateway: {gateway}");

        var args =
            $"interface ip set address \"{_interfaceName}\" static {free} {subnetMask} {gateway}";
        Console.WriteLine($"[IPMGR] Running netsh {args}");
        Run("netsh", args);
        Console.WriteLine("[IPMGR] Static IP assignment complete.");
    }

    public static void RestoreDhcp()
    {
        if (_interfaceName is null)
        {
            Console.WriteLine("[IPMGR] No interface stored—skipping DHCP restore.");
            return;
        }

        Console.WriteLine($"[IPMGR] Restoring DHCP on interface: {_interfaceName}");
        Run("netsh", $"interface ip set address name=\"{_interfaceName}\" source=dhcp");
        Run("netsh", $"interface ip set dnsservers name=\"{_interfaceName}\" source=dhcp");
        Console.WriteLine("[IPMGR] DHCP restore complete.");
    }

    private static void Run(string file, string args)
    {
        try
        {
            var psi = new ProcessStartInfo(file, args)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var p = Process.Start(psi);
            p?.WaitForExit();
            Console.WriteLine($"[IPMGR] Process exited with code {p?.ExitCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[IPMGR] Failed to start process '{file} {args}': {ex.Message}");
        }
    }

    private static uint ToUInt(IPAddress ip) =>
        BitConverter.ToUInt32([.. ip.GetAddressBytes().Reverse()], 0);

    private static IPAddress FromUInt(uint v) => new(BitConverter.GetBytes(v).Reverse().ToArray());
}

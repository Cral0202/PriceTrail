using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace PriceTrail.Services;

public class NativeNotificationService
{
    public Task SendNotificationAsync(string title, string message)
    {
        if (OperatingSystem.IsWindows())
        {
            SendWindowsToast(title, message);
        }
        else if (OperatingSystem.IsMacOS())
        {
            SendMacNotification(title, message);
        }
        else if (OperatingSystem.IsLinux())
        {
            SendLinuxNotification(title, message);
        }

        return Task.CompletedTask;
    }

    [SupportedOSPlatform("windows")]
    private static void SendWindowsToast(string title, string message)
    {
        var formattedTitle = $"PriceTrail - {Escape(title)}";

        var script =
            $"[reflection.assembly]::loadwithpartialname('System.Windows.Forms'); " +
            $"$notify = New-Object System.Windows.Forms.NotifyIcon; " +
            $"$notify.Icon = [System.Drawing.SystemIcons]::Information; " +
            $"$notify.Visible = $true; " +
            $"$notify.ShowBalloonTip(5000, '{formattedTitle}', '{Escape(message)}', [System.Windows.Forms.ToolTipIcon]::Info)";

        RunProcess("powershell", $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"");
    }

    [SupportedOSPlatform("linux")]
    private static void SendLinuxNotification(string title, string message)
    {
        RunProcess("notify-send", $"-a \"PriceTrail\" \"{Escape(title)}\" \"{Escape(message)}\"");
    }

    [SupportedOSPlatform("macos")]
    private static void SendMacNotification(string title, string message)
    {
        var script = $"display notification \"{Escape(message)}\" with title \"PriceTrail\" subtitle \"{Escape(title)}\"";
        RunProcess("osascript", $"-e '{script}'");
    }

    private static string Escape(string input)
    {
        return input.Replace("\"", "\\\"").Replace("'", "''");
    }

    private static void RunProcess(string fileName, string arguments)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
        }
        catch
        {
        }
    }
}

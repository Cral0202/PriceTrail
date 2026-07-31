/************************************************/
/* Handles enabling/disabling launch on startup */
/************************************************/

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Microsoft.Win32;

using PriceTrail.Models.Settings;
using PriceTrail.States;

namespace PriceTrail.Services;

public class StartupService
{
    private readonly SettingsState _settingsState;

    public StartupService(SettingsState settingsState)
    {
        _settingsState = settingsState;

        settingsState.Settings.PropertyChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppSettings.LaunchOnStartupEnabled))
        {
            ApplyLaunchOnStartup();
        }
    }

    public void ApplyLaunchOnStartup()
    {
        bool enable = _settingsState.Settings.LaunchOnStartupEnabled;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetWindowsStartup(enable);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            SetLinuxStartup(enable);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            SetMacStartup(enable);
        }
    }

    [SupportedOSPlatform("windows")]
    private static void SetWindowsStartup(bool enable)
    {
        string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(registryKeyPath, writable: true);

        if (key == null) return;

        string executablePath = Environment.ProcessPath ?? throw new InvalidOperationException("Unable to determine executable path.");

        if (enable)
        {
            key.SetValue(Constants.AppName, $"\"{executablePath}\" --autostart");
        }
        else
        {
            key.DeleteValue(Constants.AppName, false);
        }
    }

    [SupportedOSPlatform("linux")]
    private static void SetLinuxStartup(bool enable)
    {
        string autostartDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config", "autostart"
        );

        string desktopFilePath = Path.Combine(autostartDir, $"{Constants.AppName}.desktop");

        if (enable)
        {
            Directory.CreateDirectory(autostartDir);
            string executablePath = Environment.ProcessPath ?? throw new InvalidOperationException("Unable to determine executable path.");

            string desktopFileContent = $"""
                [Desktop Entry]
                Type=Application
                Name={Constants.AppName}
                Exec="{executablePath}" --autostart
                Terminal=false
                X-GNOME-Autostart-enabled=true
                """;

            File.WriteAllText(desktopFilePath, desktopFileContent);
        }
        else if (File.Exists(desktopFilePath))
        {
            File.Delete(desktopFilePath);
        }
    }

    [SupportedOSPlatform("macos")]
    private static void SetMacStartup(bool enable)
    {
        string launchAgentsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Library", "LaunchAgents"
        );

        string plistPath = Path.Combine(launchAgentsDir, $"{Constants.BundleId}.startup.plist");

        if (enable)
        {
            Directory.CreateDirectory(launchAgentsDir);
            string executablePath = Environment.ProcessPath ?? throw new InvalidOperationException("Unable to determine executable path.");

            string plistContent = $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                <plist version="1.0">
                <dict>
                    <key>Label</key>
                    <string>{Constants.BundleId}.startup</string>

                    <key>ProgramArguments</key>
                    <array>
                        <string>{executablePath}</string>
                        <string>--autostart</string>
                    </array>

                    <key>RunAtLoad</key>
                    <true/>
                </dict>
                </plist>
                """;

            File.WriteAllText(plistPath, plistContent);
        }
        else if (File.Exists(plistPath))
        {
            File.Delete(plistPath);
        }
    }
}

using System;
using System.Threading.Tasks;

using Avalonia.Controls.Notifications;

using PriceTrail.Models;
using PriceTrail.Services;

namespace PriceTrail.States;

public class UpdateState
{
    private readonly UpdateService _updateService = new();

    public UpdateInfo UpdateInfo { get; } = new();

    public async Task CheckForUpdatesAsync()
    {
        var result = await _updateService.CheckForUpdatesAsync();

        if (!result.IsUpdateAvailable)
            return;

        UpdateInfo.IsUpdateAvailable = true;
        UpdateInfo.LatestVersion = result.LatestVersion;
        UpdateInfo.ReleaseUrl = result.ReleaseUrl;

        ToastNotificationService.Instance.ShowMessage(
            "Update available",
            $"PriceTrail {result.LatestVersion} is available.",
            NotificationType.Information,
            TimeSpan.FromSeconds(10));
    }
}

using System;
using System.Threading.Tasks;

using Avalonia.Controls.Notifications;

using PriceTrail.Models.Updates;
using PriceTrail.Services;

namespace PriceTrail.States;

public class UpdateState(UpdateService updateService, ToastNotificationService toastService)
{
    public UpdateInfo UpdateInfo { get; } = new();

    public async Task CheckForUpdatesAsync()
    {
        var result = await updateService.CheckForUpdatesAsync();

        if (!result.IsUpdateAvailable)
            return;

        UpdateInfo.IsUpdateAvailable = true;
        UpdateInfo.LatestVersion = result.LatestVersion;
        UpdateInfo.ReleaseUrl = result.ReleaseUrl;

        toastService.ShowMessage(
            "Update available",
            $"PriceTrail {result.LatestVersion} is available. Download it from settings.",
            NotificationType.Information,
            TimeSpan.FromSeconds(10));
    }
}

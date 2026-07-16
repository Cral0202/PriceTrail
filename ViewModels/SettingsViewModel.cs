using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;
using PriceTrail.Models.Settings;
using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class SettingsViewModel(SettingsState settingsState, UpdateState updateState) : ViewModelBase
{
    public AppSettings Settings { get; } = settingsState.Settings;
    public UpdateInfo UpdateInfo { get; } = updateState.UpdateInfo;

    public string AppVersion => Constants.AppVersion;

    public ObservableCollection<TimeSpan> RefreshIntervals { get; } =
    [
        TimeSpan.FromMinutes(30),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(12),
        TimeSpan.FromDays(1)
    ];

    [RelayCommand]
    private void OpenLatestRelease()
    {
        if (string.IsNullOrWhiteSpace(UpdateInfo.ReleaseUrl))
            return;

        Process.Start(new ProcessStartInfo
        {
            FileName = UpdateInfo.ReleaseUrl,
            UseShellExecute = true
        });
    }
}

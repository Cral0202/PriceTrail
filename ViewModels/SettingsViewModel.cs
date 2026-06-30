using System;
using System.Collections.ObjectModel;

using PriceTrail.Models.Settings;
using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class SettingsViewModel(SettingsState state) : ViewModelBase
{
    public AppSettings Settings { get; } = state.Settings;

    public ObservableCollection<TimeSpan> RefreshIntervals { get; } =
    [
        TimeSpan.FromMinutes(30),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(12),
        TimeSpan.FromDays(1)
    ];
}

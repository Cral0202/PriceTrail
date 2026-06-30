using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.Models.Settings;

public partial class AppSettings : ObservableObject
{
    public int Id { get; set; } = 1;

    [ObservableProperty]
    public partial bool AutomaticPriceRefreshEnabled { get; set; } = true;

    [ObservableProperty]
    public partial TimeSpan PriceRefreshInterval { get; set; } = TimeSpan.FromMinutes(30);
}

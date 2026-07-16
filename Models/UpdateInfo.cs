using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.Models;

public partial class UpdateInfo : ObservableObject
{
    [ObservableProperty]
    public partial bool IsUpdateAvailable { get; set; }

    [ObservableProperty]
    public partial string? LatestVersion { get; set; }

    [ObservableProperty]
    public partial string? ReleaseUrl { get; set; }
}

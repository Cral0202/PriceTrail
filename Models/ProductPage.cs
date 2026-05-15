using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.Models;

public partial class ProductPage : ObservableObject
{
    public string Url { get; set; } = "";
    public string StoreName { get; set; } = "";

    [ObservableProperty]
    public partial string Price { get; set; } = "";

    [ObservableProperty]
    public partial string Currency { get; set; } = "";
}

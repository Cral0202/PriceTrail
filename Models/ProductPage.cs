using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.Models;

public partial class ProductPage : ObservableObject
{
    public int Id { get; set; }
    public int ProductId { get; set; } // Foreign key
    public Product? Product { get; set; } // Navigation property

    public string Url { get; set; } = "";
    public string StoreName { get; set; } = "";

    [ObservableProperty]
    public partial decimal Price { get; set; } = 0;

    [ObservableProperty]
    public partial string Currency { get; set; } = "";

    public ObservableCollection<PriceHistoryEntry> PriceHistory { get; set; } = [];
}

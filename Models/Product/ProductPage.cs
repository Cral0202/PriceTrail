using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.Models.Product;

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

    [ObservableProperty]
    public partial bool HasError { get; set; }

    public ObservableCollection<PriceHistoryEntry> PriceHistory { get; set; }

    public ProductPage()
    {
        PriceHistory = [];
        PriceHistory.CollectionChanged += OnPriceHistoryChanged;
    }

    private void OnPriceHistoryChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(LastUpdated));
    }

    public DateTime? LastUpdated => PriceHistory.LastOrDefault()?.Timestamp;
}

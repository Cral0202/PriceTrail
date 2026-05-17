using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.Models.Product;

public partial class Product : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = "";

    public ObservableCollection<ProductPage> ProductPages { get; set; }

    public Product()
    {
        ProductPages = [];
        ProductPages.CollectionChanged += OnProductPagesChanged;
    }

    private void OnProductPagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(RankedProductPages));
        OnPropertyChanged(nameof(LowestPricePage));
        OnPropertyChanged(nameof(LowestPrice));
        OnPropertyChanged(nameof(LowestPriceCurrency));
        OnPropertyChanged(nameof(LowestPriceStore));
    }

    [NotMapped]
    public IEnumerable<ProductPage> RankedProductPages => ProductPages.OrderBy(p => p.Price);

    public ProductPage? LowestPricePage => ProductPages.MinBy(p => p.Price);
    public decimal? LowestPrice => LowestPricePage?.Price;
    public string LowestPriceCurrency => LowestPricePage?.Currency ?? "";
    public string LowestPriceStore => LowestPricePage?.StoreName ?? "";
}

using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.Models;

public partial class Product : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = "";

    public ObservableCollection<ProductPage> ProductPages { get; set; } = [];

    public ProductPage? LowestPricePage => ProductPages.MinBy(p => p.Price);
    public decimal? LowestPrice => LowestPricePage?.Price;
    public string LowestPriceCurrency => LowestPricePage?.Currency ?? "";
    public string LowestPriceStore => LowestPricePage?.StoreName ?? "";
}

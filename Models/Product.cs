using System.Collections.ObjectModel;
using System.Linq;

namespace PriceTrail.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ObservableCollection<ProductPage> ProductPages { get; set; } = [];

    public ProductPage? LowestPricePage => ProductPages.MinBy(p => p.Price);
    public decimal? LowestPrice => LowestPricePage?.Price;
    public string LowestPriceCurrency => LowestPricePage?.Currency ?? "";
    public string LowestPriceStore => LowestPricePage?.StoreName ?? "";
}

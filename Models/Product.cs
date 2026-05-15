using System.Collections.ObjectModel;

namespace PriceTrail.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ObservableCollection<ProductPage> ProductPages { get; set; } = [];
}

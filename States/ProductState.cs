using System.Collections.ObjectModel;
using System.Threading.Tasks;

using PriceTrail.Models;
using PriceTrail.Services;

namespace PriceTrail.States;

public class ProductState
{
    private readonly DatabaseService _db = new();

    public ObservableCollection<Product> Products { get; } = [];

    public async Task LoadProductsAsync()
    {
        var products = await _db.GetProductsAsync();

        Products.Clear();

        foreach (var product in products)
        {
            Products.Add(product);
        }
    }

    public async Task AddProductAsync(Product product)
    {
        await _db.AddProductAsync(product);
        Products.Add(product);
    }

    public async Task DeleteProductAsync(Product product)
    {
        await _db.DeleteProductAsync(product);
        Products.Remove(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _db.UpdateProductAsync(product);
    }
}

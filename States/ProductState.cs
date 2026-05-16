using System.Collections.ObjectModel;
using System.Threading.Tasks;

using PriceTrail.Models;
using PriceTrail.Repositories;

namespace PriceTrail.States;

public class ProductState
{
    private readonly ProductRepository _repo = new();

    public ObservableCollection<Product> Products { get; } = [];

    public async Task LoadProductsAsync()
    {
        var products = await _repo.GetProductsAsync();

        Products.Clear();

        foreach (var product in products)
        {
            Products.Add(product);
        }
    }

    public async Task AddProductAsync(Product product)
    {
        await _repo.AddProductAsync(product);
        Products.Add(product);
    }

    public async Task DeleteProductAsync(Product product)
    {
        await _repo.DeleteProductAsync(product);
        Products.Remove(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _repo.UpdateProductAsync(product);
    }
}

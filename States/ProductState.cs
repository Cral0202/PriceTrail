using System.Collections.ObjectModel;
using System.Threading.Tasks;

using PriceTrail.Models.Product;
using PriceTrail.Repositories.Product;
using PriceTrail.Services;

namespace PriceTrail.States;

public class ProductState
{
    private readonly ProductRepository _productRepo = new();
    private readonly ProductPageRepository _productPageRepo = new();
    private readonly PriceHistoryRepository _priceHistoryRepo = new();
    private readonly ProductExtractorService _extractor = new();

    public ObservableCollection<Product> Products { get; } = [];

    public async Task LoadProductsAsync()
    {
        var products = await _productRepo.GetProductsAsync();

        Products.Clear();

        foreach (var product in products)
        {
            Products.Add(product);
        }
    }

    public async Task AddProductAsync(Product product)
    {
        await _productRepo.AddProductAsync(product);
        Products.Add(product);
    }

    public async Task DeleteProductAsync(Product product)
    {
        await _productRepo.DeleteProductAsync(product);
        Products.Remove(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _productRepo.UpdateProductAsync(product);
    }

    public async Task AddProductPageToProductAsync(Product product, string url)
    {
        var result = await _extractor.ExtractAsync(url);

        if (result == null)
            return;

        await _productPageRepo.AddProductPageAsync(product, result);
        var historyEntry = await _priceHistoryRepo.AddPriceHistoryEntryAsync(result);

        result.PriceHistory.Add(historyEntry);
        product.ProductPages.Add(result);
    }

    public async Task RefreshProductPricesAsync(Product product)
    {
        foreach (var productPage in product.ProductPages)
        {
            var result = await _extractor.ExtractAsync(productPage.Url);

            if (result == null)
                continue;

            productPage.Price = result.Price;
            productPage.Currency = result.Currency;

            await _productPageRepo.UpdateProductPageAsync(productPage);

            var historyEntry = await _priceHistoryRepo.AddPriceHistoryEntryAsync(productPage);
            productPage.PriceHistory.Add(historyEntry);
        }
    }

    public async Task DeleteProductPageFromProductAsync(Product product, ProductPage page)
    {
        await _productPageRepo.DeleteProductPageAsync(page);
        product.ProductPages.Remove(page);
    }
}

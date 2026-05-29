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

    public async Task<string?> AddProductPageToProductAsync(Product product, string url)
    {
        var result = await _extractor.ExtractAsync(url);

        if (!result.IsSuccess)
            return result.ErrorMessage;

        var productPage = result.Page!;

        await _productPageRepo.AddProductPageAsync(product, productPage);
        var historyEntry = await _priceHistoryRepo.AddPriceHistoryEntryAsync(productPage);

        productPage.PriceHistory.Add(historyEntry);
        product.ProductPages.Add(productPage);

        return null;
    }

    public async Task RefreshProductPricesAsync(Product product)
    {
        foreach (var productPage in product.ProductPages)
        {
            var result = await _extractor.ExtractAsync(productPage.Url);

            if (!result.IsSuccess)
                continue; // TODO: Show error to user

            productPage.Price = result.Page!.Price;
            productPage.Currency = result.Page!.Currency;

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

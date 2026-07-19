using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using PriceTrail.Models.Product;
using PriceTrail.Repositories.Products;
using PriceTrail.Services;

namespace PriceTrail.States;

public class ProductState(PlaywrightBrowserService playrightBrowserService)
{
    private readonly ProductRepository _productRepo = new();
    private readonly ProductPageRepository _productPageRepo = new();
    private readonly PriceHistoryRepository _priceHistoryRepo = new();
    private readonly ProductExtractorService _extractor = new(playrightBrowserService);
    private readonly HashSet<int> _refreshingProducts = [];

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
        // Prevent concurrent refreshes
        if (_refreshingProducts.Contains(product.Id))
            return;

        _refreshingProducts.Add(product.Id);

        try
        {
            foreach (var productPage in product.ProductPages)
            {
                var result = await _extractor.ExtractAsync(productPage.Url);

                if (!result.IsSuccess)
                {
                    productPage.HasError = true;
                    productPage.ErrorMessage = result.ErrorMessage!;

                    await _productPageRepo.UpdateProductPageAsync(productPage);
                    continue;
                }

                productPage.HasError = false;
                productPage.ErrorMessage = "";

                productPage.Price = result.Page!.Price;
                productPage.Currency = result.Page!.Currency;
                productPage.ImageUrl = result.Page!.ImageUrl;

                await _productPageRepo.UpdateProductPageAsync(productPage);

                var historyEntry = await _priceHistoryRepo.AddPriceHistoryEntryAsync(productPage);
                productPage.PriceHistory.Add(historyEntry);
            }
        }
        finally
        {
            _refreshingProducts.Remove(product.Id);
        }
    }

    public async Task RefreshAllProductPricesAsync()
    {
        foreach (var product in Products)
        {
            await RefreshProductPricesAsync(product);
        }
    }

    public async Task DeleteProductPageFromProductAsync(Product product, ProductPage page)
    {
        await _productPageRepo.DeleteProductPageAsync(page);
        product.ProductPages.Remove(page);
    }
}

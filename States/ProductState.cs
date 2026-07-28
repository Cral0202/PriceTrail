using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

using PriceTrail.Models.Product;
using PriceTrail.Repositories.Products;
using PriceTrail.Services;

namespace PriceTrail.States;

public class ProductState(
    ProductRepository productRepo,
    ProductPageRepository productPageRepo,
    PriceHistoryRepository priceHistoryRepo,
    ProductExtractorService extractor,
    PriceNotificationService priceNotificationService)
{
    private readonly HashSet<int> _refreshingProducts = [];

    public ObservableCollection<Product> Products { get; } = [];

    public async Task LoadProductsAsync()
    {
        var products = await productRepo.GetProductsAsync();

        Products.Clear();

        foreach (var product in products)
        {
            Products.Add(product);
        }
    }

    public async Task AddProductAsync(Product product)
    {
        await productRepo.AddProductAsync(product);
        Products.Add(product);
    }

    public async Task DeleteProductAsync(Product product)
    {
        await productRepo.DeleteProductAsync(product);
        Products.Remove(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        await productRepo.UpdateProductAsync(product);
    }

    public async Task<string?> AddProductPageToProductAsync(Product product, string url, CancellationToken cancellationToken = default)
    {
        var result = await extractor.ExtractAsync(url, cancellationToken);

        if (!result.IsSuccess)
            return result.ErrorMessage;

        var productPage = result.Page!;

        await productPageRepo.AddProductPageAsync(product, productPage);
        var historyEntry = await priceHistoryRepo.AddPriceHistoryEntryAsync(productPage);

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
                var result = await extractor.ExtractAsync(productPage.Url);

                if (!result.IsSuccess)
                {
                    productPage.HasError = true;
                    productPage.ErrorMessage = result.ErrorMessage!;

                    await productPageRepo.UpdateProductPageAsync(productPage);
                    continue;
                }

                productPage.HasError = false;
                productPage.ErrorMessage = "";

                await priceNotificationService.CheckForNotifications(product, productPage, result.Page!);

                productPage.Price = result.Page!.Price;
                productPage.Currency = result.Page!.Currency;
                productPage.ImageUrl = result.Page!.ImageUrl;

                await productPageRepo.UpdateProductPageAsync(productPage);

                var historyEntry = await priceHistoryRepo.AddPriceHistoryEntryAsync(productPage);
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

    public async Task DeleteProductPageAsync(Product product, ProductPage page)
    {
        await productPageRepo.DeleteProductPageAsync(page);
        product.ProductPages.Remove(page);
    }
}

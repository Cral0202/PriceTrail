using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;
using PriceTrail.Services;

namespace PriceTrail.ViewModels;

public partial class ProductDetailsViewModel(MainWindowViewModel mainWindow, Product product) : ObservableObject
{
    private readonly DatabaseService _db = new();
    private readonly ProductExtractorService _extractor = new();

    public Product Product { get; } = product;

    [ObservableProperty]
    public partial string Url { get; set; } = "";

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [RelayCommand]
    private async Task AddProductPageAsync()
    {
        var result = await _extractor.ExtractAsync(Url);

        if (result != null)
        {
            await _db.AddProductPageAsync(Product, result);
            var historyEntry = await _db.AddPriceHistoryEntryAsync(result);

            result.PriceHistory.Add(historyEntry);
            Product.ProductPages.Add(result);
        }

        Url = "";
    }

    [RelayCommand]
    private async Task RefreshPricesAsync()
    {
        if (IsRefreshing)
            return;

        IsRefreshing = true;

        try
        {
            foreach (var productPage in Product.ProductPages)
            {
                var result = await _extractor.ExtractAsync(productPage.Url);

                if (result != null)
                {
                    productPage.Price = result.Price;
                    productPage.Currency = result.Currency;

                    await _db.UpdateProductPageAsync(productPage);

                    var historyEntry = await _db.AddPriceHistoryEntryAsync(productPage);
                    productPage.PriceHistory.Add(historyEntry);
                }
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task DeleteProductPageAsync(ProductPage page)
    {
        await _db.DeleteProductPageAsync(page);
        Product.ProductPages.Remove(page);
    }

    [RelayCommand]
    private void GoBack()
    {
        mainWindow.CurrentViewModel = mainWindow.ProductsViewModel;
    }
}

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
    public partial bool IsAddProductPageModalOpen { get; set; }

    [ObservableProperty]
    public partial bool IsEditProductModalOpen { get; set; }

    [ObservableProperty]
    public partial string NewProductPageUrl { get; set; } = "";

    [ObservableProperty]
    public partial string NewProductName { get; set; } = "";

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [RelayCommand]
    private async Task AddProductPageAsync()
    {
        var result = await _extractor.ExtractAsync(NewProductPageUrl);

        if (result != null)
        {
            await _db.AddProductPageAsync(Product, result);
            var historyEntry = await _db.AddPriceHistoryEntryAsync(result);

            result.PriceHistory.Add(historyEntry);
            Product.ProductPages.Add(result);
        }

        NewProductPageUrl = "";
        IsAddProductPageModalOpen = false;
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

    /****************/
    /* EDIT PRODUCT */
    /****************/

    [RelayCommand]
    private async Task ChangeProductNameAsync()
    {
        var trimmedName = NewProductName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            return;

        if (Product.Name == trimmedName)
            return;

        Product.Name = trimmedName;
        await _db.UpdateProductAsync(Product);
    }

    /**********/
    /* MODALS */
    /**********/

    [RelayCommand]
    private void OpenAddProductPageModal()
    {
        IsAddProductPageModalOpen = true;
    }

    [RelayCommand]
    private void CloseAddProductPageModal()
    {
        NewProductPageUrl = "";
        IsAddProductPageModalOpen = false;
    }

    [RelayCommand]
    private void OpenEditProductModal()
    {
        NewProductName = Product.Name;
        IsEditProductModalOpen = true;
    }

    [RelayCommand]
    private async Task CloseEditProductModalAsync()
    {
        // Check if product name should be updated
        if (Product.Name != NewProductName)
        {
            await ChangeProductNameAsync();
        }

        IsEditProductModalOpen = false;
    }
}

using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;
using PriceTrail.Repositories;
using PriceTrail.Services;
using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class ProductDetailsViewModel(MainWindowViewModel mainWindow, ProductState productState, Product product) : ObservableObject
{
    private readonly ProductPageRepository _productPageRepo = new();
    private readonly PriceHistoryRepository _priceHistoryRepo = new();
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
            await _productPageRepo.AddProductPageAsync(Product, result);
            var historyEntry = await _priceHistoryRepo.AddPriceHistoryEntryAsync(result);

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

                    await _productPageRepo.UpdateProductPageAsync(productPage);

                    var historyEntry = await _priceHistoryRepo.AddPriceHistoryEntryAsync(productPage);
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
        await _productPageRepo.DeleteProductPageAsync(page);
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
        await productState.UpdateProductAsync(Product);
    }

    [RelayCommand]
    private async Task DeleteProductAsync()
    {
        await productState.DeleteProductAsync(Product);
        mainWindow.CurrentViewModel = mainWindow.ProductsViewModel;
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

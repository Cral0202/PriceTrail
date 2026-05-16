using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;
using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class ProductDetailsViewModel(MainWindowViewModel mainWindow, AppState appState, Product product) : ObservableObject
{
    private readonly ProductState _productState = appState.ProductState;

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
        await _productState.AddProductPageToProductAsync(Product, NewProductPageUrl);

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
            await _productState.RefreshProductPricesAsync(Product);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task DeleteProductPageAsync(ProductPage page)
    {
        await _productState.DeleteProductPageFromProductAsync(Product, page);
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
        await _productState.UpdateProductAsync(Product);
    }

    [RelayCommand]
    private async Task DeleteProductAsync()
    {
        await _productState.DeleteProductAsync(Product);
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

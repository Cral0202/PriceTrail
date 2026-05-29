using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.Services;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class ProductDetailsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindow;
    private readonly AppState _appState;
    private readonly ProductState _productState;

    public Product Product { get; }

    [ObservableProperty] public partial ViewModelBase CurrentTabViewModel { get; set; }
    [ObservableProperty] public partial bool IsAddProductPageModalOpen { get; set; }
    [ObservableProperty] public partial bool IsEditProductModalOpen { get; set; }
    [ObservableProperty] public partial string NewProductPageUrl { get; set; } = "";
    [ObservableProperty] public partial string NewProductName { get; set; } = "";
    [ObservableProperty] public partial bool IsRefreshing { get; set; }

    public ProductDetailsViewModel(MainWindowViewModel mainWindow, AppState appState, Product product)
    {
        _mainWindow = mainWindow;
        _appState = appState;
        _productState = appState.ProductState;
        Product = product;

        CurrentTabViewModel = new OverviewViewModel(_appState, Product);
    }

    [RelayCommand]
    private async Task AddProductPageAsync()
    {
        var success = await _productState.AddProductPageToProductAsync(Product, NewProductPageUrl);

        NewProductPageUrl = "";
        IsAddProductPageModalOpen = false;

        if (!success)
            NotificationService.Instance.ShowMessage("Error", "Failed to add URL.", Avalonia.Controls.Notifications.NotificationType.Error); // TODO: Should show error reason
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
    private void GoBack()
    {
        _mainWindow.CurrentViewModel = _mainWindow.ProductsViewModel;
    }

    [RelayCommand]
    private void SelectOverview() => CurrentTabViewModel = new OverviewViewModel(_appState, Product);

    [RelayCommand]
    private void SelectHistory() => CurrentTabViewModel = new HistoryViewModel(Product);

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
        _mainWindow.CurrentViewModel = _mainWindow.ProductsViewModel;
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

using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class ProductDetailsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindow;
    private readonly ProductState _productState;

    public Product Product { get; }

    [ObservableProperty] public partial ViewModelBase CurrentTabViewModel { get; set; }

    public ProductDetailsViewModel(MainWindowViewModel mainWindow, ProductState productState, Product product)
    {
        _mainWindow = mainWindow;
        _productState = productState;
        Product = product;

        CurrentTabViewModel = new OverviewViewModel(productState, Product);
    }

    [RelayCommand]
    private async Task RefreshPricesAsync()
    {
        await _productState.RefreshProductPricesAsync(Product);
    }

    [RelayCommand]
    private void GoBack()
    {
        _mainWindow.CurrentViewModel = _mainWindow.ProductsViewModel;
    }

    [RelayCommand]
    private void SelectOverview() => CurrentTabViewModel = new OverviewViewModel(_productState, Product);

    [RelayCommand]
    private void SelectHistory() => CurrentTabViewModel = new HistoryViewModel(Product);

    /**********/
    /* MODALS */
    /**********/

    [RelayCommand]
    private void OpenAddProductPageModal()
    {
        _mainWindow.CurrentModalViewModel = new AddProductPageModalViewModel(_mainWindow, _productState, Product);
    }

    [RelayCommand]
    private void OpenEditProductModal()
    {
        _mainWindow.CurrentModalViewModel = new EditProductModalViewModel(_mainWindow, _productState, Product);
    }
}

using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.Services;
using PriceTrail.Services.Factories;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class ProductDetailsViewModel : ViewModelBase
{
    private readonly NavigationService _navigation;
    private readonly ProductState _productState;

    private readonly AddProductPageModalViewModelFactory _addProductPageFactory;
    private readonly EditProductModalViewModelFactory _editProductFactory;
    private readonly OverviewViewModelFactory _overviewFactory;
    private readonly HistoryViewModelFactory _historyFactory;

    public Product Product { get; }

    [ObservableProperty] public partial ViewModelBase CurrentTabViewModel { get; set; }

    public ProductDetailsViewModel(
        NavigationService navigation,
        ProductState productState,
        AddProductPageModalViewModelFactory addProductPageFactory,
        EditProductModalViewModelFactory editProductFactory,
        OverviewViewModelFactory overviewFactory,
        HistoryViewModelFactory historyFactory,
        Product product)
    {
        _navigation = navigation;
        _productState = productState;
        _addProductPageFactory = addProductPageFactory;
        _editProductFactory = editProductFactory;
        _overviewFactory = overviewFactory;
        _historyFactory = historyFactory;
        Product = product;

        CurrentTabViewModel = _overviewFactory.Create(Product);
    }

    [RelayCommand]
    private async Task RefreshPricesAsync()
    {
        await _productState.RefreshProductPricesAsync(Product);
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigation.GoBack();
    }

    [RelayCommand]
    private void SelectOverview() => CurrentTabViewModel = _overviewFactory.Create(Product);

    [RelayCommand]
    private void SelectHistory() => CurrentTabViewModel = _historyFactory.Create(Product);

    /**********/
    /* MODALS */
    /**********/

    [RelayCommand]
    private void OpenAddProductPageModal()
    {
        _navigation.OpenModal(_addProductPageFactory.Create(Product));
    }

    [RelayCommand]
    private void OpenEditProductModal()
    {
        _navigation.OpenModal(_editProductFactory.Create(Product));
    }
}

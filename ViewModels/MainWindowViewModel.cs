using CommunityToolkit.Mvvm.ComponentModel;

using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ProductState _productState = new();

    [ObservableProperty]
    public partial object CurrentViewModel { get; set; }

    public ProductsViewModel ProductsViewModel { get; }

    public MainWindowViewModel()
    {
        ProductsViewModel = new ProductsViewModel(this, _productState);
        CurrentViewModel = ProductsViewModel;

        _ = _productState.LoadProductsAsync();
    }
}

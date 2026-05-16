using CommunityToolkit.Mvvm.ComponentModel;

using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AppState _appState = new();

    [ObservableProperty]
    public partial object CurrentViewModel { get; set; }

    public ProductsViewModel ProductsViewModel { get; }

    public MainWindowViewModel()
    {
        ProductsViewModel = new ProductsViewModel(this, _appState);
        CurrentViewModel = ProductsViewModel;

        _ = _appState.ProductState.LoadProductsAsync();
    }
}

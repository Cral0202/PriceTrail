using CommunityToolkit.Mvvm.ComponentModel;

using PriceTrail.States;

namespace PriceTrail.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial object CurrentViewModel { get; set; }

    public ProductsViewModel ProductsViewModel { get; }

    public MainWindowViewModel(AppState appState)
    {
        ProductsViewModel = new ProductsViewModel(this, appState);
        CurrentViewModel = ProductsViewModel;
    }
}

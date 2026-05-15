using CommunityToolkit.Mvvm.ComponentModel;

namespace PriceTrail.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial object CurrentViewModel { get; set; }

    public ProductsViewModel ProductsViewModel { get; }

    public MainWindowViewModel()
    {
        ProductsViewModel = new ProductsViewModel(this);
        CurrentViewModel = ProductsViewModel;
    }
}

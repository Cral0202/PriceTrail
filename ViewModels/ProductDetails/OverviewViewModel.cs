using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class OverviewViewModel(MainWindowViewModel mainWindow, AppState appState, Product product) : ViewModelBase
{
    private readonly ProductState _productState = appState.ProductState;

    public Product Product { get; } = product;

    [RelayCommand]
    private async Task DeleteProductPageAsync(ProductPage page)
    {
        await _productState.DeleteProductPageFromProductAsync(Product, page);
    }
}

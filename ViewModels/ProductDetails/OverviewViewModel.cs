using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class OverviewViewModel(AppState appState, Product product) : ViewModelBase
{
    private readonly ProductState _productState = appState.ProductState;

    public Product Product { get; } = product;

    [RelayCommand]
    private async Task DeleteProductPageAsync(ProductPage page)
    {
        await _productState.DeleteProductPageFromProductAsync(Product, page);
    }
}

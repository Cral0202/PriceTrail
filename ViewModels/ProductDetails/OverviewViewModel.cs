using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class OverviewViewModel(ProductState productState, Product product) : ViewModelBase
{
    public Product Product { get; } = product;

    [RelayCommand]
    private async Task DeleteProductPageAsync(ProductPage page)
    {
        await productState.DeleteProductPageAsync(Product, page);
    }
}

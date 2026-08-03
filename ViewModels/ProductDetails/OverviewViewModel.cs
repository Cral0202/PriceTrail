using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.Services;
using PriceTrail.Services.Factories;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class OverviewViewModel(NavigationService navigation, EditProductPageModalViewModelFactory editProductPageFactory, Product product) : ViewModelBase
{
    public Product Product { get; } = product;

    [RelayCommand]
    private void OpenEditProductPageModal(ProductPage productPage)
    {
        navigation.OpenModal(editProductPageFactory.Create(Product, productPage));
    }
}

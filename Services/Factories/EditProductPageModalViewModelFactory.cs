using PriceTrail.Models.Product;
using PriceTrail.States;
using PriceTrail.ViewModels.ProductDetails;

namespace PriceTrail.Services.Factories;

public class EditProductPageModalViewModelFactory(NavigationService navigation, ProductState productState)
{
    public EditProductPageModalViewModel Create(Product product, ProductPage productPage)
    {
        return new EditProductPageModalViewModel(navigation, productState, product, productPage);
    }
}

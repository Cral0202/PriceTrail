using PriceTrail.Models.Product;
using PriceTrail.States;
using PriceTrail.ViewModels.ProductDetails;

namespace PriceTrail.Services.Factories;

public class EditProductModalViewModelFactory(NavigationService navigation, ProductState productState)
{
    public EditProductModalViewModel Create(Product product)
    {
        return new EditProductModalViewModel(navigation, productState, product);
    }
}

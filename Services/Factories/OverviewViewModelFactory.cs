using PriceTrail.Models.Product;
using PriceTrail.ViewModels.ProductDetails;

namespace PriceTrail.Services.Factories;

public class OverviewViewModelFactory(NavigationService navigation, EditProductPageModalViewModelFactory editProductPageFactory)
{
    public OverviewViewModel Create(Product product)
    {
        return new OverviewViewModel(navigation, editProductPageFactory, product);
    }
}

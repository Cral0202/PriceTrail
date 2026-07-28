using PriceTrail.Models.Product;
using PriceTrail.States;
using PriceTrail.ViewModels.ProductDetails;

namespace PriceTrail.Services.Factories;

public class OverviewViewModelFactory(ProductState productState)
{
    public OverviewViewModel Create(Product product)
    {
        return new OverviewViewModel(productState, product);
    }
}

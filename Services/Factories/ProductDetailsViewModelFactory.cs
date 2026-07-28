using PriceTrail.Models.Product;
using PriceTrail.States;
using PriceTrail.ViewModels.ProductDetails;

namespace PriceTrail.Services.Factories;

public class ProductDetailsViewModelFactory(
    NavigationService navigation,
    ProductState productState,
    AddProductPageModalViewModelFactory addProductPageFactory,
    EditProductModalViewModelFactory editProductFactory,
    OverviewViewModelFactory overviewFactory,
    HistoryViewModelFactory historyFactory)
{
    public ProductDetailsViewModel Create(Product product)
    {
        return new ProductDetailsViewModel(
            navigation,
            productState,
            addProductPageFactory,
            editProductFactory,
            overviewFactory,
            historyFactory,
            product);
    }
}

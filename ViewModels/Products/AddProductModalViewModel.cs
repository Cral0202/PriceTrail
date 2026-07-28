using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models.Product;
using PriceTrail.Services;
using PriceTrail.States;

namespace PriceTrail.ViewModels.Products;

public partial class AddProductModalViewModel(NavigationService navigation, ProductState productState) : ViewModelBase
{
    [ObservableProperty]
    public partial string NewProductName { get; set; } = "";

    [RelayCommand]
    private async Task AddProductAsync()
    {
        var trimmedName = NewProductName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            return;

        var product = new Product
        {
            Name = trimmedName
        };

        await productState.AddProductAsync(product);
        Close();
    }

    [RelayCommand]
    private void Close()
    {
        navigation.CloseModal();
    }
}

using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;
using PriceTrail.Services;

namespace PriceTrail.ViewModels;

public partial class ProductDetailsViewModel(MainWindowViewModel mainWindow, Product product) : ObservableObject
{
    private readonly ProductExtractorService _extractor = new();

    public Product Product { get; } = product;

    [ObservableProperty]
    public partial string Url { get; set; } = "";

    [RelayCommand]
    private async Task AddProductPageAsync()
    {
        var result = await _extractor.ExtractAsync(Url);

        if (result != null)
        {
            Product.ProductPages.Add(result);
        }

        Url = "";
    }

    [RelayCommand]
    private void GoBack()
    {
        mainWindow.CurrentViewModel = mainWindow.ProductsViewModel;
    }
}

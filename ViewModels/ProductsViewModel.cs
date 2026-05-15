using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;
using PriceTrail.Services;

namespace PriceTrail.ViewModels;

public partial class ProductsViewModel(MainWindowViewModel mainWindow) : ObservableObject
{
    private readonly DatabaseService _db = new();

    public ObservableCollection<Product> Products { get; } = [];

    [ObservableProperty]
    public partial string NewProductName { get; set; } = "";

    public async Task LoadProductsAsync()
    {
        var products = await _db.GetProductsAsync();

        Products.Clear();

        foreach (var product in products)
        {
            Products.Add(product);
        }
    }

    [RelayCommand]
    private async Task AddProductAsync()
    {
        if (string.IsNullOrWhiteSpace(NewProductName))
            return;

        var product = new Product
        {
            Name = NewProductName
        };

        await _db.AddProductAsync(product);

        Products.Add(product);

        NewProductName = "";
    }

    [RelayCommand]
    private void OpenProduct(Product product)
    {
        mainWindow.CurrentViewModel = new ProductDetailsViewModel(mainWindow, product);
    }

    [RelayCommand]
    private async Task DeleteProductAsync(Product product)
    {
        await _db.DeleteProductAsync(product);
        Products.Remove(product);
    }
}

using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Models;

namespace PriceTrail.ViewModels;

public partial class ProductsViewModel(MainWindowViewModel mainWindow) : ObservableObject
{
    public ObservableCollection<Product> Products { get; } = [];

    [ObservableProperty]
    public partial string NewProductName { get; set; } = "";

    [RelayCommand]
    private void AddProduct()
    {
        if (string.IsNullOrWhiteSpace(NewProductName))
            return;

        Products.Add(new Product
        {
            Name = NewProductName
        });

        NewProductName = "";
    }

    [RelayCommand]
    private void OpenProduct(Product product)
    {
        mainWindow.CurrentViewModel = new ProductDetailsViewModel(mainWindow, product);
    }
}

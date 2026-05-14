using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PriceTrail.Services;

namespace PriceTrail.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ProductExtractorService _extractor = new();

    [ObservableProperty]
    public partial string Url { get; set; } = "";

    [ObservableProperty]
    public partial string Result { get; set; } = "";

    [RelayCommand]
    private async Task TrackAsync()
    {
        Result = "Loading...";

        try
        {
            var product = await _extractor.ExtractAsync(Url);

            if (product == null)
            {
                Result = "Could not find product info.";
                return;
            }

            Result = $"Price: {product.Price} {product.Currency}";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
        }
    }
}

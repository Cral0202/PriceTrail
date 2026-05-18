using System;
using System.Collections.ObjectModel;

using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;

using PriceTrail.Models.Product;
using PriceTrail.States;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class HistoryViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindow;
    private readonly AppState _appState;
    private readonly Product _product;

    public ObservableCollection<ISeries> Series { get; set; } = [];

    public Axis[] XAxes { get; set; } = [
        new Axis
        {
            Labeler = value => new DateTime((long)value, DateTimeKind.Utc).ToLocalTime().ToString("MMM dd"), // Will say e.g., May 18
            LabelsRotation = 0
        }
    ];

    public Axis[] YAxes { get; set; } = [
        new Axis
        {
            Labeler = value => value.ToString("N2") // Will say e.g., 1,234.56
        }
    ];

    public HistoryViewModel(MainWindowViewModel mainWindow, AppState appState, Product product)
    {
        _mainWindow = mainWindow;
        _appState = appState;
        _product = product;

        BuildChartSeries();
    }

    private void BuildChartSeries()
    {
        if (_product?.ProductPages == null)
            return;

        foreach (var page in _product.ProductPages)
        {
            var lineSeries = new LineSeries<PriceHistoryEntry>
            {
                Name = string.IsNullOrWhiteSpace(page.StoreName) ? "Unknown Store" : page.StoreName,
                Values = page.PriceHistory,
                GeometrySize = 8,
                GeometryFill = null,

                // Formats the X value inside the popup tooltip
                XToolTipLabelFormatter = point => new DateTime((long)point.Coordinate.SecondaryValue, DateTimeKind.Utc).ToLocalTime().ToString("g"),

                Mapping = (entry, index) => new Coordinate(entry.Timestamp.Ticks, (double)entry.Price)
            };

            Series.Add(lineSeries);
        }
    }
}

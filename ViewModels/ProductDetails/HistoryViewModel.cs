using System;
using System.Collections.ObjectModel;

using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;

using PriceTrail.Models.Product;

namespace PriceTrail.ViewModels.ProductDetails;

public partial class HistoryViewModel : ViewModelBase
{
    private readonly Product _product;

    public ObservableCollection<ISeries> Series { get; set; } = [];

    public Axis[] XAxes { get; set; } = [
        new Axis
        {
            Labeler = value =>
            {
                // Check if the value is a valid tick amount
                if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                    return string.Empty;

                return new DateTime((long)value, DateTimeKind.Utc).ToLocalTime().ToString("MMM dd"); // Will say e.g., May 18
            },

            LabelsRotation = 0
        }
    ];

    public Axis[] YAxes { get; set; } = [
        new Axis
        {
            Labeler = value => value.ToString("N2") // Will say e.g., 1,234.56
        }
    ];

    public HistoryViewModel(Product product)
    {
        _product = product;

        BuildChartSeries();
    }

    private void BuildChartSeries()
    {
        if (_product?.ProductPages?.Count is not > 0)
            return;

        foreach (var page in _product.ProductPages)
        {
            if (page.PriceHistory?.Count is not > 0)
                continue;

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

using System;
using System.Globalization;

using Avalonia.Data.Converters;

namespace PriceTrail.Converters;

public class TimeSpanToDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TimeSpan ts)
            return "";

        if (ts.TotalDays >= 1)
            return $"{ts.TotalDays:0} day{(ts.TotalDays == 1 ? "" : "s")}";

        if (ts.TotalHours >= 1)
            return $"{ts.TotalHours:0} hour{(ts.TotalHours == 1 ? "" : "s")}";

        return $"{ts.TotalMinutes:0} minute{(ts.TotalMinutes == 1 ? "" : "s")}";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

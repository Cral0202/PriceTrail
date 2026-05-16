using System;
using System.Globalization;

using Avalonia.Data.Converters;

namespace PriceTrail.Converters;

public class RelativeTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime dateTime)
            return null;

        var span = DateTime.UtcNow - dateTime;

        if (span.TotalSeconds < 60)
            return "Just now";

        if (span.TotalMinutes < 60)
        {
            var minutes = (int)span.TotalMinutes;

            return minutes == 1
                ? "1 minute ago"
                : $"{minutes} minutes ago";
        }

        if (span.TotalHours < 24)
        {
            var hours = (int)span.TotalHours;

            return hours == 1
                ? "1 hour ago"
                : $"{hours} hours ago";
        }

        if (span.TotalDays < 7)
        {
            var days = (int)span.TotalDays;

            return days == 1
                ? "1 day ago"
                : $"{days} days ago";
        }

        return dateTime.ToLocalTime().ToString("yyyy-MM-dd");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

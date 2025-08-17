using System.Globalization;

namespace ExpenseTracker.Converters;

public class BoolToColorConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
#pragma warning disable CS8605 // Unboxing a possibly null value.
        return (bool)value ? Colors.Yellow : Colors.White;
#pragma warning restore CS8605 // Unboxing a possibly null value.
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (Color)value == Colors.Yellow ? true : false;
    }
}

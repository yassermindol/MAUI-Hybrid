using System.Globalization;

namespace ExpenseTracker.Converters;

public class AndBooleanInvertConverter : IMultiValueConverter
{
    private readonly AndBooleanConverter _andConverter = new AndBooleanConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var andResult = (bool)_andConverter.Convert(values, targetType, parameter, culture);
        return !andResult; // Invert the result
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
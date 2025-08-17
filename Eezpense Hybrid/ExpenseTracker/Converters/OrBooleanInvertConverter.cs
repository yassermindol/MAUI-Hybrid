using System.Globalization;

namespace ExpenseTracker.Converters;

public class OrBooleanInvertConverter : IMultiValueConverter
{
    private readonly OrBooleanConverter _orConverter = new OrBooleanConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var orResult = (bool)_orConverter.Convert(values, targetType, parameter, culture);
        return !orResult; // Invert the result
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
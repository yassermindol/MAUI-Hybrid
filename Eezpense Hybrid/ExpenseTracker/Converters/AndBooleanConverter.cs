using System.Globalization;

namespace ExpenseTracker.Converters;
public class AndBooleanConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        foreach (var value in values)
        {
            if (value is bool boolValue && !boolValue)
                return false; // Short-circuit if any is false
        }
        return true; // All are true
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
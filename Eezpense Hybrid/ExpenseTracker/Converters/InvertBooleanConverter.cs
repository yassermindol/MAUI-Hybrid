
namespace ExpenseTracker.Converters;

public class InvertBooleanConverter : IValueConverter
{
    #region IValueConverter implementation

    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool)
        {
            return !((bool)value);
        }

        throw new InvalidCastException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion
}
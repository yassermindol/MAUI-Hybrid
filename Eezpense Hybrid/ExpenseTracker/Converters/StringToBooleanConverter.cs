using System.Globalization;

namespace ExpenseTracker.Converters
{
    public class StringToBooleanConverter : IValueConverter
    {
        // This method is called when converting back from boolean to string  
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return !string.IsNullOrEmpty(str);
            }
            return false;
        }

        // This method is not used in this instance but must be implemented  
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
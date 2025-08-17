namespace ExpenseTracker.ExtensionMethods
{
    public static class NumberExtensions
    {
        public static string ToMoney(this double value)
        {
            //string currency = value.ToString("C");
            //string money = currency.Substring(1);
            //return money = $"{currencySymbol} {money}";

            string formatted;
            if (value % 1 != 0) // check if there is a fractional part
            {
                // Include decimal part
                formatted = value.ToString("#,##0.##");
            }
            else
            {
                // No decimal part
                formatted = value.ToString("#,##0");
            }
            return formatted;
        }
    }
}

using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Models;

public class Legend : DataBinder
{
    public Color LegendColor { get; set; }

    public string ExpenseCategory { get; set; }

    public string TotalStr { get; set; }

    public double Total { get; set; }

    public string Percentage { get; set; }

    string currencySymbol = AppSettings.Account.CurrencySymbol;
    public string CurrencySymbol
    {
        get => currencySymbol;
        set => SetProperty(ref currencySymbol, value);
    }
}
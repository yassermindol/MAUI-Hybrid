
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Models.UI;

public class UiWeekItem : DataBinder
{
    public string WeekNumber { get; set; }
    public string StartDateStr { get; set; }
    public string EndDateStr { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Total { get; set; }
    public string TotalStr => Total.ToMoney();

    double barWidth;
    public double BarWidth 
    { 
        get => barWidth;
        set => SetProperty(ref barWidth, value);
    }

    Color barColor;
    public Color BarColor
    {
        get => barColor;
        set => SetProperty(ref barColor, value);
    }
    
    string currencySymbol = AppSettings.Account.CurrencySymbol;
    public string CurrencySymbol
    {
        get => currencySymbol;
        set => SetProperty(ref currencySymbol, value);
    }
    public IGrouping<int, ExpenseEntity> Expenses { get; set; }
}
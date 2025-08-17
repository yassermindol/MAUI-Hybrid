using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Models.UI;

public class UiExpenseItem: DataBinder
{
    public long ID { get; set; } = -1;// This is the local ID column in the remote server.
    public long CentralID { get; set; } = -1;

    Color backgroundColor;
    public Color BackgroundColor
    {
        get => backgroundColor;
        set => SetProperty(ref backgroundColor, value);
    }

    public DateTime DateTime { get; set; }
    public string Date => DateTime.ToHuman();
    public string Time => DateTime.ToString("h:mm tt");

    public string Category { get; set; } = string.Empty;

    string currencySymbol = AppSettings.Account.CurrencySymbol;
    public string CurrencySymbol 
    {
        get => currencySymbol;
        set => SetProperty(ref currencySymbol, value);
    } 

    public double Amount { get; set; }
    public string AmountStr => Amount.ToMoney();
    public string Note { get; set; } = string.Empty;
    public long CategoryLocalID { get; set; }

    double categoryTotal;
    public double CategoryTotal
    { 
        get => categoryTotal;
        set
        {
            SetProperty(ref categoryTotal, value);
            OnPropertyChanged(nameof(CategoryTotalStr));
        }
    }

    public string CategoryTotalStr => CategoryTotal.ToMoney();

    public ExpenseItemType ItemType { get; set; }

    bool isVisible = true;
    public bool IsVisible
    {
        get => isVisible;
        set => SetProperty(ref isVisible, value);
    }
}

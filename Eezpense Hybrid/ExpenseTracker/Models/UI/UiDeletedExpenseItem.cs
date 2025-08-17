using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features;
using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Models.UI;
public class UiDeletedExpenseItem : DataBinder
{
    public long ID { get; set; } = -1;// This is the local ID column in the remote server.
    public long CentralID { get; set; } = -1;

    public Color BackgroundColor { get; set; } //= Colors.WhiteSmoke;
    public DateTime DateTime { get; set; }
    public string Date => DateTime.ToHuman();
    public string Category { get; set; } = string.Empty;

    public string CurrencySymbol { get; set; } = AppSettings.Account.CurrencySymbol;

    public string Amount { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;

    bool isChecked;
    public bool IsChecked
    {
        get => isChecked;
        set => SetProperty(ref isChecked, value);
    }
    public long CategoryLocalID { get; internal set; }
}

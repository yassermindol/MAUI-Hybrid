using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Models.DbEntities;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Models.UI;

public class UiDayItem : UiExpenseItem
{
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

    public ObservableCollection<UiDayItem> Expenses { get; set; } = new ObservableCollection<UiDayItem>();
    public Color ItemColor { get; set; }
    public int Index { get; set; } = -1;
    public bool IsExpanded { get; set; } = false;
}
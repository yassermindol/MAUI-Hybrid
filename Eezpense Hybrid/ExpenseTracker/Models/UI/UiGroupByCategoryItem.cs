using ExpenseTracker.ExtensionMethods;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Models.UI
{
    public class UiGroupByCategoryItem
    {
        public long CategoryLocalId { get; set; }
        public string Category {  get; set; }
        public string TotalStr => Total.ToMoney();
        public double Total {  get; set; }
        public ObservableCollection<UiExpenseItem> Expenses { get; set; } = new ObservableCollection<UiExpenseItem>();
    }
}

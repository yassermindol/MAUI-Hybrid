using System.Collections.ObjectModel;

namespace ExpenseTracker.Models.UI
{
    public class UiGroupByCategoryItem
    {
        public long CategoryId { get; set; }
        public string Category {  get; set; }
        public string Total { get; set; }
        public ObservableCollection<UiExpenseItem> Expenses { get; set; } = new ObservableCollection<UiExpenseItem>();
    }
}

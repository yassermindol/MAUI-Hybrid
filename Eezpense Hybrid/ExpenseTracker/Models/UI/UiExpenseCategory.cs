using ExpenseTracker.Features.ViewModels;

namespace ExpenseTracker.Models.UI;

public class UiExpenseCategory : DataBinder
{
    public string Name { get; set; }

    bool isChecked;
    public bool IsChecked
    {
        get => isChecked;
        set => SetProperty(ref isChecked, value);
    }
}

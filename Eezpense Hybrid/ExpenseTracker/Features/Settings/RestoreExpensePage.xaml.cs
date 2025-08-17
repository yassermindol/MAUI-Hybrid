using ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;
using ExpenseTracker.Features.Settings.ViewModels;

namespace ExpenseTracker.Features.Settings;

public partial class RestoreExpensePage : ContentPage
{
    RestoreExpenseViewModel _viewModel = new();
    public RestoreExpensePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    private void SelectAllCheckBox_CheckedChanged(Object sender, CheckedChangedEventArgs e)
    {
        bool isChecked = e.Value;
        if (checkedFromCode)
        {
            checkedFromCode = false;
            return;
        }
        var categories = _viewModel.DeletedExpenses;
        for (int i = 0; i < categories.Count; i++)
        {
            checkedFromCode = true;
            categories[i].IsChecked = isChecked;
        }
        checkedFromCode = false;
    }

    bool checkedFromCode = false;
    private void ItemCheckBox_CheckedChanged(Object sender, CheckedChangedEventArgs e)
    {
        var vm = _viewModel;
        var expenses = vm.DeletedExpenses;
        int count = expenses.Count;
        int checkCount = expenses.Where(x => x.IsChecked).Count();

        if (checkedFromCode)
        {
            return;
        }

        if (count == checkCount)
        {
            if (vm.IsSelectAllChecked != true)
            {
                checkedFromCode = true;
            }
            vm.IsSelectAllChecked = true;
        }
        else
        {
            if (vm.IsSelectAllChecked != false)
            {
                checkedFromCode = true;
            }
            vm.IsSelectAllChecked = false;
        }
    }
}
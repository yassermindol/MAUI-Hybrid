using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.DetailsOfExpenseList;
using ExpenseTracker.Features.DetailsOfExpenseList.ViewModels;
using System.ComponentModel;

namespace ExpenseTracker.Features.Daily.BlazorPage;

public class DailyBlazorViewModel : DailyViewModel
{
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectDateButtonText))
        {
            StateHasChanged?.Invoke();
        }
    }

    public override async Task LoadDataAsync()
    {
        StateHasChanged?.Invoke();
        Busy();
        await Task.Delay(10);
        var expenses = _service.GetDaily(StartDate, EndDate);
        var uiItems = _dataProvider.GetUiDays(expenses, IsShowSubItems, out double total);
        Total = total.ToMoney();
        DailyItems.Clear();
        foreach (var item in uiItems)
            DailyItems.Add(item);
        NotBusy();
        StateHasChanged?.Invoke();
    }

    public async Task UiExpenseItemSelectedAsync(long id, Action callback)
    {
        var page = new ExpensePage();
        var expenseEntity = await ExpenseTableDb.Get(id);
        page.BindingContext = new ExpenseViewModel(expenseEntity); ;
        await _navigation.PushAsync(page);
        callback.Invoke();
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Features.Weekly.ViewModels;

public partial class WeeksInYearViewModel : BaseWeeklyViewModel
{
    string[] _yearSelections = new string[12];

    WeeksInYearService service = new WeeksInYearService();

    public WeeksInYearViewModel()
    {
        WeakReferenceMessenger.Default.Register<AddExpenseMessage>(this, OnNewExpenseSaved);
        WeakReferenceMessenger.Default.Register<EditExpenseCategoryMessage>(this, OnEditedExpenseCategory);
        WeakReferenceMessenger.Default.Register<DeleteExpenseCategoryMessage>(this, OnDeleteExpenseCategory);
        WeakReferenceMessenger.Default.Register<RestoreExpenseCategoryMessage>(this, OnRestoredExpenseCategory);
        WeakReferenceMessenger.Default.Register<EditExpenseMessage>(this, OnEditExpense);
        WeakReferenceMessenger.Default.Register<DeleteExpenseMessage>(this, OnDeleteExpense);
        WeakReferenceMessenger.Default.Register<RestoreExpenseMessage>(this, OnRestoreExpense);
        WeakReferenceMessenger.Default.Register<CurrencySymbolChangedMessage>(this, OnCurrencySymbolChanged);
    }

    private void OnCurrencySymbolChanged(object recipient, CurrencySymbolChangedMessage message)
    {
        string symbol = message.Value;
        foreach (var item in WeekItems)
            item.CurrencySymbol = symbol;

        if (_summaryDetailsViewModel != null)
            _summaryDetailsViewModel.RefreshCurrencySymbol(symbol);
    }

    private void OnRestoreExpense(object recipient, RestoreExpenseMessage message)
    {
        ShouldRefreshData = true;
        SetDetailsOfSummaryShouldRefresh();
    }

    private void OnDeleteExpense(object recipient, DeleteExpenseMessage message)
    {
        ShouldRefreshData = true;
        SetDetailsOfSummaryShouldRefresh();
    }

    private void OnEditExpense(object recipient, EditExpenseMessage message)
    {
        ShouldRefreshData = true;
        SetDetailsOfSummaryShouldRefresh();
    }

    private void OnRestoredExpenseCategory(object recipient, RestoreExpenseCategoryMessage message)
    {
        ShouldRefreshData = true;
        SetDetailsOfSummaryShouldRefresh();
    }

    private void OnDeleteExpenseCategory(object recipient, DeleteExpenseCategoryMessage message)
    {
        ShouldRefreshData = true;
        SetDetailsOfSummaryShouldRefresh();
    }

    private void OnEditedExpenseCategory(object recipient, EditExpenseCategoryMessage message)
    {
        ShouldRefreshData = true;
        SetDetailsOfSummaryShouldRefresh();
    }


    private void OnNewExpenseSaved(object recipient, AddExpenseMessage message)
    {
        ShouldRefreshData = true;
        SetDetailsOfSummaryShouldRefresh();
    }

    public async Task ReloadDataIfShouldAsync()
    {
        if (ShouldRefreshData)
        {
            await LoadDataAsync();
            ShouldRefreshData = false;
        }
    }

    [RelayCommand]
    private async Task SelectYearAsync()
    {
        int currentYear = DateTime.Now.Year;
        int past5years = currentYear - 5;
        for (int i = 0; i < 10; i++)
        {
            past5years++;
            _yearSelections[i] = past5years.ToString();
        }
        string year = await ShowActionSheet("Select Year", _yearSelections);
        if (year == null)
            return;
        if (year == "Cancel")
            return;
        SelectedYear = year;
        LoadDataAsync();
    }

    [ObservableProperty]
    string selectedYear = DateTime.Now.Year.ToString();


    [ObservableProperty]
    private double width = AppConstants.BoxViewMaxWidth;

    [ObservableProperty]
    bool isNoRecordsToShowVisible;

    [ObservableProperty]
    bool isListVisible;

    public async Task LoadDataAsync()
    {
        if (IsBusy)
            return;
        Busy();
        var items = await service.GetWeekItems(int.Parse(SelectedYear));
        WeekItems.Clear();
        _collectionChanged.Monitor(WeekItems, items.Count, NotBusy, RefreshUI);
        foreach (var item in items)
            WeekItems.Add(item);
    }

    protected override void Busy()
    {
        IsBusy = true;
        IsListVisible = false;
        IsNoRecordsToShowVisible = false;
    }

    protected override void NotBusy()
    {
        IsBusy = false;
        IsNoRecordsToShowVisible = WeekItems.Count == 0;
        IsListVisible = WeekItems.Count > 0;
    }
}

using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Services;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Settings;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Weekly.ViewModels;

public partial class WeeksInMonthViewModel : BaseWeeklyViewModel
{
    Dictionary<int, string> _months;
    WeeksInMothService _service = new WeeksInMothService();
    CalendarService _calendar = new CalendarService();

    string _selectedMonthOfYear;
    private int _yearNow = DateTime.Now.Year;

    public WeeksInMonthViewModel()
    {
        _months = _calendar.Months;
        _selectedMonthOfYear = _calendar.Months[DateTime.Now.Month];
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

    [ObservableProperty]
    double width = AppConstants.BoxViewMaxWidth;   

    [ObservableProperty]
    bool isNoRecordsToShowVisible;

    [ObservableProperty]
    bool isListVisible;

    string buttonTextSelectMonth;
    public string ButtonTextSelectMonth
    {
        get
        {
            if (buttonTextSelectMonth == null)
            {
                buttonTextSelectMonth = $"{_months[DateTime.Now.Month]} {_yearNow}";
            }
            return $"{buttonTextSelectMonth}";
        }
        set => SetProperty(ref buttonTextSelectMonth, value);
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy)
            return;
        Busy();
        int month = _months.First(x => x.Value == _selectedMonthOfYear).Key;
        var items = await _service.GetWeekItems(_yearNow, month);
        WeekItems.Clear();
        //_collectionChanged.Monitor(WeekItems, items.Count, NotBusy, InvalidateMeasure);
        foreach (var item in items)
            WeekItems.Add(item);
        NotBusy();
    }


    [RelayCommand]
    private async Task SelectMonthAsync()
    {
        string selected = await ShowActionSheet($"Select Month of {_yearNow}", _months.Values.ToArray());
        if (selected == null)
            return;
        if (selected == "Cancel")
            return;
        _selectedMonthOfYear = selected;
        ButtonTextSelectMonth = $"{_selectedMonthOfYear} {_yearNow}";
        LoadDataAsync();
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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.DetailsOfExpenseList;
using ExpenseTracker.Features.DetailsOfExpenseList.ViewModels;
using ExpenseTracker.Models.UI;
using ExpenseTracker.PopupViews.SelectDateRange;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Services.UIModelGenerators;
using ExpenseTracker.Settings;
using Mopups.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ExpenseTracker.Features.Daily;

public partial class DailyViewModel : BaseViewModel
{
    protected DailyService _service = new();
    protected UiListDataProvider _dataProvider = new();

    [ObservableProperty]
    ObservableCollection<UiDayItem> dailyItems = new ObservableCollection<UiDayItem>();

    [ObservableProperty]
    UiDayItem uiExpenseSelectedItem;

    [ObservableProperty]
    string total;

    [ObservableProperty]
    bool isTotalVisible;

    public Action StateHasChanged { get; set; }

    public DailyViewModel()
    {
        WeakReferenceMessenger.Default.Register<AddExpenseMessage>(this, OnNewExpenseSaved);
        WeakReferenceMessenger.Default.Register<EditExpenseCategoryMessage>(this, OnEditedExpenseCategory);
        WeakReferenceMessenger.Default.Register<DeleteExpenseCategoryMessage>(this, OnDeleteExpenseCategory);
        WeakReferenceMessenger.Default.Register<RestoreExpenseCategoryMessage>(this, OnRestoredExpenseCategory);
        WeakReferenceMessenger.Default.Register<EditExpenseMessage>(this, OnEditExpense);
        WeakReferenceMessenger.Default.Register<DeleteExpenseMessage>(this, OnDeleteExpense);
        WeakReferenceMessenger.Default.Register<RestoreExpenseMessage>(this, OnRestoreExpense);
        WeakReferenceMessenger.Default.Register<CurrencySymbolChangedMessage>(this, OnCurrencySymbolChanged);

        if (StartDate == EndDate)
            SelectDateButtonText = $"{StartDate.ToHuman()}";
        else
            SelectDateButtonText = $"{StartDate.ToHuman()} to {EndDate.ToHuman()}";
    }

    private void OnCurrencySymbolChanged(object recipient, CurrencySymbolChangedMessage message)
    {
        CurrencySymbol = message.Value;
        string symbol = message.Value;
        foreach (var item in DailyItems)
        {
            item.CurrencySymbol = symbol;
            if (item.ItemType == Models.ExpenseItemType.Header)
            {
                foreach (var expense in item.Expenses)
                {
                    expense.CurrencySymbol = symbol;
                }
            }
        }

        StateHasChanged();
    }


    private void OnRestoreExpense(object recipient, RestoreExpenseMessage message)
    {
        ShouldRefreshData = true;
    }

    private void OnDeleteExpense(object recipient, DeleteExpenseMessage message)
    {
        ShouldRefreshData = true;
    }

    private void OnEditExpense(object recipient, EditExpenseMessage message)
    {
        ShouldRefreshData = true;
    }

    private void OnRestoredExpenseCategory(object recipient, RestoreExpenseCategoryMessage message)
    {
        ShouldRefreshData = true;
    }

    private void OnDeleteExpenseCategory(object recipient, DeleteExpenseCategoryMessage message)
    {
        ShouldRefreshData = true;
    }

    private void OnEditedExpenseCategory(object recipient, EditExpenseCategoryMessage message)
    {
        ShouldRefreshData = true;
    }

    private void OnNewExpenseSaved(object recipient, AddExpenseMessage message)
    {
        ShouldRefreshData = true;
    }

    public async Task ReloadDataIfShouldAsync()
    {
        if (ShouldRefreshData)
        {
            MainThread.InvokeOnMainThreadAsync(LoadDataAsync);
            ShouldRefreshData = false;
        }
    }

    public bool IsShowSubItems
    {
        get => AppSettings.Account.ShowSubItems;
        internal set => AppSettings.Account.ShowSubItems = value;
    }

    private void Busy()
    {
        IsBusy = true;
        IsListVisible = false;
        IsNoRecordsToShowVisible = false;
        IsTotalVisible = false;
    }

    private void NotBusy()
    {
        IsBusy = false;
        IsListVisible = true;
        IsNoRecordsToShowVisible = DailyItems.Count == 0;
        IsTotalVisible = !IsNoRecordsToShowVisible;
    }

    [RelayCommand]
    private async Task SelectDateRangeAsync()
    {
        var vm = new SelectDateRangeViewModel(Submit);
        vm.SelectedStartDate = AppSettings.Account.StartDateDaily;
        vm.SelectedEndDate = EndDate;
        var page = new SelectDateRangePopup();
        page.BindingContext = vm;
        await MopupService.Instance.PushAsync(page);
    }

    private DateTime StartDate
    {
        get
        {
            if (AppSettings.Account.StartDateDaily == DateTime.MinValue)
            {
                AppSettings.Account.StartDateDaily = DateTime.Now;
            }
            return AppSettings.Account.StartDateDaily;
        }
        set
        {
            AppSettings.Account.StartDateDaily = value;
        }
    }

    [ObservableProperty]
    DateTime endDate = DateTime.Now;

    [ObservableProperty]
    string selectDateButtonText;

    private async void Submit(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
        SelectDateButtonText = $"{StartDate.ToHuman()} to {EndDate.ToHuman()}";
        await MopupService.Instance.PopAsync();
        MainThread.InvokeOnMainThreadAsync(LoadDataAsync);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectDateButtonText))
        {
            StateHasChanged?.Invoke();
        }
    }

    public async Task LoadDataAsync()
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

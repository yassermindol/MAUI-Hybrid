using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Features.DetailsOfExpenseList.ViewModels;
using ExpenseTracker.Features.DetailsOfExpenseList;
using ExpenseTracker.Models.UI;
using ExpenseTracker.PopupViews.SelectDateRange;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Services.UIModelGenerators;
using ExpenseTracker.Settings;
using Mopups.Services;
using System.Collections.ObjectModel;
using ExpenseTracker.ExtensionMethods;

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

    [ObservableProperty]
    string currencySymbol = AppSettings.Account.CurrencySymbol;

    bool _isClicked = false;

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

    [RelayCommand]
    private async Task UiExpenseSelected()
    {
        if (UiExpenseSelectedItem == null)
            return;

        if (_isClicked)
            return;
        _isClicked = true;
        if (UiExpenseSelectedItem.ItemType == Models.ExpenseItemType.Header)
        {
            if (UiExpenseSelectedItem.IsExpanded)
            {
                int i = UiExpenseSelectedItem.Index;
                foreach (var item in UiExpenseSelectedItem.Expenses)
                    DailyItems.RemoveAt(i + 1);
                UiExpenseSelectedItem.IsExpanded = false;
            }
            else
            {
                int index = UiExpenseSelectedItem.Index;
                if (index == DailyItems.Count - 1) // last header was clicked
                {
                    foreach (var item in UiExpenseSelectedItem.Expenses)
                        DailyItems.Add(item);
                }
                else
                {
                    int i = 1;
                    foreach (var item in UiExpenseSelectedItem.Expenses)
                    {
                        DailyItems.Insert(index + i, item);
                        i++;
                    }
                }
                UiExpenseSelectedItem.IsExpanded = true;
            }

            int newIndex = 0;
            List<UiDayItem> tempList = new List<UiDayItem>();
            foreach (var item in DailyItems)
            {
                item.Index = newIndex;
                tempList.Add(item);
                newIndex++;
            }
            DailyItems.Clear();
            foreach (var item in tempList)
                DailyItems.Add(item);
            RefreshUI();
        }
        else
        {
            var page = new ExpensePage();
            var expenseEntity = await ExpenseTableDb.Get(UiExpenseSelectedItem.ID);
            page.BindingContext = new ExpenseViewModel(expenseEntity); ;
            await _navigation.PushAsync(page);
        }
        UiExpenseSelectedItem = null;
        _isClicked = false;
    }

    public virtual async Task LoadDataAsync()
    {
        StateHasChanged?.Invoke();
        Busy();
        await Task.Delay(10);
        var expenses = _service.GetDaily(StartDate, EndDate);
        var uiItems = _dataProvider.GetUiDays(expenses, IsShowSubItems, out double total);
        Total = total.ToMoney();
        DailyItems.Clear();
        _collectionChanged.Monitor(DailyItems, uiItems.Count, NotBusy, RefreshUI);
        foreach (var item in uiItems)
            DailyItems.Add(item);
        StateHasChanged?.Invoke();
    }

    protected void Busy()
    {
        IsBusy = true;
        IsListVisible = false;
        IsNoRecordsToShowVisible = false;
        IsTotalVisible = false;
    }

    protected void NotBusy()
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

    protected DateTime StartDate
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
}

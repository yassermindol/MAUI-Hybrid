using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Models;
using ExpenseTracker.Models.UI;
using ExpenseTracker.PopupViews.SelectDateRange;
using ExpenseTracker.Resources.Localization;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Settings;
using Mopups.Services;
using OxyPlot;
using System.Collections.ObjectModel;
using accounSettings = ExpenseTracker.Settings.AppSettings.Account;

namespace ExpenseTracker.Features.DateRange.ViewModels;

public partial class DateRangeViewModel : ExpenseListBaseViewModel
{
    DateRangeService service = new();

    public DateRangeViewModel()
    {
        WeakReferenceMessenger.Default.Register<AddExpenseMessage>(this, OnNewExpenseSaved);
        WeakReferenceMessenger.Default.Register<EditExpenseCategoryMessage>(this, OnEditedExpenseCategory);
        WeakReferenceMessenger.Default.Register<DeleteExpenseCategoryMessage>(this, OnDeleteExpenseCategory);
        WeakReferenceMessenger.Default.Register<RestoreExpenseCategoryMessage>(this, OnRestoredExpenseCategory);
        WeakReferenceMessenger.Default.Register<EditExpenseMessage>(this, OnEditExpense);
        WeakReferenceMessenger.Default.Register<DeleteExpenseMessage>(this, OnDeleteExpense);
        WeakReferenceMessenger.Default.Register<RestoreExpenseMessage>(this, (OnRestoreExpense));
        WeakReferenceMessenger.Default.Register<CurrencySymbolChangedMessage>(this, OnCurrencySymbolChanged);
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
            await LoadDataAsync();
            ShouldRefreshData = false;
        }
    }

    [RelayCommand]
    private async Task SelectDateRangeAsync()
    {
        var vm = new SelectDateRangeViewModel(Submit);
        vm.SelectedStartDate = accounSettings.StartDateRange;
        vm.SelectedEndDate = accounSettings.EndDateRange;
        var page = new SelectDateRangePopup();
        page.BindingContext = vm;
        await MopupService.Instance.PushAsync(page);
    }

    [RelayCommand]
    private async Task UiExpenseSelectedAsync()
    {
        if (UiExpenseSelectedItem == null)
            return;
        if (_isItemClicked)
            return;
        _isItemClicked = true;
        await OpenExpensePageAsync();
        _isItemClicked = false;
    }

    private async void Submit(DateTime startDate, DateTime endDate)
    {
        accounSettings.StartDateRange = startDate;
        accounSettings.EndDateRange = endDate;
        DateRangeButtonText = GetDateRangeStr();
        await MopupService.Instance.PopAsync();
        LoadDataAsync();
    }

    private void OnCurrencySymbolChanged(object recipient, CurrencySymbolChangedMessage message)
    {
        string symbol = message.Value;

        CurrencySymbol = symbol;

        foreach (var legend in Legends)
            legend.CurrencySymbol = symbol;

        foreach (var item in UiExpenses)
            item.CurrencySymbol = symbol;
    }

    [ObservableProperty]
    PlotModel pieChartModel;

    public string Title => $"{AppResources.DateRange} : {CurrencySymbol} {AmountStr}";

    public string amountStr;
    public string AmountStr
    {
        get => amountStr;
        set
        {
            SetProperty(ref amountStr, value);
            OnPropertyChanged(nameof(Title));
        }
    }

    string currencySymbol = AppSettings.Account.CurrencySymbol;
    public new string CurrencySymbol
    {
        get => currencySymbol;
        set
        {
            SetProperty(ref currencySymbol, value);
            OnPropertyChanged(nameof(Title));
        }
    }

    protected override void NotBusy()
    {
        IsBusy = false;
        if (UiExpenses.Count == 0 && UiGroupByCategoryExpenses.Count == 0)
        {
            IsNoRecordsToShowVisible = true;
            IsPieChartVisible = false;
            IsListVisible = false;
        }
        else
        {
            IsNoRecordsToShowVisible = false;
            if (ViewTypeText == AppResources.ViewList)
            {
                IsPieChartVisible = true;
                IsListVisible = false;
            }
            else
            {
                IsPieChartVisible = false;
                IsListVisible = true;
            }

            DateRangeButtonText = GetDateRangeStr();
        }
    }

    protected override void Busy()
    {
        IsBusy = true;
        IsListVisible = false;
        IsPieChartVisible = false;
        IsNoRecordsToShowVisible = false;
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy)
            return;
        Busy();
        double total = await service.SetData(accounSettings.StartDateRange, accounSettings.EndDateRange);
        AmountStr = total.ToMoney();

        var chart = service.PieChart;
        PieChartModel = chart.PlotModel;
        PieChartModel.SelectionColor = OxyColor.FromRgb(255, 255, 255); ;
        Legends = chart.Legends;
        InvalidateOxyPlot?.Invoke();

        _expenseEntities = service.ExpenseEntities;
        ObservableCollection<UiExpenseItem> expenses = new ObservableCollection<UiExpenseItem>();
        ObservableCollection<UiGroupByCategoryItem> groupedExpenses = new ObservableCollection<UiGroupByCategoryItem>();
        if (CurrentSortType == SortType.DateDescending)
            expenses = _uiDataProvider.GetDateDescending(_expenseEntities);
        else if (CurrentSortType == SortType.AmountDescending)
            expenses = _uiDataProvider.GetAmountDescending(_expenseEntities);
        else if (CurrentSortType == SortType.GroupedByCategoryDateDescending)
            groupedExpenses = _uiDataProvider.GetGroupedByCategoryDateDescendingV2(_expenseEntities);
        else if (CurrentSortType == SortType.GroupedByCategoryAmountDescending)
            groupedExpenses = _uiDataProvider.GetGroupedByCategoryAmountDescendingV2(_expenseEntities);

        UiExpenses.Clear();
        UiGroupByCategoryExpenses.Clear();

        if (expenses.Count > 0)
        {
            foreach (var item in expenses)
                UiExpenses.Add(item);
            IsExpenseListGroupedByCategory = false;
        }

        if (groupedExpenses.Count > 0)
        {
            foreach (var item in groupedExpenses)
                UiGroupByCategoryExpenses.Add(item);
            IsExpenseListGroupedByCategory = true;
        }

        NotBusy();
        StateHasChanged();
    }

    [ObservableProperty]
    List<Legend> legends;

    [ObservableProperty]
    bool isPieChartVisible = true;

    string dateRangeButtonText;
    public string DateRangeButtonText
    {
        get
        {
            if (dateRangeButtonText == null)
                dateRangeButtonText = GetDateRangeStr();
            return dateRangeButtonText;
        }
        set => SetProperty(ref dateRangeButtonText, value);
    }

    public override SortType CurrentSortType
    {
        get => (SortType)AppSettings.Account.PreferredExpenseListSortTypeDateRange;
        set => AppSettings.Account.PreferredExpenseListSortTypeDateRange = (int)value;
    }

    public Action InvalidateOxyPlot { get; internal set; }

    /// <summary>
    ///  Get dates from shared preferences.
    /// </summary>
    private string GetDateRangeStr()
    {
        string startDate = accounSettings.StartDateRange.ToString(_dateFormat);
        string endDate = accounSettings.EndDateRange.ToString(_dateFormat);
        string dateRange = $"{startDate} to {endDate}";
        return dateRange;
    }

    public void OpenSearchPageAsync()
    {
        OpenSearchPageAsync(accounSettings.StartDateRange.ToString(_dateFormat),
            accounSettings.EndDateRange.ToString(_dateFormat));
    }

    string _dateFormat = AppConstants.DateFormat;
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Resources.Localization;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Services.UIModelGenerators;
using ExpenseTracker.Settings;
using OxyPlot;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.DetailsOfSummaryReport.ViewModels;

public partial class SummaryDetailsViewModel : ExpenseListBaseViewModel
{
    BaseApiService _service = new BaseApiService();
    public string Title => $"{TitlePartial}: {CurrencySymbol} {AmountStr}";

    private void OnCurrencySymbolChanged(object recipient, CurrencySymbolChangedMessage message)
    {
        CurrencySymbol = message.Value;
        StateHasChanged();
        OnPropertyChanged(nameof(Title));
    }

    string amountStr;
    public string AmountStr
    {
        get => amountStr;
        set
        {
            SetProperty(ref amountStr, value);
            OnPropertyChanged(nameof(Title));
        }
    }

    public string TitlePartial { get; set; }

    DateTime _startDate;
    DateTime _endDate;
    public SummaryDetailsViewModel(List<ExpenseEntity> expenses, DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
        _expenseEntities = expenses;
        WeakReferenceMessenger.Default.Register<CurrencySymbolChangedMessage>(this, OnCurrencySymbolChanged);
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

    string category;
    public string Category
    {
        get => category;
        set => SetProperty(ref category, value);
    }

    PlotModel pieChartModel;
    public PlotModel PieChartModel
    {
        get => pieChartModel;
        set => SetProperty(ref pieChartModel, value);
    }


    public async Task ReloadDataIfShouldAsync()
    {
        if (ShouldRefreshData)
        {
            if (IsBusy)
                return;
            Busy();
            _expenseEntities = await _service.GetExpenses(_startDate, _endDate);
            await LoadDataAsync();
            ShouldRefreshData = false;
        }
    }

    protected override void Busy()
    {
        IsBusy = true;
        IsPieChartVisible = false;
        IsListVisible = false;
        IsNoRecordsToShowVisible = false;
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
        }
    }

    public async Task LoadDataAsync()
    {
        Busy();

        UiPieChartDataProvider provider = new();
        var chart = provider.GetPieChart(_expenseEntities);
        PieChartModel = chart.PlotModel;
        Legends = chart.Legends;
        InvalidateOxyPlot?.Invoke();

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

        double total = _expenseEntities.Sum(x => x.Amount);
        AmountStr = total.ToMoney();

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

    public void OpenSearchPageAsync()
    {
        OpenSearchPageAsync(_startDate.ToString("MMM-dd-yyyy"), _endDate.ToString("MMM-dd-yyyy"));
    }

    public void RefreshCurrencySymbol(string symbol)
    {
        CurrencySymbol = symbol;
        foreach (var legend in Legends)
            legend.CurrencySymbol = symbol;
        foreach (var item in UiExpenses)
            item.CurrencySymbol = symbol;
    }

    List<Legend> legends;
    public List<Legend> Legends
    {
        get => legends;
        set => SetProperty(ref legends, value);
    }
    public override SortType CurrentSortType
    {
        get => (SortType)AppSettings.Account.PreferredExpenseListSortTypeSummaryDetails;
        set => AppSettings.Account.PreferredExpenseListSortTypeSummaryDetails = (int)value;
    }

    public Action InvalidateOxyPlot { get; internal set; }

    [ObservableProperty]
    bool isPieChartVisible;

    [ObservableProperty]
    string dateRange;

    [ObservableProperty]
    bool isDateRangeVisible;
}

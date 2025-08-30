using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.DetailsOfSummaryReport;
using ExpenseTracker.Features.DetailsOfSummaryReport.ViewModels;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Settings;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Weekly.ViewModels;

public abstract partial class BaseWeeklyViewModel : BaseViewModel
{
    protected string _strWeekNumberSelected;

    protected SummaryDetailsViewModel _summaryDetailsViewModel;    

    protected void SetDetailsOfSummaryShouldRefresh()
    {
        if (_summaryDetailsViewModel != null)
            _summaryDetailsViewModel.ShouldRefreshData = true;
    }

    bool isClicked;
    [RelayCommand]
    private async Task WeekSelectedAsync()
    {
        if (SelectedWeek == null)
            return;
        if (isClicked)
            return;
        var weekItem = SelectedWeek;
        isClicked = true;
        var expenses = weekItem.Expenses.ToList();
        var viewModel = new SummaryDetailsViewModel(expenses, weekItem.StartDate, weekItem.EndDate);
        viewModel.TitlePartial = $"Week {weekItem.WeekNumber}";
        viewModel.AmountStr = expenses.Sum(x => x.Amount).ToMoney();
        viewModel.DateRange = $"{weekItem.StartDateStr} - {weekItem.EndDateStr}";
        viewModel.IsDateRangeVisible = true;
        SummaryDetailsPage page = new();
        page.BindingContext = viewModel;
        Type runtimeType = this.GetType();
        string name = runtimeType.Name;
        DiContainerSummaryDetails.RegisterViewModel(name, viewModel);
        _summaryDetailsViewModel = viewModel;
        await _navigation.PushAsync(page);
        _strWeekNumberSelected = SelectedWeek.WeekNumber;
        SelectedWeek = null;
        isClicked = false;
    }

    [ObservableProperty]
    UiWeekItem selectedWeek;

    [ObservableProperty]
    ObservableCollection<UiWeekItem> weekItems = new ObservableCollection<UiWeekItem>();

    protected abstract void NotBusy();
    protected abstract void Busy();
}

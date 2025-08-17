using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Resources.Localization;
using ExpenseTracker.Settings;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.SearchExpenseList;
public partial class SearchExpenseListViewModel : ExpenseListBaseViewModel
{
    public SearchExpenseListViewModel(List<ExpenseEntity> expenseEntities,
        string startDate, string endDate)
    {
        SearchCoverage = $"Search coverage: {startDate} to {endDate}";
        _expenseEntities = expenseEntities;
        LoadData();
    }

    [ObservableProperty]
    string searchCoverage;

    [ObservableProperty]
    bool isExpenseListVisible;

    [ObservableProperty]
    string searchTextResult;

    [ObservableProperty]
    ObservableCollection<string> categories = new ObservableCollection<string>();

    [ObservableProperty]
    string selectedCategory = string.Empty;


    string amount = string.Empty;
    public string Amount
    {
        get => amount;
        set
        {
            if (SetProperty(ref amount, value))
            {
                IsSearchButtonEnabled = !string.IsNullOrWhiteSpace(value) || !string.IsNullOrWhiteSpace(Note) || !string.IsNullOrWhiteSpace(SelectedCategory);
            }
        }
    }

    string note = string.Empty;
    public string Note
    {
        get => note;
        set
        {
            if (SetProperty(ref note, value))
            {
                IsSearchButtonEnabled = !string.IsNullOrWhiteSpace(value) || !string.IsNullOrWhiteSpace(Amount) || !string.IsNullOrWhiteSpace(SelectedCategory);
            }
        }
    }

    [ObservableProperty]
    bool isSearchButtonEnabled = false;

    public override SortType CurrentSortType
    {
        get => (SortType)AppSettings.Account.PreferredExpenseListSortTypeSearchExpense;
        set => AppSettings.Account.PreferredExpenseListSortTypeSearchExpense = (int)value;
    }

    private void LoadData()
    {
        foreach (var item in CategoryNamesDb)
        {
            Categories.Add(item);
        }
        Categories.Add(AppResources.ClearSelected);

        UiExpenses = _uiDataProvider.GetDateDescending(_expenseEntities);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (IsBusy)
            return;
        IsBusy = true;
        IsSearchButtonEnabled = false;
        IsExpenseListVisible = false;
        SearchTextResult = string.Empty;

        UiExpenses = GetSearchResults();

        if (UiExpenses.Count > 0)
        {
            string result = UiExpenses.Count == 1 ? "result" : "results";
            SearchTextResult = $"Your search returned {UiExpenses.Count} {result}";
            IsExpenseListVisible = true;
        }
        else
        {
            SearchTextResult = $"No results found.";
            IsExpenseListVisible = false;
        }

        IsSearchButtonEnabled = true;
        IsBusy = false;
    }

    private ObservableCollection<UiExpenseItem> GetSearchResults()
    {
        List<ExpenseEntity> list = new List<ExpenseEntity>();

        if (!string.IsNullOrWhiteSpace(SelectedCategory))
        {
            list = _expenseEntities.Where(e => e.Category == SelectedCategory).ToList();
            if (!string.IsNullOrWhiteSpace(Amount))
                list = list.Where(e => e.Amount.ToString().Contains(Amount)).ToList();
            if (!string.IsNullOrWhiteSpace(Note))
                list = list.Where(e => e.Note.Contains(Note, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        else if (!string.IsNullOrWhiteSpace(Amount))
        {
            list = _expenseEntities.Where(e => e.Amount.ToString().Contains(Amount)).ToList();
            if (!string.IsNullOrWhiteSpace(Note))
                list = list.Where(e => e.Note.Contains(Note, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        else if (!string.IsNullOrWhiteSpace(Note))
        {
            list = _expenseEntities.Where(e => e.Note.Contains(Note, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var uiEpenses = _uiDataProvider.GetDateDescending(list);
        return uiEpenses;
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

    [RelayCommand]
    private void CategorySelected()
    {
        if(SelectedCategory == AppResources.ClearSelected)
            SelectedCategory = string.Empty;
 
        IsSearchButtonEnabled = !string.IsNullOrWhiteSpace(Note) || !string.IsNullOrWhiteSpace(Amount) || !string.IsNullOrWhiteSpace(SelectedCategory);
    }

    protected override void NotBusy()
    {
    }

    protected override void Busy()
    {
    }
}

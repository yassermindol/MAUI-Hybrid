using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.Features.DetailsOfExpenseList;
using ExpenseTracker.Features.DetailsOfExpenseList.ViewModels;
using ExpenseTracker.Features.SearchExpenseList;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Resources.Localization;
using ExpenseTracker.Services.UIModelGenerators;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ExpenseTracker.Features.ViewModels;
public abstract partial class ExpenseListBaseViewModel : BaseViewModel
{
    [ObservableProperty]
    ObservableCollection<UiExpenseItem> uiExpenses = new ObservableCollection<UiExpenseItem>();

    [ObservableProperty]
    ObservableCollection<UiGroupByCategoryItem> uiGroupByCategoryExpenses = new ObservableCollection<UiGroupByCategoryItem>();

    [ObservableProperty]
    bool isExpenseListGroupedByCategory;

    [ObservableProperty]
    UiExpenseItem uiExpenseSelectedItem;

    protected UiListDataProvider _uiDataProvider = new();
    protected List<ExpenseEntity> _expenseEntities;

    [ObservableProperty]
    string viewTypeText = AppResources.ViewList;

    public Action<long> ScrollToItem { get; set; }

    protected abstract void NotBusy();
    protected abstract void Busy();

    protected bool _isItemClicked;

    public abstract SortType CurrentSortType { get; set; }

    public async Task GroupByCategoryDateDescendingAsync()
    {
        if (CurrentSortType == SortType.GroupedByCategoryDateDescending)
            return;
        if (IsBusy)
            return;
        Busy();
        CurrentSortType = SortType.GroupedByCategoryDateDescending;
        var groupedItems = _uiDataProvider.GetGroupedByCategoryDateDescendingV2(_expenseEntities);
        UiGroupByCategoryExpenses.Clear();
        foreach (var item in groupedItems)
            UiGroupByCategoryExpenses.Add(item);
        NotBusy();
    }

    public async Task GroupByCategoryAmountDescendingAsync()
    {
        if (CurrentSortType == SortType.GroupedByCategoryAmountDescending)
            return;
        if (IsBusy)
            return;
        Busy();
        CurrentSortType = SortType.GroupedByCategoryAmountDescending;

        var groupedExpenses = _uiDataProvider.GetGroupedByCategoryAmountDescendingV2(_expenseEntities);
        UiGroupByCategoryExpenses.Clear();
        foreach (var item in groupedExpenses)
            UiGroupByCategoryExpenses.Add(item);
        NotBusy();
    }

    public async Task SortbyDateDescendingAsync()
    {
        if (CurrentSortType == SortType.DateDescending)
            return;
        if (IsBusy)
            return;
        Busy();
        CurrentSortType = SortType.DateDescending;
        var expenses = _uiDataProvider.GetDateDescending(_expenseEntities);
        UiExpenses.Clear();
        foreach (var item in expenses)
            UiExpenses.Add(item);
        NotBusy();
    }

    public async Task SortbyAmountDescendingAsync()
    {
        if (CurrentSortType == SortType.AmountDescending)
            return;
        if (IsBusy)
            return;
        Busy();
        CurrentSortType = SortType.AmountDescending;
        var expenses = _uiDataProvider.GetAmountDescending(_expenseEntities);
        UiExpenses.Clear();
        foreach (var item in expenses)
            UiExpenses.Add(item);
        NotBusy();
    }

    public ICommand UiExpenseItemAppearingCommand => new Command<ItemVisibilityEventArgs>(UiExpenseItemAppearing);
    private void UiExpenseItemAppearing(ItemVisibilityEventArgs e)
    {

    }

    [RelayCommand]
    private void UiExpenseSizeChanged(EventArgs e)
    {
    }

    protected async Task OpenSearchPageAsync(string startDate, string endDate)
    {
        SearchExpenseListViewModel viewModel = new(_expenseEntities, startDate, endDate);
        var page = new SearchExpenseListPage();
        page.BindingContext = viewModel;
        _navigation.PushAsync(page);
    }


    protected async Task OpenExpensePageAsync()
    {
        if (UiExpenseSelectedItem.ItemType == ExpenseItemType.Header)
            return;

        var page = new ExpensePage();
        var expenseEntity = await ExpenseTableDb.Get(UiExpenseSelectedItem.ID);
        page.BindingContext = new ExpenseViewModel(expenseEntity); ;
        await _navigation.PushAsync(page);
        UiExpenseSelectedItem = null;
    }
}

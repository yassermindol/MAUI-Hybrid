using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public Action StateHasChanged { get;set; }

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
        NotBusy();
        return;
        Task.Run(() =>
        {
            var expenses = _uiDataProvider.GetGroupedByCategoryDateDescending(_expenseEntities);            
            UiExpenses.Clear();
            _collectionChanged.Monitor(UiExpenses, expenses.Count, NotBusy, RefreshUI);
            foreach (var item in expenses)
                UiExpenses.Add(item);
        });
    }

    public async Task GroupByCategoryAmountDescendingAsync()
    {
        if (CurrentSortType == SortType.GroupedByCategoryAmountDescending)
            return;
        if (IsBusy)
            return;
        Busy();
        CurrentSortType = SortType.GroupedByCategoryAmountDescending;
        NotBusy();
        return;
        Task.Run(() =>
        {
            var expenses = _uiDataProvider.GetGroupedByCategoryAmountDescending(_expenseEntities);
            UiExpenses.Clear();
            _collectionChanged.Monitor(UiExpenses, expenses.Count, NotBusy, RefreshUI);
            foreach (var item in expenses)
                UiExpenses.Add(item);
        });
    }

    public async Task SortbyDateDescendingAsync()
    {
        if (CurrentSortType == SortType.DateDescending)
            return;
        if (IsBusy)
            return;
        Busy();
        CurrentSortType = SortType.DateDescending;
        NotBusy();
        return;
        Task.Run(() =>
        {
            var expenses = _uiDataProvider.GetDateDescending(_expenseEntities);
            UiExpenses.Clear();
            _collectionChanged.Monitor(UiExpenses, expenses.Count, NotBusy, RefreshUI);
            foreach (var item in expenses)
                UiExpenses.Add(item);
        });
    }

    public async Task SortbyAmountDescendingAsync()
    {
        if (CurrentSortType == SortType.AmountDescending)
            return;
        if (IsBusy)
            return;
        Busy();
        CurrentSortType = SortType.AmountDescending;
        NotBusy();
        return;
        Task.Run(() =>
        {
            var expenses = _uiDataProvider.GetAmountDescending(_expenseEntities);
            UiExpenses.Clear();
            _collectionChanged.Monitor(UiExpenses, expenses.Count, NotBusy, RefreshUI);
            foreach (var item in expenses)
                UiExpenses.Add(item);
        });
    }

    public ICommand UiExpenseItemAppearingCommand => new Command<ItemVisibilityEventArgs>(UiExpenseItemAppearing);
    private void UiExpenseItemAppearing(ItemVisibilityEventArgs e)
    {
        /*
        if (e.Item == null)
            return;

        if (e.Item is UiExpenseItem item)
        {
            Console.WriteLine($"****************** UiExpenseItemAppearing: Item added: ID: {item.ID} Category:{item.Category}");
        }
        */
    }

    [RelayCommand]
    private void UiExpenseSizeChanged(EventArgs e)
    {
        Console.WriteLine($"****************** UiExpenseSizeChanged");
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

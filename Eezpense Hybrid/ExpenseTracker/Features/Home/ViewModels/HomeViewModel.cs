using Android.Webkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Database;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.PopupViews.SelectDateTime;
using ExpenseTracker.Resources.Localization;
using ExpenseTracker.Services;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Settings;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Home.ViewModels;

public partial class HomeViewModel : ExpenseListBaseViewModel
{
    double _total = 0;

    HomeService _homeService = new();
    CalendarService _calendarService = new();

    public Action<UiExpenseItem> SaveExpenseDelegate { get; set; }
    public Action NoteCompletedDelegate { get; set; }
    public Func<Task> AnimateClickDelegate { get; set; }

    public HomeViewModel()
    {
        AddExpenseIconSource = IsDarkTheme ? "ic_expand_white.png" : "ic_expand.png";
#if ANDROID
        EntryHandler.Mapper.AppendToMapping(nameof(EntryHandler), (handler, view) => handler.PlatformView.UpdateReturnType(view));
#endif
        WeakReferenceMessenger.Default.Register<AddExpenseCategoryMessage>(this, OnAddedExpenseCategory);
        WeakReferenceMessenger.Default.Register<SelectedExpenseCategoryMessage>(this, OnSelectedExpenseCategory);
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
        CurrencySymbol = message.Value;
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

    private void OnAddedExpenseCategory(object recipient, AddExpenseCategoryMessage message)
    {
        SelectedExpenseCategory = message.Value;
    }

    private void OnSelectedExpenseCategory(object sender, SelectedExpenseCategoryMessage message)
    {
        var item = message.Value;
        if (item != "Cancel")
            SelectedExpenseCategory = item;
    }

    public async Task OpenSearchPageAsync()
    {
        OpenSearchPageAsync(SelectedStartDate.ToString("MMM-dd-yyyy"), "Present");
    }

    [ObservableProperty]
    bool isAddExpenseVisible = true;

    [ObservableProperty]
    string categorySelectionErrorText = string.Empty;

    [ObservableProperty]
    string amountErrorText = string.Empty;

    [ObservableProperty]
    string amountStr = string.Empty;

    [ObservableProperty]
    string note = string.Empty;

    [ObservableProperty]
    string selectedExpenseCategory = string.Empty;

    [ObservableProperty]
    Color clickableStackBackgroundColor = Colors.Transparent;

    [ObservableProperty]
    ImageSource addExpenseIconSource = "ic_expand.png";

    [ObservableProperty]
    bool isBottomSheetPresented = false;

    DateTime selectedStartDate = AppSettings.Account.StartDateHome;
    public DateTime SelectedStartDate
    {
        get
        {
            if (selectedStartDate == DateTime.MinValue)
            {
                selectedStartDate = DateTime.Now.Date;
                AppSettings.Account.StartDateHome = selectedStartDate;
            }
            return selectedStartDate;
        }
        set
        {
            SetProperty(ref selectedStartDate, value);
            AppSettings.Account.StartDateHome = value;
        }
    }

    [ObservableProperty]
    string totalExpense;

    [RelayCommand]
    private async Task UiExpenseSelectedAsync()
    {
        if (IsKeyboardVisible)
            return;

        if (_isItemClicked)
            return;
        _isItemClicked = true;
        await OpenExpensePageAsync();
        _isItemClicked = false;
    }

    partial void OnIsBottomSheetPresentedChanged(bool value)
    {
        if (IsDarkTheme)
            AddExpenseIconSource = value ? "ic_collapse_white.png" : "ic_expand_white.png";
        else
            AddExpenseIconSource = value ? "ic_collapse.png" : "ic_expand.png";
    }

    private void ResetFields()
    {
        SelectedExpenseCategory = AppResources.ChooseExpenseCategory;
        AmountStr = string.Empty;
        Note = string.Empty;
    }

    private bool IsInputsValid()
    {
        AmountErrorText = string.Empty;
        CategorySelectionErrorText = string.Empty;

        bool isValid = true;
        if (string.IsNullOrWhiteSpace(AmountStr))
        {
            AmountErrorText = "Amount input is required.";
            isValid = false;
        }
        else
        {
            if (!double.TryParse(AmountStr, out double result))
            {
                AmountErrorText = "Amount input is invalid.";
                isValid = false;
            }
        }

        if (SelectedExpenseCategory.Equals(AppResources.ChooseExpenseCategory)
            || string.IsNullOrWhiteSpace(SelectedExpenseCategory))
        {
            CategorySelectionErrorText = "Category selection is required.";
            isValid = false;
        }
        return isValid;
    }

    [RelayCommand]
    private void DateSelected()
    {
        LoadDataAsync();
    }

    [RelayCommand]
    private async Task NoteCompletedAsync()
    {
        if (SelectedExpenseCategory.Equals(AppResources.ChooseExpenseCategory) || string.IsNullOrWhiteSpace(SelectedExpenseCategory))
            await ShowExpenseCategoryAsync();
        NoteCompletedDelegate?.Invoke();
    }


    DateTime _selectedExpenseDate = DateTime.MinValue;

    [RelayCommand]
    private void Save()
    {
        if (!IsInputsValid())
            return;
        if (IsBusy)
            return;

        IsBusy = true;
        double amount = double.Parse(AmountStr);
        DateTime date = _selectedExpenseDate == DateTime.MinValue ? DateTime.Now : _selectedExpenseDate;
        ExpenseEntity expenseEntity;
        SaveToDb(amount, SelectedExpenseCategory, Note, date, out long Id, out expenseEntity);
        _expenseEntities.Add(expenseEntity);
        _total += amount;
        TotalExpense = _total.ToMoney();
        var expenseItem = new UiExpenseItem
        {
            ID = Id,
            Amount = amount,
            Note = Note,
            DateTime = date,
            Category = SelectedExpenseCategory,            
            ItemType = ExpenseItemType.ExpenseItem,
        };

        InsertAmountToUiList(expenseItem);
        SaveExpenseDelegate?.Invoke(expenseItem);
        ResetFields();
        IsNoRecordsToShowVisible = false;
        IsListVisible = true;
        _selectedExpenseDate = DateTime.MinValue;
        IsBusy = false;
        StateHasChanged();
        ScrollToItem(Id);
    }

    private void InsertAmountToUiList(UiExpenseItem expenseToInsert)
    {
        if (CurrentSortType == SortType.DateDescending)
        {
            InsertToDateDescendingList(UiExpenses, expenseToInsert);
        }
        else if (CurrentSortType == SortType.AmountDescending)
        {
            InsertToAmountDescendingList(UiExpenses, expenseToInsert);
        }
        else if (CurrentSortType == SortType.GroupedByCategoryAmountDescending)
        {
            var group = UiGroupByCategoryExpenses.First(x => x.Category == expenseToInsert.Category);
            group.Total += expenseToInsert.Amount;
            InsertToAmountDescendingList(group.Expenses, expenseToInsert);
        }
        else if (CurrentSortType == SortType.GroupedByCategoryDateDescending)
        {
            var group = UiGroupByCategoryExpenses.First(x => x.Category == expenseToInsert.Category);
            group.Total += expenseToInsert.Amount;
            InsertToDateDescendingList(group.Expenses, expenseToInsert);
        }
    }

    private void InsertToDateDescendingList(ObservableCollection<UiExpenseItem> list, UiExpenseItem itemToInsert)
    {
        int low = 0;
        int high = list.Count - 1;
        int mid = 0;
        while (low <= high)
        {
            mid = low + (high - low) / 2;
            if (list[mid].DateTime < itemToInsert.DateTime)
            {
                // If middle element is less than new, search the left half
                high = mid - 1;
            }
            else
            {
                // If middle element is greater or equal, search the right half
                low = mid + 1;
            }
        }
        // 'low' is the insertion index
        if (list.Count == low)
            list.Add(itemToInsert);
        else
            list.Insert(low, itemToInsert);
    }

    private void InsertToAmountDescendingList(ObservableCollection<UiExpenseItem> list, UiExpenseItem itemToInsert)
    {
        int index = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Amount < itemToInsert.Amount)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
            list.Add(itemToInsert);
        else
            list.Insert(index, itemToInsert);
    }

    [RelayCommand]
    private async Task SelectDateTimeAsync()
    {
        var viewModel = new SelectDateTimeViewModel(OnDateTimeSelected);
        viewModel.SelectedDate = _selectedExpenseDate == DateTime.MinValue ? DateTime.Now.Date : _selectedExpenseDate.Date;
        viewModel.SelectedTime = _selectedExpenseDate == DateTime.MinValue ? DateTime.Now.TimeOfDay : _selectedExpenseDate.TimeOfDay;
        var page = new SelectDateTimePopup();
        page.BindingContext = viewModel;
        await MopupService.Instance.PushAsync(page);
    }

    private void OnDateTimeSelected(DateTime date, TimeSpan span)
    {
        if (date == DateTime.MinValue)
            _selectedExpenseDate = DateTime.MinValue;
        else
            _selectedExpenseDate = date.Date.Add(span);
    }

    [RelayCommand]
    private void ShowExpenseCategoryList()
    {
        if (IsBusy)
            return;
        IsBusy = true;
        //if (DeviceInfo.Platform == DevicePlatform.iOS)
        //{
        //    var selectedValue = DisplayActionSheet("Select an option", "Cancel", null, "Option 1", "Option 2", "Option 3",
        //    "Option 4", "Option 5", "Option 6", "Option 7", "Option 8", "Option 9").Result;
        //}
        //else
        //{
        //AnimateClickDelegate?.Invoke();
        ShowExpenseCategoryAsync();
        IsBusy = false;
        //}
    }


    public bool IsKeyboardVisible { get; internal set; }
    public override SortType CurrentSortType
    {
        get => (SortType)AppSettings.Account.PreferredExpenseListSortTypeHome;
        set => AppSettings.Account.PreferredExpenseListSortTypeHome = (int)value;
    }

    private async Task ShowExpenseCategoryAsync()
    {
        await MopupService.Instance.PushAsync(new CategoriesPopup());
    }

    protected override void Busy()
    {
        IsBusy = true;
        IsAddExpenseVisible = false;
        if (IsBottomSheetPresented)
            IsBottomSheetPresented = false;
        IsListVisible = false;
        IsNoRecordsToShowVisible = false;
    }

    protected override void NotBusy()
    {
        IsBusy = false;
        IsAddExpenseVisible = true;
        IsNoRecordsToShowVisible = UiExpenses.Count == 0 && UiGroupByCategoryExpenses.Count == 0;
        IsListVisible = !IsNoRecordsToShowVisible;
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy)
            return;
        Busy();
        _expenseEntities = _homeService.GetRecent(SelectedStartDate, out _total);
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
        TotalExpense = _total.ToMoney();

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

    public async Task ReloadDataIfShouldAsync()
    {
        if (ShouldRefreshData)
        {
            LoadDataAsync();
            ShouldRefreshData = false;
        }
    }

    private void SaveToDb(double amount, string selectedExpenseCategory, string note, DateTime date, out long dbId, out ExpenseEntity outExpenseEntity)
    {
        dbId = -1;
        var category = CategoriesDb.First(x => x.Name == selectedExpenseCategory);
        int weekNumber = _calendarService.GetWeekOfYear(date).Number;
        var entity = new ExpenseEntity
        {
            Amount = amount,
            Note = note,
            Date = date,
            CategoryLocalID = category.ID,
            Category = category.Name,
            CategoryCentralID = category.CentralID,
            WeekNumber = weekNumber,
        };
        outExpenseEntity = entity;
        ExpenseTableDb.Add(entity);
        dbId = entity.ID; // this is important
        WeakReferenceMessenger.Default.Send(new AddExpenseMessage(entity));
    }
}

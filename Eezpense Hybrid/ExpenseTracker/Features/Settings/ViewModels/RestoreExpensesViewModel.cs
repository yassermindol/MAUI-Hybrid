using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Resources.Localization;
using ExpenseTracker.Services.UIModelGenerators;
using ExpenseTracker.Settings;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Settings.ViewModels;

public partial class RestoreExpenseViewModel : BaseViewModel
{

    public RestoreExpenseViewModel()
    {
        LoadDeletedExpenses();
        EventMessengerManager.Register(this, OnDeletedExpenseMessage);
    }

    private void OnDeletedExpenseMessage(object recipient, DeleteExpenseMessage message)
    {
        LoadDeletedExpenses();
    }

    [ObservableProperty]
    string currencySymbol = AppSettings.Account.CurrencySymbol;

    [ObservableProperty]
    ObservableCollection<UiDeletedExpenseItem> deletedExpenses = new();

    [ObservableProperty]
    bool isRestoreControlsVisible = false;

    [ObservableProperty]
    Color garbageIconTintColor = Colors.Gray;

    [ObservableProperty]
    bool isSelectAllChecked = false;

    [ObservableProperty]
    bool isNoDeletedExpensesToShowVisible = false;

    [RelayCommand]
    private async Task Restore()
    {
        if (!DeletedExpenses.Any(x => x.IsChecked))
        {
            await ShowMessage(string.Empty, "Select an item to restore.");
            return;
        }

        List<ExpenseEntity> expensesToRestore = new();
        var deletedExpensesDb = DeletedExpensesDb;

        foreach (var item in DeletedExpenses)
        {
            if (item.IsChecked)
            {
                var itemToRestore = deletedExpensesDb.FirstOrDefault(x => x.ID == item.ID);
                itemToRestore.IsDeleted = false;
                expensesToRestore.Add(itemToRestore);
            }
        }

        bool accepted = await ShowMessage("Confirm Restore", "The selected items will now be included in the expense report. Click restore to proceed.", "Restore", "Cancel");

        if (accepted)
        {
            //_navigation.Popups.ShowStatus("Restoring deleted expense records");
            ExpenseTableDb.UpdateAll(expensesToRestore);
            //await Task.Delay(6000);
            LoadDeletedExpenses();
            WeakReferenceMessenger.Default.Send(new RestoreExpenseMessage(""));
            //_navigation.Popups.DismissStatus();
            ShowMessage("", AppResources.RestoreCategorySuccessful);
        }
    }

    bool checkedFromCode = false;
    [RelayCommand]
    private void SelectAllCheckBoxTapped(bool isChecked)
    {
        if (checkedFromCode)
        {
            checkedFromCode = false;
            return;
        }
        for (int i = 0; i < DeletedExpenses.Count; i++)
        {
            checkedFromCode = true;
            DeletedExpenses[i].IsChecked = isChecked;
        }
        checkedFromCode = false;
    }

    [RelayCommand]
    private void ItemCheckBoxTapped(UiDeletedExpenseItem selectedItem)
    {
        int checkCount = DeletedExpenses.Where(x => x.IsChecked).Count();
        if (checkedFromCode)
        {
            return;
        }

        if (DeletedExpenses.Count == checkCount)
        {
            if (IsSelectAllChecked != true)
            {
                checkedFromCode = true;
            }
            IsSelectAllChecked = true;
        }
        else
        {
            if (IsSelectAllChecked != false)
            {
                checkedFromCode = true;
            }
            IsSelectAllChecked = false;
        }
    }

    public async Task LoadDeletedExpenses()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        DeletedExpenses.Clear();

        UiListDataProvider uiListProvider = new UiListDataProvider();
        var uiDataList = uiListProvider.GetUiDeletedExpenses(DeletedExpensesDb);

        foreach (var item in uiDataList)
        {
            DeletedExpenses.Add(item);
        }

        if (DeletedExpenses.Count > 0)
        {
            IsNoDeletedExpensesToShowVisible = false;
            IsRestoreControlsVisible = true;
        }
        else
        {
            IsNoDeletedExpensesToShowVisible = true;
            IsRestoreControlsVisible = false;
        }
        IsSelectAllChecked = false;
        IsBusy = false;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Resources.Localization;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;
public partial class RestoreCategoryViewModel : BaseViewModel
{
    public RestoreCategoryViewModel()
    {
        //EventMessengerManager.Register(this, OnDeleteExpenseCategory);
    }

    private void OnDeleteExpenseCategory(object recipient, DeleteExpenseCategoryMessage message)
    {
        LoadCategories();
    }

    [ObservableProperty]
    ObservableCollection<UiExpenseCategory> deletedCategories;

    [ObservableProperty]
    bool isRestoreControlsVisible = false;

    [ObservableProperty]
    Color garbageIconTintColor = Colors.Gray;

    [ObservableProperty]
    bool isSelectAllChecked = false;

    [ObservableProperty]
    bool isNoCategoriesToShowVisible = false;

    [RelayCommand]
    private async Task Restore()
    {
        List<string> categoriesToRestore = new List<string>();

        foreach (var deletedCategory in DeletedCategories)
        {
            if (deletedCategory.IsChecked)
            {
                categoriesToRestore.Add(deletedCategory.Name);
            }
        }

        if (categoriesToRestore.Count == 0)
        {
            await ShowMessage(string.Empty, "Select an item to restore.");
            return;
        }

        string categoriesToDisplay = Environment.NewLine;
        foreach (var categoryToRestore in categoriesToRestore)
        {
            categoriesToDisplay += Environment.NewLine + "-" + categoryToRestore + Environment.NewLine;
        }

        bool accepted = await ShowMessage("Confirm Restore", "The following expense categories will be restored and will be included in the expense report." + categoriesToDisplay, "Restore", "Cancel");

        if (accepted)
        {
            await _navigation.PopAsync();            
            string category = categoriesToRestore.Count > 1 ? "categories" : "category";
            _navigation.Popups.ShowStatusAsync($"Restoring {category} and updating affected records (if any).");

            await Task.Delay(5000);

            CategoryTableDb.Restore(categoriesToRestore);

            foreach (var name in categoriesToRestore)
            {
                var expenses = await ExpenseTableDb.Where(x => x.Category == name);

                foreach (var expense in expenses)
                {
                    expense.IsCategoryDeleted = false;
                }

                ExpenseTableDb.UpdateAll(expenses);
            }

            WeakReferenceMessenger.Default.Send(new RestoreExpenseCategoryMessage("Name"));            
            _navigation.Popups.DismissStatus();
            ShowMessage("Restore Category Successful", AppResources.RestoreCategorySuccessful);
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
        for (int i = 0; i < DeletedCategories.Count; i++)
        {
            checkedFromCode = true;
            DeletedCategories[i].IsChecked = isChecked;
        }
        checkedFromCode = false;
    }

    [RelayCommand]
    private void ItemCheckBoxTapped(UiExpenseCategory selectedItem)
    {
        int checkCount = DeletedCategories.Where(x => x.IsChecked).Count();
        if (checkedFromCode)
        {
            return;
        }

        if (DeletedCategories.Count == checkCount)
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

    public async Task LoadCategories()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        ObservableCollection<UiExpenseCategory> deletedCategories = new ObservableCollection<UiExpenseCategory>();
        foreach (var category in DeletedCategoryNamesDb)
        {
            UiExpenseCategory uiExpenseCategory = new UiExpenseCategory();
            uiExpenseCategory.Name = category;
            deletedCategories.Add(uiExpenseCategory);
        }
        if (deletedCategories.Count > 0)
        {
            IsNoCategoriesToShowVisible = false;
            IsRestoreControlsVisible = true;
        }
        else
        {
            IsNoCategoriesToShowVisible = true;
            IsRestoreControlsVisible = false;
        }
        DeletedCategories = deletedCategories;
        IsSelectAllChecked = false;
        IsBusy = false;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Resources.Localization;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;
public partial class DeleteCategoryViewModel : BaseViewModel
{
    public DeleteCategoryViewModel()
    {
        EventMessengerManager.Register(this, OnAddExpenseCategory);
    }

    private void OnAddExpenseCategory(object recipient, AddExpenseCategoryMessage message)
    {
        LoadCategories();
    }

    [ObservableProperty]
    ObservableCollection<UiExpenseCategory> categories;

    [ObservableProperty]
    Color garbageIconTintColor = Colors.Gray;

    [ObservableProperty]
    bool isSelectAllChecked = false;

    [ObservableProperty]
    bool isNoCategoriesToShowVisible = false;

    [ObservableProperty]
    bool isDeleteControlsVisible = false;

    [RelayCommand]
    private async Task Delete()
    {
        List<string> categoriesToDelete = new List<string>();

        foreach (var item in Categories.Where(x => x.IsChecked).ToList())
        {
            categoriesToDelete.Add(item.Name);
        }

        if (categoriesToDelete.Count == 0)
        {
            await ShowMessage(string.Empty, "Select an item to delete.");
            return;
        }

        string categoriesToDisplay = Environment.NewLine;

        foreach (var categoryToDelete in categoriesToDelete)
        {
            categoriesToDisplay += Environment.NewLine + "-" + categoryToDelete + Environment.NewLine;
        }

        bool accepted = await ShowMessage("Confirm Delete", "The expense records for below categories will be soft deleted and will not be included in the expense report. It can be restored any time." + categoriesToDisplay, "Delete", "Cancel");

        if (accepted)
        {
            _navigation.PopAsync();
            _navigation.Popups.ShowStatusAsync("Deleting category and updating affected records (if any).");
            CategoryTableDb.Delete(categoriesToDelete);
            foreach (var name in categoriesToDelete)
            {
                var expenses = await ExpenseTableDb.Where(x => x.Category == name);

                foreach (var expense in expenses)
                {
                    expense.IsCategoryDeleted = true;
                }

                ExpenseTableDb.UpdateAll(expenses);
            }

            WeakReferenceMessenger.Default.Send(new DeleteExpenseCategoryMessage("CategoryName"));
            _navigation.Popups.DismissStatus();
            ShowMessage("Delete Category Successful", AppResources.DeleteCategorySuccessful);
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
        for (int i = 0; i < Categories.Count; i++)
        {
            checkedFromCode = true;
            Categories[i].IsChecked = isChecked;
        }
        checkedFromCode = false;
    }

    [RelayCommand]
    private void ItemCheckBoxTapped(UiExpenseCategory selectedItem)
    {
        int checkCount = Categories.Where(x => x.IsChecked).Count();
        if (checkedFromCode)
        {
            return;
        }

        if (Categories.Count == checkCount)
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

        ObservableCollection<UiExpenseCategory> categoriesToDelete = new ObservableCollection<UiExpenseCategory>();
        foreach (var category in CategoryNamesDb)
        {
            UiExpenseCategory uiExpenseCategory = new UiExpenseCategory();
            uiExpenseCategory.Name = category;
            categoriesToDelete.Add(uiExpenseCategory);
        }
        if (categoriesToDelete.Count > 0)
        {
            IsNoCategoriesToShowVisible = false;
            IsDeleteControlsVisible = true;
        }
        else
        {
            IsNoCategoriesToShowVisible = true;
            IsDeleteControlsVisible = false;
        }
        Categories = categoriesToDelete;
        IsSelectAllChecked = false;
        IsBusy = false;
    }
}

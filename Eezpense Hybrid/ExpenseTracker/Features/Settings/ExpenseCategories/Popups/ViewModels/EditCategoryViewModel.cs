using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Models;
using ExpenseTracker.Resources.Localization;
using Newtonsoft.Json;

namespace ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;

public partial class EditCategoryViewModel : BaseViewModel
{
    [ObservableProperty]
    private string categoryName;

    [ObservableProperty]
    string newCategoryName;

    public EditCategoryViewModel(string categoryName)
    {
        CategoryName = categoryName;
        NewCategoryName = categoryName;
    }


    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy)
            return;
        if (!IsEdited())
        {
            await ShowMessage(string.Empty, "You have not changed the category name.");
            return;
        }

        if (string.IsNullOrEmpty(NewCategoryName))
        {
            await ShowMessage(string.Empty, $"The field is blank.");
            return;
        }

        foreach (var item in CategoryNamesDb)
        {
            if (item.ToUpper() == NewCategoryName.ToUpper())
            {
                await ShowMessage(string.Empty, $"The category '{NewCategoryName}' already exists.");
                return;
            }
        }

        bool isConfirmed = true; //await ShowMessage("Confirm Edit", $"Confirm renaming category name from '{CategoryName}' to '{NewCategoryName}'?", "Confirm", AppResources.Cancel);
        IsBusy = true;
        if (isConfirmed)
        {
            await _navigation.Popups.DismissAsync();
            _navigation.Popups.ShowStatusAsync("Updating category and affected expense records (if any).");
            UpdateCategory();
        }
    }

    private async Task UpdateCategory()
    {   
        string oldName = CategoryName;
        var category = CategoriesDb.First(x => x.Name == CategoryName);

        string updateType = Enum.GetName(typeof(ExpenseCategoryUpdateType), ExpenseCategoryUpdateType.ReName);
        var history = category.History;

        List<ExpenseCategoryHistory> listOfHistory;
        if (string.IsNullOrWhiteSpace(history))
        {            
            listOfHistory = new List<ExpenseCategoryHistory>()
            {
                new ExpenseCategoryHistory
                {
                    Name = category.Name,
                    Date = DateTime.Now,
                    NewName = NewCategoryName,
                    UpdateType = updateType
                }
            };            
        }
        else
        {
            listOfHistory = JsonConvert.DeserializeObject<List<ExpenseCategoryHistory>>(history);
            var historyItem = new ExpenseCategoryHistory
            {
                Name = category.Name,
                Date = DateTime.Now,
                NewName = NewCategoryName,
                UpdateType = updateType
            };
            listOfHistory.Add(historyItem);
        }

        string historyJson = JsonConvert.SerializeObject(listOfHistory);

        category.History = historyJson;
        category.Name = NewCategoryName;
        CategoryTableDb.Update(category);
        var message = new EditedCategoryName(CategoryName, NewCategoryName);

        //Updating the affected expenses with the new category name.
        var expenses = await ExpenseTableDb.Where(x => x.Category == oldName);
        foreach (var expense in expenses)
        {
            expense.Category = NewCategoryName;
        }
        ExpenseTableDb.UpdateAll(expenses);              
        WeakReferenceMessenger.Default.Send(new EditExpenseCategoryMessage(message));
        _navigation.Popups.DismissStatus();
        ShowMessage("Edit Category Successful", AppResources.EditCategorySuccessful);
        IsBusy = false;
    }

    private bool IsEdited()
    {
        return !CategoryName.Equals(NewCategoryName, StringComparison.Ordinal);
    }

    [ObservableProperty]
    string editMessage = "Rename expense category.";

    [RelayCommand]
    private async Task Cancel()
    {
        await _navigation.Popups.DismissAsync();
    }

    [RelayCommand]
    private void Undo()
    {
        NewCategoryName = CategoryName;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;

public partial class AddCategoryViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrEmpty(CategoryName))
            return;

        foreach (var item in CategoryNamesDb)
        {
            if (item.ToUpper() == CategoryName.ToUpper())
            {
                await ShowMessage("Category Already Exists", $"The category '{CategoryName}' already exists.");
                return;
            }
        }

        bool isConfirmed = true; //await ShowMessage("Confirm Add Category", $"Are you sure you want to add category '{CategoryName}'?", "Confirm", AppResources.Cancel);
        IsBusy = true;
        if (isConfirmed)
        {
            ExpenseCategoryEntity category = new()
            {
                Name = CategoryName,                 
            };

            CategoryTableDb.Add(category);
            await _navigation.Popups.DismissAsync();
            WeakReferenceMessenger.Default.Send(new AddExpenseCategoryMessage(CategoryName));
        }
        IsBusy = false;
    }


    [RelayCommand]
    private async Task Cancel()
    {
        await _navigation.Popups.DismissAsync();
    }

    [ObservableProperty]
    string categoryName;
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Features.Settings.ExpenseCategories;
using ExpenseTracker.Resources.Localization;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Home.ViewModels;

public partial class CategoriesPopupViewModel : BaseViewModel
{
    public CategoriesPopupViewModel()
    {
        ExpenseCategories?.Clear();
        ObservableCollection<string> categories = new ObservableCollection<string>();
        foreach (var item in CategoriesDb)
        {
            categories.Add(item.Name);
        }
        categories.Add(AppResources.AddCategory);

        ExpenseCategories = categories;
    }

    [ObservableProperty]
    ObservableCollection<string> expenseCategories;

    [ObservableProperty]
    double lisViewHeight;

    [ObservableProperty]
    string selectedItem;

    [RelayCommand]
    private void OnItemSelected(string selectedItem)
    {
        if (selectedItem == AppResources.AddCategory)
        {
            MopupService.Instance.PopAsync();
            _navigation.Popups.ShowAsync(new AddCategoryPopup());
            return;
        }
        MopupService.Instance.PopAllAsync();
        WeakReferenceMessenger.Default.Send(new SelectedExpenseCategoryMessage(selectedItem));
    }
}

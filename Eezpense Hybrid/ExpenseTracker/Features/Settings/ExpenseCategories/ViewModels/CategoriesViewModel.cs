using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;

public partial class CategoriesViewModel : BaseViewModel
{
    [ObservableProperty]
    bool isListViewVisible = true;

    [ObservableProperty]
    bool isNoCategoriesToShowVisible;

    public CategoriesViewModel()
    {
        LoadData();
        WeakReferenceMessenger.Default.Register<EditExpenseCategoryMessage>(this, OnEditExpenseMessage);
        WeakReferenceMessenger.Default.Register<DeleteExpenseCategoryMessage>(this, OnDeleteExpenseCategory);
        WeakReferenceMessenger.Default.Register<AddExpenseCategoryMessage>(this, OnAddExpenseCategory);
        WeakReferenceMessenger.Default.Register<RestoreExpenseCategoryMessage>(this, OnRestoredExpenseCategory);
    }

    public void LoadData()
    {
        Categories.Clear();
        foreach (var item in CategoryNamesDb)
            Categories.Add(new ExpenseCategory { Name = item });
        IsNoCategoriesToShowVisible = CategoryNamesDb.Count == 0;
        IsListViewVisible = !IsNoCategoriesToShowVisible;
    }

    private void OnRestoredExpenseCategory(object recipient, RestoreExpenseCategoryMessage message)
    {
        LoadData();
    }

    private void OnAddExpenseCategory(object recipient, AddExpenseCategoryMessage message)
    {
        LoadData();
    }

    private void OnDeleteExpenseCategory(object recipient, DeleteExpenseCategoryMessage message)
    {
        LoadData();
    }

    private void OnEditExpenseMessage(object recipient, EditExpenseCategoryMessage message)
    {
        LoadData();
        Console.WriteLine($"*********Category Count: {Categories.Count}*************");
    }

    //Because ItemsSource has been sent to an array, the content will not update as the underlying list or array changes.
    //If you want the ListView to ////automatically update as items are added, removed and changed in the underlying list, you'll need to use an
    //ObservableCollection. ObservableCollection is defined in System.Collections.ObjectModel and is just like List, except
    //that it can notify ListView of any changes:

    //While a ListView will update in response to changes in its underlying ObservableCollection,
    //a ListView will not update if a different ObservableCollection instance is assigned to the
    //original ObservableCollection reference (e.g. employees = otherObservableCollection;).
    [ObservableProperty]
    ObservableCollection<ExpenseCategory> categories = new();

    [ObservableProperty]
    Color pageBackgroundColor;

    [ObservableProperty]
    string selectedItem;

    [RelayCommand]
    private void EditCategory(ExpenseCategory selectedCategory)
    {
        var page = new EditCategoryPopup();
        page.BindingContext = new EditCategoryViewModel(selectedCategory.Name);
        _navigation.Popups.ShowAsync(page);
    }

    [RelayCommand]
    private async Task AddCategory()
    {
        _navigation.Popups.ShowAsync(new AddCategoryPopup());
    }

    [RelayCommand]
    private async Task DeleteCategory()
    {
        _navigation.PushAsync(new DeleteCategoryPage());
    }

    [RelayCommand]
    private async Task RestoreCategory()
    {
        _navigation.PushAsync(new RestoreCategoryPage());
    }

    [RelayCommand]
    private void AcceptEdit(ExpenseCategory selectedCategory)
    {
        Categories.Clear();
        foreach (var item in CategoryNamesDb)
        {
            var category = new ExpenseCategory { Name = item };
            Categories.Add(category);
        }
    }

    [RelayCommand]
    private void ClearEdit(ExpenseCategory selectedCategory)
    {
        Categories.Clear();
        foreach (var item in CategoryNamesDb)
        {
            var category = new ExpenseCategory { Name = item };
            Categories.Add(category);
        }
    }
}

public class ExpenseCategory
{
    public string Name { get; set; }
    public bool IsEntryVisible { get; set; }
    public bool IsEditIconVisible { get; set; } = true;
    public bool IsCheckIconVisible { get; set; }
    public bool IsClearIconVisible { get; set; }
}

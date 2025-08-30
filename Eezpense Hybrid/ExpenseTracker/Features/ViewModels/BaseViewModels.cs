using CommunityToolkit.Mvvm.ComponentModel;
using ExpenseTracker.Database;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Resources.Localization;
using ExpenseTracker.Services;
using ExpenseTracker.Services.Navigation;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Features;

public abstract partial class BaseViewModel : ObservableObject
{
    public bool ShouldRefreshData { get; set; } = false;
    protected bool IsDarkTheme => App.IsDarkMode;
    protected NavigationService _navigation = new();
    protected bool isLoaded;

    protected CollectionChanged _collectionChanged = new();

    [ObservableProperty]
    bool isBusy;
    Page MainPage => Application.Current.MainPage;

    [ObservableProperty]
    string currencySymbol = AppSettings.Account.CurrencySymbol;

    [ObservableProperty]
    bool isNoRecordsToShowVisible = true;

    [ObservableProperty]
    bool isListVisible;

    public Action RefreshUI { get; set; }

    public async Task<string> DisplayActionSheet(string title, string cancel, string? destruction, params string[] buttons)
    {
        return await MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
    }

    ResourceDictionary resourceDictionary = App.Current.Resources;
    protected Color TextThemeColor
    {
        get
        {
            var color = (Color)resourceDictionary["TextColor"];
            return color;
        }
    }

    protected async Task<bool> CheckInternetConnection()
    {
        NetworkService service = new NetworkService();
        return await service.IsConnectedToInternet();
    }

    protected async Task<string> ShowActionSheet(string title, params string[] selections)
    {
        string selected = null;
        await MainThread.InvokeOnMainThreadAsync(async () => selected = await Application.Current.MainPage.
        DisplayActionSheet(title, AppResources.Cancel, null, selections));
        return selected;
    }

    protected async Task ShowMessage(string title, string message)
    {
        MessageBoxService service = new MessageBoxService();
        await service.ShowMessage(title, message);
    }

    protected async Task<bool> ShowMessage(string title, string message, string accept, string cancel)
    {
        MessageBoxService service = new MessageBoxService();
        bool isAccepted = await service.ShowMessage(title, message, accept, cancel);
        return isAccepted;
    }

    protected List<ExpenseEntity> DeletedExpensesDb => SqLiteDb.Instance.Expenses.GetAllDeleted();

    protected ExpenseTable ExpenseTableDb => SqLiteDb.Instance.Expenses;

    protected List<string> CategoryNamesDb =>
    SqLiteDb.Instance.Categories.GetAllNotDeleted().Count > 0
        ? SqLiteDb.Instance.Categories.GetAllNotDeleted().Select(x => x.Name).ToList()
        : new List<string>();

    protected List<string> DeletedCategoryNamesDb =>
        SqLiteDb.Instance.Categories.Where(x => x.IsDeleted).Count() > 0
            ? SqLiteDb.Instance.Categories.Where(x => x.IsDeleted).Select(x => x.Name).ToList()
            : new List<string>();

    /// <summary>
    /// Use to get all categories that are not deleted.
    /// </summary>
    protected List<ExpenseCategoryEntity> CategoriesDb => SqLiteDb.Instance.Categories.GetAllNotDeleted();

    /// <summary>
    /// Use for CRUD DB opearations
    /// </summary>
    protected ExpenseCategoryTable CategoryTableDb => SqLiteDb.Instance.Categories;

}

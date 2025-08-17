using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Models;
using ExpenseTracker.Settings;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.Settings.ViewModels;

public partial class ChangeCurrencyViewModel : BaseViewModel
{
    private MyCurrency _myCurrency = new MyCurrency();

    public ChangeCurrencyViewModel()
    {
        Initialize();
    }

    private void Initialize()
    {
        var symbol = AppSettings.Account.CurrencySymbol;
        var country = AppSettings.Account.Country;
        var currencies = _myCurrency.Currencies;
        var currency = currencies.FirstOrDefault(x => x.Country == country);
        if (currency == null)
        {
            SelectedCurrency = "Select Currency";
            HeaderInstruction = "Select your currency symbol.";
        }
        else
        {
            SelectedCurrency = currency.NameSymbol;
        }
    }

    private ObservableCollection<string> GetCurrencies()
    {
        ObservableCollection<string> result = new();
        var currencies = _myCurrency.Currencies;
        foreach (var item in currencies)
        {
            result.Add(item.NameSymbol);
        }
        return result;
    }

    public int Order { get; set; }
    public Action<string> CurrencySymbolChangeCallback { get; internal set; }

    [ObservableProperty]
    ObservableCollection<string> currencies;

    [ObservableProperty]
    string selectedCurrency = "";

    [ObservableProperty]
    ObservableCollection<string> currencyList = new ObservableCollection<string>();

    [ObservableProperty]
    ObservableCollection<string> categoryList = new ObservableCollection<string>();

    [ObservableProperty]
    string listCurrencyItemSelected;

    [ObservableProperty]
    string searchText;

    [ObservableProperty]
    bool isSearchEntryVisible;

    [ObservableProperty]
    bool isListViewVisible;

    [ObservableProperty]
    string headerInstruction = "Select your currency if the detected currency is incorrect. You can change this anytime in the app settings.";

    [RelayCommand]
    private void ShowCurrencyList()
    {
        IsSearchEntryVisible = true;
        IsListViewVisible = true;

        var allCurrencies = GetCurrencies();
        if (CurrencyList.Count == 0)
        {
            CurrencyList = allCurrencies;
        }
        else
        {
            if (CurrencyList.Count != allCurrencies.Count)
            {
                CurrencyList = allCurrencies;
            }
        }
    }

   [RelayCommand]
    private void CurrencyItemSelected()
    {
        IsListViewVisible = false;
        IsSearchEntryVisible = false;
        SelectedCurrency = ListCurrencyItemSelected;
    }

    [RelayCommand]
    private void SearchCompleted()
    {
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var result = CurrencyList.Where(c => c.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToObservableCollection();
            if (result.Count > 0)
            {
                CurrencyList = result;
                SearchText = "";
                return;
            }
        }
        CurrencyList = GetCurrencies();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var currency = _myCurrency.Currencies.First(x => x.NameSymbol == SelectedCurrency);
        AppSettings.Account.CurrencySymbol = currency.Symbol;
        AppSettings.Account.Country = currency.Country;
        _navigation.PopAsync();
        CurrencySymbolChangeCallback?.Invoke(currency.Symbol);
    }
}

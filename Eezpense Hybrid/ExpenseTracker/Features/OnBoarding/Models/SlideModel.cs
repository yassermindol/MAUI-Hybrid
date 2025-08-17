using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Models;
using ExpenseTracker.Settings;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Features.OnBoarding.Models;

public partial class SlideModel : BaseViewModel
{
    private MyCurrency _myCurrency = new MyCurrency();

    public SlideModel()
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
            SelectedCurrency = "";
            HeaderInstruction = "Select your currency. You can change the currency symbol anytime in the app settings. No amount conversion will be done, it is merely a symbol change.";
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
    string headerInstruction = "If the detected currency isn't correct, please select the appropriate one. You can change this at any time in the app settings. Note: No currency conversion will be performed; this is only a symbol change.";

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
        var currency = _myCurrency.Currencies.First(x => x.NameSymbol == SelectedCurrency);
        AppSettings.Account.CurrencySymbol = currency.Symbol;
        AppSettings.Account.Country = currency.Country;
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
}
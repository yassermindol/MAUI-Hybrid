using ExpenseTracker.Features.Settings.ViewModels;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Features.Settings;

public partial class SettingsPage : ContentPage
{
    SettingsViewModel vm = new SettingsViewModel();
    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = vm;
        vm.CurrentCurrency = AppSettings.Account.CurrencySymbol;
    }
}
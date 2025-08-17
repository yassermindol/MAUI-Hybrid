using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.Features.Settings.ExpenseCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Features.Settings.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        string currentCurrency;

        [RelayCommand]
        private async Task Categories()
        {
            await _navigation.PushAsync(new CategoriesPage());
        }

        private async Task Profile()
        {
            await _navigation.PushAsync(new ProfilePage());
        }

        [RelayCommand]
        private async Task About()
        {
            await _navigation.PushAsync(new AboutAppPage());
        }

        [RelayCommand]
        private async Task RestoreExpenses()
        {
            await _navigation.PushAsync(new RestoreExpensePage());
        }

        [RelayCommand]
        private async Task ChangeCurrencySymbol()
        {
            ChangeCurrencyViewModel viewModel = new ChangeCurrencyViewModel();
            viewModel.CurrencySymbolChangeCallback = OnCurrencySymbolChanged;
            ChangeCurrencyPage page = new ChangeCurrencyPage();
            page.BindingContext = viewModel;
            await _navigation.PushAsync(page);
        }

        private void OnCurrencySymbolChanged(string newSymbol)
        {
            CurrentCurrency = newSymbol;
            WeakReferenceMessenger.Default.Send(new CurrencySymbolChangedMessage(newSymbol));
        }

        private async Task Signout()
        {
            await ShowMessage("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
        }
    }
}

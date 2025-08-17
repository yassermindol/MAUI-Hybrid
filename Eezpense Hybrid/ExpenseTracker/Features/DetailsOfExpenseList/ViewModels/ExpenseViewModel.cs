using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Resources.Localization;

namespace ExpenseTracker.Features.DetailsOfExpenseList.ViewModels
{
    public partial class ExpenseViewModel : BaseViewModel
    {
        ExpenseEntity _expenseEntity;
        public ExpenseViewModel(ExpenseEntity expenseEntity)
        {
            _expenseEntity = expenseEntity;
        }

        [ObservableProperty]
        string dateTime;

        [ObservableProperty]
        string category;

        [ObservableProperty]
        string amount;

        [ObservableProperty]
        string notes;

        [RelayCommand]
        private async Task Delete()
        {
            bool isAccepted = await ShowMessage(AppResources.ConfirmEdit,
                AppResources.AreYouSureYouWantToDeleteThisRecord, AppResources.Delete, AppResources.Cancel);
            if (isAccepted)
            {
                IsBusy = true;
                //_navigation.Popups.ShowStatus(AppResources.DeletingExpenseRecord); //causes crash when not running in visual studio
                ExpenseTableDb.SoftDelete(_expenseEntity);
                WeakReferenceMessenger.Default.Send(new DeleteExpenseMessage(_expenseEntity));
                //_navigation.Popups.DismissStatus();
                IsBusy = false;
                await _navigation.PopAsync();
            }
        }
        [RelayCommand]
        private async Task Edit()
        {
            Page page = new EditExpensePage();
            page.BindingContext = new EditExpenseViewModel(_expenseEntity);
            await _navigation.PushAsync(page);
        }

        public async Task LoadData()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                var item = _expenseEntity;
                DateTime = item.Date.ToString("MMM dd, yyyy @ hh:mm: tt");
                Category = item.Category;
                Amount = item.Amount.ToMoney();
                Notes = item.Note;
                IsBusy = false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

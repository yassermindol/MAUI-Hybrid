using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Database;
using ExpenseTracker.EventMessages;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Resources.Localization;
using ExpenseTracker.Services;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Features.DetailsOfExpenseList.ViewModels;
public partial class EditExpenseViewModel : BaseViewModel
{

    ExpenseEntity _expenseEntity;
    public EditExpenseViewModel(ExpenseEntity expenseEntity)
    {
        _expenseEntity = expenseEntity;
    }

    [ObservableProperty]
    private List<string> pickerSource = new List<string>();

    [ObservableProperty]
    string selectedCategory;

    [ObservableProperty]
    double amount;

    [ObservableProperty]
    string notes;

    [ObservableProperty]
    DateTime selectedDate;

    [ObservableProperty]
    TimeSpan selectedTime;

    [ObservableProperty]
    string currencySymbol;

    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy)
            return;

        bool isAccepted = await ShowMessage(AppResources.ConfirmEdit,
            AppResources.AreYouSureYouWantToSaveTheChanges, AppResources.Save, AppResources.Cancel);

        if (isAccepted)
        {
            IsBusy = true;
            var expense = _expenseEntity;
            expense.Date = SelectedDate + SelectedTime;
            expense.Category = SelectedCategory;
            expense.Amount = Amount;
            expense.Note = Notes;
            //_navigation.Popups.ShowStatus(AppResources.UpdatingExpenseRecord);
            Update(expense);
            WeakReferenceMessenger.Default.Send(new EditExpenseMessage(expense));
            //_navigation.Popups.DismissStatus();
            IsBusy = false;
            await _navigation.PopAsync();
        }
    }

    public void LoadData()
    {
        SelectedDate = _expenseEntity.Date;
        SelectedTime = _expenseEntity.Date.TimeOfDay;
        PickerSource = CategoryNamesDb;
        SelectedCategory = PickerSource.First(item => item == _expenseEntity.Category);
        Amount = _expenseEntity.Amount;
        Notes = _expenseEntity.Note;
        CurrencySymbol = AppSettings.Account.CurrencySymbol;
    }

    private void Update(ExpenseEntity entity)
    {
        CalendarService calendarService = new();
        var category = CategoriesDb.First(x => x.Name == entity.Category);
        entity.CategoryLocalID = category.ID;
        entity.CategoryCentralID = category.CentralID;
        int weekNumber = calendarService.GetWeekOfYear(entity.Date).Number;
        entity.WeekNumber = weekNumber;
        ExpenseTableDb.Update(entity);
        WeakReferenceMessenger.Default.Send(new AddExpenseMessage(entity));
    }
}

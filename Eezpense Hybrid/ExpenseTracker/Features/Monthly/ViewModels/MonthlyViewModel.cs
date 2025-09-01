using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.DetailsOfSummaryReport;
using ExpenseTracker.Features.DetailsOfSummaryReport.ViewModels;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Services.Api;
using ExpenseTracker.Settings;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ExpenseTracker.Features.Monthly.ViewModels
{
    public partial class MonthlyViewModel : BaseViewModel
    {
        protected SummaryDetailsViewModel _summaryDetailsViewModel;
        string _strSelectedMonth;
        string[] _yearSelections = new string[12];
        MonthlyService service = new MonthlyService();

        public MonthlyViewModel()
        {
            WeakReferenceMessenger.Default.Register<AddExpenseMessage>(this, OnNewExpenseSaved);
            WeakReferenceMessenger.Default.Register<EditExpenseCategoryMessage>(this, OnEditedExpenseCategory);
            WeakReferenceMessenger.Default.Register<DeleteExpenseCategoryMessage>(this, OnDeleteExpenseCategory);
            WeakReferenceMessenger.Default.Register<RestoreExpenseCategoryMessage>(this, OnRestoredExpenseCategory);
            WeakReferenceMessenger.Default.Register<EditExpenseMessage>(this, OnEditExpense);
            WeakReferenceMessenger.Default.Register<DeleteExpenseMessage>(this, OnDeleteExpense);
            WeakReferenceMessenger.Default.Register<RestoreExpenseMessage>(this, OnRestoreExpense);
            WeakReferenceMessenger.Default.Register<CurrencySymbolChangedMessage>(this, OnCurrencySymbolChanged);
        }

        private void OnCurrencySymbolChanged(object recipient, CurrencySymbolChangedMessage message)
        {
            string symbol = message.Value;
            foreach (var item in MonthItems)
                item.CurrencySymbol = symbol;

            if (_summaryDetailsViewModel != null)
                _summaryDetailsViewModel.RefreshCurrencySymbol(symbol);
        }

        private void SetDetailsOfSummaryShouldRefresh()
        {
            if (_summaryDetailsViewModel != null)
                _summaryDetailsViewModel.ShouldRefreshData = true;
        }

        private void OnRestoreExpense(object recipient, RestoreExpenseMessage message)
        {
            ShouldRefreshData = true;
            SetDetailsOfSummaryShouldRefresh();
        }

        private void OnDeleteExpense(object recipient, DeleteExpenseMessage message)
        {
            ShouldRefreshData = true;
            SetDetailsOfSummaryShouldRefresh();
        }

        private void OnEditExpense(object recipient, EditExpenseMessage message)
        {
            ShouldRefreshData = true;
            SetDetailsOfSummaryShouldRefresh();
        }

        private void OnRestoredExpenseCategory(object recipient, RestoreExpenseCategoryMessage message)
        {
            ShouldRefreshData = true;
            SetDetailsOfSummaryShouldRefresh();
        }

        private void OnDeleteExpenseCategory(object recipient, DeleteExpenseCategoryMessage message)
        {
            ShouldRefreshData = true;
            SetDetailsOfSummaryShouldRefresh();
        }

        private void OnEditedExpenseCategory(object recipient, EditExpenseCategoryMessage message)
        {
            ShouldRefreshData = true;
            SetDetailsOfSummaryShouldRefresh();
        }

        private void OnNewExpenseSaved(object recipient, AddExpenseMessage message)
        {
            ShouldRefreshData = true;
            SetDetailsOfSummaryShouldRefresh();
        }

        [ObservableProperty]
        private double width = AppConstants.BoxViewMaxWidth;


        [ObservableProperty]
        ObservableCollection<UiMonthItem> monthItems = new ObservableCollection<UiMonthItem>();

        [ObservableProperty]
        string selectedYear = DateTime.Now.Year.ToString();

        [ObservableProperty]
        UiMonthItem selectedMonth;

        [ObservableProperty]
        bool isNoRecordsToShowVisible;

        [ObservableProperty]
        bool isListVisible;

        public Color PageBackgroundColor { get; set; }

        bool isCLicked;
        [RelayCommand]
        private async Task MonthSelectedAsync()
        {
            if (SelectedMonth == null)
                return;

            if (isCLicked)
                return;
            isCLicked = true;
            var monthItem = SelectedMonth;            
            var expenses = monthItem.Expenses.ToList();
            var viewModel = new SummaryDetailsViewModel(expenses, monthItem.StartDate, monthItem.EndDate);
            viewModel.TitlePartial = $"{monthItem.Month}";
            viewModel.AmountStr = expenses.Sum(x => x.Amount).ToMoney();
            viewModel.IsDateRangeVisible = false;
            SummaryDetailsPage page = new();
            page.BindingContext = viewModel;
            _summaryDetailsViewModel = viewModel;
            Type runtimeType = this.GetType();
            string name = runtimeType.Name;
            DiContainerForRazor.RegisterViewModel(name, viewModel);
            await _navigation.PushAsync(page);          
            _strSelectedMonth = SelectedMonth.Month;
            SelectedMonth = null;
            isCLicked = false;
        }

        [RelayCommand]
        private async Task SelectYear()
        {
            int currentYear = DateTime.Now.Year;
            int past5years = currentYear - 5;

            for (int i = 0; i < 10; i++)
            {
                past5years++;
                _yearSelections[i] = past5years.ToString();
            }
            string year = await ShowActionSheet("Select Year", _yearSelections);
            if (year == null)
                return;
            if (year == "Cancel")
                return;
            SelectedYear = year;
            LoadDataAsync();
        }

        private void Busy()
        {
            IsBusy = true;
            IsListVisible = false;
            IsNoRecordsToShowVisible = false;
        }

        private void NotBusy()
        {
            IsBusy = false;
            IsListVisible = MonthItems.Count > 0;
            IsNoRecordsToShowVisible = MonthItems.Count == 0;
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy)
                return;
            Busy();
            double total = 0;
            var items = service.GetMonthItems(int.Parse(SelectedYear), out total);
            //foreach (var item in items)
            //item.ItemColor = PageBackgroundColor;
            MonthItems.Clear();
            _collectionChanged.Monitor(MonthItems, items.Count, NotBusy, RefreshUI);
            foreach (var item in items)
                MonthItems.Add(item);
        }

        public async Task ReloadDataIfShouldAsync()
        {
            if (ShouldRefreshData)
            {
                await LoadDataAsync();
                ShouldRefreshData = false;
            }
        }
    }
}

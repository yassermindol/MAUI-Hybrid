using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Features;

namespace ExpenseTracker.PopupViews.SelectDateRange
{
    public partial class SelectDateRangeViewModel : BaseViewModel
    {
        Action<DateTime, DateTime> submit;
        public SelectDateRangeViewModel(Action<DateTime, DateTime> submit)
        {
            this.submit = submit;
        }

        [ObservableProperty]
        DateTime selectedStartDate;

        [ObservableProperty]
        DateTime selectedEndDate;

        [RelayCommand]
        private void Submit()
        {
            if (IsDateRangeValid())
            {
                var end = SelectedEndDate;
                SelectedEndDate = end.Date.AddDays(1).AddSeconds(-1); //sets it to 23:59:59.999 of that day (with milliseconds for more precision).
                submit(SelectedStartDate, SelectedEndDate);
            }               
        }

        private bool IsDateRangeValid()
        {
            if (SelectedStartDate > SelectedEndDate)
            {
                ShowMessage("Invalid Date", "The selected start date should be earlier than the end date. Please check your inputs.");
                return false;
            }
            return true;
        }
    }
}

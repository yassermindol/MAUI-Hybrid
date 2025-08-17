using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Features;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.PopupViews.SelectDateTime;
    
public partial class SelectDateTimeViewModel : BaseViewModel
{
    Action<DateTime, TimeSpan> _onDateTimeSelected;

    public SelectDateTimeViewModel(Action<DateTime, TimeSpan> onDateTimeSelected)
    {
        _onDateTimeSelected = onDateTimeSelected;
    }

    [ObservableProperty]
    public DateTime selectedDate = DateTime.Now.Date;

    [ObservableProperty]
    public TimeSpan selectedTime = DateTime.Now.TimeOfDay;

    [RelayCommand]
    private void Done()
    {
        _onDateTimeSelected(SelectedDate, SelectedTime);
        MopupService.Instance.PopAsync();
    }

    [RelayCommand]
    private void Cancel()
    {
        MopupService.Instance.PopAsync();
    }

    [RelayCommand]
    private void Reset()
    {
        SelectedDate = DateTime.Now.Date;
        SelectedTime = DateTime.Now.TimeOfDay;
        _onDateTimeSelected(DateTime.MinValue, TimeSpan.MinValue);
    }
}

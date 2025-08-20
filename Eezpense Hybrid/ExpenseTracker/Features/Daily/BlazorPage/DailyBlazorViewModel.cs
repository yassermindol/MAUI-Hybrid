

namespace ExpenseTracker.Features.Daily.BlazorPage;

public class DailyBlazorViewModel: DailyViewModel
{
    public event Action OnDateChanged;

    public DatePicker DatePicker { get; internal set; }

    DateTime _selectedDate = DateTime.Today;
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            SetProperty(ref _selectedDate, value);
            OnDateChanged?.Invoke();
        } 
    }
}

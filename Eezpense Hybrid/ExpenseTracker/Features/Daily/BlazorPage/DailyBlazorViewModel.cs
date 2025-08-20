using System.ComponentModel;

namespace ExpenseTracker.Features.Daily.BlazorPage;

public class DailyBlazorViewModel : DailyViewModel
{
    public event Action StateHasChanged;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectDateButtonText))
        {
            StateHasChanged?.Invoke();
        }
    }
}

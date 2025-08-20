using ExpenseTracker.Features.Daily.BlazorPage;

namespace ExpenseTracker.Features.Daily;

public partial class DailyPageV2 : ContentPage
{
	DailyBlazorViewModel _viewModel;
	public DailyPageV2(DailyBlazorViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		_viewModel.DatePicker = HiddenDatePicker;
        HiddenDatePicker.DateSelected += HiddenDatePicker_DateSelected;
    }

    private void HiddenDatePicker_DateSelected(object? sender, DateChangedEventArgs e)
    {
		_viewModel.SelectedDate = e.NewDate;
		Console.WriteLine("******************* chate *********************");
    }
}
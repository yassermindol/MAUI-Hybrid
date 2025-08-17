using ExpenseTracker.Features.Weekly.ViewModels;

namespace ExpenseTracker.Features.Weekly;

public partial class WeeksInMonthPage : ContentPage
{
    WeeksInMonthViewModel _viewModel = new();
    public WeeksInMonthPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;        
        _viewModel.RefreshUI = RefreshUi;
        _viewModel.LoadDataAsync();
    }

    protected override void OnAppearing()
    {  
        base.OnAppearing();
        _viewModel.ReloadDataIfShouldAsync();
    }

    private void RefreshUi()
    {
        InvalidateMeasure();
    }
}
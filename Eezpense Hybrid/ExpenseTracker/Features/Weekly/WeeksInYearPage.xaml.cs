using ExpenseTracker.Features.Weekly.ViewModels;

namespace ExpenseTracker.Features.Weekly;

public partial class WeeksInYearPage : ContentPage
{
    WeeksInYearViewModel _viewModel = new();
    public WeeksInYearPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        _viewModel.RefreshUI = RefreshUI;
        Task.Run(_viewModel.LoadDataAsync);
    }

    private void RefreshUI()
    {
        InvalidateMeasure();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ReloadDataIfShouldAsync();
    }
}
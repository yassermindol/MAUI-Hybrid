using ExpenseTracker.Features.Monthly.ViewModels;

namespace ExpenseTracker.Features.Monthly;

public partial class MonthlyPage : ContentPage
{
    MonthlyViewModel _viewModel = new();

    public MonthlyPage()
    {
        InitializeComponent();
        _viewModel.PageBackgroundColor = BackgroundColor;
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
        //Console.WriteLine("*************** MonthlyPage OnAppearing");
        base.OnAppearing();
        _viewModel.ReloadDataIfShouldAsync();
    }
}
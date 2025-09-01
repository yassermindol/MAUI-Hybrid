using ExpenseTracker.Features.DateRange.ViewModels;
using ExpenseTracker.Helpers;
using ExpenseTracker.Resources.Localization;

namespace ExpenseTracker.Features.DateRange;

public partial class DateRangePage : ContentPage
{
    DateRangeViewModel _viewModel = new();
    ExpenseListSortToolbarManager _sortToolbarManager;

    public DateRangePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        DiContainerForRazor.RegisterViewModel(nameof(DateRangePage), _viewModel);
        _sortToolbarManager = new ExpenseListSortToolbarManager(ToolbarItems);
        _viewModel.RefreshUI = RefreshUI;
        _viewModel.InvalidateOxyPlot = InvalidateOxyPlot;
        Task.Run(_viewModel.LoadDataAsync);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();        
        _viewModel.ReloadDataIfShouldAsync();
    }

    private void InvalidateOxyPlot()
    {
        oxyplot.InvalidatePlot();
    }

    private void RefreshUI()
    {
        InvalidateMeasure();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is DateRangeViewModel vm)
        {
            ShowChart(); //default toolbar item
        }
    }

    private void ShowList()
    {
        ToolbarItems.Clear();
        ToolbarItems.Add(new ToolbarItem
        {
            Text = AppResources.ViewChart,
            Command = new Command(ShowChart)
        });
        _sortToolbarManager.AddSortToolbar(ToolbarItems, _viewModel);

        var searchToolbarItem = new ToolbarItem();
        searchToolbarItem.Priority = 0;
        searchToolbarItem.Order = ToolbarItemOrder.Secondary;
        searchToolbarItem.IconImageSource = "ic_search_black";
        searchToolbarItem.Text = "Search List";
        searchToolbarItem.Clicked += OnSearchClicked;
        ToolbarItems.Add(searchToolbarItem);

        _viewModel.ViewTypeText = AppResources.ViewChart;
        _viewModel.IsPieChartVisible = false;
        _viewModel.IsListVisible = true;
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
        _viewModel.OpenSearchPageAsync();
    }

    private void ShowChart()
    {
        ToolbarItems.Clear();
        ToolbarItems.Add(new ToolbarItem
        {
            Text = AppResources.ViewList,
            Command = new Command(ShowList)
        });
        _viewModel.ViewTypeText = AppResources.ViewList;
        _viewModel.IsPieChartVisible = true;
        _viewModel.IsListVisible = false;
    }
}
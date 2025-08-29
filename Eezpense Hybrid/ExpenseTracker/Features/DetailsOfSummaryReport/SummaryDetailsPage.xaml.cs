using ExpenseTracker.Features.DetailsOfSummaryReport.ViewModels;
using ExpenseTracker.Resources.Localization;

namespace ExpenseTracker.Features.DetailsOfSummaryReport;

public partial class SummaryDetailsPage
{
    SummaryDetailsViewModel _viewModel;
    ExpenseListSortToolbarManager _sortToolbarManager;

    public SummaryDetailsPage()
    {
        InitializeComponent();
        _sortToolbarManager = new ExpenseListSortToolbarManager(ToolbarItems);
    }

    private void RefreshUI()
    {
        InvalidateMeasure();
    }

    protected override void OnAppearing()
    {
        Console.WriteLine("*************** SummaryDetailsPage OnAppearing");
        base.OnAppearing();
        _viewModel.ReloadDataIfShouldAsync();        
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is SummaryDetailsViewModel vm)
        {
            _viewModel = vm;
            _viewModel.InvalidateOxyPlot = InvalidatePlot;
            _viewModel.RefreshUI = RefreshUI;
            _viewModel.LoadDataAsync();
            ShowChartToolbarItem();
        }
    }

    private void InvalidatePlot()
    {
        oxyplot.InvalidatePlot();
    }

    private void ShowListToolbarItem()
    {
        ToolbarItems.Clear();

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "View Chart",
            Command = new Command(ShowChartToolbarItem)
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
        _viewModel.IsListVisible = true;
        _viewModel.IsPieChartVisible = false;
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
        _viewModel.OpenSearchPageAsync();
    }

    private void ShowChartToolbarItem()
    {
        ToolbarItems.Clear();
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "View List",
            Command = new Command(ShowListToolbarItem)
        });

        _viewModel.ViewTypeText = AppResources.ViewList;
        _viewModel.IsListVisible = false;
        _viewModel.IsPieChartVisible = true;
    }
}
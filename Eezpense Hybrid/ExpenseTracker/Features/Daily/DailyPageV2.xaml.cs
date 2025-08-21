using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.Daily.BlazorPage;

namespace ExpenseTracker.Features.Daily;

public partial class DailyPageV2 : ContentPage
{
    DailyBlazorViewModel _viewModel;
    public DailyPageV2(DailyBlazorViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        SetToolbarItems(); ;
    }

    private void SetToolbarItems()
    {
        ToolbarItem item1 = new ToolbarItem
        {
            Priority = 0,
            Order = ToolbarItemOrder.Secondary,
            Command = new Command(() => ShowSubItems(false))
        };

        ToolbarItems.Add(item1);
        ToolbarItem item2 = item1.Clone();
        item2.Command = new Command(() => ShowSubItems(true));
        ToolbarItems.Add(item2);
        SetActiveToolbarItems();
    }

    private void SetActiveToolbarItems()
    {
        if (_viewModel.IsShowSubItems)
        {
            ToolbarItems[0].Text = "Hide sub items";
            ToolbarItems[1].Text = "Show sub items (✓)";

        }
        else
        {
            ToolbarItems[0].Text = "Hide sub items (✓)";
            ToolbarItems[1].Text = "Show sub items";
        }
    }

    private void ShowSubItems(bool showSubItems)
    {
        _viewModel.IsShowSubItems = showSubItems;
        SetActiveToolbarItems();
        Task.Run(_viewModel.LoadDataAsync);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ReloadDataIfShouldAsync();
    }
}
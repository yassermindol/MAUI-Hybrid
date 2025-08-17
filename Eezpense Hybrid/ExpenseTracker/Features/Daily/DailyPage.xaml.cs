
using AndroidX.Lifecycle;
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Models;
using ExpenseTracker.Settings;
using Microsoft.Maui.Controls;

namespace ExpenseTracker.Features.Daily;

public partial class DailyPage : ContentPage
{
    DailyViewModel _viewModel = new();
    public DailyPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
        _viewModel.RefreshUI = RefreshUI;
        _viewModel.LoadDataAsync();
        SetToolbarItems();
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

    private void RefreshUI()
    {
        listview.BeginRefresh();
        Task.Delay(50).Wait();
        listview.EndRefresh();
        InvalidateMeasure();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ReloadDataIfShouldAsync();
    }
}
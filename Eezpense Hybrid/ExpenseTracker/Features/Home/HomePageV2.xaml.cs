using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.Features.Home.ViewModels;
using ExpenseTracker.Models.UI;

namespace ExpenseTracker.Features.Home;

public partial class HomePageV2
{
    HomeViewModeV2 _viewModel;
    ExpenseListSortToolbarManager _sortToolbarManager;

    public HomePageV2(HomeViewModeV2 viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        _viewModel.SaveExpenseDelegate = OnExpenseSave;
        _viewModel.NoteCompletedDelegate = OnNoteCompleted;
        _viewModel.AnimateClickDelegate = OnAnimateClick; ;
        BindingContext = _viewModel;
        _sortToolbarManager = new ExpenseListSortToolbarManager(ToolbarItems);
        SetToolbar();
        _viewModel.RefreshUI = RefreshUI;
        _viewModel.LoadDataAsync();
    }


    private void MeasureDistance(object? sender, EventArgs e)
    {
        // Get the positions of the elements
        var topUiPosition = topUi.Bounds; // or use other methods
        var bottomUiPosition = bottomSheetView.Bounds;

        // Calculate the vertical distance
        double distance = Math.Abs(bottomUiPosition.Y - topUiPosition.Y);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        WeakReferenceMessenger.Default.Unregister<KeyboardVisibilityMessage>(this);
        WeakReferenceMessenger.Default.Register<KeyboardVisibilityMessage>(this, HandleKeyboardVisibility);
        _viewModel.ReloadDataIfShouldAsync();
    }

    private void HandleKeyboardVisibility(object recipient, KeyboardVisibilityMessage message)
    {
        bool isKeyBoardVisible = message.Value;
        //bottomSheetViewHeader.IsEnabled = !isKeyBoardVisible; // causes issue on my huawei phone, so disabling it for now. coz isKeyBoardVisible=true eventhough keyboard is not physically visible on the phone.
        if (isKeyBoardVisible)
            _viewModel.IsKeyboardVisible = isKeyBoardVisible;
        else
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                _viewModel.IsKeyboardVisible = isKeyBoardVisible;
            });
    }

    private void RefreshUI()
    {
        InvalidateMeasure();
    }

    private void SetToolbar()
    {
        var searchToolbarItem = new ToolbarItem();
        searchToolbarItem.IconImageSource = App.IsDarkMode ? "ic_search" : "ic_search_black_16x16";
        searchToolbarItem.Clicked += OnSearchClicked;
        ToolbarItems.Add(searchToolbarItem);
        _sortToolbarManager.AddSortToolbar(ToolbarItems, _viewModel);
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
        _viewModel.OpenSearchPageAsync();
    }

    private async Task OnAnimateClick()
    {
        var originalColor = clickableStack.BackgroundColor;
        var targetColor = Colors.LightBlue;
        await clickableStack.FadeTo(0.5, 100);
        clickableStack.BackgroundColor = targetColor;
        await clickableStack.FadeTo(1, 100);
        clickableStack.BackgroundColor = originalColor;
    }

    private void OnNoteCompleted()
    {
        noteTextField.Unfocus();
    }

    private void OnExpenseSave(UiExpenseItem item)
    {
        InvalidateMeasure();
    }

    protected override void OnDisappearing()
    {
        WeakReferenceMessenger.Default.Unregister<KeyboardVisibilityMessage>(this);
    }
}
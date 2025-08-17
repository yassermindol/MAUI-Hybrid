using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.Features.Home.ViewModels;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Features.Home;

public partial class HomePage
{
    HomeViewModel _viewModel = new();
    ExpenseListSortToolbarManager _sortToolbarManager;

    public HomePage()
    {
        InitializeComponent();
        _viewModel.SaveExpenseDelegate = OnExpenseSave;
        _viewModel.NoteCompletedDelegate = OnNoteCompleted;
        _viewModel.AnimateClickDelegate = OnAnimateClick; ;
        BindingContext = _viewModel;
        _sortToolbarManager = new ExpenseListSortToolbarManager(ToolbarItems);
        SetToolbar();
        _viewModel.RefreshUI = RefreshUI;
        _viewModel.LoadDataAsync();
        //SizeChanged += MeasureDistance;
        //expenseListView.ItemAppearing += ExpenseListView_ItemAppearing;
    }

    private void ExpenseListView_ItemAppearing(object? sender, ItemVisibilityEventArgs e)
    {
        Console.WriteLine($"************ Expense Listview Height:{expenseListView.Height}");
    }

    private void MeasureDistance(object? sender, EventArgs e)
    {
        // Get the positions of the elements
        var topUiPosition = topUi.Bounds; // or use other methods
        var bottomUiPosition = bottomSheetView.Bounds;

        // Calculate the vertical distance
        double distance = Math.Abs(bottomUiPosition.Y - topUiPosition.Y);

        // Optionally, do something with the distance
        Console.WriteLine($"Distance: {distance}");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        WeakReferenceMessenger.Default.Unregister<KeyboardVisibilityMessage>(this);
        WeakReferenceMessenger.Default.Register<KeyboardVisibilityMessage>(this, HandleKeyboardVisibility);
        _viewModel.ReloadDataIfShouldAsync();

        var screenHeight = DeviceDisplay.Current.MainDisplayInfo.Height;
        Console.WriteLine($"**************** Screen Height: {screenHeight}");
        if (screenHeight == 1600)
        {
            expenseListView.HeightRequest = 560;
        }
        else
        {
            double height = (560 * screenHeight) / 1600; // using ratio and proportion my phone as reference.
            expenseListView.HeightRequest = height - 200; 
        }
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
        expenseListView.ScrollTo(item, position: ScrollToPosition.MakeVisible, animated: false);
    }

    protected override void OnDisappearing()
    {
        WeakReferenceMessenger.Default.Unregister<KeyboardVisibilityMessage>(this);
        AppConstants.BoxViewMaxWidth = expenseListView.Width;
    }
}
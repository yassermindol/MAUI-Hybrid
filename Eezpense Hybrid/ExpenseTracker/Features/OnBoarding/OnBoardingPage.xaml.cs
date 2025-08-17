using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.Features.OnBoarding;

public partial class OnBoardingPage : ContentPage
{
    private OnBoardingViewModel _viewModel = new OnBoardingViewModel();
    public OnBoardingPage()
    {
        InitializeComponent();
        BindingContext = new OnBoardingViewModel(); ;

        // Assign the count to IndicatorView
        MyIndicator.Count = 3;

        // Handle position changes
        MyCarousel.PositionChanged += (s, e) =>
        {
            MyIndicator.Position = e.CurrentPosition;
        };

        // Optional: Sync IndicatorView with CarouselView's position
        MyIndicator.Position = MyCarousel.Position;
    }

    private void OnNextClicked(object sender, EventArgs e)
    {
        if (MyCarousel.Position < _viewModel.Slides.Count - 1)
        {
            MyCarousel.Position += 1; // Move to next item
        }
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
        if (MyCarousel.Position > 0)
        {
            MyCarousel.Position -= 1; // Move to previous item
        }
    }

    private void Search_Entry_Focused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry)
        {
        }
    }
    private void Category_Entry_Focused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry)
        {
            MyCarousel.ScrollTo(2); // fix the weird behavior when category entry is clicked the carousel auto scroll back.
        }
    }
}
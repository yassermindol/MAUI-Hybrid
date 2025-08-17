using ExpenseTracker.Services.Navigation;

namespace ExpenseTracker.Features.Test;

public partial class TestPage : ContentPage
{
	NavigationService navigation = new NavigationService();
	public TestPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked_Show(object sender, EventArgs e)
    {
		navigation.Popups.ShowStatusAsync("Updating category and affected records...");
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}
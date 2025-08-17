using ExpenseTracker.Features.DetailsOfExpenseList.ViewModels;
using ExpenseTracker.Services.Navigation;

namespace ExpenseTracker.Features.DetailsOfExpenseList;

public partial class ExpensePage : ContentPage
{
	public ExpensePage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ExpenseViewModel).LoadData();
    }

    protected override bool OnBackButtonPressed() // Di gumagana
    {
        NavigationService navigation = new NavigationService();
        navigation.PopAsync();
        return false;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
    }
}
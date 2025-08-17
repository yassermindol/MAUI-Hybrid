using ExpenseTracker.Features.DetailsOfExpenseList.ViewModels;

namespace ExpenseTracker.Features.DetailsOfExpenseList;

public partial class EditExpensePage : ContentPage
{
	public EditExpensePage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as EditExpenseViewModel).LoadData();
    }
}
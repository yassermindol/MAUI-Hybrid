using ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;

namespace ExpenseTracker.Features.Settings.ExpenseCategories;

public partial class AddCategoryPopup
{
	AddCategoryViewModel viewModel = new();
	public AddCategoryPopup()
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
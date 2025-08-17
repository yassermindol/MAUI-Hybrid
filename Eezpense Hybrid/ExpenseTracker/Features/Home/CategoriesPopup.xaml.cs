using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.Home.ViewModels;

namespace ExpenseTracker.Features.Home;

public partial class CategoriesPopup
{
	CategoriesPopupViewModel viewModel = new();
	public CategoriesPopup()
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		//SetListViewHeight();
	}

	private void SetListViewHeight()
	{
		expenseCategoryListView.SetHeight(viewModel.ExpenseCategories.Count);
	}
}
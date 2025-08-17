using ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;

namespace ExpenseTracker.Features.Settings.ExpenseCategories;

public partial class RestoreCategoryPage
{
	RestoreCategoryViewModel _viewModel = new();
	public RestoreCategoryPage()
	{
		InitializeComponent();
		BindingContext = _viewModel;
	}

    private void SelectAllCheckBox_CheckedChanged(Object sender, CheckedChangedEventArgs e)
    {
        bool isChecked = e.Value;
        if (checkedFromCode)
        {
            checkedFromCode = false;
            return;
        }
        var categories = _viewModel.DeletedCategories;
        for (int i = 0; i < categories.Count; i++)
        {
            checkedFromCode = true;
            categories[i].IsChecked = isChecked;
        }
        checkedFromCode = false;
    }

    bool checkedFromCode = false;
    private void ItemCheckBox_CheckedChanged(Object sender, CheckedChangedEventArgs e)
    {
        var vm = _viewModel;
        var categories = vm.DeletedCategories;
        int count = categories.Count;
        int checkCount = categories.Where(x => x.IsChecked).Count();

        if (checkedFromCode)
        {
            return;
        }

        if (count == checkCount)
        {
            if (vm.IsSelectAllChecked != true)
            {
                checkedFromCode = true;
            }
            vm.IsSelectAllChecked = true;
        }
        else
        {
            if (vm.IsSelectAllChecked != false)
            {
                checkedFromCode = true;
            }
            vm.IsSelectAllChecked = false;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadCategories();
    }
}
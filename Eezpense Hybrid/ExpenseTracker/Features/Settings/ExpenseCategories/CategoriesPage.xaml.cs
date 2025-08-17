using ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;

namespace ExpenseTracker.Features.Settings.ExpenseCategories;

public partial class CategoriesPage : ContentPage
{
    CategoriesViewModel viewModel = new();
    public CategoriesPage()
    {
        InitializeComponent();
        viewModel.PageBackgroundColor = BackgroundColor;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        viewModel.LoadData();
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        viewModel.EditCategoryCommand.Execute(e.SelectedItem);
        /*
		
		// Deselect the item if null is encountered   
		if (e.SelectedItem == null) return;

		// Get the selected item (e.g. a category)  
		var selectedItem = e.SelectedItem as ExpenseCategory; // Replace with your actual class  

		// Find the visual tree of the ListView to get the Entry  
		if (selectedItem != null)
		{
			var listView = sender as ListView;
			if (listView != null)
			{
				// Get the index of the selected item  
				int index = listView.ItemsSource.Cast<ExpenseCategory>().ToList().IndexOf(selectedItem);

				// Get the cell corresponding to the selected item  
				var cell = (listView.TemplatedItems[index] as ViewCell);

				// Retrieve the Entry from the cell's View  
				var entry = cell.View.FindByName<Entry>("editableEntry");
				if (entry != null)
				{
					Thread.Sleep(100); // Wait for the Entry to be rendered
					var c = entry.Focus(); // Set focus to the Entry  
				}
			}
		}
		*/
        // Optionally, deselect the item  
        // ((ListView)sender).SelectedItem = null;
    }
}
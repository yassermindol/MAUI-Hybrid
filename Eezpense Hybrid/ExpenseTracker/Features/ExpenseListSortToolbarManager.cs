using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Features.ViewModels;
using ExpenseTracker.Models;

namespace ExpenseTracker.Features;
public class ExpenseListSortToolbarManager
{
    const string activeSymbol = " (✓)";

    public ExpenseListSortToolbarManager(IList<ToolbarItem> toolbarItems)
    {
        _toolbarItems = toolbarItems;
    }

    IList<ToolbarItem> _toolbarItems;

    public void AddSortToolbar(IList<ToolbarItem> toolbarItems, ExpenseListBaseViewModel viewModel)
    {
        SortType currentSortType = viewModel.CurrentSortType;

        ToolbarItem item1 = new ToolbarItem
        {
            ClassId = SortType.DateDescending.ToString(),
            Priority = 0,
            Order = ToolbarItemOrder.Secondary,
            Text = "Sort by Date ↓ ",
            Command = new Command(() =>
            {
                SetActive(SortType.DateDescending);
                viewModel.SortbyDateDescendingAsync();                
            }),
        };
        toolbarItems.Add(item1);

        var item2 = item1.Clone();
        item2.ClassId = SortType.AmountDescending.ToString();
        item2.Text = "Sort by Amount ↓ ";
        item2.Command = new Command(() =>
        {
            SetActive(SortType.AmountDescending);
            viewModel.SortbyAmountDescendingAsync();            
        });
        toolbarItems.Add(item2);

        var item3 = item1.Clone();
        item3.ClassId = SortType.GroupedByCategoryDateDescending.ToString();
        item3.Text = "Group by Category Date ↓";
        item3.Command = new Command(() =>
        {
            SetActive(SortType.GroupedByCategoryDateDescending);
            viewModel.GroupByCategoryDateDescendingAsync();            
        });
        toolbarItems.Add(item3);

        var item4 = item1.Clone();
        item4.Text = "Group by Category Amount ↓";
        item4.ClassId = SortType.GroupedByCategoryAmountDescending.ToString();
        item4.Command = new Command(() =>
        {
            SetActive(SortType.GroupedByCategoryAmountDescending);
            viewModel.GroupByCategoryAmountDescendingAsync();            
        });
        toolbarItems.Add(item4);

        var item = toolbarItems.First(x => x.ClassId == currentSortType.ToString());
        item.Text += activeSymbol;
    }

    private void SetActive(SortType sortType)
    {
        ClearActiveToolbarItem();
        var item = _toolbarItems.FirstOrDefault(x => x.ClassId == sortType.ToString());
        if (item != null)
        {
            item.Text += activeSymbol;
        }
    }

    private void ClearActiveToolbarItem()
    {
        foreach (var item in _toolbarItems)
        {
            if (!string.IsNullOrWhiteSpace(item.Text))
                item.Text = item.Text.Replace(activeSymbol, string.Empty);
        }
    }
}

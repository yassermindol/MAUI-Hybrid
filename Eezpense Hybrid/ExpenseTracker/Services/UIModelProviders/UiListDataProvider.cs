using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Models;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Settings;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Services.UIModelGenerators;

public class UiListDataProvider
{
    public ObservableCollection<UiWeekItem> GetUiWeeks(IEnumerable<ExpenseEntity> expenses)
    {
        var groups = expenses.GroupBy(x => x.WeekNumber);
        ColorService colorService = new ColorService();
        ObservableCollection<UiWeekItem> uiWeekItems = new ObservableCollection<UiWeekItem>();
        double highest = 0;
        int i = 0;

        CalendarService calendarService = new CalendarService();
        foreach (var group in groups)
        {
            UiWeekItem uiWeekItem = new UiWeekItem();
            uiWeekItem.Expenses = group;
            uiWeekItem.WeekNumber = group.Key.ToString();
            var weekOfYear = calendarService.WeeksOfYear.First(x => x.Number == group.Key);
            uiWeekItem.StartDateStr = weekOfYear.StartDate.ToString("MMMM dd");
            uiWeekItem.StartDate = weekOfYear.StartDate;
            uiWeekItem.EndDateStr = weekOfYear.EndDate.ToString("MMMM dd");
            uiWeekItem.EndDate = weekOfYear.EndDate;
            double total = group.Sum(x => x.Amount);
            if (total > highest)
                highest = total;
            uiWeekItem.Total = total;
            uiWeekItem.BarColor = colorService.GetColor(i);
            uiWeekItems.Add(uiWeekItem);
            i++;
        }

        BarWidthService widthService = new BarWidthService(AppConstants.BoxViewMaxWidth, highest);
        foreach (var uiWeek in uiWeekItems)
        {
            uiWeek.BarWidth = widthService.GetWidth(uiWeek.Total);
        }

        //Console.WriteLine("*************** List of Week Groups *****************");
        /*
        foreach (var z in groups)
        {
            var key = z.Key;
            var wk = calendarService.WeeksOfYear.First(x => x.Number == key);
            var val = z.ToList();
            Console.WriteLine($"Week:{wk.Number} | Start Date:{wk.StartDate} | End Date:{wk.EndDate} ");
            double total = 0;
            foreach (var e in val)
            {
                Console.WriteLine($"Amount:{e.Amount} Date:{e.Date} ");
                total += e.Amount;
            }
            Console.WriteLine($"Total:{total}");
        }
        */
        return uiWeekItems;
    }

    [Obsolete]
    public ObservableCollection<UiExpenseItem> GetUiExpenses(List<ExpenseEntity> expenses)
    {
        Color alternateColor = AppSettings.IsDarkMode ? Color.FromArgb("#242424") : Colors.LightGray;
        var uiExpenses = new ObservableCollection<UiExpenseItem>();
        for (int i = 0; i < expenses.Count; i++)
        {
            var uiExpense = new UiExpenseItem();
            var expense = expenses[i];

            if (i % 2 == 0)
                uiExpense.BackgroundColor = Colors.WhiteSmoke;//alternateColor;
            else
                uiExpense.BackgroundColor = Colors.GhostWhite;

            uiExpense.ID = expense.ID;
            uiExpense.CentralID = expense.CentralID;
            uiExpense.Amount = expense.Amount;
            uiExpense.Category = expense.Category;
            uiExpense.Note = expense.Note;
            uiExpense.DateTime = expense.Date;
            uiExpense.CategoryLocalID = expense.CategoryLocalID;
            uiExpenses.Add(uiExpense);
        }

        return uiExpenses;
    }

    public ObservableCollection<UiExpenseItem> GetDateDescending(List<ExpenseEntity> expenses)
    {
        //var sortedExpenses = expenses.OrderByDescending(x => x.Date).ToList(); // db already returning a descending date
        return GetUiData(expenses);
    }

    public ObservableCollection<UiExpenseItem> GetAmountDescending(List<ExpenseEntity> expenses)
    {
        var sortedExpenses = expenses.OrderByDescending(x => x.Amount).ToList();
        return GetUiData(sortedExpenses);
    }

    private ObservableCollection<UiExpenseItem> GetUiData(List<ExpenseEntity> sortedExpenses)
    {
        var uiExpenses = new ObservableCollection<UiExpenseItem>();
        for (int i = 0; i < sortedExpenses.Count; i++)
        {
            var uiExpense = new UiExpenseItem();
            var expense = sortedExpenses[i];

            //if (i % 2 == 0)
            //    uiExpense.BackgroundColor = Colors.GhostWhite;
            //else
            //    uiExpense.BackgroundColor = Colors.WhiteSmoke;

            uiExpense.ID = expense.ID;
            uiExpense.CentralID = expense.CentralID;
            uiExpense.Amount = expense.Amount;
            uiExpense.Category = expense.Category;
            uiExpense.Note = expense.Note;
            uiExpense.DateTime = expense.Date;
            uiExpense.CategoryLocalID = expense.CategoryLocalID;
            uiExpense.ItemType = ExpenseItemType.ExpenseItem;
            uiExpenses.Add(uiExpense);
        }

        return uiExpenses;
    }


    private ObservableCollection<UiExpenseItem> GetUiDataForGroupedByCategory(IEnumerable<IGrouping<long, ExpenseEntity>> groups,
        Func<IGrouping<long, ExpenseEntity>, List<ExpenseEntity>> sorter)
    {
        var uiSortedExpenses = new ObservableCollection<UiExpenseItem>();
        foreach (var group in groups)
        {
            var header = new UiExpenseItem();
            header.Category = group.First().Category;
            //header.BackgroundColor = Colors.LightSteelBlue;
            header.ItemType = ExpenseItemType.Header;
            uiSortedExpenses.Add(header);
            double total = 0;

            var expensesList = sorter(group);

            for (int i = 0; i < expensesList.Count(); i++)
            {
                var uiExpense = new UiExpenseItem();
                var expense = expensesList[i];

                //if (i % 2 == 0)
                //    uiExpense.BackgroundColor = Colors.GhostWhite;
                //else
                //    uiExpense.BackgroundColor = Colors.WhiteSmoke;

                uiExpense.ID = expense.ID;
                uiExpense.CentralID = expense.CentralID;
                uiExpense.Amount = expense.Amount;
                uiExpense.Category = expense.Category;
                uiExpense.Note = expense.Note;
                uiExpense.DateTime = expense.Date;
                uiExpense.CategoryLocalID = expense.CategoryLocalID;
                uiExpense.ItemType = Models.ExpenseItemType.ExpenseItem;
                uiSortedExpenses.Add(uiExpense);
                total += expense.Amount;
            }

            header.CategoryTotal = total;
        }

        return uiSortedExpenses;
    }

    /// <summary>
    /// Date is descending here
    /// </summary>
    public ObservableCollection<UiExpenseItem> GetGroupedByCategoryDateDescending(List<ExpenseEntity> expenses)
    {
        var groups = expenses.GroupBy(x => x.CategoryLocalID);
        groups = groups.OrderByDescending(group => group.Max(item => item.Date));
        Func<IGrouping<long, ExpenseEntity>, List<ExpenseEntity>> sorter = (x) => x.OrderByDescending(x => x.Date).ToList();
        return GetUiDataForGroupedByCategory(groups, sorter);
    }

    public ObservableCollection<UiExpenseItem> GetGroupedByCategoryAmountDescending(List<ExpenseEntity> expenses)
    {
        var groups = expenses.GroupBy(item => item.CategoryLocalID);
        groups = groups.OrderByDescending(group => group.Sum(y => y.Amount));
        Func<IGrouping<long, ExpenseEntity>, List<ExpenseEntity>> sorter = (group) => group.OrderByDescending(x => x.Amount).ToList();
        return GetUiDataForGroupedByCategory(groups, sorter);
    }

    public UiExpenseItem GetUiExpenseHeader(UiExpenseItem expenseItem)
    {
        var header = new UiExpenseItem
        {
            Category = expenseItem.Category,
            CategoryTotal = expenseItem.Amount,
            ItemType = ExpenseItemType.Header,
            //BackgroundColor = Colors.LightSteelBlue
        };

        return header;
    }


    public ObservableCollection<UiDeletedExpenseItem> GetUiDeletedExpenses(List<ExpenseEntity> expenses)
    {
        Color alternateColor = AppSettings.IsDarkMode ? Color.FromArgb("#242424") : Colors.LightGray;
        var uiExpenses = new ObservableCollection<UiDeletedExpenseItem>();
        for (int i = 0; i < expenses.Count; i++)
        {
            var uiExpense = new UiDeletedExpenseItem();
            var expense = expenses[i];

            if (i % 2 == 0)
                uiExpense.BackgroundColor = Colors.WhiteSmoke;//alternateColor;
            else
                uiExpense.BackgroundColor = Colors.GhostWhite;

            uiExpense.ID = expense.ID;
            uiExpense.CentralID = expense.CentralID;
            uiExpense.Amount = expense.Amount.ToMoney();
            uiExpense.Category = expense.Category;
            uiExpense.Note = expense.Note;
            uiExpense.DateTime = expense.Date;
            uiExpense.CategoryLocalID = expense.CategoryLocalID;
            uiExpenses.Add(uiExpense);
        }

        return uiExpenses;
    }

    public List<UiMonthItem> GetUiMonths(List<ExpenseEntity> expenses)
    {
        var groups = expenses.GroupBy(x => x.Date.Month);
        ColorService colorService = new ColorService();
        List<UiMonthItem> uiMonthItems = new List<UiMonthItem>();
        double highest = 0;
        int i = 0;
        CalendarService calendar = new CalendarService();
        foreach (var group in groups)
        {
            UiMonthItem uiMonthItem = new UiMonthItem();
            uiMonthItem.Expenses = group;
            double total = group.Sum(x => x.Amount);
            if (total > highest)
                highest = total;
            DateTime givenDate = group.First().Date;
            DateTime startOfMonth = new DateTime(givenDate.Year, givenDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);
            uiMonthItem.StartDate = startOfMonth;
            uiMonthItem.EndDate = endOfMonth;
            uiMonthItem.Month = calendar.Months[group.Key];
            uiMonthItem.Total = total;
            uiMonthItem.BarColor = colorService.GetColor(i);
            uiMonthItems.Add(uiMonthItem);
            i++;
        }

        BarWidthService widthService = new BarWidthService(AppConstants.BoxViewMaxWidth, highest);
        foreach (var uiWeek in uiMonthItems)
            uiWeek.BarWidth = widthService.GetWidth(uiWeek.Total);
        return uiMonthItems;
    }

    public ObservableCollection<UiDayItem> GetUiDays(List<ExpenseEntity> expenses, bool showSubItems, out double total)
    {
        total = 0;
        var UiDailyItems = new ObservableCollection<UiDayItem>();
        var groups = expenses.GroupBy(x => x.Date.Date).OrderByDescending(x => x.Key.Date);
        double highest = 0;
        ColorService colorService = new ColorService();
        int index = 0;
        foreach (var group in groups)
        {
            var header = new UiDayItem() { ItemType = ExpenseItemType.Header, IsVisible = true };
            double subItemsTotal = group.Sum(x => x.Amount);
            if (subItemsTotal > highest)
                highest = subItemsTotal;
            header.DateTime = group.Key;
            header.Total = subItemsTotal;
            total = total + subItemsTotal;
            header.BarColor = colorService.GetColor(index);
            header.ID = index;
            UiDailyItems.Add(header);
            index++;
            foreach (var item in group)
            {
                var uiExpense = new UiDayItem();
                uiExpense.ID = item.ID;
                uiExpense.CentralID = item.CentralID;
                uiExpense.Amount = item.Amount;
                uiExpense.Category = item.Category;
                uiExpense.Note = item.Note;
                uiExpense.DateTime = item.Date;
                uiExpense.CategoryLocalID = item.CategoryLocalID;
                uiExpense.ItemType = ExpenseItemType.ExpenseItem;
                if (showSubItems)
                    UiDailyItems.Add(uiExpense);
                header.Expenses.Add(uiExpense);
            }
        }

        BarWidthService widthService = new BarWidthService(AppConstants.BoxViewMaxWidth, highest);
        var headers = UiDailyItems.Where(x => x.ItemType == ExpenseItemType.Header).ToList();
        foreach (var header in headers)
        {
            header.IsExpanded = showSubItems ? true : false; // if showSubItems is true, then expand the header by default
            header.BarWidth = widthService.GetWidth(header.Total) - 16; // 20 is to align the margin on the right coz I cannot set it on the xaml with Margin
            header.BarWidthPercentage = widthService.GetWidthPercentage(header.Total);
        }           

        index = 0;
        foreach (var item in UiDailyItems)
        {
            item.Index = index;
            index++;
        }

        return UiDailyItems;
    }
}

using ExpenseTracker.Models.UI;
using ExpenseTracker.Services.UIModelGenerators;

namespace ExpenseTracker.Services.Api;

public class MonthlyService : BaseApiService
{
    public List<UiMonthItem> GetMonthItems(int year, out double total)
    {
        var yearExpenses = GetExpenses(year).Result;
        total = yearExpenses.Select(x => x.Amount).Sum();
        UiListDataProvider provider = new UiListDataProvider();
        var uiData = provider.GetUiMonths(yearExpenses);
        return uiData;
    }
}

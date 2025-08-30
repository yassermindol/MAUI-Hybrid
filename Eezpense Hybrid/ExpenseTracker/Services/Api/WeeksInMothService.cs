
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Services.UIModelGenerators;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Services.Api;

public class WeeksInMothService : BaseApiService
{
    private async Task<List<ExpenseEntity>> GetExpenses(int year, int month)
    {
        // Start date: first day of the month
        var start = new DateTime(year, month, 1);
        // End date: last day of the month
        int daysInMonth = DateTime.DaysInMonth(year, month);
        var end = new DateTime(year, month, daysInMonth, 23, 59, 59);
        var result = await GetExpenses(start, end);
        return result;
    }

    public async Task<ObservableCollection<UiWeekItem>> GetWeekItems(int year, int month)
    {
        var monthExpenses = await GetExpenses(year, month);
        UiListDataProvider provider = new UiListDataProvider();
        var uiData = provider.GetUiWeeks(monthExpenses);
        return uiData;
    }
}
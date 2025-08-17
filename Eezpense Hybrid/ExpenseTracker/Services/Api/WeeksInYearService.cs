

using ExpenseTracker.Models.UI;
using ExpenseTracker.Services.UIModelGenerators;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Services.Api;

public class WeeksInYearService : BaseApiService
{
    public async Task<ObservableCollection<UiWeekItem>> GetWeekItems(int year)
    {
        var yearExpenses = await GetExpenses(year);
        UiListDataProvider provider = new UiListDataProvider();
        var uiData = provider.GetUiWeeks(yearExpenses);
        return uiData;
    }
}

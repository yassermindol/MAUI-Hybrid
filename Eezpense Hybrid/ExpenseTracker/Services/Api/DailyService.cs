using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.Services.Api;

public class DailyService: BaseApiService
{
    public List<ExpenseEntity> GetDaily(DateTime start, DateTime end)
    {
        var expenses = GetExpenses(start, end).Result;
        return expenses;
    }
}

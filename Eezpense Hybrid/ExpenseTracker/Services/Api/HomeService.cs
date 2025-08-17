using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.Services.Api;

public class HomeService : BaseApiService
{
    public List<ExpenseEntity> GetRecent(DateTime start, out double total)
    {
        DateTime end = DateTime.Now;
        var expenses = GetExpenses(start, end).Result;
        total = expenses.Select(x=>x.Amount).Sum();
        return expenses;
    }
}
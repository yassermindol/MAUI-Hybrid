using ExpenseTracker.Database;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Services.Implementations;

namespace ExpenseTracker.Services.Api;

public class BaseApiService
{
    protected SqLiteDb _localDb = SqLiteDb.Instance;

    ///Todo: Make sure to get only expenses with undeleted categories.
    /// <summary>
    /// Get the expenses for the whole year. Make sure to get only expenses with undeleted categories.
    /// </summary>
    protected async Task<List<ExpenseEntity>> GetExpenses(int year)
    {
        //make sure time is up to just before midnight/next day
        var start = new DateTime(year, 1, 1);
        var end = new DateTime(year, 12, 31, 23, 59, 59);
        var result = await _localDb.Expenses.Get(start, end);
        return result;
    }

    protected async Task<List<ExpenseEntity>> GetExpenses(DateTime start, DateTime end)
    {
        var result = await _localDb.Expenses.Get(start, end);
        return result.OrderByDescending(x=>x.Date).ToList();
    }
}

using ExpenseTracker.Database;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Services.Implementations;
using Java.Time;

namespace ExpenseTracker.Services.Api;

public class BaseApiService
{
    protected SqLiteDb _localDb = SqLiteDb.Instance;

    ///Todo: Make sure to get only expenses with undeleted categories.
    /// <summary>
    /// Get the expenses for the whole year. Make sure to get only expenses with undeleted categories.
    /// </summary>
    public async Task<List<ExpenseEntity>> GetExpenses(int year)
    {
        //make sure time is up to just before midnight/next day
        var start = new DateTime(year, 1, 1);
        var end = new DateTime(year, 12, 31, 23, 59, 59);
        var result = await _localDb.Expenses.Get(start, end);
        return result.OrderByDescending(x => x.Date).ToList(); ;
    }

    public async Task<List<ExpenseEntity>> GetExpenses(DateTime start, DateTime end)
    {
        //make sure time is up to just before midnight/next day
        start = new DateTime(start.Year, start.Month, start.Day);
        end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
        var result = await _localDb.Expenses.Get(start, end);
        return result.OrderByDescending(x=>x.Date).ToList();
    }
}

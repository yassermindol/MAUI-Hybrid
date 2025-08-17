using ExpenseTracker.Database;
using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.Services.Implementations
{
    public class ExpenseServiceLocalDb
    {
        SqLiteDb _localDb = SqLiteDb.Instance;

        public Task<List<ExpenseEntity>> GetExpenses(DateTime startDate, DateTime endDate)
        {
            var result = _localDb.Expenses.Get(startDate, endDate);
            return result;
        }
    }
}

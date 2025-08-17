using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.Services.Implementations
{
    public class ExpenserServiceRemoteDb
    {
        public Task<List<ExpenseEntity>> GetExpenses(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}

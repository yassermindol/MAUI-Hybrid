
namespace ExpenseTracker.Models.DbEntities;

public class ExpenseCategoryEntity : BaseEntity
{    
    public string History { get; set; } // List of ExpenseCategoryHistory in json format.
    public string Name { get; set; }
}
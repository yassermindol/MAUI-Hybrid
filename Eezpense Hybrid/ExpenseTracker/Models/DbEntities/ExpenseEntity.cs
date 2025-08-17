using SQLite;

namespace ExpenseTracker.Models.DbEntities;

public class ExpenseEntity : BaseEntity
{
    [Indexed]
    public DateTime Date { get; set; }
    public string Category { get; set; }
    public double Amount { get; set; }
    public string Note { get; set; }
    public int WeekNumber { get; set; }
    public long CategoryLocalID { get; set; }
    public long CategoryCentralID { get; set; } = -1;
    public bool IsCategoryDeleted { get; set; }
}

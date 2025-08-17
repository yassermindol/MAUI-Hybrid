namespace ExpenseTracker.Models;

public class ExpenseCategoryHistory
{
    public string Name { get; set; }
    public string NewName { get; set; } //Will have value only for change name update type.
    public string UpdateType { get; set; }
    public DateTime Date { get; set; }
}

public enum ExpenseCategoryUpdateType { ReName, Delete, Restore }


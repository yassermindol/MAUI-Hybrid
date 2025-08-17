
using System.Collections.ObjectModel;
using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.Test;

public class InMemoryStorage
{
    public static List<ExpenseEntity> Expenses { get; set; }

    public static ObservableCollection<ExpenseCategoryEntity> Categories { get; set; }
}

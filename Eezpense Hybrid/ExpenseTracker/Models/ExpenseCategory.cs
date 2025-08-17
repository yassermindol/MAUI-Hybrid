using System;

namespace ExpenseTracker.Models;

public class ExpenseCategory
{
    //This is the ID primary key in DB.
    public long LocalID { get; set; }
    public long CentralID { get; set; }

    //Delete and Edit history
    public List<string> History { get; set; }
    public string Name { get; set; }
}


using SQLite;

namespace ExpenseTracker.Models.DbEntities;

public class BaseEntity
{
    [PrimaryKey, AutoIncrement]
    public long ID { get; set; } = -1;// This is the local ID column in the remote server.

    //This is the uniqueID in the remote server,
    //If synchronization with the server has been done, this should have some value greater than 0.
    public long CentralID { get; set; } = -1;
    public long UserID { get; set; } = -1;
    public bool IsDeleted { get; set; }
}

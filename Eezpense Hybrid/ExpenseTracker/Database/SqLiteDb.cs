using ExpenseTracker.Models.DbEntities;
using SQLite;
using System.Diagnostics;

namespace ExpenseTracker.Database;

public class SqLiteDb
{
    public const SQLiteOpenFlags Flags =
       // open the database in read/write mode
       SQLiteOpenFlags.ReadWrite |
       // create the database if it doesn't exist
       SQLiteOpenFlags.Create |
       // enable multi-threaded database access
       SQLiteOpenFlags.SharedCache;
    const string DatabaseFilename = "Expenses.db3";
    static string DatabasePath
    {
        get
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(basePath, DatabaseFilename);
        }
    }

    public ExpenseCategoryTable Categories { get; private set; }
    public ExpenseTable Expenses { get; private set; }

    SQLiteConnection _connection;


    private static Lazy<SqLiteDb> instance = new Lazy<SqLiteDb>(() => new SqLiteDb());
    public static SqLiteDb Instance => instance.Value;


    private SqLiteDb()
    {
        _connection = new SQLiteConnection(DatabasePath, Flags);
        Categories = new ExpenseCategoryTable(_connection);
        Expenses = new ExpenseTable(_connection);

        // Set up async initialization
        Initialize();
    }

    private void Initialize()
    {
        if (IsCategoriesTableExist())
        {
            //AlterTable();
            Debug.WriteLine("Db tables already created.");
        }
        else
        {
            _connection.CreateTable<ExpenseCategoryEntity>();
            _connection.CreateTable<ExpenseEntity>();
            //GenerateCategories();
        }
    }

    private void AlterTable()
    {
        string sql = "ALTER TABLE ExpenseCategoryEntity ADD COLUMN IsCategoryDeleted INTEGER DEFAULT 0";
        SQLiteCommand command = _connection.CreateCommand(sql, Array.Empty<string>());
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private void GenerateCategories()
    {
        Categories.Add(new ExpenseCategoryEntity { Name = "Food" });
        Categories.Add(new ExpenseCategoryEntity { Name = "Transportation" });
        Categories.Add(new ExpenseCategoryEntity { Name = "Bills" });
        Categories.Add(new ExpenseCategoryEntity { Name = "Miscellaneous" });
    }

    /// <summary>
    /// Close the SQL connection.
    /// </summary>
    public void Close()
    {
        _connection.Close();
    }

    private bool IsCategoriesTableExist()
    {
        var query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{nameof(ExpenseCategoryEntity)}';";

        var result = _connection.ExecuteScalar<string>(query);

        if (!string.IsNullOrEmpty(result))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

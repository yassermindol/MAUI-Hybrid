using System;

namespace ExpenseTracker.Settings;

public class AppConstants
{
    public static string Path_AddExpense = "Features/Home/AddExpensePage";
    public static string Path_TestPage = "Features/Home/TestPage";
    public static double BoxViewMaxWidth = 320;

    public static string DateFormat = "MMM d yyyy";

    public const SQLite.SQLiteOpenFlags Flags =
       // open the database in read/write mode
       SQLite.SQLiteOpenFlags.ReadWrite |
       // create the database if it doesn't exist
       SQLite.SQLiteOpenFlags.Create |
       // enable multi-threaded database access
       SQLite.SQLiteOpenFlags.SharedCache;

    const string DatabaseFilename = "Expenses.db3";
    public static string DatabasePath
    {
        get
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(basePath, DatabaseFilename);
        }
    }
}

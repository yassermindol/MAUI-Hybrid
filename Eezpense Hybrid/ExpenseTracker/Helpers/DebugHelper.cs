using System.Diagnostics;

namespace ExpenseTracker.Helpers;

public class DebugHelper
{
    public static void WriteLine(string message)
    {
        Debug.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")}**** {message} ****");
    }
}

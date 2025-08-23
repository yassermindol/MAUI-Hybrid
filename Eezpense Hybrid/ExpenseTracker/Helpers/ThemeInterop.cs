using Microsoft.JSInterop;
namespace ExpenseTracker.Helpers
{
    public static class ThemeInterop
    {
        [JSInvokable]
        public static string GetCurrentTheme()
        {
            // Return "dark" or "light" based on MAUI theme
            return App.Current?.RequestedTheme == AppTheme.Dark ? "dark" : "light";
        }
    }
}

using ExpenseTracker.Features.Main;
using ExpenseTracker.Services;

namespace ExpenseTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            
            // Process any unacknowledged purchases on startup
            _ = Task.Run(async () =>
            {
                try
                {
                    var billingService = new BillingService();
                    await billingService.ProcessPendingPurchases();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing pending purchases: {ex.Message}");
                }
            });
        }

        public static Action<bool> OnThemeChanged;
        private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            OnThemeChanged?.Invoke(e.RequestedTheme == AppTheme.Dark);
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    if (AppSettings.Account.OnBoardingCompleted)
        //        return new Window(new AppShell());
        //    else
        //        return new Window(new OnBoardingPage());
        //}

        public static bool IsDarkMode
        {
            get
            {
                return Current?.RequestedTheme == AppTheme.Dark;
            }
        }
    }
}

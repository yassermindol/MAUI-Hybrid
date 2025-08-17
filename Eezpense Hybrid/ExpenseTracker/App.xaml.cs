using ExpenseTracker.Features.Main;
using ExpenseTracker.Features.OnBoarding;
using ExpenseTracker.Settings;

namespace ExpenseTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            if (AppSettings.Account.OnBoardingCompleted)
                return new Window(new AppShell()) { Title = "App Shell" };
            else
                return new Window(new OnBoardingPage()) { Title = "On Boarding" };
        }

        public static bool IsDarkMode
        {
            get
            {
                return Current?.RequestedTheme == AppTheme.Dark;
            }
        }
    }
}

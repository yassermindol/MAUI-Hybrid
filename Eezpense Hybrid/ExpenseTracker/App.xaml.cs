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
            if (AppSettings.Account.OnBoardingCompleted)
                MainPage = new AppShell();
            else
                MainPage = new OnBoardingPage();
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

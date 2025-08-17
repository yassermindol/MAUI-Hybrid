using ExpenseTracker.Features.Main;

namespace ExpenseTracker.Services.Navigation;

public class NavigationService
{

    private static Page MainPage
    {
        get => App.Current.MainPage;
        set => App.Current.MainPage = value;
    }


    public Popups Popups { get; private set; } = new Popups(MainPage);

    public async Task GoToAsync(string url)
    {
        await (MainPage as AppShell).GoToAsync(url);
    }

    public async Task PushAsync(Page page)
    {
        await MainPage.Navigation.PushAsync(page);
    }

    public async Task PopAsync()
    {
        await MainPage.Navigation.PopAsync(true);
    }

    public async Task Home()
    {
        MainPage = new AppShell();
    }
}

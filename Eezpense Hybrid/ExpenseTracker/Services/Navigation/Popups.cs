using CommunityToolkit.Maui.Views;
using ExpenseTracker.PopupViews;
using Mopups.Services;

namespace ExpenseTracker.Services.Navigation;
public class Popups
{
    Page mainPage;

    public Popups(Page mainPage)
    {
        this.mainPage = mainPage;
    }

    public async Task DismissAsync()
    {
        if (MopupService.Instance.PopupStack.Count == 0)
            return;
        await MopupService.Instance.PopAsync();
    }

    public async Task ShowAsync(Mopups.Pages.PopupPage popup)
    {
        await MopupService.Instance.PushAsync(popup);
    }


    public async Task ShowStatusAsync(string message)
    {
        MopupService.Instance.PushAsync(new MopupStatus(message));
    }
    public async Task DismissStatus()
    {
        if (MopupService.Instance.PopupStack.Count == 0)
            return;
        MopupService.Instance.PopAsync();
    }


    /*
     * Buggy
    Popup toolkitPopup;
    public void ShowStatus(string message)
    {
        toolkitPopup = new PopupStatus(message);
        mainPage.ShowPopup(toolkitPopup);
    }

    public void DismissStatus()
    {
        if (toolkitPopup != null)
        {
            toolkitPopup.Close();
            toolkitPopup = null;
        }
    }
    */
}

namespace ExpenseTracker.Services;

public class MessageBoxService
{
    public async Task ShowMessage(string title, string message)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        await Application.Current.MainPage.DisplayAlert(title, message, "Ok"));
    }

    public async Task<bool> ShowMessage(string title, string message, string accept, string cancel)
    {
        bool isAccepted = false;
        await MainThread.InvokeOnMainThreadAsync(async () => isAccepted = await Application.Current.MainPage.
        DisplayAlert(title, message, accept, cancel));
        return isAccepted;
    }
}
using CommunityToolkit.Maui.Views;

namespace ExpenseTracker.PopupViews;

public partial class PopupStatus : Popup
{
	public PopupStatus()
	{
		InitializeComponent();
	}

    public PopupStatus(string message) : this()
    {
        lblMessage.Text = message;
    }

    public void SetMessage(string message)
    {
        MainThread.BeginInvokeOnMainThread(() => lblMessage.Text = message);
    }
}
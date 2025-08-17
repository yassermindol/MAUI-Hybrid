namespace ExpenseTracker.PopupViews;

public partial class MopupStatus
{
	public MopupStatus()
	{
		InitializeComponent();
	}

    public MopupStatus(string message)
    {
        InitializeComponent();
        label.Text = message;
    }
}
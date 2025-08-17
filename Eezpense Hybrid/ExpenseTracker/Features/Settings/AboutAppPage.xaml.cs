namespace ExpenseTracker.Features.Settings;

public partial class AboutAppPage : ContentPage
{
	public AboutAppPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        var version = AppInfo.Version;
        var build = AppInfo.BuildString;
        lblVersion.Text = $"Version: {version} Build: {build}";
    }
}
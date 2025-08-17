using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;

namespace ExpenseTracker;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, ViewTreeObserver.IOnGlobalLayoutListener
{
    Android.Views.View? contentView;
    //private BillingManager billingManager;

    protected override void OnPostCreate(Bundle? savedInstanceState)
    {
        base.OnPostCreate(savedInstanceState);
        
        contentView = FindViewById(Android.Resource.Id.Content).RootView;
        contentView?.ViewTreeObserver?.AddOnGlobalLayoutListener(this);
    }
    public void OnGlobalLayout()
    {
        try
        {
            if (contentView == null || contentView.RootWindowInsets == null)
                return;

            bool isKeyBoardVisible = WindowInsetsCompat.ToWindowInsetsCompat(contentView.RootWindowInsets).IsVisible(WindowInsetsCompat.Type.Ime());
            WeakReferenceMessenger.Default.Send(new KeyboardVisibilityMessage(isKeyBoardVisible));
        }
        catch
        {
        }
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        //billingManager = new BillingManager(this);

        /*
        // Get the color from MAUI resources
        var mauiColorObject = App.Current.Resources["MidnightBlue"];

        // Ensure it's a Color
        if (mauiColorObject is Color mauiColor)
        {
            // Convert MAUI Color to Android color
            var androidColor = mauiColor.ToAndroid();

            // Set the navigation bar color
            Window.SetNavigationBarColor(androidColor);
        }
        */
        //Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Window.SetNavigationBarColor(Colors.Blue.ToAndroid());
    }
}

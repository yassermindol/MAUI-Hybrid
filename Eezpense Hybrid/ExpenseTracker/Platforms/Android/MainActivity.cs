using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using Plugin.InAppBilling;
using ExpenseTracker.Services;

namespace ExpenseTracker;

[Activity(Theme = "@style/Maui.MainTheme.NoActionBar", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, ViewTreeObserver.IOnGlobalLayoutListener
{
    Android.Views.View? contentView;
    private BillingService _billingService;

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
        
        _billingService = new BillingService();
        
        // Check subscription status on startup
        _ = CheckSubscriptionStatus();

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
    }

    /// <summary>
    /// Check the current subscription status and handle pending purchases
    /// </summary>
    public async Task CheckSubscriptionStatus()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Checking subscription status...");
            
            // Check if user has active subscription
            var hasActiveSubscription = await _billingService.HasActivePremiumSubscription();
            System.Diagnostics.Debug.WriteLine($"Has active subscription: {hasActiveSubscription}");
            
            // Get all subscription purchases to handle pending ones
            var purchases = await _billingService.GetAllSubscriptionPurchases();
            
            foreach (var purchase in purchases)
            {
                System.Diagnostics.Debug.WriteLine($"Purchase found: {purchase.ProductId}, State: {purchase.State}");
                
                if (purchase.State == PurchaseState.PaymentPending)
                {
                    System.Diagnostics.Debug.WriteLine("Found pending payment, setting up callback...");
                    SetupPurchaseUpdateCallback();
                    break;
                }
                else if (purchase.State == PurchaseState.Purchased)
                {
                    // Try to acknowledge unacknowledged purchases if supported
                    System.Diagnostics.Debug.WriteLine("Purchase found, attempting acknowledgment...");
                    await _billingService.TryAcknowledgePurchase(purchase.PurchaseToken);
                }
            }
            
            // Get subscription product details
            var product = await _billingService.GetSubscriptionProduct();
            if (product != null)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription product: {product.Name}, Price: {product.LocalizedPrice}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking subscription status: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Setup callback for purchase updates (for pending payments)
    /// </summary>
    private void SetupPurchaseUpdateCallback()
    {
        InAppBillingImplementation.OnAndroidPurchasesUpdated = async (billingResult, purchases) =>
        {
            System.Diagnostics.Debug.WriteLine($"Purchase update callback triggered. Result: {billingResult?.ResponseCode}");
            
            if (purchases != null)
            {
                foreach (var purchase in purchases)
                {
                    System.Diagnostics.Debug.WriteLine($"Updated purchase: {purchase.ProductId}, State: {purchase.State}");
                    
                    if (purchase.State == PurchaseState.Purchased)
                    {
                        // Try to acknowledge the purchase
                        await _billingService.TryAcknowledgePurchase(purchase.PurchaseToken);
                        System.Diagnostics.Debug.WriteLine("Purchase completed and acknowledgment attempted");
                        
                        // You might want to send a message to update the UI
                        // WeakReferenceMessenger.Default.Send(new SubscriptionPurchasedMessage());
                    }
                }
            }
        };
    }
    
    /// <summary>
    /// Method to purchase premium subscription (call this from your UI)
    /// </summary>
    public async Task<bool> PurchasePremiumSubscription()
    {
        try
        {
            SetupPurchaseUpdateCallback();
            return await _billingService.PurchasePremiumSubscription();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in purchase method: {ex.Message}");
            return false;
        }
    }
}

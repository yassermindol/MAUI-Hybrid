using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Services;

/// <summary>
/// Service to access MainActivity billing methods from anywhere in the app
/// </summary>
public class PlatformBillingService
{
    /// <summary>
    /// Purchase premium subscription using MainActivity
    /// </summary>
    /// <returns>True if purchase was initiated successfully</returns>
    public async Task<bool> PurchasePremiumFromMainActivity()
    {
        try
        {
#if ANDROID
            if (Platform.CurrentActivity is MainActivity mainActivity)
            {
                return await mainActivity.PurchasePremiumSubscription();
            }
#endif
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error purchasing from MainActivity: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check subscription status using MainActivity
    /// </summary>
    public async Task CheckSubscriptionFromMainActivity()
    {
        try
        {
#if ANDROID
            if (Platform.CurrentActivity is MainActivity mainActivity)
            {
                await mainActivity.CheckSubscriptionStatus();
            }
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking subscription from MainActivity: {ex.Message}");
        }
    }
}
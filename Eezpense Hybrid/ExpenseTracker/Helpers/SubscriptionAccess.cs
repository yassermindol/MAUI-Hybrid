using ExpenseTracker.Services;

namespace ExpenseTracker.Helpers;

/// <summary>
/// Simple subscription access helper for all-or-nothing model
/// </summary>
public static class SubscriptionAccess
{
    private static SubscriptionService _subscriptionService = new();
    
    /// <summary>
    /// Quick check if user has subscription access (cached for performance)
    /// </summary>
    public static async Task<bool> HasAccess()
    {
        try
        {
            return await _subscriptionService.IsPremiumUser();
        }
        catch
        {
            return false; // Default to no access on error
        }
    }
    
    /// <summary>
    /// Show subscription required dialog and optionally start purchase flow
    /// </summary>
    public static async Task<bool> ShowSubscriptionRequired(string featureName = "this feature")
    {
        var result = await Application.Current.MainPage.DisplayAlert(
            "Subscription Required",
            $"A subscription is required to access {featureName}. Would you like to subscribe now?",
            "Subscribe Now",
            "Not Now");
            
        if (result)
        {
            // Start purchase flow
            var subscriptionService = new SubscriptionService();
            var purchaseResult = await subscriptionService.PurchasePremium();
            
            if (purchaseResult.Success)
            {
                // Purchase initiated - check again after a moment
                await Task.Delay(2000);
                return await HasAccess();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Purchase Failed", 
                    purchaseResult.Message, 
                    "OK");
                return false;
            }
        }
        
        return false; // User declined
    }
}

/// <summary>
/// Your subscription model - simple all-or-nothing approach
/// </summary>
public static class SubscriptionModel
{
    /// <summary>
    /// What users get with subscription (for marketing/UI)
    /// </summary>
    public static readonly string[] SubscriptionBenefits = new[]
    {
        "? Unlimited expense tracking",
        "? All report types and analytics", 
        "? Data export and backup",
        "? Advanced categories and filters",
        "? Premium themes and customization",
        "? Priority customer support",
        "? No ads or limitations"
    };
    
    /// <summary>
    /// What free users can do (if you want to offer any free features)
    /// </summary>
    public static readonly string[] FreeLimitations = new[]
    {
        "? Limited to 50 expenses per month",
        "? Basic reports only",
        "? No data export",
        "? Limited categories",
        "? Ads supported"
    };
}
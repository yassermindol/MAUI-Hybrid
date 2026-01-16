using Plugin.InAppBilling;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ExpenseTracker.Services;

/// <summary>
/// Production-ready subscription management service
/// </summary>
public class SubscriptionManager
{
    private readonly BillingService _billingService;
    private const string SUBSCRIPTION_ID = "eezpense_premium_monthly";

    public SubscriptionManager(BillingService billingService)
    {
        _billingService = billingService;
    }

    /// <summary>
    /// Get detailed subscription status for production use
    /// </summary>
    public async Task<DetailedSubscriptionStatus> GetDetailedSubscriptionStatus()
    {
        try
        {
            var purchases = await _billingService.GetAllSubscriptionPurchases();
            var activePurchase = purchases.FirstOrDefault(p => 
                p.ProductId == SUBSCRIPTION_ID && 
                (p.State == PurchaseState.Purchased || p.State == PurchaseState.PaymentPending));

            if (activePurchase == null)
            {
                return new DetailedSubscriptionStatus
                {
                    IsActive = false,
                    Status = SubscriptionStatusType.NotSubscribed,
                    Message = "No active subscription found"
                };
            }

            return new DetailedSubscriptionStatus
            {
                IsActive = activePurchase.State == PurchaseState.Purchased,
                ProductId = activePurchase.ProductId,
                PurchaseToken = activePurchase.PurchaseToken,
                PurchaseDate = activePurchase.TransactionDateUtc,
                Status = GetSubscriptionStatusType(activePurchase),
                AutoRenewing = activePurchase.AutoRenewing,
                Message = GetStatusMessage(activePurchase)
            };
        }
        catch (Exception ex)
        {
            return new DetailedSubscriptionStatus
            {
                IsActive = false,
                Status = SubscriptionStatusType.Error,
                Message = "Unable to check subscription status"
            };
        }
    }

    /// <summary>
    /// Restore purchases for account transfers or reinstalls
    /// </summary>
    public async Task<RestoreResult> RestorePurchases()
    {
        try
        {
            Debug.WriteLine("Restoring purchases...");
            
            var purchases = await _billingService.GetAllSubscriptionPurchases();
            var validPurchases = purchases.Where(p => 
                p.ProductId == SUBSCRIPTION_ID && 
                p.State == PurchaseState.Purchased).ToList();

            if (validPurchases.Any())
            {
                Debug.WriteLine($"Restored {validPurchases.Count} purchase(s)");
                return new RestoreResult
                {
                    Success = true,
                    RestoredCount = validPurchases.Count,
                    Message = $"Successfully restored {validPurchases.Count} purchase(s)"
                };
            }
            else
            {
                Debug.WriteLine("No purchases to restore");
                return new RestoreResult
                {
                    Success = true,
                    RestoredCount = 0,
                    Message = "No previous purchases found to restore"
                };
            }
        }
        catch (Exception ex)
        {
            return new RestoreResult
            {
                Success = false,
                Message = "Failed to restore purchases"
            };
        }
    }

    /// <summary>
    /// Open Google Play subscription management
    /// </summary>
    public async Task OpenSubscriptionManagement()
    {
        try
        {
            // Direct link to user's subscriptions in Google Play
            var subscriptionUrl = $"https://play.google.com/store/account/subscriptions?sku={SUBSCRIPTION_ID}&package=com.ymitech.eezpense";
            
            // Use MAUI Launcher to open URL
            await Launcher.OpenAsync(subscriptionUrl);
            
            Debug.WriteLine("Opened subscription management");
        }
        catch (Exception ex)
        {            
            // Fallback: Open general subscription page
            try
            {
                await Launcher.OpenAsync("https://play.google.com/store/account/subscriptions");
            }
            catch (Exception fallbackEx)
            {
            }
        }
    }

    private SubscriptionStatusType GetSubscriptionStatusType(InAppBillingPurchase purchase)
    {
        return purchase.State switch
        {
            PurchaseState.Purchased => SubscriptionStatusType.Active,
            PurchaseState.PaymentPending => SubscriptionStatusType.PaymentPending,
            PurchaseState.Canceled => SubscriptionStatusType.Canceled,
            _ => SubscriptionStatusType.Unknown
        };
    }

    private string GetStatusMessage(InAppBillingPurchase purchase)
    {
        return purchase.State switch
        {
            PurchaseState.Purchased => "Your premium subscription is active!",
            PurchaseState.PaymentPending => "Payment is being processed. Premium features will be available once payment completes.",
            PurchaseState.Canceled => "Your subscription has been canceled.",
            _ => "Unable to determine subscription status."
        };
    }
}

/// <summary>
/// Detailed subscription status information
/// </summary>
public class DetailedSubscriptionStatus
{
    public bool IsActive { get; set; }
    public string ProductId { get; set; }
    public string PurchaseToken { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public SubscriptionStatusType Status { get; set; }
    public bool AutoRenewing { get; set; }
    public string Message { get; set; }
}

/// <summary>
/// Result of purchase restoration
/// </summary>
public class RestoreResult
{
    public bool Success { get; set; }
    public int RestoredCount { get; set; }
    public string Message { get; set; }
}

/// <summary>
/// Subscription status types
/// </summary>
public enum SubscriptionStatusType
{
    NotSubscribed,
    Active,
    PaymentPending,
    Expired,
    Canceled,
    Unknown,
    Error
}
using ExpenseTracker.Services;
using Plugin.InAppBilling;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Services;

/// <summary>
/// Service to handle subscription-related UI operations
/// </summary>
public class SubscriptionService
{
    private readonly BillingService _billingService;
    private readonly ILogger<SubscriptionService> _logger;
    
    public SubscriptionService(ILogger<SubscriptionService> logger = null)
    {
        _billingService = new BillingService(null); // Pass null for logger type mismatch
        _logger = logger;
    }
    
    /// <summary>
    /// Check if user is a premium subscriber
    /// </summary>
    /// <returns>True if user has premium access</returns>
    public async Task<bool> IsPremiumUser()
    {
        try
        {
            return await _billingService.HasActivePremiumSubscription();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error checking premium status");
            return false; // Default to non-premium on error
        }
    }
    
    /// <summary>
    /// Get subscription information for display in UI
    /// </summary>
    /// <returns>Subscription info or null if not available</returns>
    public async Task<SubscriptionInfo> GetSubscriptionInfo()
    {
        try
        {
            var product = await _billingService.GetSubscriptionProduct();
            var hasActive = await _billingService.HasActivePremiumSubscription();
            var purchases = await _billingService.GetAllSubscriptionPurchases();
            
            var activePurchase = purchases.FirstOrDefault(p => 
                p.ProductId == "eezpense_premium_monthly" && 
                (p.State == PurchaseState.Purchased || p.State == PurchaseState.PaymentPending));
            
            return new SubscriptionInfo
            {
                ProductName = product?.Name ?? "Premium Monthly",
                LocalizedPrice = product?.LocalizedPrice ?? "N/A",
                HasActiveSubscription = hasActive,
                PurchaseState = activePurchase?.State,
                PurchaseDate = activePurchase?.TransactionDateUtc
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting subscription info");
            return null;
        }
    }
    
    /// <summary>
    /// Initiate premium subscription purchase
    /// </summary>
    /// <returns>Purchase result</returns>
    public async Task<PurchaseResult> PurchasePremium()
    {
        try
        {
            var success = await _billingService.PurchasePremiumSubscription();
            return new PurchaseResult 
            { 
                Success = success, 
                Message = success ? "Purchase initiated successfully" : "Purchase failed to initiate" 
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error purchasing premium");
            return new PurchaseResult 
            { 
                Success = false, 
                Message = $"Purchase error: {ex.Message}" 
            };
        }
    }
}

/// <summary>
/// Information about the user's subscription
/// </summary>
public class SubscriptionInfo
{
    public string ProductName { get; set; }
    public string LocalizedPrice { get; set; }
    public bool HasActiveSubscription { get; set; }
    public PurchaseState? PurchaseState { get; set; }
    public DateTime? PurchaseDate { get; set; }
}

/// <summary>
/// Result of a purchase operation
/// </summary>
public class PurchaseResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
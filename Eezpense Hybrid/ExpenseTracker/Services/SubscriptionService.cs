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
    
    public SubscriptionService()
    {
        _billingService = new BillingService(); // Pass null for logger type mismatch
    }
    
    /// <summary>
    /// Check if user is a premium subscriber (includes trial period)
    /// </summary>
    /// <returns>True if user has premium access</returns>
    public async Task<bool> IsPremiumUser()
    {
        try
        {
            bool result = await _billingService.HasActivePremiumSubscription();
            return result;
        }
        catch (Exception ex)
        {
            return false; // Default to non-premium on error
        }
    }
    
    /// <summary>
    /// Check if user is currently in trial period
    /// </summary>
    /// <returns>True if user is in free trial</returns>
    public async Task<bool> IsInTrialPeriod()
    {
        try
        {
            var purchases = await _billingService.GetAllSubscriptionPurchases();
            var activePurchase = purchases.FirstOrDefault(p => 
                p.ProductId == "eezpense_premium_monthly" && 
                (p.State == PurchaseState.Purchased || p.State == PurchaseState.PaymentPending));
            
            if (activePurchase == null)
                return false;
                
            // Assume trial period is 14 days from purchase date
            var trialEndDate = activePurchase.TransactionDateUtc.AddDays(14);
            return DateTime.UtcNow < trialEndDate;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Get remaining trial days
    /// </summary>
    /// <returns>Number of days remaining in trial, 0 if not in trial</returns>
    public async Task<int> GetTrialDaysRemaining()
    {
        try
        {
            var purchases = await _billingService.GetAllSubscriptionPurchases();
            var activePurchase = purchases.FirstOrDefault(p => 
                p.ProductId == "eezpense_premium_monthly" && 
                (p.State == PurchaseState.Purchased || p.State == PurchaseState.PaymentPending));
            
            if (activePurchase == null)
                return 0;
                
            // Calculate days remaining based on purchase date + 14 days
            var trialEndDate = activePurchase.TransactionDateUtc.AddDays(14);
            var daysRemaining = (int)(trialEndDate - DateTime.UtcNow).TotalDays;
            
            return Math.Max(0, daysRemaining);
        }
        catch (Exception ex)
        {
            return 0;
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
            var isInTrial = await IsInTrialPeriod();
            var trialDaysRemaining = await GetTrialDaysRemaining();
            
            var activePurchase = purchases.FirstOrDefault(p => 
                p.ProductId == "eezpense_premium_monthly" && 
                (p.State == PurchaseState.Purchased || p.State == PurchaseState.PaymentPending));
            
            return new SubscriptionInfo
            {
                ProductName = product?.Name ?? "Premium Monthly",
                LocalizedPrice = product?.LocalizedPrice ?? "N/A",
                HasActiveSubscription = hasActive,
                PurchaseState = activePurchase?.State,
                PurchaseDate = activePurchase?.TransactionDateUtc,
                IsInTrialPeriod = isInTrial,
                TrialDaysRemaining = trialDaysRemaining,
                TrialEndDate = isInTrial && activePurchase != null ? 
                    activePurchase.TransactionDateUtc.AddDays(14) : null
            };
        }
        catch (Exception ex)
        {
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
    public bool IsInTrialPeriod { get; set; }
    public int TrialDaysRemaining { get; set; }
    public DateTime? TrialEndDate { get; set; }
}

/// <summary>
/// Result of a purchase operation
/// </summary>
public class PurchaseResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
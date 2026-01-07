using Plugin.InAppBilling;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Services;

public class BillingService
{
    private readonly ILogger<BillingService> _logger;
    private const string MONTHLY_SUBSCRIPTION_ID = "eezpense_premium_monthly";
    
    public BillingService(ILogger<BillingService> logger = null)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Check if the user has an active premium subscription
    /// </summary>
    /// <returns>True if user has active subscription, false otherwise</returns>
    public async Task<bool> HasActivePremiumSubscription()
    {
        try
        {
            var billing = CrossInAppBilling.Current;
            var connected = await billing.ConnectAsync();
            
            if (!connected)
            {
                Debug.WriteLine("Could not connect to billing service");
                return false;
            }

            // Get all subscription purchases
            var purchases = await billing.GetPurchasesAsync(ItemType.Subscription);
            
            // Check if the user has the premium subscription
            var premiumPurchase = purchases?.FirstOrDefault(p => 
                p.ProductId == MONTHLY_SUBSCRIPTION_ID && 
                (p.State == PurchaseState.Purchased || p.State == PurchaseState.PaymentPending));
                
            return premiumPurchase != null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking subscription status: {ex.Message}");
            _logger?.LogError(ex, "Error checking subscription status");
            return false;
        }
        finally
        {
            await CrossInAppBilling.Current.DisconnectAsync();
        }
    }
    
    /// <summary>
    /// Get subscription product details from the store
    /// </summary>
    /// <returns>Product information if available</returns>
    public async Task<InAppBillingProduct> GetSubscriptionProduct()
    {
        try
        {
            var billing = CrossInAppBilling.Current;
            var connected = await billing.ConnectAsync();
            
            if (!connected)
            {
                Debug.WriteLine("Could not connect to billing service");
                return null;
            }

            // Get subscription product details
            var productIds = new[] { MONTHLY_SUBSCRIPTION_ID };
            var products = await billing.GetProductInfoAsync(ItemType.Subscription, productIds);
            
            return products?.FirstOrDefault(p => p.ProductId == MONTHLY_SUBSCRIPTION_ID);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting subscription product: {ex.Message}");
            _logger?.LogError(ex, "Error getting subscription product");
            return null;
        }
        finally
        {
            await CrossInAppBilling.Current.DisconnectAsync();
        }
    }
    
    /// <summary>
    /// Purchase the premium subscription
    /// </summary>
    /// <returns>True if purchase was successful, false otherwise</returns>
    public async Task<bool> PurchasePremiumSubscription()
    {
        try
        {
            var billing = CrossInAppBilling.Current;
            var connected = await billing.ConnectAsync();
            
            if (!connected)
            {
                Debug.WriteLine("Could not connect to billing service");
                return false;
            }

            // Make the purchase
            var purchase = await billing.PurchaseAsync(MONTHLY_SUBSCRIPTION_ID, ItemType.Subscription);
            
            if (purchase?.State == PurchaseState.Purchased)
            {
                Debug.WriteLine("Subscription purchased successfully");
                return true;
            }
            else if (purchase?.State == PurchaseState.PaymentPending)
            {
                Debug.WriteLine("Payment is pending for subscription");
                return true; // Consider pending as success for now
            }
            
            Debug.WriteLine($"Purchase failed with state: {purchase?.State}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error purchasing subscription: {ex.Message}");
            _logger?.LogError(ex, "Error purchasing subscription");
            return false;
        }
        finally
        {
            await CrossInAppBilling.Current.DisconnectAsync();
        }
    }
    
    /// <summary>
    /// Get all subscription purchase history
    /// </summary>
    /// <returns>List of subscription purchases</returns>
    public async Task<IEnumerable<InAppBillingPurchase>> GetAllSubscriptionPurchases()
    {
        try
        {
            var billing = CrossInAppBilling.Current;
            var connected = await billing.ConnectAsync();
            
            if (!connected)
            {
                Debug.WriteLine("Could not connect to billing service");
                return new List<InAppBillingPurchase>();
            }

            var purchases = await billing.GetPurchasesAsync(ItemType.Subscription);
            return purchases ?? new List<InAppBillingPurchase>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting subscription purchases: {ex.Message}");
            _logger?.LogError(ex, "Error getting subscription purchases");
            return new List<InAppBillingPurchase>();
        }
        finally
        {
            await CrossInAppBilling.Current.DisconnectAsync();
        }
    }
    
    /// <summary>
    /// Finalize a purchase (required for subscriptions in Plugin.InAppBilling v10)
    /// </summary>
    /// <param name="purchaseToken">Purchase token to finalize</param>
    /// <returns>True if finalized successfully</returns>
    public async Task<bool> FinalizePurchase(string purchaseToken)
    {
        try
        {
            var billing = CrossInAppBilling.Current;
            var connected = await billing.ConnectAsync();
            
            if (!connected)
            {
                Debug.WriteLine("Could not connect to billing service");
                return false;
            }

            // Use FinalizePurchaseAsync with array parameter (Plugin.InAppBilling v10)
            var purchaseTokens = new[] { purchaseToken };
            var results = await billing.FinalizePurchaseAsync(purchaseTokens);
            
            // Check if our specific purchase token was finalized successfully
            var result = results?.FirstOrDefault(r => r.Id == purchaseToken);
            var success = result?.Success ?? false;
            
            Debug.WriteLine($"Purchase finalization result for {purchaseToken}: {success}");
            return success;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error finalizing purchase: {ex.Message}");
            _logger?.LogError(ex, "Error finalizing purchase");
            return false;
        }
        finally
        {
            await CrossInAppBilling.Current.DisconnectAsync();
        }
    }
    
    /// <summary>
    /// Try to acknowledge a purchase if the method exists
    /// Note: Updated for Plugin.InAppBilling v10 - now uses FinalizePurchaseAsync
    /// </summary>
    /// <param name="purchaseToken">Purchase token to acknowledge</param>
    /// <returns>True if acknowledged or finalized successfully</returns>
    public async Task<bool> TryAcknowledgePurchase(string purchaseToken)
    {
        try
        {
            // Plugin.InAppBilling v10 uses FinalizePurchaseAsync
            return await FinalizePurchase(purchaseToken);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error acknowledging/finalizing purchase: {ex.Message}");
            _logger?.LogError(ex, "Error acknowledging/finalizing purchase");
            return false;
        }
    }
}
using ExpenseTracker.Services;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Services;

/// <summary>
/// Centralized subscription status manager for the entire app
/// </summary>
public class AppSubscriptionManager
{
    private readonly SubscriptionManager _subscriptionManager;
    private readonly ILogger<AppSubscriptionManager> _logger;
    private static AppSubscriptionManager _instance;
    private static readonly object _lock = new object();
    
    // Cache subscription status to avoid repeated API calls
    private DetailedSubscriptionStatus _cachedStatus;
    private DateTime _lastCheck = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5); // Cache for 5 minutes
    
    public static AppSubscriptionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var billingService = new BillingService();
                        var subscriptionManager = new SubscriptionManager(billingService);
                        _instance = new AppSubscriptionManager(subscriptionManager);
                    }
                }
            }
            return _instance;
        }
    }
    
    public AppSubscriptionManager(SubscriptionManager subscriptionManager, ILogger<AppSubscriptionManager> logger = null)
    {
        _subscriptionManager = subscriptionManager;
        _logger = logger;
    }
    
    /// <summary>
    /// Check subscription status with caching
    /// </summary>
    public async Task<DetailedSubscriptionStatus> GetSubscriptionStatusAsync(bool forceRefresh = false)
    {
        try
        {
            // Return cached result if still valid
            if (!forceRefresh && _cachedStatus != null && 
                DateTime.Now - _lastCheck < _cacheExpiry)
            {
                return _cachedStatus;
            }
            
            // Get fresh status
            _cachedStatus = await _subscriptionManager.GetDetailedSubscriptionStatus();
            _lastCheck = DateTime.Now;
            
            return _cachedStatus;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting subscription status");
            
            // Return cached status if available, otherwise return error status
            return _cachedStatus ?? new DetailedSubscriptionStatus
            {
                IsActive = false,
                Status = SubscriptionStatusType.Error,
                Message = "Unable to check subscription status"
            };
        }
    }
    
    /// <summary>
    /// Quick check if user has premium access (cached)
    /// </summary>
    public async Task<bool> IsPremiumUserAsync()
    {
        var status = await GetSubscriptionStatusAsync();
        return status.IsActive;
    }
    
    /// <summary>
    /// Force refresh subscription status (use sparingly)
    /// </summary>
    public async Task RefreshSubscriptionStatusAsync()
    {
        await GetSubscriptionStatusAsync(forceRefresh: true);
    }
    
    /// <summary>
    /// Check if user can access a premium feature
    /// </summary>
    public async Task<FeatureAccessResult> CanAccessPremiumFeatureAsync(string featureName)
    {
        var status = await GetSubscriptionStatusAsync();
        
        if (status.IsActive)
        {
            return new FeatureAccessResult
            {
                CanAccess = true,
                Message = "Premium feature available"
            };
        }
        
        return new FeatureAccessResult
        {
            CanAccess = false,
            Message = $"'{featureName}' is a premium feature. Upgrade to unlock!",
            RequiresUpgrade = true
        };
    }
    
    /// <summary>
    /// Open subscription management
    /// </summary>
    public async Task OpenSubscriptionManagementAsync()
    {
        await _subscriptionManager.OpenSubscriptionManagement();
    }
    
    /// <summary>
    /// Clear cached status (call when user makes a purchase)
    /// </summary>
    public void ClearCache()
    {
        _cachedStatus = null;
        _lastCheck = DateTime.MinValue;
    }
}

/// <summary>
/// Result of feature access check
/// </summary>
public class FeatureAccessResult
{
    public bool CanAccess { get; set; }
    public string Message { get; set; }
    public bool RequiresUpgrade { get; set; }
}
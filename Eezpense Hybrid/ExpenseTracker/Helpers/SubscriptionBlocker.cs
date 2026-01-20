using ExpenseTracker.Services;
using ExpenseTracker.Settings;

namespace ExpenseTracker.Helpers;

/// <summary>
/// Comprehensive subscription blocking system for non-subscribed users
/// </summary>
public static class SubscriptionBlocker
{
    private static SubscriptionService _subscriptionService = new();

    /// <summary>
    /// Different blocking strategies for your app
    /// </summary>
    public enum BlockingStrategy
    {
        CompletePaywall,     // Block everything except subscription
        LimitedFunctionality, // Allow basic use but limit features
        TrialPeriod,         // Allow full access for X days then block
        FreemiumModel        // Some features free forever, premium features blocked
    }

    public static BlockingStrategy CurrentStrategy = BlockingStrategy.FreemiumModel;

    /// <summary>
    /// Check if user can perform an action based on blocking strategy
    /// </summary>
    public static async Task<bool> CanPerformAction(string actionName, ActionType actionType)
    {
        var isPremium = await _subscriptionService.IsPremiumUser();
        
        if (isPremium)
        {
            return true; // Premium users can do everything
        }

        return CurrentStrategy switch
        {
            BlockingStrategy.CompletePaywall => await HandleCompletePaywall(actionName),
            BlockingStrategy.LimitedFunctionality => await HandleLimitedFunctionality(actionName, actionType),
            BlockingStrategy.TrialPeriod => await HandleTrialPeriod(actionName),
            BlockingStrategy.FreemiumModel => await HandleFreemiumModel(actionName, actionType),
            _ => false
        };
    }

    /// <summary>
    /// STRATEGY 1: Complete Paywall - Block everything
    /// </summary>
    private static async Task<bool> HandleCompletePaywall(string actionName)
    {
        var result = await Application.Current.MainPage.DisplayAlert(
            "Subscription Required",
            $"Eezpense requires a subscription to {actionName}. Subscribe now to continue!",
            "Subscribe Now",
            "Exit App");
            
        if (result)
        {
            var subscriptionService = new SubscriptionService();
            var purchaseResult = await subscriptionService.PurchasePremium();
            
            if (purchaseResult.Success)
            {
                return true; // Purchase initiated - allow action
            }
        }
        else
        {
            Application.Current.Quit(); // User chose to exit
        }
        
        return false;
    }

    /// <summary>
    /// STRATEGY 2: Limited Functionality - Allow basic actions
    /// </summary>
    private static async Task<bool> HandleLimitedFunctionality(string actionName, ActionType actionType)
    {
        // Define what's allowed for free users
        var freeActions = new[] 
        {
            ActionType.AddBasicExpense,
            ActionType.ViewRecentExpenses,
            ActionType.BasicSettings
        };

        if (freeActions.Contains(actionType))
        {
            return true; // Allow basic actions
        }

        // Block premium actions
        var result = await Application.Current.MainPage.DisplayAlert(
            "Premium Feature",
            $"{actionName} is a premium feature. Upgrade to unlock!",
            "Upgrade Now",
            "Use Free Version");
            
        if (result)
        {
            var subscriptionService = new SubscriptionService();
            await subscriptionService.PurchasePremium();
        }
        
        return false; // Block the action
    }

    /// <summary>
    /// STRATEGY 3: Trial Period - Full access then paywall (DISABLED - Not needed for all-or-nothing model)
    /// </summary>
    private static async Task<bool> HandleTrialPeriod(string actionName)
    {
        // Since you don't want premium features, this is simplified
        // Just show subscription dialog for everything
        return await ShowPremiumUpgradeDialog(actionName);
        
        /*
        var trialStartDate = AppSettings.Account.TrialStartDate;
        var trialDays = 7;

        if (trialStartDate == DateTime.MinValue)
        {
            // Start trial for new users
            AppSettings.Account.TrialStartDate = DateTime.Now;
            await Application.Current.MainPage.DisplayAlert(
                "Trial Started",
                $"Welcome! You have {trialDays} days of free access to all features.",
                "OK");
            return true;
        }

        var trialExpired = DateTime.Now > trialStartDate.AddDays(trialDays);

        if (trialExpired)
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Trial Expired",
                $"Your {trialDays}-day trial has expired. Subscribe to continue using {actionName}.",
                "Subscribe Now",
                "Exit");
                
            if (result)
            {
                var subscriptionService = new SubscriptionService();
                await subscriptionService.PurchasePremium();
            }
            else
            {
                Application.Current.Quit();
            }
            
            return false;
        }

        // Trial still active
        var daysLeft = (trialStartDate.AddDays(trialDays) - DateTime.Now).Days;
        if (daysLeft <= 2)
        {
            // Show reminder for last 2 days
            await Application.Current.MainPage.DisplayAlert(
                "Trial Ending Soon",
                $"Your trial expires in {daysLeft} day(s). Subscribe now to avoid interruption!",
                "Subscribe",
                "Remind Later");
        }

        return true;
        */
    }

    /// <summary>
    /// STRATEGY 4: Freemium Model - Some features free, others premium
    /// </summary>
    private static async Task<bool> HandleFreemiumModel(string actionName, ActionType actionType)
    {
        // Define your freemium limits
        switch (actionType)
        {
            case ActionType.AddBasicExpense:
                return await CheckMonthlyExpenseLimit();
                
            case ActionType.ViewRecentExpenses:
                return true; // Always allowed
                
            case ActionType.GenerateReports:
            case ActionType.ExportData:
            case ActionType.AdvancedSettings:
            case ActionType.CloudBackup:
                return await ShowPremiumUpgradeDialog(actionName);
                
            default:
                return true; // Allow by default
        }
    }

    /// <summary>
    /// Check monthly expense limit for freemium users
    /// </summary>
    private static async Task<bool> CheckMonthlyExpenseLimit()
    {
        // This would need to be implemented based on your data access
        // For now, return true to allow
        return true;
        
        /*
        var currentMonth = DateTime.Now.ToString("yyyy-MM");
        var monthlyCount = GetExpenseCountForMonth(currentMonth);
        var freeLimit = 50;
        
        if (monthlyCount >= freeLimit)
        {
            return await ShowPremiumUpgradeDialog($"adding more than {freeLimit} expenses per month");
        }
        
        return true;
        */
    }

    /// <summary>
    /// Show premium upgrade dialog
    /// </summary>
    private static async Task<bool> ShowPremiumUpgradeDialog(string featureName)
    {
        var result = await Application.Current.MainPage.DisplayAlert(
            "Premium Feature",
            $"{featureName} is available with Premium subscription. Upgrade now to unlock all features!",
            "Upgrade Now",
            "Maybe Later");
            
        if (result)
        {
            var subscriptionService = new SubscriptionService();
            var purchaseResult = await subscriptionService.PurchasePremium();
            return purchaseResult.Success;
        }
        
        return false;
    }

    /// <summary>
    /// Action types for categorizing different app functions
    /// </summary>
    public enum ActionType
    {
        AddBasicExpense,
        ViewRecentExpenses,
        GenerateReports,
        ExportData,
        AdvancedSettings,
        CloudBackup,
        BasicSettings,
        ViewAllExpenses
    }
}

/// <summary>
/// Extension methods for easy subscription checking
/// </summary>
public static class SubscriptionBlockerExtensions
{
    /// <summary>
    /// Easy check for adding expenses
    /// </summary>
    public static async Task<bool> CanAddExpense()
    {
        return await SubscriptionBlocker.CanPerformAction("add expenses", SubscriptionBlocker.ActionType.AddBasicExpense);
    }

    /// <summary>
    /// Easy check for generating reports
    /// </summary>
    public static async Task<bool> CanGenerateReports()
    {
        return await SubscriptionBlocker.CanPerformAction("generate reports", SubscriptionBlocker.ActionType.GenerateReports);
    }

    /// <summary>
    /// Easy check for exporting data
    /// </summary>
    public static async Task<bool> CanExportData()
    {
        return await SubscriptionBlocker.CanPerformAction("export data", SubscriptionBlocker.ActionType.ExportData);
    }
}
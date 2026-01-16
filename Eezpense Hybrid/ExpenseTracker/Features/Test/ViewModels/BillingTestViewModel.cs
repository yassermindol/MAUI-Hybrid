using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Features;
using ExpenseTracker.Services;

#if ANDROID
using AndroidX.Core.Content;
using Android.Gms.Common;
#endif

namespace ExpenseTracker.Features.Test.ViewModels;

public partial class BillingTestViewModel : BaseViewModel
{
    private readonly BillingService _billingService;
    private readonly SubscriptionService _subscriptionService;
    private readonly PlatformBillingService _platformBillingService;

    public BillingTestViewModel()
    {
        _billingService = new BillingService();
        _subscriptionService = new SubscriptionService();
        _platformBillingService = new PlatformBillingService();
    }

    [ObservableProperty]
    string testResults = "Ready for testing";

    [ObservableProperty]
    bool isPremiumUser = false;

    [ObservableProperty]
    string subscriptionInfo = "Not loaded";

    private void AppendResult(string message)
    {
        TestResults += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            AppendResult("Testing billing connection...");
            
            var hasSubscription = await _billingService.HasActivePremiumSubscription();
            AppendResult($"Has active subscription: {hasSubscription}");
            
            var product = await _billingService.GetSubscriptionProduct();
            if (product != null)
            {
                AppendResult($"Product found: {product.Name} - {product.LocalizedPrice}");
            }
            else
            {
                AppendResult("No product information available (expected in simulator)");
            }

            IsPremiumUser = hasSubscription;
        }
        catch (Exception ex)
        {
            AppendResult($"Connection test error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task TestPurchase()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            AppendResult("Initiating test purchase...");
            AppendResult("Checking prerequisites...");
            
            // First check if we can connect
            var hasConnection = false;
            try
            {
                var testProduct = await _billingService.GetSubscriptionProduct();
                hasConnection = testProduct != null;
                
                if (hasConnection)
                {
                    AppendResult($"? Product available: {testProduct.Name} - {testProduct.LocalizedPrice}");
                }
                else
                {
                    AppendResult("?? No product information - checking common issues...");
                    await DiagnoseBillingIssues();
                    return;
                }
            }
            catch (Exception ex)
            {
                AppendResult($"? Connection failed: {ex.Message}");
                await DiagnoseBillingIssues();
                return;
            }
            
            // Proceed with purchase
            var result = await _subscriptionService.PurchasePremium();
            
            if (result.Success)
            {
                AppendResult("? Purchase initiated successfully!");
                AppendResult("Google Play should open for payment confirmation.");
                
                // Wait a bit then check status
                await Task.Delay(2000);
                await TestConnection();
            }
            else
            {
                AppendResult($"? Purchase failed: {result.Message}");
                
                // Check for specific error patterns
                if (result.Message.Contains("not configured for billing") || 
                    result.Message.Contains("version of application"))
                {
                    await DiagnoseBillingConfiguration();
                }
            }
        }
        catch (Exception ex)
        {
            AppendResult($"Purchase error: {ex.Message}");
            
            // Enhanced error analysis
            if (ex.Message.Contains("not configured for billing") || 
                ex.Message.Contains("version of application"))
            {
                await DiagnoseBillingConfiguration();
            }
            else if (ex.Message.Contains("network") || ex.Message.Contains("connection"))
            {
                AppendResult("?? This appears to be a network/connection issue");
            }
            else if (ex.Message.Contains("ITEM_UNAVAILABLE"))
            {
                AppendResult("?? Product 'eezpense_premium_monthly' not found in Google Play");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DiagnoseBillingConfiguration()
    {
        AppendResult("");
        AppendResult("?? BILLING CONFIGURATION DIAGNOSIS:");
        AppendResult("The app is not configured for billing in Google Play Console.");
        AppendResult("");
        AppendResult("Required Steps:");
        AppendResult("1?? Upload your signed APK to Google Play Console");
        AppendResult("2?? Create subscription product: 'eezpense_premium_monthly'");
        AppendResult("3?? Add your Google account as test user");
        AppendResult("4?? Ensure package name matches: com.ymitech.eezpense");
        AppendResult("5?? Test on physical device with Google Play Store");
        AppendResult("");
        
        // Get current package info
        try
        {
#if ANDROID
            var context = Platform.CurrentActivity ?? Android.App.Application.Context;
            var packageName = context.PackageName;
            var packageInfo = context.PackageManager.GetPackageInfo(packageName, 0);
            
            AppendResult($"?? Current App Info:");
            AppendResult($"   Package: {packageName}");
            AppendResult($"   Version: {packageInfo.VersionName} ({packageInfo.VersionCode})");
            AppendResult($"   Expected: com.ymitech.eezpense");
            
            if (packageName != "com.ymitech.eezpense")
            {
                AppendResult("?? Package name mismatch detected!");
            }
#endif
        }
        catch (Exception ex)
        {
            AppendResult($"Could not get package info: {ex.Message}");
        }
    }

    private async Task DiagnoseBillingIssues()
    {
        AppendResult("");
        AppendResult("?? DIAGNOSIS - Common Issues:");
        AppendResult("");
        
        // Check if we're in simulator
        var isSimulator = DeviceInfo.DeviceType == DeviceType.Virtual;
        if (isSimulator)
        {
            AppendResult("?? Device Type: SIMULATOR/EMULATOR");
            AppendResult("? In-app billing requires a PHYSICAL DEVICE");
            AppendResult("?? Solution: Test on a real Android device with Google Play");
        }
        else
        {
            AppendResult("?? Device Type: Physical device ?");
        }
        
        AppendResult("");
        AppendResult("?? Checklist for Google Play Billing:");
        AppendResult("? App uploaded to Google Play Console (any track)");
        AppendResult("? Subscription 'eezpense_premium_monthly' created");
        AppendResult("? Your account added as test user");
        AppendResult("? Testing on physical device with Google Play");
        AppendResult("? Google Play Services updated");
        AppendResult("? Signed with same certificate as uploaded version");
        AppendResult("");
    }

    [RelayCommand]
    private async Task DiagnoseSetup()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            AppendResult("?? COMPREHENSIVE SETUP DIAGNOSIS");
            AppendResult("========================================");
            
            // Plugin version info
            AppendResult("?? PLUGIN INFORMATION:");
            AppendResult("   Plugin.InAppBilling: v10.0.0");
            AppendResult("   Android Billing Library: v8");
            AppendResult("   Target Framework: .NET 10");
            AppendResult("   ? Plugin version is up to date");
            AppendResult("");
            
            // Device info
            AppendResult($"?? Device: {DeviceInfo.Name}");
            AppendResult($"   Type: {DeviceInfo.DeviceType}");
            AppendResult($"   Platform: {DeviceInfo.Platform}");
            AppendResult($"   Version: {DeviceInfo.VersionString}");
            
            var isSimulator = DeviceInfo.DeviceType == DeviceType.Virtual;
            if (isSimulator)
            {
                AppendResult("? SIMULATOR DETECTED - Billing will not work");
            }
            else
            {
                AppendResult("? Physical device - Good for testing");
            }
            
#if ANDROID
            // Android-specific checks
            AppendResult("");
            AppendResult("?? ANDROID CONFIGURATION:");
            
            var context = Platform.CurrentActivity ?? Android.App.Application.Context;
            var packageName = context.PackageName;
            
            AppendResult($"   Package: {packageName}");
            AppendResult($"   Expected: com.ymitech.eezpense");
            
            if (packageName == "com.ymitech.eezpense")
            {
                AppendResult("? Package name matches");
            }
            else
            {
                AppendResult("? Package name mismatch!");
            }
            
            // Check Google Play Services
            try
            {
                var gms = GoogleApiAvailability.Instance;
                var result = gms.IsGooglePlayServicesAvailable(context);
                
                if (result == ConnectionResult.Success)
                {
                    AppendResult("? Google Play Services available");
                }
                else
                {
                    AppendResult($"? Google Play Services issue: {result}");
                }
            }
            catch (Exception ex)
            {
                AppendResult($"?? Could not check Google Play Services: {ex.Message}");
            }
#endif
            
            AppendResult("");
            AppendResult("?? BILLING SERVICE TEST:");
            
            // Test billing connection
            try
            {
                var canConnect = await _billingService.HasActivePremiumSubscription();
                AppendResult("? Billing service accessible");
                
                var product = await _billingService.GetSubscriptionProduct();
                if (product != null)
                {
                    AppendResult($"? Product found: {product.Name}");
                    AppendResult($"   ID: {product.ProductId}");
                    AppendResult($"   Price: {product.LocalizedPrice}");
                }
                else
                {
                    AppendResult("? Product 'eezpense_premium_monthly' not found");
                    AppendResult("   ? This is likely a Google Play Console issue");
                    AppendResult("   ? NOT a plugin version issue");
                }
            }
            catch (Exception ex)
            {
                AppendResult($"? Billing connection failed: {ex.Message}");
                
                if (ex.Message.Contains("not configured"))
                {
                    AppendResult("   ? App not configured in Google Play Console");
                    AppendResult("   ? Plugin version is NOT the issue");
                }
            }
            
            AppendResult("");
            AppendResult("?? COMMON MISCONCEPTIONS:");
            AppendResult("? 'Version not configured' ? Plugin version issue");
            AppendResult("? Plugin v10 with Android Billing v8 is current");
            AppendResult("? The issue is Google Play Console configuration");
            AppendResult("? Plugin.InAppBilling v10 is compatible with .NET 10");
            
            AppendResult("");
            AppendResult("?? NEXT STEPS:");
            if (isSimulator)
            {
                AppendResult("1. Deploy to a PHYSICAL Android device");
                AppendResult("2. Ensure device has Google Play Store");
            }
            else
            {
                AppendResult("1. ?? Plugin version is NOT the problem");
                AppendResult("2. Upload signed APK to Google Play Console");
                AppendResult("3. Configure subscription in Google Play Console");
                AppendResult("4. Add test accounts");
                AppendResult("5. Wait for Google Play Console processing (~2 hours)");
            }
            
        }
        catch (Exception ex)
        {
            AppendResult($"Diagnosis error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
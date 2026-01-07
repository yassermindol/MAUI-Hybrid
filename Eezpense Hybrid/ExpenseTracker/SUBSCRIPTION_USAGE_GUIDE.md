## How to Query Your Subscription: `eezpense_premium_monthly`

Your Google Play Console shows a subscription with product ID `eezpense_premium_monthly`. Here's how to query and manage it:

### 1. **Check if User has Active Subscription**

```csharp
var subscriptionService = new SubscriptionService();
bool isPremium = await subscriptionService.IsPremiumUser();

if (isPremium)
{
    // User has active premium subscription
    // Enable premium features
}
```

### 2. **Get Detailed Subscription Information**

```csharp
var subscriptionService = new SubscriptionService();
var info = await subscriptionService.GetSubscriptionInfo();

if (info != null)
{
    Console.WriteLine($"Product: {info.ProductName}");
    Console.WriteLine($"Price: {info.LocalizedPrice}");
    Console.WriteLine($"Active: {info.HasActiveSubscription}");
    Console.WriteLine($"State: {info.PurchaseState}");
    Console.WriteLine($"Purchase Date: {info.PurchaseDate}");
}
```

### 3. **Purchase Premium Subscription**

```csharp
var subscriptionService = new SubscriptionService();
var result = await subscriptionService.PurchasePremium();

if (result.Success)
{
    // Purchase initiated successfully
    // The actual purchase might still be pending
}
else
{
    // Show error message
    Console.WriteLine($"Purchase failed: {result.Message}");
}
```

### 4. **Direct Billing Service Usage**

For more advanced operations, use `BillingService` directly:

```csharp
var billingService = new BillingService();

// Get all subscription purchases
var purchases = await billingService.GetAllSubscriptionPurchases();

foreach (var purchase in purchases)
{
    Console.WriteLine($"Product ID: {purchase.ProductId}");
    Console.WriteLine($"State: {purchase.State}");
    Console.WriteLine($"Transaction Date: {purchase.TransactionDateUtc}");
    Console.WriteLine($"Purchase Token: {purchase.PurchaseToken}");
}
```

### 5. **MainActivity Integration**

Your `MainActivity` now automatically:
- Checks subscription status on app startup
- Handles pending purchases
- Attempts to acknowledge completed purchases (if supported by Plugin version)
- Sets up purchase update callbacks

### 6. **Example in ViewModel**

```csharp
public class YourViewModel : BaseViewModel
{
    private readonly SubscriptionService _subscriptionService;
    
    public YourViewModel()
    {
        _subscriptionService = new SubscriptionService();
    }
    
    [RelayCommand]
    private async Task CheckSubscription()
    {
        var isPremium = await _subscriptionService.IsPremiumUser();
        
        if (isPremium)
        {
            // Show premium features
            await ShowMessage("Premium", "You have premium access!");
        }
        else
        {
            // Show upgrade option
            var result = await ShowMessage("Upgrade", "Would you like to upgrade to premium?", "Yes", "No");
            if (result)
            {
                await PurchasePremium();
            }
        }
    }
    
    private async Task PurchasePremium()
    {
        var result = await _subscriptionService.PurchasePremium();
        
        if (result.Success)
        {
            await ShowMessage("Success", "Premium purchase initiated!");
        }
        else
        {
            await ShowMessage("Error", result.Message);
        }
    }
}
```

### 7. **HomeViewModel Integration**

Your HomeViewModel now has:
- `IsPremiumUser` property
- `SubscriptionStatus` property  
- `CheckPremiumStatus()` method
- `PurchasePremiumCommand`

Usage in your HomePage:

```xml
<Button Text="{Binding SubscriptionStatus}" 
        Command="{Binding PurchasePremiumCommand}" 
        IsVisible="{Binding IsPremiumUser, Converter={StaticResource InverseBoolConverter}}" />

<Label Text="Premium User!" 
       IsVisible="{Binding IsPremiumUser}" />
```

### 8. **Testing Requirements**

To test this properly, you need:

1. **Physical Android device** with Google Play Store
2. **Google account** added as a test account in Google Play Console
3. **Signed APK** uploaded to Google Play Console (Internal Testing track)
4. **Test subscription** set up in Google Play Console

### 9. **Key Points**

- Product ID: `eezpense_premium_monthly`
- Subscription Type: `ItemType.Subscription`
- Purchase acknowledgment depends on Plugin.InAppBilling version
- Handle pending payments with callbacks
- Test on real device with Google Play Store
- Billing permission is added to AndroidManifest.xml
- Services return `false` in simulator (expected behavior)

### 10. **Available Properties in InAppBillingPurchase**

Based on Plugin.InAppBilling, you can access:
- `ProductId` - Your subscription ID
- `State` - Purchase state (Purchased, Pending, etc.)
- `PurchaseToken` - Token for acknowledgment
- `TransactionDateUtc` - When the purchase was made
- `ProductIds` - List of product IDs
- `AutoRenewing` - Whether subscription auto-renews

### 11. **Testing in Simulator**

The connection will fail in simulators, which is normal. The services are designed to handle this gracefully and return appropriate defaults.
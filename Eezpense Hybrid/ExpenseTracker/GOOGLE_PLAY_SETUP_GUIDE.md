# Google Play Console Setup Guide for In-App Billing

## Current Error: "version of application is not configured for billing through Google Play"

This error occurs because your app needs to be properly configured in Google Play Console. Follow these steps:

## Step 1: Prepare Your App for Upload

### 1.1 Create a Signed APK/AAB
```bash
# In Visual Studio or Command Line
dotnet publish -f net9.0-android -c Release
```

### 1.2 Verify Package Information
Your app should have:
- **Package Name**: `com.ymitech.eezpense` (matches AndroidManifest.xml)
- **Version Code**: Should increment with each upload
- **Signing**: Must be signed with your keystore

## Step 2: Google Play Console Setup

### 2.1 Create/Access Your App
1. Go to [Google Play Console](https://play.google.com/console)
2. Create new app or access existing: "Eezpense"
3. Set package name: `com.ymitech.eezpense`

### 2.2 Upload App Bundle/APK
1. Navigate to **Release > Internal testing**
2. Create new release
3. Upload your signed APK/AAB
4. Complete release notes
5. **Save and Publish** (even just to internal testing)

### 2.3 Configure In-App Products
1. Navigate to **Monetize > Products > Subscriptions**
2. Click **Create subscription**
3. Fill in details:
   ```
   Product ID: eezpense_premium_monthly
   Name: Premium Monthly Subscription
   Description: Premium features for Eezpense
   Price: Set your desired price (e.g., $4.99)
   Billing period: Monthly (1 month)
   Free trial: Optional
   ```
4. **Save and Activate** the subscription

## Step 3: Configure Test Accounts

### 3.1 Add Test Accounts
1. Navigate to **Setup > License testing**
2. Add your Gmail account to **License testers**
3. Set test response to **RESPOND_NORMALLY**

### 3.2 Internal Testing Track
1. Navigate to **Release > Internal testing**
2. **Manage testers** ? Add your Gmail account
3. Save changes

## Step 4: App Configuration Verification

### 4.1 AndroidManifest.xml Check
Ensure you have:
```xml
<uses-permission android:name="com.android.vending.BILLING" />
```

### 4.2 Package Name Consistency
- AndroidManifest.xml: `com.ymitech.eezpense` ?
- Google Play Console: Must match exactly
- Your test shows current package name

## Step 5: Testing Setup

### 5.1 Device Requirements
- **Physical Android device** (not emulator)
- **Google Play Store** installed and updated
- **Google Play Services** updated
- Signed in with **test account**

### 5.2 App Installation
1. Install your app from Google Play Console internal testing link, OR
2. Install signed APK directly (same signature as uploaded)

## Step 6: Common Issues & Solutions

### Issue: "Application not configured for billing"
**Solutions:**
- Wait 2-4 hours after Google Play Console changes
- Verify package names match exactly
- Ensure subscription product is **Active** in console
- Test account must be added to internal testing

### Issue: "Item not available"
**Solutions:**
- Product ID must be exactly: `eezpense_premium_monthly`
- Subscription must be **Active** (not Draft)
- App must be published to at least internal testing

### Issue: Connection fails
**Solutions:**
- Use physical device with Google Play
- Ensure Google Play Services updated
- Check internet connection
- Verify signed APK matches uploaded version

## Step 7: Verification Steps

Use your **BillingTestPage** to verify:

1. **Run Diagnosis** ? Should show physical device ?
2. **Test Connection** ? Should find your product
3. **Test Purchase** ? Should open Google Play dialog

### Expected Results After Setup:
```
?? Device Type: Physical device ?
?? Package: com.ymitech.eezpense ?
? Google Play Services available
? Product found: Premium Monthly Subscription
   ID: eezpense_premium_monthly
   Price: $4.99
```

## Step 8: Testing the Purchase Flow

### Successful Test Purchase Flow:
1. Click "Test Purchase"
2. Google Play billing dialog opens
3. Use test credit card or select test account
4. Purchase completes
5. App receives purchase confirmation

### Test Credit Cards:
- **Visa**: 4242 4242 4242 4242
- **Mastercard**: 5555 5555 5555 4444
- Any future date for expiry
- Any 3-digit CVV

## Troubleshooting Timeline

- **Immediate**: Upload app, create subscription
- **~30 minutes**: Product becomes available for testing
- **~2-4 hours**: Full billing configuration active
- **~24 hours**: Maximum time for all changes to propagate

## Quick Checklist

- [ ] App uploaded to Google Play Console (any track)
- [ ] Subscription `eezpense_premium_monthly` created and **Active**
- [ ] Test account added to License testing
- [ ] Test account added to Internal testing track
- [ ] Testing on physical Android device
- [ ] Device has Google Play Store and Services
- [ ] Package name matches: `com.ymitech.eezpense`
- [ ] App signed with same certificate as uploaded

After completing these steps, your billing should work and the error should be resolved.
using ExpenseTracker.Features.Test.ViewModels;

namespace ExpenseTracker.Features.Test;

public partial class BillingTestPage : ContentPage
{
    public BillingTestPage()
    {
        InitializeComponent();
        BindingContext = new BillingTestViewModel();
    }
}
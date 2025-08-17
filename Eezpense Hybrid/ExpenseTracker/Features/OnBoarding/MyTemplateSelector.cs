using ExpenseTracker.Features.OnBoarding.Models;

namespace ExpenseTracker.Features.OnBoarding;
public class MyTemplateSelector : DataTemplateSelector
{
    public DataTemplate WelcomeTemplate { get; set; }
    public DataTemplate CurrencyTemplate { get; set; }
    public DataTemplate CategoryTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var slideData = item as SlideModel;
        if (slideData.Order == 1)
            return WelcomeTemplate;
        else if (slideData.Order == 2)
            return CurrencyTemplate;
        else if (slideData.Order == 3)
            return CategoryTemplate;
        // Handle default or fallback case if needed
        return null;
    }
}
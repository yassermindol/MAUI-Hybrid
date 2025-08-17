using ExpenseTracker.Features.Templates;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Models;

namespace ExpenseTracker.Features.Daily;


public class DailyTemplateSelector : DataTemplateSelector
{
    public DataTemplate ParentTemplate { get; set; }
    public DataTemplate ChildTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is UiDayItem model)
        {
            // Return the appropriate DataTemplate based on the model's property  
            switch (model.ItemType)
            {
                case ExpenseItemType.Header:
                    return ParentTemplate;
                case ExpenseItemType.ExpenseItem:
                    return ChildTemplate;
            }
        }
        return null;
    }
}
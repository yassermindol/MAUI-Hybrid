using ExpenseTracker.Models;
using ExpenseTracker.Models.UI;

namespace ExpenseTracker.Features.Templates;
public class MyDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate ExpenseHeaderTemplate { get; set; }

    public DataTemplate ExpenseTemplate { get; set; }    

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is UiExpenseItem model)
        {
            // Return the appropriate DataTemplate based on the model's property  
            switch (model.ItemType)
            {
                case ExpenseItemType.Header:
                    return ExpenseHeaderTemplate;
                case ExpenseItemType.ExpenseItem:
                    return ExpenseTemplate;
            }
        }
        return null;
    }
}


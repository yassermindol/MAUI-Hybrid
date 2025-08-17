using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ExpenseTracker.EventMessages.ExpenseCategoryEvents
{
    public class RestoreExpenseCategoryMessage : ValueChangedMessage<string>
    {
        public RestoreExpenseCategoryMessage(string categoryName) : base(categoryName)
        {
        }
    }
}

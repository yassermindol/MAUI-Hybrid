using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ExpenseTracker.EventMessages.ExpenseCategoryEvents
{
    public class AddExpenseCategoryMessage : ValueChangedMessage<string>
    {
        public AddExpenseCategoryMessage(string categoryName) : base(categoryName)
        {
        }
    }
}

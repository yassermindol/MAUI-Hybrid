using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ExpenseTracker.EventMessages.ExpenseCategoryEvents
{
    public class DeleteExpenseCategoryMessage : ValueChangedMessage<string>
    {
        public DeleteExpenseCategoryMessage(string categoryName) : base(categoryName)
        {
        }
    }
}

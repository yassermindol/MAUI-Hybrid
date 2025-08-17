using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ExpenseTracker.EventMessages
{
    public class RestoreExpenseMessage : ValueChangedMessage<string>
    {
        public RestoreExpenseMessage(string message) : base(message)
        {
        }
    }
}

using CommunityToolkit.Mvvm.Messaging.Messages;
using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.EventMessages;

public class EditExpenseMessage : ValueChangedMessage<ExpenseEntity>
{
    public EditExpenseMessage(ExpenseEntity expense) : base(expense)
    {
    }
}

using CommunityToolkit.Mvvm.Messaging.Messages;
using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.EventMessages;

public class AddExpenseMessage : ValueChangedMessage<ExpenseEntity>
{
    public AddExpenseMessage(ExpenseEntity expense) : base(expense)
    {
    }
}

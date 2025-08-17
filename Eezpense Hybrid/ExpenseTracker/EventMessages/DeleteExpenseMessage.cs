using CommunityToolkit.Mvvm.Messaging.Messages;
using ExpenseTracker.Models.DbEntities;

namespace ExpenseTracker.EventMessages;
public class DeleteExpenseMessage : ValueChangedMessage<ExpenseEntity>
{
    public DeleteExpenseMessage(ExpenseEntity expense) : base(expense)
    {
    }
}


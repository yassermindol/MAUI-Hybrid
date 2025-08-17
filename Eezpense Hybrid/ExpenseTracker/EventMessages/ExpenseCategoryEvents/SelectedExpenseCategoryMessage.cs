using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ExpenseTracker.Models;

namespace ExpenseTracker.EventMessages.ExpenseCategoryEvents;

public class SelectedExpenseCategoryMessage : ValueChangedMessage<string>
{
    public SelectedExpenseCategoryMessage(string value) : base(value)
    {
    }
}

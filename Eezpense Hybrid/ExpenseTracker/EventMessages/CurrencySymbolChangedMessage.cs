using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ExpenseTracker.EventMessages;

public class CurrencySymbolChangedMessage : ValueChangedMessage<string>
{
    public CurrencySymbolChangedMessage(string symbol) : base(symbol)
    {
    }
}


using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages.ExpenseCategoryEvents;
using ExpenseTracker.Features.Settings.ExpenseCategories.ViewModels;
using ExpenseTracker.Features.Settings.ViewModels;

namespace ExpenseTracker.EventMessages;
public class EventMessengerManager
{
    static Dictionary<string, object> _receivers = new();

    /// <summary>
    /// Use this method for children of a shell page because they are always created/instantiated when opened or re-opened.
    /// </summary>
    public static void Register(RestoreExpenseViewModel receiver, MessageHandler<object, DeleteExpenseMessage> messageHandler)
    {
        string key = nameof(RestoreExpenseViewModel) + "|" + nameof(DeleteExpenseMessage);
        if (_receivers.ContainsKey(key))
        {
            _receivers.TryGetValue(key, out var value);
            {
                WeakReferenceMessenger.Default.Unregister<DeleteExpenseMessage>(value);
                _receivers.Remove(key);
            }
        }

        _receivers.Add(key, receiver);
        WeakReferenceMessenger.Default.Register(receiver, messageHandler);
    }

    public static void Register(RestoreExpenseViewModel receiver, MessageHandler<object, CurrencySymbolChangedMessage> messageHandler)
    {
        string key = nameof(RestoreExpenseViewModel) + "|" + nameof(CurrencySymbolChangedMessage);
        if (_receivers.ContainsKey(key))
        {
            _receivers.TryGetValue(key, out var value);
            {
                WeakReferenceMessenger.Default.Unregister<CurrencySymbolChangedMessage>(value);
                _receivers.Remove(key);
            }
        }

        _receivers.Add(key, receiver);
        WeakReferenceMessenger.Default.Register(receiver, messageHandler);
    }

    public static void Register(DeleteCategoryViewModel receiver, MessageHandler<object, AddExpenseCategoryMessage> messageHandler)
    {
        string key = nameof(DeleteCategoryViewModel) + "|" + nameof(AddExpenseCategoryMessage);
        if (_receivers.ContainsKey(key))
        {
            _receivers.TryGetValue(key, out var value);
            {
                WeakReferenceMessenger.Default.Unregister<AddExpenseCategoryMessage>(value);
                _receivers.Remove(key);
            }
        }

        _receivers.Add(key, receiver);
        WeakReferenceMessenger.Default.Register(receiver, messageHandler);
    }

    public static void Register(RestoreCategoryViewModel receiver, MessageHandler<object, DeleteExpenseCategoryMessage> messageHandler)
    {
        string key = nameof(RestoreCategoryViewModel) + "|" + nameof(DeleteExpenseCategoryMessage);
        if (_receivers.ContainsKey(key))
        {
            _receivers.TryGetValue(key, out var value);
            {
                WeakReferenceMessenger.Default.Unregister<DeleteExpenseCategoryMessage>(value);
                _receivers.Remove(key);
            }
        }

        _receivers.Add(key, receiver);
        WeakReferenceMessenger.Default.Register(receiver, messageHandler);
    }

    public static void Register<TMessage>(object receiver, MessageHandler<object, TMessage> messageHandler)
    where TMessage : class
    {
        WeakReferenceMessenger.Default.Register(receiver, messageHandler);
    }

    public static void UnRegister<TMessage>(object receiver)
    where TMessage : class
    {
        WeakReferenceMessenger.Default.Unregister<TMessage>(receiver);
    }
}

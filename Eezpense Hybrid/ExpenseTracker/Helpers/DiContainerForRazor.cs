using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.EventMessages;
using ExpenseTracker.Features;

namespace ExpenseTracker.Helpers;

/// <summary>
/// DI Container Summary Details viewmodel
/// </summary>
public class DiContainerForRazor
{
    private static string lastAppearedParentPageVm = string.Empty;

    private static Dictionary<string, BaseViewModel> _viewModels = new();
    public static void RegisterViewModel(string parentPage, BaseViewModel viewModel)
    {
        if (_viewModels.ContainsKey(parentPage))
        {
            WeakReferenceMessenger.Default.Unregister<CurrencySymbolChangedMessage>(_viewModels[parentPage]);
            _viewModels.Remove(parentPage);
        }
            
        _viewModels.Add(parentPage, viewModel);
        lastAppearedParentPageVm = parentPage;
        //Console.WriteLine($"************ Registering viewmodel: {viewModel.GetType().Name} for parent:{parentPage}");
    }

    public static BaseViewModel GetViewModel(out string parent)
    {
        var viewModel = _viewModels[lastAppearedParentPageVm];
        if(viewModel != null)
            parent = lastAppearedParentPageVm;
        else
            parent = "No Parent Page";
        //Console.WriteLine($"************ Getting viewmodel: {viewModel.GetType().Name} for parent:{parent}");
        return viewModel;
    }
}
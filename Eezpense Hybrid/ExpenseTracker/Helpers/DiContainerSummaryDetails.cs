using ExpenseTracker.Features;
using ExpenseTracker.Features.DetailsOfSummaryReport.ViewModels;
using ExpenseTracker.Features.ViewModels;

namespace ExpenseTracker.Helpers;

/// <summary>
/// DI Container Summary Details viewmodel
/// </summary>
public class DiContainerSummaryDetails
{
    private static string lastAppearedParentPageVm = string.Empty;

    private static Dictionary<string, BaseViewModel> _viewModels = new();
    public static void RegisterViewModel(string parentPage, BaseViewModel viewModel)
    {
        if (_viewModels.ContainsKey(parentPage))
            _viewModels.Remove(parentPage);
        _viewModels.Add(parentPage, viewModel);
        lastAppearedParentPageVm = parentPage;
        Console.WriteLine($"************ Registering viewmodel: {viewModel.GetType().Name} for parent:{parentPage}");
    }

    public static BaseViewModel GetViewModel(out string parent)
    {
        var viewModel = _viewModels[lastAppearedParentPageVm];
        if(viewModel != null)
            parent = lastAppearedParentPageVm;
        else
            parent = "No Parent Page";
        Console.WriteLine($"************ Getting viewmodel: {viewModel.GetType().Name} for parent:{parent}");
        return viewModel;
    }
}
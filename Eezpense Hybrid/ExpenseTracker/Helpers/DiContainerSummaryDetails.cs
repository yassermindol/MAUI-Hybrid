using ExpenseTracker.Features.DetailsOfSummaryReport.ViewModels;

namespace ExpenseTracker.Helpers;

/// <summary>
/// DI Container Summary Details viewmodel
/// </summary>
public class DiContainerSummaryDetails
{
    private static string lastAppearedParentPageVm = string.Empty;

    private static Dictionary<string, SummaryDetailsViewModel> _viewModels = new();
    public static void RegisterViewModel(string parentPage, SummaryDetailsViewModel viewModel)
    {
        if (_viewModels.ContainsKey(parentPage))
            _viewModels.Remove(parentPage);
        _viewModels.Add(parentPage, viewModel);
        lastAppearedParentPageVm = parentPage;
    }

    public static SummaryDetailsViewModel GetViewModel(out string parent)
    {
        var viewModel = _viewModels[lastAppearedParentPageVm];
        if(viewModel != null)
            parent = lastAppearedParentPageVm;
        else
            parent = "No Parent Page";
        return viewModel;
    }
}


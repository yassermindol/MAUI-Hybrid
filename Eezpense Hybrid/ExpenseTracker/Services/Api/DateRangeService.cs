using ExpenseTracker.Models;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Models.UI;
using ExpenseTracker.Services.UIModelGenerators;
using System.Collections.ObjectModel;

namespace ExpenseTracker.Services.Api;

public class DateRangeService : BaseApiService
{
    /// <summary>
    /// Sets the PieChart and UiExpenses properties. Use or access the values after this call.
    /// </summary>
    public async Task<double> SetData(DateTime startDate, DateTime endDate)
    {
        var expenses = await GetExpenses(startDate, endDate);
        ExpenseEntities = expenses;
        UiPieChartDataProvider chartProvider = new UiPieChartDataProvider();
        PieChart = chartProvider.GetPieChart(expenses);

        //UiListDataProvider uiListProvider = new UiListDataProvider();
        //UiExpenses = uiListProvider.GetUiExpenses(expenses);
        return expenses.Sum(x => x.Amount);
    }


    public PieChart PieChart { get; private set; }
    public ObservableCollection<UiExpenseItem> UiExpenses { get; private set; }
    public List<ExpenseEntity> ExpenseEntities { get; private set; }
}

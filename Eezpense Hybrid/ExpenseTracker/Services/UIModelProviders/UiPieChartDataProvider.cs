
using ExpenseTracker.ExtensionMethods;
using ExpenseTracker.Models.DbEntities;
using OxyPlot;
using OxyPlot.Series;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services.UIModelGenerators;

public class UiPieChartDataProvider
{
    public PieChart GetPieChart(List<ExpenseEntity> expenses)
    {
        var isDarkMode = App.IsDarkMode;
        var model = new PlotModel();
        var ps = new PieSeries
        {
            TextColor = isDarkMode? Colors.White.ToOxyColor(): Colors.Black.ToOxyColor(),
            InsideLabelColor = OxyColors.Black,
            StrokeThickness = .75,
            InsideLabelPosition = .6,
            AngleSpan = 360,
            StartAngle = 0,
            Diameter = .8,
            AreInsideLabelsAngled = true,
            LegendFormat = "",
        };

        var slices = ps.Slices;
        var pieSummary = GetPieSummary(expenses);

        int i = 0;
        ColorService colorService = new ColorService();
        foreach (var item in pieSummary)
        {
            slices.Add(new MyPieSlice(item.Key, item.Value) { Fill = colorService.GetColor(i).ToOxyColor() });
            i++;
        }

        double total = slices.Sum(x => x.Value);
        int j = 0;
        var legends = new List<Legend>();
        foreach (MyPieSlice slice in slices)
        {
            double percentage = slice.Value / total * 100.00;
            slice.Percentage = Math.Round(percentage);
            var legend = new Legend
            {
                ExpenseCategory = slice.Label,
                TotalStr = slice.Value.ToMoney(),
                Total =  slice.Value,
                Percentage = slice.Percentage + "%",
                LegendColor = colorService.GetColor(j)
            };
            j++;
            legends.Add(legend);
        }
        slices = slices.OrderBy(x => x.Value).ToList();
        legends = legends.OrderBy(x => x.Total).ToList();
        legends.Reverse();
        slices = slices.Reverse().ToList();
        ps.Slices = slices;
        model.Series.Add(ps);

        return new PieChart
        {
            Legends = legends,
            PlotModel = model
        };
    }

    private Dictionary<string, double> GetPieSummary(List<ExpenseEntity> expenses)
    {
        var groups = expenses.GroupBy(x => x.Category);
        var pieSummary = new Dictionary<string, double>();
        foreach (var group in groups)
        {
            double sum = group.Sum(x => x.Amount);
            pieSummary.Add(group.Key, sum);
        }
        return pieSummary;
    }
}

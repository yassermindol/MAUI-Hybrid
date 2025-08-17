
using OxyPlot.Series;

namespace ExpenseTracker.Models;

public class MyPieSlice : PieSlice
{
    public MyPieSlice(string label, double value) : base(label, value)
    {
        
    }

    public double Percentage { get; set; }
}
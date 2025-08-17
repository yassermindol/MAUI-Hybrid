
namespace ExpenseTracker.Services;

public class BarWidthService
{
    double _maxWidthReference;
    double _maxAmount;

    public BarWidthService(double maxWidthReference, double maxAmount)
    {
        _maxWidthReference = maxWidthReference;
        _maxAmount = maxAmount;
    }

    public double GetWidth(double amount)
    {
        double width = (amount / _maxAmount) * _maxWidthReference;
        return width;
    }
}
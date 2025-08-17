
namespace ExpenseTracker.Services;

public class CalendarService
{
    public CalendarService()
    {
        CreateWeeksOfYear();
        CreateMonthList();
    }

    private void CreateWeeksOfYear()
    {
        int year = DateTime.Now.Year;
        DateTime startDate = new DateTime(year, 1, 1);
        DateTime endDate;

        if (startDate.DayOfWeek != DayOfWeek.Monday)
        {
            startDate = GetLastMondayOfLastYear(year - 1);
        }

        List<WeekOfYear> weeksOfYear = new List<WeekOfYear>();

        WeekOfYear wk = new WeekOfYear();
        wk.Number = 1;
        wk.StartDate = startDate;
        endDate = startDate.AddDays(6);
        wk.EndDate = endDate;
        weeksOfYear.Add(wk);

        DateTime endWeekDate = new DateTime(year, 12, 31);
        int i = 2;
        while (wk.EndDate < endWeekDate)
        {
            wk = new WeekOfYear();
            wk.Number = i;
            startDate = endDate.AddDays(1);
            wk.StartDate = startDate;
            endDate = startDate.AddDays(6);
            wk.EndDate = endDate;
            weeksOfYear.Add(wk);
            i++;
        }

        WeeksOfYear = weeksOfYear;
    }

    public List<WeekOfYear> WeeksOfYear { get; private set; }

    private DateTime GetLastMondayOfLastYear(int year)
    {
        DateTime lastMonday = DateTime.Now;
        for (int i = 31; i >= 25; i--)
        {
            var dt = new DateTime(year, 12, i);
            if (dt.DayOfWeek == DayOfWeek.Monday)
            {
                lastMonday = dt;
                break;
            }
        }

        return lastMonday;
    }

    public WeekOfYear GetWeekOfYear(DateTime date)
    {
        DateTime dt = date.Date;
        foreach (var wk in WeeksOfYear)
        {
            int startDateComparison = DateTime.Compare(dt, wk.StartDate);

            if (startDateComparison >= 0)
            {
                int endDateComparison = DateTime.Compare(dt, wk.EndDate);
                if (endDateComparison <= 0)
                {
                    return wk;
                }
            }
        }

        return null;
    }

    Dictionary<int, string> months = new Dictionary<int, string>();
    public Dictionary<int, string> Months { get => months; }
    private void CreateMonthList()
    {
        months.Add(1, "January");
        months.Add(2, "February");
        months.Add(3, "March");
        months.Add(4, "April");
        months.Add(5, "May");
        months.Add(6, "June");
        months.Add(7, "July");
        months.Add(8, "August");
        months.Add(9, "September");
        months.Add(10, "October");
        months.Add(11, "November");
        months.Add(12, "December");
    }
}

public class WeekOfYear
{
    public int Number { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string StartDateStr { get => StartDate.ToString(); }
    public string EndDateStr { get => EndDate.ToString(); }
}
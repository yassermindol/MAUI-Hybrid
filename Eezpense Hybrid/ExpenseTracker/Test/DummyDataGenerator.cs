using ExpenseTracker.Features;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Services;

namespace ExpenseTracker.Test;

public class DummyDataGenerator : BaseViewModel
{
    public List<ExpenseEntity> GenerateExpenses()
    {
        List<ExpenseEntity> expenses = new List<ExpenseEntity>();
        CalendarService calendar = new CalendarService();
        int year = DateTime.Now.Year;

        var categories = CategoryNamesDb;

        for (int month = 1; month <= 12; month++)
        {
            DateTime date = new DateTime(year, month, 1, 0, 0, 0);

            for (int day = 0; day < DateTime.DaysInMonth(year, month); day++)
            {
                //Add food
                ExpenseEntity e = new ExpenseEntity();
                e.Date = date;
                Random random = new Random();
                int amt = random.Next(200, 400);
                e.Amount = amt;
                e.Category = categories[0];
                e.WeekNumber = calendar.GetWeekOfYear(date).Number;
                e.Note = $"{categories[0]} for " + date;
                expenses.Add(e);

                //Add Transportation
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    e = new ExpenseEntity();
                    e.Date = date;
                    random = new Random();
                    e.Amount = random.Next(20, 100);
                    e.Category = categories[1];
                    e.WeekNumber = calendar.GetWeekOfYear(date).Number;
                    e.Note = $"{categories[1]} for " + date;
                    expenses.Add(e);
                }

                //Add Bills
                if (date.Day == 28)
                {
                    e = new ExpenseEntity();
                    e.Date = date;
                    random = new Random();
                    e.Amount = random.Next(1800, 2500);
                    e.Category = categories[2];
                    e.WeekNumber = calendar.GetWeekOfYear(date).Number;
                    e.Note = $"{categories[2]} for " + date;
                    expenses.Add(e);
                }

                //Add Miscellaneous
                if (date.Day % 5 == 0)
                {
                    e = new ExpenseEntity();
                    e.Date = date;
                    random = new Random();
                    e.Amount = random.Next(500, 2000);
                    e.Category = categories[3];
                    e.WeekNumber = calendar.GetWeekOfYear(date).Number;
                    e.Note = $"{categories[3]} for " + date;
                    expenses.Add(e);
                }

                date = date.AddDays(1);
            }
        }
        return expenses;
    }
}

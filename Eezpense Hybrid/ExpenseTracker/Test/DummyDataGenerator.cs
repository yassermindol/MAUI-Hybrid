using ExpenseTracker.Features;
using ExpenseTracker.Models.DbEntities;
using ExpenseTracker.Services;
using System.Text.Json;

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

    CalendarService _calendarService = new CalendarService();
    public void SeedDataBase()
    {
        if (ExpenseTableDb.GetAll().Count > 0)
            return;

        foreach (var item in Categories)
        {
            CategoryTableDb.Add(item);
        }       

        //LoadJson("ExpenseTracker.Test.Data.sample.json");
        foreach (var expense in Expenses)
        {
            var category = CategoriesDb.First(x => x.Name == expense.Category);
            int weekNumber = _calendarService.GetWeekOfYear(expense.Date).Number;
            var entity = new ExpenseEntity
            {
                Amount = expense.Amount,
                Note = expense.Note,
                Date = expense.Date,
                CategoryLocalID = category.ID,
                Category = category.Name,
                CategoryCentralID = category.CentralID,
                WeekNumber = weekNumber,
            };
            ExpenseTableDb.Add(entity);
        }
    }

    private List<string> Categories = new List<string>
    {
        "Food",
        "Entertainment",
        "Bills",
        "School",
        "Gas",
        "Grocery",
        "Miscellaneous"
    };

    private Expense LoadJson(string resourceName)
    {
        var assembly = typeof(MauiProgram).GetType().Assembly;
        // If you want to verify the resource name, you can inspect assembly.GetManifestResourceNames()
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new FileNotFoundException("Resource not found", resourceName);

        var result = JsonSerializer.Deserialize<Expense>(stream);
        return result;
    }

    private List<Expense> Expenses => GetExpenses();

    public class Expense
    {
        public string Category { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
    }

    private static List<Expense> GetExpenses()
    {
        return new List<Expense>
        {
            new Expense { Category = "Food", Amount = 16.26, Date = DateTime.Parse("2025-09-09 12:07") , Note = "Subway sandwich" },
            new Expense { Category = "Bills", Amount = 145.67, Date = DateTime.Parse("2025-09-09 09:14") , Note = "Cell phone bill" },
            new Expense { Category = "School", Amount = 223.08, Date = DateTime.Parse("2025-09-09 12:33") , Note = "Textbooks" },
            new Expense { Category = "Food", Amount = 141.55, Date = DateTime.Parse("2025-09-08 16:58") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Gas", Amount = 45.61, Date = DateTime.Parse("2025-09-08 18:45") , Note = "BP gas" },
            new Expense { Category = "Food", Amount = 15.77, Date = DateTime.Parse("2025-09-07 09:26") , Note = "Coffee & bagel" },
            new Expense { Category = "Gas", Amount = 45.33, Date = DateTime.Parse("2025-09-07 09:38") , Note = "Gas station fill-up" },
            new Expense { Category = "Food", Amount = 67.03, Date = DateTime.Parse("2025-09-06 18:36") , Note = "Costco grocery run" },
            new Expense { Category = "Food", Amount = 71.77, Date = DateTime.Parse("2025-09-06 16:33") , Note = "Safeway grocery" },
            new Expense { Category = "Food", Amount = 72.7, Date = DateTime.Parse("2025-09-05 16:36") , Note = "Costco grocery run" },
            new Expense { Category = "School", Amount = 224.85, Date = DateTime.Parse("2025-09-05 14:57") , Note = "Textbooks" },
            new Expense { Category = "Food", Amount = 22.53, Date = DateTime.Parse("2025-09-04 20:28") , Note = "Pizza delivery" },
            new Expense { Category = "Miscellaneous", Amount = 18.87, Date = DateTime.Parse("2025-09-04 16:34") , Note = "Gift wrap & card" },
            new Expense { Category = "Food", Amount = 132.59, Date = DateTime.Parse("2025-09-04 17:30") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Food", Amount = 92.56, Date = DateTime.Parse("2025-09-03 16:31") , Note = "Costco grocery run" },
            new Expense { Category = "Entertainment", Amount = 74.88, Date = DateTime.Parse("2025-09-03 19:10") , Note = "Netflix subscription" },
            new Expense { Category = "Food", Amount = 23.87, Date = DateTime.Parse("2025-09-02 13:15") , Note = "Subway sandwich" },
            new Expense { Category = "Bills", Amount = 84.86, Date = DateTime.Parse("2025-09-02 09:25") , Note = "Electric bill" },
            new Expense { Category = "Bills", Amount = 96.96, Date = DateTime.Parse("2025-09-02 10:30") , Note = "Water bill" },
            new Expense { Category = "Food", Amount = 101.08, Date = DateTime.Parse("2025-09-01 17:30") , Note = "Safeway grocery" },
            new Expense { Category = "Bills", Amount = 120.0, Date = DateTime.Parse("2025-09-01 11:27") , Note = "Internet bill" },
            new Expense { Category = "Food", Amount = 21.67, Date = DateTime.Parse("2025-08-31 12:33") , Note = "Lunch at Chipotle" },
            new Expense { Category = "Entertainment", Amount = 47.94, Date = DateTime.Parse("2025-08-31 19:34") , Note = "Concert ticket" },
            new Expense { Category = "Entertainment", Amount = 82.39, Date = DateTime.Parse("2025-08-31 19:01") , Note = "Movie night" },
            new Expense { Category = "Food", Amount = 108.42, Date = DateTime.Parse("2025-08-30 18:27") , Note = "Costco grocery run" },
            new Expense { Category = "Food", Amount = 132.21, Date = DateTime.Parse("2025-08-30 16:09") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Food", Amount = 135.86, Date = DateTime.Parse("2025-08-29 18:43") , Note = "Costco grocery run" },
            new Expense { Category = "Gas", Amount = 47.35, Date = DateTime.Parse("2025-08-29 16:18") , Note = "Shell gas refill" },
            new Expense { Category = "Food", Amount = 136.63, Date = DateTime.Parse("2025-08-28 18:14") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Miscellaneous", Amount = 22.31, Date = DateTime.Parse("2025-08-28 14:51") , Note = "Amazon order" },
            new Expense { Category = "Food", Amount = 108.9, Date = DateTime.Parse("2025-08-27 16:30") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Bills", Amount = 144.87, Date = DateTime.Parse("2025-08-27 09:17") , Note = "Internet bill" },
            new Expense { Category = "Food", Amount = 109.23, Date = DateTime.Parse("2025-08-26 17:49") , Note = "Aldi grocery" },
            new Expense { Category = "Gas", Amount = 37.64, Date = DateTime.Parse("2025-08-26 18:04") , Note = "Gas station fill-up" },
            new Expense { Category = "Entertainment", Amount = 57.81, Date = DateTime.Parse("2025-08-26 19:40") , Note = "Concert ticket" },
            new Expense { Category = "Food", Amount = 109.57, Date = DateTime.Parse("2025-08-25 18:57") , Note = "Safeway grocery" },
            new Expense { Category = "Food", Amount = 120.39, Date = DateTime.Parse("2025-08-25 18:05") , Note = "Costco grocery run" },
            new Expense { Category = "Food", Amount = 124.39, Date = DateTime.Parse("2025-08-24 16:46") , Note = "Safeway grocery" },
            new Expense { Category = "Gas", Amount = 42.52, Date = DateTime.Parse("2025-08-24 18:37") , Note = "Gas station fill-up" },
            new Expense { Category = "Miscellaneous", Amount = 20.21, Date = DateTime.Parse("2025-08-24 14:49") , Note = "Gift wrap & card" },
            new Expense { Category = "Food", Amount = 16.58, Date = DateTime.Parse("2025-08-23 19:45") , Note = "Dinner takeout (Chinese)" },
            new Expense { Category = "Entertainment", Amount = 56.38, Date = DateTime.Parse("2025-08-23 22:37") , Note = "Netflix subscription" },
            new Expense { Category = "Food", Amount = 122.55, Date = DateTime.Parse("2025-08-22 17:21") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Gas", Amount = 36.72, Date = DateTime.Parse("2025-08-22 09:29") , Note = "Shell gas refill" },
            new Expense { Category = "Entertainment", Amount = 77.46, Date = DateTime.Parse("2025-08-22 19:16") , Note = "Netflix subscription" },
            new Expense { Category = "Food", Amount = 71.48, Date = DateTime.Parse("2025-08-21 16:17") , Note = "Costco grocery run" },
            new Expense { Category = "Entertainment", Amount = 58.79, Date = DateTime.Parse("2025-08-21 20:17") , Note = "Concert ticket" },
            new Expense { Category = "Food", Amount = 16.17, Date = DateTime.Parse("2025-08-20 18:51") , Note = "Dinner at Olive Garden" },
            new Expense { Category = "Gas", Amount = 43.65, Date = DateTime.Parse("2025-08-20 16:30") , Note = "Shell gas refill" },
            new Expense { Category = "Food", Amount = 94.11, Date = DateTime.Parse("2025-08-19 18:20") , Note = "Aldi grocery" },
            new Expense { Category = "Food", Amount = 144.55, Date = DateTime.Parse("2025-08-19 18:57") , Note = "Aldi grocery" },
            new Expense { Category = "Food", Amount = 125.14, Date = DateTime.Parse("2025-08-18 17:09") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Food", Amount = 93.08, Date = DateTime.Parse("2025-08-18 17:34") , Note = "Costco grocery run" },
            new Expense { Category = "Food", Amount = 110.15, Date = DateTime.Parse("2025-08-18 18:05") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Food", Amount = 10.68, Date = DateTime.Parse("2025-08-17 19:14") , Note = "Dinner at Olive Garden" },
            new Expense { Category = "Entertainment", Amount = 32.21, Date = DateTime.Parse("2025-08-17 19:44") , Note = "Movie night" },
            new Expense { Category = "Food", Amount = 97.43, Date = DateTime.Parse("2025-08-17 16:40") , Note = "Safeway grocery" },
            new Expense { Category = "Food", Amount = 88.27, Date = DateTime.Parse("2025-08-16 18:24") , Note = "Costco grocery run" },
            new Expense { Category = "Bills", Amount = 128.7, Date = DateTime.Parse("2025-08-16 10:22") , Note = "Water bill" },
            new Expense { Category = "Food", Amount = 116.37, Date = DateTime.Parse("2025-08-15 17:40") , Note = "Safeway grocery" },
            new Expense { Category = "School", Amount = 66.36, Date = DateTime.Parse("2025-08-15 12:16") , Note = "Lab fees" },
            new Expense { Category = "Food", Amount = 121.4, Date = DateTime.Parse("2025-08-14 16:02") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Food", Amount = 94.16, Date = DateTime.Parse("2025-08-14 18:22") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Food", Amount = 64.76, Date = DateTime.Parse("2025-08-13 18:34") , Note = "Safeway grocery" },
            new Expense { Category = "Bills", Amount = 86.04, Date = DateTime.Parse("2025-08-13 10:57") , Note = "Cell phone bill" },
            new Expense { Category = "Miscellaneous", Amount = 28.01, Date = DateTime.Parse("2025-08-13 16:21") , Note = "Amazon order" },
            new Expense { Category = "Food", Amount = 116.13, Date = DateTime.Parse("2025-08-12 17:19") , Note = "Safeway grocery" },
            new Expense { Category = "Bills", Amount = 142.62, Date = DateTime.Parse("2025-08-12 09:44") , Note = "Internet bill" },
            new Expense { Category = "Food", Amount = 139.55, Date = DateTime.Parse("2025-08-11 17:27") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Miscellaneous", Amount = 30.89, Date = DateTime.Parse("2025-08-11 14:51") , Note = "Office supplies" },
            new Expense { Category = "Entertainment", Amount = 86.35, Date = DateTime.Parse("2025-08-11 21:23") , Note = "Spotify subscription" },
            new Expense { Category = "Food", Amount = 77.12, Date = DateTime.Parse("2025-08-10 17:44") , Note = "Safeway grocery" },
            new Expense { Category = "Miscellaneous", Amount = 12.88, Date = DateTime.Parse("2025-08-10 17:04") , Note = "Office supplies" },
            new Expense { Category = "School", Amount = 274.73, Date = DateTime.Parse("2025-08-10 15:15") , Note = "School supplies" },
            new Expense { Category = "Food", Amount = 22.09, Date = DateTime.Parse("2025-08-09 08:15") , Note = "Coffee & bagel" },
            new Expense { Category = "Gas", Amount = 43.8, Date = DateTime.Parse("2025-08-09 17:52") , Note = "Shell gas refill" },
            new Expense { Category = "Miscellaneous", Amount = 37.71, Date = DateTime.Parse("2025-08-09 14:29") , Note = "Amazon order" },
            new Expense { Category = "Food", Amount = 94.11, Date = DateTime.Parse("2025-08-08 17:02") , Note = "Safeway grocery" },
            new Expense { Category = "Miscellaneous", Amount = 26.78, Date = DateTime.Parse("2025-08-08 15:10") , Note = "Gift wrap & card" },
            new Expense { Category = "School", Amount = 256.4, Date = DateTime.Parse("2025-08-08 13:08") , Note = "School supplies" },
            new Expense { Category = "Food", Amount = 111.26, Date = DateTime.Parse("2025-08-07 16:20") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Food", Amount = 136.36, Date = DateTime.Parse("2025-08-07 18:34") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Food", Amount = 125.44, Date = DateTime.Parse("2025-08-06 16:07") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Entertainment", Amount = 71.23, Date = DateTime.Parse("2025-08-06 22:35") , Note = "Concert ticket" },
            new Expense { Category = "Gas", Amount = 40.08, Date = DateTime.Parse("2025-08-06 08:36") , Note = "Exxon gas" },
            new Expense { Category = "Food", Amount = 14.22, Date = DateTime.Parse("2025-08-05 19:58") , Note = "Pizza delivery" },
            new Expense { Category = "Miscellaneous", Amount = 23.87, Date = DateTime.Parse("2025-08-05 16:45") , Note = "Gift wrap & card" },
            new Expense { Category = "Food", Amount = 65.61, Date = DateTime.Parse("2025-08-04 17:56") , Note = "Safeway grocery" },
            new Expense { Category = "Entertainment", Amount = 29.33, Date = DateTime.Parse("2025-08-04 19:22") , Note = "Spotify subscription" },
            new Expense { Category = "Entertainment", Amount = 11.22, Date = DateTime.Parse("2025-08-04 20:40") , Note = "Spotify subscription" },
            new Expense { Category = "Food", Amount = 23.27, Date = DateTime.Parse("2025-08-03 12:26") , Note = "Lunch at Chipotle" },
            new Expense { Category = "Miscellaneous", Amount = 33.23, Date = DateTime.Parse("2025-08-03 13:46") , Note = "Gift wrap & card" },
            new Expense { Category = "Miscellaneous", Amount = 28.16, Date = DateTime.Parse("2025-08-03 16:13") , Note = "Office supplies" },
            new Expense { Category = "Food", Amount = 148.99, Date = DateTime.Parse("2025-08-02 18:52") , Note = "Walmart grocery shopping" },
            new Expense { Category = "Bills", Amount = 119.78, Date = DateTime.Parse("2025-08-02 10:34") , Note = "Cell phone bill" },
            new Expense { Category = "Food", Amount = 82.14, Date = DateTime.Parse("2025-08-01 16:27") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Food", Amount = 147.64, Date = DateTime.Parse("2025-08-01 16:43") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Food", Amount = 90.92, Date = DateTime.Parse("2025-07-31 17:02") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Bills", Amount = 105.6, Date = DateTime.Parse("2025-07-31 09:47") , Note = "Cell phone bill" },
            new Expense { Category = "Food", Amount = 9.66, Date = DateTime.Parse("2025-07-30 13:28") , Note = "Subway sandwich" },
            new Expense { Category = "Gas", Amount = 45.54, Date = DateTime.Parse("2025-07-30 16:49") , Note = "BP gas" },
            new Expense { Category = "Food", Amount = 79.42, Date = DateTime.Parse("2025-07-29 18:02") , Note = "Safeway grocery" },
            new Expense { Category = "Bills", Amount = 83.22, Date = DateTime.Parse("2025-07-29 10:55") , Note = "Water bill" },
            new Expense { Category = "Food", Amount = 109.85, Date = DateTime.Parse("2025-07-28 18:10") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Bills", Amount = 95.6, Date = DateTime.Parse("2025-07-28 09:21") , Note = "Internet bill" },
            new Expense { Category = "Food", Amount = 82.31, Date = DateTime.Parse("2025-07-27 16:16") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Entertainment", Amount = 32.75, Date = DateTime.Parse("2025-07-27 19:44") , Note = "Spotify subscription" },
            new Expense { Category = "Food", Amount = 16.52, Date = DateTime.Parse("2025-07-26 18:52") , Note = "Pizza delivery" },
            new Expense { Category = "Miscellaneous", Amount = 14.81, Date = DateTime.Parse("2025-07-26 14:43") , Note = "Home decor" },
            new Expense { Category = "Food", Amount = 9.85, Date = DateTime.Parse("2025-07-25 20:53") , Note = "Dinner takeout (Chinese)" },
            new Expense { Category = "Entertainment", Amount = 81.36, Date = DateTime.Parse("2025-07-25 19:21") , Note = "Spotify subscription" },
            new Expense { Category = "Food", Amount = 92.3, Date = DateTime.Parse("2025-07-24 17:17") , Note = "Walmart grocery shopping" },
            new Expense { Category = "Bills", Amount = 100.33, Date = DateTime.Parse("2025-07-24 11:19") , Note = "Water bill" },
            new Expense { Category = "Food", Amount = 12.81, Date = DateTime.Parse("2025-07-23 13:01") , Note = "Lunch at Chipotle" },
            new Expense { Category = "Gas", Amount = 36.21, Date = DateTime.Parse("2025-07-23 17:27") , Note = "Gas station fill-up" },
            new Expense { Category = "Miscellaneous", Amount = 38.24, Date = DateTime.Parse("2025-07-23 14:42") , Note = "Office supplies" },
            new Expense { Category = "Food", Amount = 111.23, Date = DateTime.Parse("2025-07-22 16:27") , Note = "Walmart grocery shopping" },
            new Expense { Category = "Food", Amount = 60.44, Date = DateTime.Parse("2025-07-22 16:58") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Food", Amount = 116.36, Date = DateTime.Parse("2025-07-21 18:39") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Miscellaneous", Amount = 36.3, Date = DateTime.Parse("2025-07-21 17:56") , Note = "Amazon order" },
            new Expense { Category = "Miscellaneous", Amount = 24.29, Date = DateTime.Parse("2025-07-21 16:42") , Note = "Office supplies" },
            new Expense { Category = "Food", Amount = 133.47, Date = DateTime.Parse("2025-07-20 16:04") , Note = "Aldi grocery" },
            new Expense { Category = "Food", Amount = 114.75, Date = DateTime.Parse("2025-07-20 18:03") , Note = "Aldi grocery" },
            new Expense { Category = "Food", Amount = 11.39, Date = DateTime.Parse("2025-07-19 20:56") , Note = "Dinner at Olive Garden" },
            new Expense { Category = "Gas", Amount = 45.11, Date = DateTime.Parse("2025-07-19 09:47") , Note = "Gas station fill-up" },
            new Expense { Category = "School", Amount = 235.85, Date = DateTime.Parse("2025-07-19 13:46") , Note = "Textbooks" },
            new Expense { Category = "Food", Amount = 144.67, Date = DateTime.Parse("2025-07-18 16:52") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Entertainment", Amount = 53.97, Date = DateTime.Parse("2025-07-18 22:09") , Note = "Netflix subscription" },
            new Expense { Category = "Food", Amount = 13.62, Date = DateTime.Parse("2025-07-17 18:34") , Note = "Pizza delivery" },
            new Expense { Category = "School", Amount = 77.17, Date = DateTime.Parse("2025-07-17 15:02") , Note = "School supplies" },
            new Expense { Category = "Food", Amount = 18.19, Date = DateTime.Parse("2025-07-16 20:18") , Note = "Dinner at Olive Garden" },
            new Expense { Category = "Entertainment", Amount = 34.49, Date = DateTime.Parse("2025-07-16 22:36") , Note = "Movie night" },
            new Expense { Category = "Food", Amount = 76.14, Date = DateTime.Parse("2025-07-15 18:26") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Gas", Amount = 48.0, Date = DateTime.Parse("2025-07-15 08:36") , Note = "Shell gas refill" },
            new Expense { Category = "Food", Amount = 88.93, Date = DateTime.Parse("2025-07-14 18:27") , Note = "Costco grocery run" },
            new Expense { Category = "Bills", Amount = 90.07, Date = DateTime.Parse("2025-07-14 10:08") , Note = "Internet bill" },
            new Expense { Category = "Food", Amount = 134.09, Date = DateTime.Parse("2025-07-14 17:16") , Note = "Walmart grocery shopping" },
            new Expense { Category = "Food", Amount = 61.94, Date = DateTime.Parse("2025-07-13 17:57") , Note = "Walmart grocery shopping" },
            new Expense { Category = "Entertainment", Amount = 72.71, Date = DateTime.Parse("2025-07-13 20:47") , Note = "Concert ticket" },
            new Expense { Category = "Food", Amount = 11.82, Date = DateTime.Parse("2025-07-12 12:39") , Note = "Lunch at Chipotle" },
            new Expense { Category = "Gas", Amount = 38.68, Date = DateTime.Parse("2025-07-12 16:18") , Note = "Exxon gas" },
            new Expense { Category = "Bills", Amount = 73.92, Date = DateTime.Parse("2025-07-12 10:18") , Note = "Electric bill" },
            new Expense { Category = "Food", Amount = 103.2, Date = DateTime.Parse("2025-07-11 16:30") , Note = "Aldi grocery" },
            new Expense { Category = "Bills", Amount = 84.63, Date = DateTime.Parse("2025-07-11 10:12") , Note = "Electric bill" },
            new Expense { Category = "Bills", Amount = 74.63, Date = DateTime.Parse("2025-07-11 10:46") , Note = "Water bill" },
            new Expense { Category = "Food", Amount = 144.59, Date = DateTime.Parse("2025-07-10 18:17") , Note = "Safeway grocery" },
            new Expense { Category = "Miscellaneous", Amount = 32.88, Date = DateTime.Parse("2025-07-10 13:38") , Note = "Gift wrap & card" },
            new Expense { Category = "Food", Amount = 17.05, Date = DateTime.Parse("2025-07-09 20:07") , Note = "Pizza delivery" },
            new Expense { Category = "Bills", Amount = 149.26, Date = DateTime.Parse("2025-07-09 11:02") , Note = "Cell phone bill" },
            new Expense { Category = "School", Amount = 277.98, Date = DateTime.Parse("2025-07-09 13:31") , Note = "Lab fees" },
            new Expense { Category = "Food", Amount = 13.37, Date = DateTime.Parse("2025-07-08 19:52") , Note = "Pizza delivery" },
            new Expense { Category = "Bills", Amount = 123.46, Date = DateTime.Parse("2025-07-08 11:04") , Note = "Internet bill" },
            new Expense { Category = "Bills", Amount = 77.16, Date = DateTime.Parse("2025-07-08 09:29") , Note = "Water bill" },
            new Expense { Category = "Food", Amount = 115.03, Date = DateTime.Parse("2025-07-07 16:41") , Note = "Costco grocery run" },
            new Expense { Category = "Miscellaneous", Amount = 39.4, Date = DateTime.Parse("2025-07-07 16:54") , Note = "Home decor" },
            new Expense { Category = "Gas", Amount = 39.03, Date = DateTime.Parse("2025-07-07 10:00") , Note = "Gas station fill-up" },
            new Expense { Category = "Food", Amount = 149.97, Date = DateTime.Parse("2025-07-06 16:46") , Note = "Trader Joe's groceries" },
            new Expense { Category = "Food", Amount = 63.51, Date = DateTime.Parse("2025-07-06 16:36") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Food", Amount = 15.32, Date = DateTime.Parse("2025-07-05 13:37") , Note = "Lunch at Chipotle" },
            new Expense { Category = "School", Amount = 138.43, Date = DateTime.Parse("2025-07-05 11:52") , Note = "School supplies" },
            new Expense { Category = "Miscellaneous", Amount = 22.38, Date = DateTime.Parse("2025-07-05 16:13") , Note = "Office supplies" },
            new Expense { Category = "Food", Amount = 17.28, Date = DateTime.Parse("2025-07-04 12:35") , Note = "Lunch at Chipotle" },
            new Expense { Category = "Bills", Amount = 109.93, Date = DateTime.Parse("2025-07-04 10:23") , Note = "Cell phone bill" },
            new Expense { Category = "Food", Amount = 65.0, Date = DateTime.Parse("2025-07-03 18:17") , Note = "Costco grocery run" },
            new Expense { Category = "Bills", Amount = 73.1, Date = DateTime.Parse("2025-07-03 09:23") , Note = "Cell phone bill" },
            new Expense { Category = "Entertainment", Amount = 19.09, Date = DateTime.Parse("2025-07-03 22:45") , Note = "Movie night" },
            new Expense { Category = "Food", Amount = 142.59, Date = DateTime.Parse("2025-07-02 16:50") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Miscellaneous", Amount = 39.52, Date = DateTime.Parse("2025-07-02 15:55") , Note = "Gift wrap & card" },
            new Expense { Category = "Food", Amount = 105.8, Date = DateTime.Parse("2025-07-02 18:00") , Note = "Walmart grocery shopping" },
            new Expense { Category = "Food", Amount = 15.54, Date = DateTime.Parse("2025-07-01 18:40") , Note = "Dinner takeout (Chinese)" },
            new Expense { Category = "Food", Amount = 99.09, Date = DateTime.Parse("2025-07-01 18:46") , Note = "Costco grocery run" },
            new Expense { Category = "Food", Amount = 114.51, Date = DateTime.Parse("2025-06-30 16:32") , Note = "Safeway grocery" },
            new Expense { Category = "Entertainment", Amount = 25.46, Date = DateTime.Parse("2025-06-30 21:05") , Note = "Concert ticket" },
            new Expense { Category = "Gas", Amount = 48.86, Date = DateTime.Parse("2025-06-30 17:25") , Note = "BP gas" },
            new Expense { Category = "Food", Amount = 113.8, Date = DateTime.Parse("2025-06-29 18:50") , Note = "Safeway grocery" },
            new Expense { Category = "Miscellaneous", Amount = 19.45, Date = DateTime.Parse("2025-06-29 14:49") , Note = "Amazon order" },
            new Expense { Category = "Food", Amount = 116.45, Date = DateTime.Parse("2025-06-28 17:14") , Note = "Aldi grocery" },
            new Expense { Category = "Gas", Amount = 40.39, Date = DateTime.Parse("2025-06-28 17:36") , Note = "Shell gas refill" },
            new Expense { Category = "Food", Amount = 82.15, Date = DateTime.Parse("2025-06-27 17:42") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Miscellaneous", Amount = 23.06, Date = DateTime.Parse("2025-06-27 15:45") , Note = "Amazon order" },
            new Expense { Category = "Food", Amount = 85.62, Date = DateTime.Parse("2025-06-26 17:48") , Note = "Costco grocery run" },
            new Expense { Category = "Miscellaneous", Amount = 23.8, Date = DateTime.Parse("2025-06-26 15:08") , Note = "Office supplies" },
            new Expense { Category = "Miscellaneous", Amount = 17.87, Date = DateTime.Parse("2025-06-26 16:19") , Note = "Amazon order" },
            new Expense { Category = "Food", Amount = 77.55, Date = DateTime.Parse("2025-06-25 16:09") , Note = "Walmart grocery shopping" },
            new Expense { Category = "Bills", Amount = 129.15, Date = DateTime.Parse("2025-06-25 09:05") , Note = "Cell phone bill" },
            new Expense { Category = "Food", Amount = 18.68, Date = DateTime.Parse("2025-06-24 18:47") , Note = "Pizza delivery" },
            new Expense { Category = "Entertainment", Amount = 38.78, Date = DateTime.Parse("2025-06-24 20:12") , Note = "Spotify subscription" },
            new Expense { Category = "Bills", Amount = 145.4, Date = DateTime.Parse("2025-06-24 10:25") , Note = "Water bill" },
            new Expense { Category = "Food", Amount = 107.0, Date = DateTime.Parse("2025-06-23 16:33") , Note = "Costco grocery run" },
            new Expense { Category = "Miscellaneous", Amount = 29.55, Date = DateTime.Parse("2025-06-23 17:05") , Note = "Gift wrap & card" },
            new Expense { Category = "Gas", Amount = 35.64, Date = DateTime.Parse("2025-06-23 10:48") , Note = "Exxon gas" },
            new Expense { Category = "Food", Amount = 71.04, Date = DateTime.Parse("2025-06-22 16:35") , Note = "Walmart grocery shopping" },
            new Expense { Category = "School", Amount = 183.93, Date = DateTime.Parse("2025-06-22 15:11") , Note = "Lab fees" },
            new Expense { Category = "Food", Amount = 92.2, Date = DateTime.Parse("2025-06-21 16:03") , Note = "Aldi grocery" },
            new Expense { Category = "Miscellaneous", Amount = 37.3, Date = DateTime.Parse("2025-06-21 16:01") , Note = "Office supplies" },
            new Expense { Category = "Bills", Amount = 99.74, Date = DateTime.Parse("2025-06-21 11:50") , Note = "Water bill" },
            new Expense { Category = "Food", Amount = 114.55, Date = DateTime.Parse("2025-06-20 17:35") , Note = "Safeway grocery" },
            new Expense { Category = "Miscellaneous", Amount = 26.27, Date = DateTime.Parse("2025-06-20 13:22") , Note = "Home decor" },
            new Expense { Category = "Food", Amount = 140.68, Date = DateTime.Parse("2025-06-19 18:20") , Note = "Costco grocery run" },
            new Expense { Category = "Gas", Amount = 45.14, Date = DateTime.Parse("2025-06-19 18:29") , Note = "Exxon gas" },
            new Expense { Category = "Food", Amount = 9.08, Date = DateTime.Parse("2025-06-18 12:40") , Note = "Subway sandwich" },
            new Expense { Category = "Entertainment", Amount = 82.3, Date = DateTime.Parse("2025-06-18 20:40") , Note = "Spotify subscription" },
            new Expense { Category = "Entertainment", Amount = 24.67, Date = DateTime.Parse("2025-06-18 22:01") , Note = "Spotify subscription" },
            new Expense { Category = "Food", Amount = 129.5, Date = DateTime.Parse("2025-06-17 17:35") , Note = "Safeway grocery" },
            new Expense { Category = "Miscellaneous", Amount = 25.46, Date = DateTime.Parse("2025-06-17 16:29") , Note = "Amazon order" },
            new Expense { Category = "Food", Amount = 148.76, Date = DateTime.Parse("2025-06-17 16:21") , Note = "Costco grocery run" },
            new Expense { Category = "Food", Amount = 144.75, Date = DateTime.Parse("2025-06-16 16:21") , Note = "Walmart grocery shopping" },
            new Expense { Category = "Miscellaneous", Amount = 29.87, Date = DateTime.Parse("2025-06-16 17:08") , Note = "Home decor" },
            new Expense { Category = "Miscellaneous", Amount = 13.15, Date = DateTime.Parse("2025-06-16 17:05") , Note = "Amazon order" },
            new Expense { Category = "Food", Amount = 89.76, Date = DateTime.Parse("2025-06-15 18:49") , Note = "Aldi grocery" },
            new Expense { Category = "School", Amount = 249.22, Date = DateTime.Parse("2025-06-15 10:16") , Note = "School supplies" },
            new Expense { Category = "Gas", Amount = 44.28, Date = DateTime.Parse("2025-06-15 16:32") , Note = "BP gas" },
            new Expense { Category = "Food", Amount = 86.57, Date = DateTime.Parse("2025-06-14 18:25") , Note = "Farmer's market vegetables" },
            new Expense { Category = "Bills", Amount = 143.69, Date = DateTime.Parse("2025-06-14 11:36") , Note = "Water bill" },
            new Expense { Category = "School", Amount = 136.0, Date = DateTime.Parse("2025-06-14 15:32") , Note = "School supplies" },
            new Expense { Category = "Food", Amount = 16.14, Date = DateTime.Parse("2025-06-13 20:19") , Note = "Dinner at Olive Garden" },
            new Expense { Category = "Miscellaneous", Amount = 22.26, Date = DateTime.Parse("2025-06-13 13:40") , Note = "Gift wrap & card" },
            new Expense { Category = "Food", Amount = 12.12, Date = DateTime.Parse("2025-06-12 20:46") , Note = "Pizza delivery" },
            new Expense { Category = "Miscellaneous", Amount = 25.11, Date = DateTime.Parse("2025-06-12 14:03") , Note = "Home decor" },
            new Expense { Category = "Entertainment", Amount = 78.55, Date = DateTime.Parse("2025-06-12 21:42") , Note = "Netflix subscription" },
        };
    }
}

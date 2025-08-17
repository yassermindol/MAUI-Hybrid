using ExpenseTracker.Models;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ExpenseTracker.Settings;

public class AppSettings : KeyProvider
{
    public static bool IsDarkMode
    {
        get => Preferences.Get(Key(), false);
        set => Preferences.Set(Key(), value);
    }

    public class Account
    {
        public static int UserID
        {
            get => Preferences.Get(Key(), -1);
            set => Preferences.Set(Key(), value);
        }

        public static int SelectedMonth
        {
            get => Preferences.Get(UserID + Key(), DateTime.Now.Month);
            set => Preferences.Set(UserID + Key(), value);
        }

        public static int SelectedYear
        {
            get => Preferences.Get(UserID + Key(), DateTime.Now.Year);
            set => Preferences.Set(UserID + Key(), value);
        }

        public static DateTime StartDateRange
        {
            get
            {
                var date = Preferences.Get(UserID + Key(), DateTime.MinValue);

                if (date == DateTime.MinValue)
                {
                    var tempDate = EndDateRange.AddMonths(-6);
                    date = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 0, 0, 0);
                }

                return date;
            }
            set => Preferences.Set(UserID + Key(), value);
        }

        public static DateTime EndDateRange
        {
            get => Preferences.Get(UserID + Key(), DateTime.Now);
            set => Preferences.Set(UserID + Key(), value);
        }

        static CultureInfo culture = CultureInfo.CurrentCulture;
        public static string CurrencySymbol
        {
            get => Preferences.Get(UserID + Key(), culture.NumberFormat.CurrencySymbol);
            set => Preferences.Set(UserID + Key(), value);
        }

        public static string Country
        {
            get => Preferences.Get(UserID + Key(), culture.EnglishName.Replace("English ", "").Replace("(", "").Replace(")", ""));
            set => Preferences.Set(UserID + Key(), value);
        }

        public static int PreferredExpenseListSortTypeHome
        {
            get => Preferences.Get(UserID + Key(), (int)SortType.DateDescending);
            set => Preferences.Set(UserID + Key(), value);
        }      

        public static int PreferredExpenseListSortTypeDateRange
        {
            get => Preferences.Get(UserID + Key(), (int)SortType.DateDescending);
            set => Preferences.Set(UserID + Key(), value);
        }

        public static int PreferredExpenseListSortTypeSummaryDetails
        {
            get => Preferences.Get(UserID + Key(), (int)SortType.DateDescending);
            set => Preferences.Set(UserID + Key(), value);
        }

        public static int PreferredExpenseListSortTypeSearchExpense
        {
            get => Preferences.Get(UserID + Key(), (int)SortType.DateDescending);
            set => Preferences.Set(UserID + Key(), value);
        }

        public static bool OnBoardingCompleted
        {
            get => Preferences.Get(UserID + Key(), false);
            set => Preferences.Set(UserID + Key(), value);
        }

        public static bool ShowSubItems
        {
            get => Preferences.Get(UserID + Key(), false);
            set => Preferences.Set(UserID + Key(), value);
        }
        public static DateTime StartDateHome
        {
            get => Preferences.Get(UserID + Key(), DateTime.MinValue);
            set => Preferences.Set(UserID + Key(), value);
        }
        public static DateTime StartDateDaily
        {
            get => Preferences.Get(UserID + Key(), DateTime.MinValue);
            set => Preferences.Set(UserID + Key(), value);
        }
    }
}

public class KeyProvider
{
    protected static string Key(string className = "", [CallerMemberName] string propertyName = "")
    {
        var key = String.IsNullOrWhiteSpace(className)
            ? $"{propertyName}"
            : $"{className}.{propertyName}";

        return key;
    }
}
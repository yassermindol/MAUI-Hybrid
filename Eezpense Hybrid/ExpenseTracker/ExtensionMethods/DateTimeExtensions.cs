using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static bool IsToday(this DateTime dt)
        {
            var now = DateTime.Now;

            return (dt.Year == now.Year && dt.Month == now.Month && dt.Day == now.Day);
        }

        public static bool IsYesterday(this DateTime dt)
        {
            var now = DateTime.Now;

            return (dt.Year == now.Year && dt.Month == now.Month && dt.Day == now.Day - 1);
        }

        public static string ToHuman(this DateTime dt)
        {
            if (dt.IsToday())
                return "Today";
            else if (dt.IsYesterday())
                return "Yesterday";
            else if (dt.Year == DateTime.Now.Year)
                return dt.ToString("MMM d ddd");
            else
                return dt.ToString("MMM d yyyy");
        }
    }
}

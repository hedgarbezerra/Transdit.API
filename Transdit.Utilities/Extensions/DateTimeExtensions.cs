using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Utilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static void FirstNLastDaysOfMonth(this DateTime date, out DateTime firstDay, out DateTime lastDay)
        {
            firstDay = new DateTime(date.Year, date.Month, 1);
            var daysInDateMonth = DateTime.DaysInMonth(date.Year, date.Month);
            lastDay = new DateTime(date.Year, date.Month, daysInDateMonth);
        }
    }
}

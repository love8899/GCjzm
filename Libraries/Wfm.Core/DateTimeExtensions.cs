using System;

namespace Wfm.Core
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
        public static DateTime GetDayOfThisWeek(this DateTime dt, DayOfWeek dayOfWeek)
        {
            return dt.StartOfWeek(DayOfWeek.Sunday).AddDays((int)dayOfWeek);
        }
    }
}

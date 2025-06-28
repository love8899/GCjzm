using System;
using System.Collections.Generic;
using System.Globalization;

namespace Wfm.Services.Tasks
{

    public static  class DateService
    {

        public static int GetWeekNumberOfYear(DateTime now)
        {
            CultureInfo ci = CultureInfo.CurrentCulture;
            int weekNumber = ci.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
            return weekNumber;
        }

        public static int GetWeekRows(int year, int month)
        {

            DateTime CurrentMonth = new DateTime(year, month, 1);
            int TotalDaysInThisMonth = DateTime.DaysInMonth(year, month);
            int DayInWeek = (int)CurrentMonth.DayOfWeek;
            int DayInWeekDiffer = 0;
            while (CurrentMonth.DayOfWeek != DayOfWeek.Sunday)
            {
                DayInWeekDiffer++;
                CurrentMonth = CurrentMonth.AddDays(-1); // reverse
            }

            int TotalDays = TotalDaysInThisMonth + DayInWeekDiffer;

            int Weeks = TotalDays / 7;

            if (TotalDays - (Weeks * 7) > 0)
            {
                Weeks++;
            }


            return Weeks;

        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);

            int daysOffset = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);

            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }

            return firstMonday.AddDays(weekOfYear * 7);

        }

        public static string[] WeekDateOfWeek(int year, int weekOfYear)
        {
            string[] wd = new string[7];

            DateTime tempWd = FirstDateOfWeek(year, weekOfYear);

            for (int j = 0; j < 7; j++)
            {
                TimeSpan tempSpan = new TimeSpan(j, 0, 0, 0);
                wd[j] = (tempWd + tempSpan).ToString("MM/dd");
            }

            return wd;

        }

        public static string[,] GetMonthDays (int year, int month)
        {
            int day = 1;

            DateTime date=new DateTime(year,month,day);

            int weeks= GetWeekNumberOfYear(date);

            DateTime tempWd = FirstDateOfWeek(year, weeks);

            string[,] md=new string[6,7];
            for (int i = 0; i < 6; i++) // 6 weeks
            {
                for (int k = 0; k < 7; k++)
                {
                    TimeSpan tempSpan = new TimeSpan(k, 0, 0, 0);

                    md[i, k] = (tempWd + tempSpan).ToString("MM/dd");
                   // md[i, k] = (tempWd + tempSpan).ToString("dd");
                }
                weeks++;
                tempWd = FirstDateOfWeek(year, weeks);
            }
            return md;
        }      

        public static Dictionary<int, DateTime> GetDaysInWeek(int weeknumber, int year)
        {

            DateTime dt = new DateTime(year, 1, 1).AddDays((weeknumber - 1) * 7);
            //while (dt.DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
            //    dt = dt.AddDays(-1);

            // DayOfWeek.Sunday;
            while (dt.DayOfWeek != DayOfWeek.Sunday)
                dt = dt.AddDays(-1);

            Dictionary<int, DateTime> _DaysInWeek = new Dictionary<int, DateTime>();

            for (int i = 0; i < 7; i++)
            {
                _DaysInWeek.Add(i, dt.AddDays(i));
            }

            return _DaysInWeek;

        }

        public static string GetStringForMonthAndYear(DateTime datetime)
        {
            string _MonthYear = string.Empty;

            _MonthYear = datetime.ToString("MMMM yyyy");

            return _MonthYear;
        }

        

        public static string GetStringForMonthDayAndYear(DateTime datetime)
        {
            string _MonthDayYear = string.Empty;

            _MonthDayYear = datetime.ToString("MMMM d, yyyy");

            return _MonthDayYear;
        }

        public static int GetFirstDayOfWeekInMonth(int year, int month)
        {
            int BeginDayOfWeek = 0;
            DateTime _DT = new DateTime(year, month, 1);
            BeginDayOfWeek = (int)_DT.DayOfWeek; 

            return BeginDayOfWeek;

        }

        public static int GetWeekNumberOfYear(int year, int month, int day)
        {
            DateTime now = new DateTime(year, month, day);
            CultureInfo ci = CultureInfo.CurrentCulture;
            int weekNumber = ci.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
            return weekNumber;
        }

        public static int GetWeekNthOfMonth(int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);
            DateTime beginningOfMonth = new DateTime(year, month, 1);

            //while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
            //    date = date.AddDays(1);

            while (date.Date.AddDays(1).DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        public static string ShortMonthDay(int Year, int Month, int Day)
        {
            string _ShortDate = string.Empty;

            DateTime now = new DateTime(Year, Month, Day);

            _ShortDate = now.ToString("MMM d");

            return _ShortDate;
        }

        public static bool IsCurrentDay(int Year, int Month, int Day)
        {
            bool _IsCurrentDay = false;

            DateTime _Now = System.DateTime.Now;

            _IsCurrentDay = (_Now.Year == Year && _Now.Month == Month && _Now.Day == Day) ? true : false;

            return _IsCurrentDay;
        }

        public static bool IsCurrentHour(DateTime RequestHour)
        {
            bool _IsCurrentHour = false;

            DateTime _Now = System.DateTime.Now;

            _IsCurrentHour = (_Now.Year == RequestHour.Year  && _Now.Month == RequestHour.Month  && _Now.Day ==  RequestHour.Day && _Now.Hour == RequestHour.Hour) ? true : false;

            return _IsCurrentHour;
        }

    } // end of class

}
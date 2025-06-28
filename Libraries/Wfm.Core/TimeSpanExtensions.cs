using System;

namespace Wfm.Core
{
    public static class TimeSpanExtensions
    {
        public static DateTime AsDateTime(this TimeSpan ts)
        {
            return DateTime.Today + ts;
        }

        public static String ToShortTimeString(this TimeSpan ts)
        {
            return (DateTime.Today + ts).ToShortTimeString();
        }
    }
}

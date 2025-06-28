using System;

namespace Wfm.Core
{
    public static class StringExtensions
    {
        public static String Truncate(this string value, int maxLength)
        {
            if (String.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}

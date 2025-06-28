using System.Collections.Generic;

namespace Wfm.Core
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            HashSet<T> hashSet = new HashSet<T>();

            foreach (var en in enumerable)
            {
                if (!hashSet.Contains(en))
                    hashSet.Add(en);
            }

            return hashSet;
        }
    }
}

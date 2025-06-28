using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Infrastructure;
using Wfm.Services.Helpers;
using Wfm.Services.Localization;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Wfm.Web.Framework
{
    public static class Extensions
    {
        #region Kendo Paging

        public static IEnumerable<T> ForCommand<T>(this IEnumerable<T> current, DataSourceRequest command)
        {
            var queryable = current.AsQueryable() as IQueryable;
            if (command.Filters.Any())
            {
                queryable = queryable.Where(command.Filters.AsEnumerable()).AsQueryable();
            }

            IList<SortDescriptor> temporarySortDescriptors = new List<SortDescriptor>();

            if (!command.Sorts.Any() && queryable.Provider.IsEntityFrameworkProvider())
            {
                // The Entity Framework provider demands OrderBy before calling Skip.
                SortDescriptor sortDescriptor = new SortDescriptor
                {
                    Member = queryable.ElementType.FirstSortableProperty()
                };
                command.Sorts.Add(sortDescriptor);
                temporarySortDescriptors.Add(sortDescriptor);
            }

            if (command.Groups.Any())
            {
                command.Groups.Reverse().Each(groupDescriptor =>
                {
                    SortDescriptor sortDescriptor = new SortDescriptor
                    {
                        Member = groupDescriptor.Member,
                        SortDirection = groupDescriptor.SortDirection
                    };

                    command.Sorts.Insert(0, sortDescriptor);
                    temporarySortDescriptors.Add(sortDescriptor);
                });
            }

            if (command.Sorts.Any())
            {
                queryable = queryable.Sort(command.Sorts);
            }

            //NOTE: This could return all, NOT good
            return queryable as IQueryable<T>;
        }
        /// <summary>
        /// Extract filter which cannot be translate to linq expression directly from the request filter
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string ExtractFilterValue(this DataSourceRequest request, string fieldName)
        {
            string result = null;
            if (request.Filters.Any(x => x as FilterDescriptor != null && ((FilterDescriptor)x).Member.Equals(fieldName)))
            {
                var filter = request.Filters.OfType<FilterDescriptor>().First(x => x.Member.Equals(fieldName));
                result = filter.Value.ToString();
                request.Filters.Remove(filter);
            }
            if (request.Filters.Any(x => x as CompositeFilterDescriptor != null))
            {
                return ExtractFilterValueFromCompositeFilterDescriptor(request.Filters.OfType<CompositeFilterDescriptor>().First(), fieldName);
            }
            return result;
        }
        private static string ExtractFilterValueFromCompositeFilterDescriptor(CompositeFilterDescriptor composit, string fieldName)
        {
            string result = null;
            var filters = composit.FilterDescriptors;
            if (filters.Any(x => x as FilterDescriptor != null && ((FilterDescriptor)x).Member.Equals(fieldName)))
            {
                var filter = filters.OfType<FilterDescriptor>().First(x => x.Member.Equals(fieldName));
                result = filter.Value.ToString();
                filters.Remove(filter);
            }
            if (filters.Any(x => x as CompositeFilterDescriptor != null))
            {
                return ExtractFilterValueFromCompositeFilterDescriptor(filters.OfType<CompositeFilterDescriptor>().First(), fieldName);
            }
            return result;
        }
        /// <summary>
        /// Paged for command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current">The current.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static IPagedList<T> PagedForCommand<T>(this IEnumerable<T> current, DataSourceRequest request)
        {
            var queryable = current.AsQueryable() as IQueryable;
            if (request.Filters != null && request.Filters.Any())
            {
                var temps = request.Filters.AsEnumerable();
                try
                {
                    queryable = queryable.Where(request.Filters.AsEnumerable()).AsQueryable() as IQueryable;
                }
                catch (ArgumentException)
                {

                }
            }

            IList<SortDescriptor> temporarySortDescriptors = new List<SortDescriptor>();

            if ((request.Sorts == null || !request.Sorts.Any()) && queryable.Provider.IsEntityFrameworkProvider())
            {
                // The Entity Framework provider demands OrderBy before calling Skip.
                SortDescriptor sortDescriptor = new SortDescriptor
                {
                    Member = queryable.ElementType.FirstSortableProperty()
                };
                request.Sorts.Add(sortDescriptor);
                temporarySortDescriptors.Add(sortDescriptor);
            }

            if (request.Groups != null && request.Groups.Any())
            {
                request.Groups.Reverse().Each(groupDescriptor =>
                {
                    SortDescriptor sortDescriptor = new SortDescriptor
                    {
                        Member = groupDescriptor.Member,
                        SortDirection = groupDescriptor.SortDirection
                    };

                    request.Sorts.Insert(0, sortDescriptor);
                    temporarySortDescriptors.Add(sortDescriptor);
                });
            }

            if (request.Sorts != null && request.Sorts.Any())
            {
                queryable = queryable.Sort(request.Sorts);
            }


            //----------------------------
            // Do paging list
            //----------------------------

            if (request.PageSize < 1)
            {
                request.PageSize = 10;
            }

            //total record
            int total = queryable.Count();

            //return new PagedList<T>(queryable as IQueryable<T>, request.Page - 1, request.PageSize);
            return new PagedList<T>(queryable as IQueryable<T>, request.Page - 1, request.PageSize, total);

        }


        //public static IEnumerable<T> PagedForCommand<T>(this IEnumerable<T> current, DataSourceRequest command)
        //{
        //    return current.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize);
        //}
        #endregion

        #region String Helper
        public static string ExtractNumericText(this string str)
        {
            if (str == null) return null;
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            //return Regex.Match(str, @"\d+").Value;
            return new String(str.Where(Char.IsDigit).ToArray());
        }

        public static string ExtractAlphanumericText(this string str)
        {
            if (str == null) return null;
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            return Regex.Replace(str, @"[^a-zA-Z 0-9]", "").Trim();
        }

        public static string ToToken(this string str, int numOfGuid = 1)
        {
            if (str == null) return null;
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            var sbResult = new StringBuilder();
            for (int i = 0; i < numOfGuid; i++)
            {
                sbResult.Append(System.Guid.NewGuid().ToString("N"));
            }

            return sbResult.ToString();
        }

        public static string SanitizeContent(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ' || c == '-' || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(' ');
                }
            }

            string result = Regex.Replace(sb.ToString(), @"[ ]{2,}", @" ");

            return result.Trim();
        }

        public static string SanitizeContent(this string str, int length)
        {
            string result = SanitizeContent(str);

            if (result.Length <= length)
                return result;
            else
                return result.Substring(0, length);
        }

        #endregion

        #region Formating
        public static string ToPrettyTelephone(this string phoneNumberValue)
        {
            if (phoneNumberValue == null) return null;
            if (string.IsNullOrWhiteSpace(phoneNumberValue)) return string.Empty;

            try
            {
                return String.Format(new TelephoneFormatter(), "{0}", phoneNumberValue);
            }
            catch
            {
                // Ignore error
            }
            return phoneNumberValue;
        }

        public static string ToPrettySocialInsuranceNumber(this string sinNumberValue)
        {
            if(string.IsNullOrWhiteSpace(sinNumberValue))
                return sinNumberValue;

            if(sinNumberValue.Length != 9) // Social Insurance Number
                return sinNumberValue;

            return sinNumberValue.Substring(0, 3) + " " + sinNumberValue.Substring(3, 3) + " " + sinNumberValue.Substring(6);
        }

        public static string ToPrettyName(this string str)
        {
            if (str == null) return null;
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            str = str.Trim().ToLower();

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }


        public static string ToMaskedSocialInsuranceNumber(this string sinNumberValue) 
        {
            if (string.IsNullOrWhiteSpace(sinNumberValue))
                return sinNumberValue;
            
            // remove '-' and blank spaces.
            sinNumberValue = sinNumberValue.ExtractNumericText();           

            if (sinNumberValue.Length != 9) // Social Insurance Number
                return sinNumberValue;

            return sinNumberValue.Substring(0, 1) + " ***** " + sinNumberValue.Substring(6);
        }


        public static string MaskSubString(this string str, int startIndex, int length, char mask = '*')
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < startIndex + length)
                return str;

            var masked = new String(mask, length);
            var left = startIndex > 0 ? str.Substring(0, startIndex) : string.Empty;
            var right = str.Length > startIndex + length ? str.Substring(startIndex + length) : string.Empty;

            return string.Concat(left, masked, right);
        }


        public static string MaskExceptLastDigits(this string str, int length, char mask = '*')
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < length)
                return str;

            return MaskSubString(str, 0, str.Length - length, mask);
        }

        #endregion

        #region Others
        public static bool IsEntityFrameworkProvider(this IQueryProvider provider)
        {
            return provider.GetType().FullName == "System.Data.Objects.ELinq.ObjectQueryProvider";
        }

        public static bool IsLinqToObjectsProvider(this IQueryProvider provider)
        {
            return provider.GetType().FullName.Contains("EnumerableQuery");
        }

        public static string FirstSortableProperty(this Type type)
        {
            PropertyInfo firstSortableProperty = type.GetProperties().FirstOrDefault(property => property.PropertyType.IsPredefinedType());

            if (firstSortableProperty == null)
            {
                throw new NotSupportedException("Cannot find property to sort by.");
            }

            return firstSortableProperty.Name;
        }

        internal static bool IsPredefinedType(this Type type)
        {
            return PredefinedTypes.Any(t => t == type);
        }

        public static readonly Type[] PredefinedTypes = {
            typeof(Object),
            typeof(Boolean),
            typeof(Char),
            typeof(String),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert)
        };

        //public static GridBoundColumnBuilder<T> Centered<T>(this GridBoundColumnBuilder<T> columnBuilder) where T:class
        //{
        //    return columnBuilder.HtmlAttributes(new { align = "center" })
        //                    .HeaderHtmlAttributes(new { style = "text-align:center;" });
        //}

        //public static GridTemplateColumnBuilder<T> Centered<T>(this GridTemplateColumnBuilder<T> columnBuilder) where T : class
        //{
        //    return columnBuilder.HtmlAttributes(new { align = "center" })
        //                    .HeaderHtmlAttributes(new { style = "text-align:center;" });
        //}

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, bool markCurrentAsSelected = true, int[] valuesToExclude = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         where valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue))
                         select new { ID = Convert.ToInt32(enumValue), Name = enumValue.GetLocalizedEnum(localizationService, workContext) };
            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            return new SelectList(values, "ID", "Name", selectedValue);
        }

        public static string GetValueFromAppliedFilter(this IFilterDescriptor filter, string valueName, FilterOperator? filterOperator = null)
        {
            if (filter is CompositeFilterDescriptor)
            {
                foreach (IFilterDescriptor childFilter in ((CompositeFilterDescriptor)filter).FilterDescriptors)
                {
                    var val1 = GetValueFromAppliedFilter(childFilter, valueName, filterOperator);
                    if (!String.IsNullOrEmpty(val1))
                        return val1;
                }
            }
            else
            {
                var filterDescriptor = (FilterDescriptor)filter;
                if (filterDescriptor != null &&
                    filterDescriptor.Member.Equals(valueName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!filterOperator.HasValue || filterDescriptor.Operator == filterOperator.Value)
                        return Convert.ToString(filterDescriptor.Value);
                }
            }

            return "";
        }

        public static string GetValueFromAppliedFilters(this IList<IFilterDescriptor> filters, string valueName, FilterOperator? filterOperator = null)
        {
            foreach (var filter in filters)
            {
                var val1 = GetValueFromAppliedFilter(filter, valueName, filterOperator);
                if (!String.IsNullOrEmpty(val1))
                    return val1;
            }
            return "";
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source)
        {
            return RelativeFormat(source, string.Empty);
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="defaultFormat">Default format string (in case relative formatting is not applied)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source, string defaultFormat)
        {
            return RelativeFormat(source, false, defaultFormat);
        }

        /// <summary>
        /// Relative formatting of DateTime (e.g. 2 hours ago, a month ago)
        /// </summary>
        /// <param name="source">Source (UTC format)</param>
        /// <param name="convertToUserTime">A value indicating whether we should convet DateTime instance to user local time (in case relative formatting is not applied)</param>
        /// <param name="defaultFormat">Default format string (in case relative formatting is not applied)</param>
        /// <returns>Formatted date and time string</returns>
        public static string RelativeFormat(this DateTime source,
            bool convertToUserTime, string defaultFormat)
        {
            string result = "";

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - source.Ticks);
            double delta = ts.TotalSeconds;

            if (delta > 0)
            {
                if (delta < 60) // 60 (seconds)
                {
                    result = ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
                }
                else if (delta < 120) //2 (minutes) * 60 (seconds)
                {
                    result = "a minute ago";
                }
                else if (delta < 2700) // 45 (minutes) * 60 (seconds)
                {
                    result = ts.Minutes + " minutes ago";
                }
                else if (delta < 5400) // 90 (minutes) * 60 (seconds)
                {
                    result = "an hour ago";
                }
                else if (delta < 86400) // 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    int hours = ts.Hours;
                    if (hours == 1)
                        hours = 2;
                    result = hours + " hours ago";
                }
                else if (delta < 172800) // 48 (hours) * 60 (minutes) * 60 (seconds)
                {
                    result = "yesterday";
                }
                else if (delta < 2592000) // 30 (days) * 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    result = ts.Days + " days ago";
                }
                else if (delta < 31104000) // 12 (months) * 30 (days) * 24 (hours) * 60 (minutes) * 60 (seconds)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    result = months <= 1 ? "one month ago" : months + " months ago";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    result = years <= 1 ? "one year ago" : years + " years ago";
                }
            }
            else
            {
                DateTime tmp1 = source;
                if (convertToUserTime)
                {
                    tmp1 = EngineContext.Current.Resolve<IDateTimeHelper>().ConvertToUserTime(tmp1, DateTimeKind.Utc);
                }
                //default formatting
                if (!String.IsNullOrEmpty(defaultFormat))
                {
                    result = tmp1.ToString(defaultFormat);
                }
                else
                {
                    result = tmp1.ToString();
                }
            }
            return result;
        }


        public static int CalculateAge(this DateTime birthDay)
        {
            int years = DateTime.Now.Year - birthDay.Year;

            if ((birthDay.Month > DateTime.Now.Month) || (birthDay.Month == DateTime.Now.Month && birthDay.Day > DateTime.Now.Day))
                years--;

            return years;
        }

        #endregion

    }
}

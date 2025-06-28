using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RecogSys.RdrAccess;
using Wfm.Core.ComponentModel;
using ZXing;
using ZXing.Common;


namespace Wfm.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class CommonHelper
    {
        /// <summary>
        /// Ensures the subscriber email or throw.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email) {
            string output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if(!IsValidEmail(output)) {
                throw new WfmException("Email is not valid.");
            }

            return output;
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                var result = str.Substring(0, maxLength);
                if (!String.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }

            return str;
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
        public static string EnsureNumericOnly(string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            if (str == null)
                return string.Empty;

            return str;
        }

        /// <summary>
        /// Indicates whether the specified strings are null or empty strings
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate) {
            bool result = false;
            Array.ForEach(stringsToValidate, str => {
                if (string.IsNullOrEmpty(str)) result = true;
            });
            return result;
        }

        /// <summary>
        /// Compare two arrasy
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="a1">Array 1</param>
        /// <param name="a2">Array 2</param>
        /// <returns>Result</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            //also see Enumerable.SequenceEqual(a1, a2);
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        private static AspNetHostingPermissionLevel? _trustLevel;
        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in new[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal 
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return _trustLevel.Value;
        }

        /// <summary>
        /// Sets a property on an object to a valuae.
        /// </summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new WfmException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new WfmException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        public static TypeConverter GetWfmCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here

            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();

            return TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                TypeConverter destinationConverter = GetWfmCustomTypeConverter(destinationType);
                TypeConverter sourceConverter = GetWfmCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }
        
        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            string result = string.Empty;
            char[] letters = str.ToCharArray();
            foreach (char c in letters)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();
            return result;
        }

        /// <summary>
        /// Set Telerik (Kendo UI) culture
        /// </summary>
        public static void SetTelerikCulture()
        {
            //little hack here
            //always set culture to 'en-US' (Kendo UI has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs

            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }


        public static bool OneToManyCollectionWrapperEnabled
        {
            get
            {
                bool enabled = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["OneToManyCollectionWrapperEnabled"]) &&
                   Convert.ToBoolean(ConfigurationManager.AppSettings["OneToManyCollectionWrapperEnabled"]);
                return enabled;
            }
        }


        /// <summary>
        /// Determines whether [is digits only] [the specified string].
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static bool IsDigitsOnly(string str)
        {
            int len = str.Length;
            for (int i = 0; i < len; i++)
            {
                char c = str[i];
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        public static bool IsValidCanadianSin(string strNumber)
        {
            if (String.IsNullOrWhiteSpace(strNumber))
                return false;
            
            // If it is masked, skip the validation and assule it is valid
            if (strNumber.Contains("***"))
                return true;

            //cleanup, normalize
            strNumber = Regex.Replace(strNumber, @"[^0-9]", "");
            if ( String.IsNullOrEmpty(strNumber) || strNumber.Length != 9)
                return false;

            var multipliers = "121212121";
            int product, result = 0;

            for (int i = 0; i < 9; i++)
            {
                product = Convert.ToInt32(multipliers.Substring(i, 1)) * Convert.ToInt32(strNumber.Substring(i, 1));
                if (product > 9)
                {
                    string productString = product.ToString();
                    result += Convert.ToInt32(productString.Substring(0, 1)) + Convert.ToInt32(productString.Substring(1, 1));
                }
                else
                    result += product;
            }

            return (result % 10) == 0;
        }


        public static bool IsUsOrCanadianZipCode(string zipCode)
        {
            string _usZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";
            string _caZipRegEx = @"^([ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ])\ {0,1}(\d[ABCEGHJKLMNPRSTVWXYZ]\d)$";
            bool validZipCode = true;

            if ((!Regex.Match(zipCode, _usZipRegEx).Success) && (!Regex.Match(zipCode, _caZipRegEx).Success))
                validZipCode = false;

            return validZipCode;
        }


        public static string ToCanadianPhone(string str)
        {
            if (str == null) return null;
            if (String.IsNullOrWhiteSpace(str)) return string.Empty;

            // cleanup
            string sResult = Regex.Replace(str, @"[^0-9]", "");

            // format
            sResult = Regex.Replace(sResult, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");

            return sResult;
        }

        public static string ToCanadianSin(string str)
        {
            if (str == null) return null;
            if (String.IsNullOrWhiteSpace(str)) return string.Empty;

            // cleanup
            string sResult = Regex.Replace(str, @"[^0-9]", "");

            // format
            sResult = Regex.Replace(sResult, @"(\d{3})(\d{3})(\d{3})", "$1 $2 $3");

            return sResult;
        }


        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat. If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }


        private static int _GetWeekNumber(DateTime date)
        {
            // Ensure all 7 day with the same week number
            // Similar with ISO 8601 above, but with Sunday as first day
            // Use 2nd half of week to determine, if rule is FirstFourDayWeek
            if (date.DayOfWeek < DayOfWeek.Wednesday)
                date = date.AddDays(3);

            var weekNum = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

            return weekNum;
        }


        public static int GetYearAndWeekNumber(DateTime date, out int weekNum)
        {
            weekNum = _GetWeekNumber(date);

            // Ensure the year and week number consistent
            var year = date.Year;
            if (date.Month == 12 && weekNum == 1)
                year++;
            else if (date.Month == 1 && weekNum > 51)
                year--;

            return year;
        }


        public static DateTime GetDesiredWeekDay(int year, int month, int occurrence, DayOfWeek desiredDay)
        {

            var dayOne = new DateTime(year, month, 1);
            var nth = (int)desiredDay - (int)dayOne.DayOfWeek;
            if (nth < 0)
                nth += 7;

            return dayOne.AddDays(nth + (occurrence - 1) * 7);
        }


        #region String Helper

        public static string SanitizeContent(string str)
        {
            if (str == null) return null;
            if (String.IsNullOrWhiteSpace(str)) return string.Empty;

            // clean up
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if (
                    (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || 
                    c == ' ' || c == '-' || c == '.' || c == ',' || c == ';' || c == '(' || c == ')' ||
                    c == '@' || c == '#' || c == '$' || c == '%' || c == '+' || c == '[' || c == ']'
                    )
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(' ');
                }
            }

            string result = Regex.Replace(sb.ToString(), @"[ ]{2,}", @" ");
            result = Regex.Replace(sb.ToString(), @"[-]{2,}", @"-");

            return result.Trim();
        }


        public static string ExtractNumericText(string str)
        {
            if (str == null) return null;
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            return new String(str.Where(Char.IsDigit).ToArray());
        }


        public static string ExtractAlphanumericText(string str)
        {
            if (str == null) return null;
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            return Regex.Replace(str, @"[^a-zA-Z0-9]", "").Trim();
        }


        public static string TrimAndToLower(string str)
        {
            return String.IsNullOrWhiteSpace(str) ? string.Empty : str.Trim().ToLower();
        }


        public static string ToPrettyName(string str)
        {
            if (str == null) return null;
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            str = str.Trim().ToLower();

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }


        public static string ReplaceInvalidCharacters(string str, string replacement)
        {
            var illegalInFileName = Path.GetInvalidFileNameChars();
            foreach (var c in illegalInFileName)
                str = str.Replace(new string(c, 1), replacement);

            return str;
        }


        public static string ToPrettySocialInsuranceNumber(string sinNumberValue)
        {
            if (string.IsNullOrWhiteSpace(sinNumberValue))
                return sinNumberValue;

            if (sinNumberValue.Length != 9) // Social Insurance Number
                return sinNumberValue;

            return sinNumberValue.Substring(0, 3) + " " + sinNumberValue.Substring(3, 3) + " " + sinNumberValue.Substring(6);
        }


        public static IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ').Concat(new[] { "" });
            return
                words
                    .Skip(1)
                    .Aggregate(
                        words.Take(1).ToList(),
                        (a, w) =>
                        {
                            var last = a.Last();
                            while (last.Length > maximumLineLength)
                            {
                                a[a.Count() - 1] = last.Substring(0, maximumLineLength);
                                last = last.Substring(maximumLineLength);
                                a.Add(last);
                            }
                            var test = last + " " + w;
                            if (test.Length > maximumLineLength)
                            {
                                a.Add(w);
                            }
                            else
                            {
                                a[a.Count() - 1] = test;
                            }
                            return a;
                        });
        }


        public static string SpacedString(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return string.Empty;
            else
                return String.Join(" ", input.ToCharArray());
        }


        public static string PadLeftWithMaxLenght(string input, int maxLenght, char paddingChar = ' ')
        {
            if (input == null)
                return new String(paddingChar, maxLenght);

            string result = input.Trim().PadLeft(maxLenght, paddingChar);
            return result; //.Substring(0, maxLenght);
        }


        public static string PadRightWithMaxLenght(string input, int maxLenght, char paddingChar)
        {
            if (input == null)
                return new String(paddingChar, maxLenght);
            string result = input.Trim().PadRight(maxLenght, paddingChar);
            return result.Substring(0, maxLenght);
        }

        #endregion

        #region Decimal rounding

        public static decimal RoundUp(decimal number, int places)
        {
            decimal factor = RoundFactor(places);
            number *= factor;
            number = Math.Ceiling(number);
            number /= factor;
            return number;
        }

        public static decimal RoundDown(decimal number, int places)
        {
            decimal factor = RoundFactor(places);
            number *= factor;
            number = Math.Floor(number);
            number /= factor;
            return number;
        }

        internal static decimal RoundFactor(int places)
        {
            decimal factor = 1m;

            if (places < 0)
            {
                places = -places;
                for (int i = 0; i < places; i++)
                    factor /= 10m;
            }
            else
            {
                for (int i = 0; i < places; i++)
                    factor *= 10m;
            }

            return factor;
        }

        #endregion

        #region File Helper

        public static string GetMimeType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            string mime = "application/octetstream";

            string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension);
            if (rk != null && rk.GetValue("Content Type") != null)
                mime = rk.GetValue("Content Type").ToString();

            return mime;
        }


        public static byte[] Compress(byte[] input)
        {
            byte[] output;
            using (var unzipStream = new MemoryStream(input))
            {
                using (var ms = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(ms, CompressionMode.Compress))
                        unzipStream.CopyTo(zipStream);
                    output = ms.ToArray();
                }
            }

            return output;
        }


        public static byte[] Decompress(byte[] input)
        {
            byte[] output;
            using (var zipStream = new MemoryStream(input))
            {
                using (var ms = new MemoryStream())
                {
                    using (var unzipStream = new GZipStream(zipStream, CompressionMode.Decompress))
                        unzipStream.CopyTo(ms);
                    output = ms.ToArray();
                }
            }

            return output;
        }

        #endregion


        #region PDF helper

        public static void AddTextToPdf(PdfStamper stamper, int pageNum, float x, float y, string text)
        {
            var canvas = stamper.GetOverContent(pageNum);
            canvas.SetColorFill(BaseColor.RED);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(text), x, y, 0);
        }

        #endregion


        #region Byte Array

        public static byte[] Combine(IEnumerable<byte[]> arrays)
        {
            var result = new byte[arrays.Sum(a => a.Length)];

            var offset = 0;
            foreach (var array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }


        public static IEnumerable<byte[]> Split(byte[] array, int size)
        {
            return array.Select((v, k) => new { Value = v, Key = k })
                        .GroupBy(x => x.Key / size)
                        .Select(grp => grp.Select(x => x.Value).ToArray());
        }

        #endregion


        #region RSI Time Date

        public static DateTime? ToDateTime(RSI_TIME_DATE rsiTimeDate)
        {
            if (rsiTimeDate.month > 0 && rsiTimeDate.day > 0)
                return new DateTime(2000 + rsiTimeDate.year, rsiTimeDate.month, rsiTimeDate.day, rsiTimeDate.hour, rsiTimeDate.minute, rsiTimeDate.second);
            else
                return null;
        }


        public static RSI_TIME_DATE ToRsiTimeDate(DateTime dateTime)
        {
            return new RSI_TIME_DATE()
            {
                year = (byte) (dateTime.Year - 2000),
                month = (byte) dateTime.Month,
                day = (byte) dateTime.Day,
                hour = (byte) dateTime.Hour,
                minute = (byte) dateTime.Minute,
                second = (byte) dateTime.Second
            };
        }

        #endregion


        #region Object

        public static dynamic Merge(object item1, object item2)
        {
            if (item1 == null || item2 == null)
                return item1 ?? item2 ?? new ExpandoObject();

            var expando = new ExpandoObject();
            var result = expando as IDictionary<string, object>;
            foreach (System.Reflection.PropertyInfo fi in item1.GetType().GetProperties())
                result[fi.Name] = fi.GetValue(item1, null);
            foreach (System.Reflection.PropertyInfo fi in item2.GetType().GetProperties())
                result[fi.Name] = fi.GetValue(item2, null);

            return result;
        }

        #endregion


        #region Barcode

        public static byte[] ToQrCode(string value, int height = 250, int width = 250, int margin = 4, string text = null)
        {
            var qrValue = value;
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin,
                    //PureBarcode = false
                }
            };

            using (var bitmap = barcodeWriter.Write(qrValue))
            {
                if (!String.IsNullOrWhiteSpace(text))
                {
                    var textHeight = 18;
                    var rectf = new RectangleF(0, height - textHeight - textHeight / 2, width, textHeight);
                    var g = Graphics.FromImage(bitmap);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.FillRectangle(Brushes.White, rectf);
                    using (var sf = new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                    })
                    {
                        g.DrawString(text, new System.Drawing.Font("Arial", 12), Brushes.Blue, rectf, sf);
                    }
                    g.Flush();
                }

                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }


        public static string ToQrCodeImg(string value, int height = 250, int width = 250, int margin = 4, string text = null)
        {
            var bytes = ToQrCode(value, height, width, margin, text);

            var img = new TagBuilder("img");
            img.MergeAttribute("alt", "QR Code");
            img.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(bytes)));

            return img.ToString(TagRenderMode.SelfClosing);
        }


        #endregion
    }
}

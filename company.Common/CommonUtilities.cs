using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace company.Common
{
    public class Utilities
    {
        /// <summary>
        /// Converts number to A-Z, AA etc.
        /// </summary>
        /// <param name="nextNum"></param>
        /// <returns></returns>
        public static object OrdinalToLetters(int nextNum)
        {
            int numeric, num2;
            string text;
	        if (nextNum <= 0)
		        return string.Empty;
            numeric = nextNum % 26;
            text = Convert.ToChar(64 + numeric).ToString();
            num2 = nextNum / 26;
            if (num2 > 0)
            {
                return OrdinalToLetters(num2) + text;
            }
            else
            {
                return text;
            }
        }

        public static string CapitalizeFirstLetter(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return text.Substring(0, 1).ToUpper() + (text.Length > 1 ? text.Substring(1) : "");
            }
            return text;
        }

        public static int LettersToOrdinal(string text)
        {
            if (String.IsNullOrEmpty(text)) throw new ArgumentNullException("columnName");

            text = text.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < text.Length; i++)
            {
                sum *= 26;
                sum += (text[i] - 'A' + 1);
            }

            return sum;
        }

        public static int GetMonth21FromDate(DateTime date)
        {
            return (date.Year - 2000) * 100 + date.Month;
        }

        public static DateTime FirstDateOfWeek(int year, int weekNum, CalendarWeekRule rule)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
	        
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            DateTime firstMonday = jan1.AddDays(daysOffset);
            
	        var cal = CultureInfo.CurrentCulture.Calendar;
	        var firstMondayWeek = cal.GetWeekOfYear(firstMonday, rule, DayOfWeek.Monday);
			
            DateTime result = firstMonday.AddDays((weekNum-firstMondayWeek) * 7);

            return result;
        }

        public static DateTime GetFirstDayInWeek(DateTime date)
        {
            int delta = DayOfWeek.Monday - date.DayOfWeek;
            if (delta > 0)
                delta -= 7;
            return date.AddDays(delta);
        }

        public static int? WeekNumberFromDate(DateTime date)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            if (dfi != null)
            {
                Calendar cal = dfi.Calendar;
                return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            }
            return null;
        }

        public static string AppendSuffixToFileName(string fileName, string suffix)
        {
            return String.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(fileName), suffix, Path.GetExtension(fileName));
        }

        public static int DeCryptBrochureId(int encryptedId)
        {
            return encryptedId / 9;
        }

        public static int EncryptBrochureId(int id)
        {
            return id * 9;
        }

        public static string ArrayToString(string[] array, string separator, bool removeEmpty = true)
        {
            return String.Join(separator, (from a in array where !removeEmpty || (!String.IsNullOrEmpty(a) && a.Trim().Length > 0) select a).ToArray());
        }

        public static string ShortenText(string text, int length, string suffix = "...")
        {
            if (length >= 0 && length < text.Length)
                return text.Substring(0, length) + suffix;
            else
                return text;
        }

        public static List<int> GetIdsFromString(string commaSeparatedIds)
        {
            if (string.IsNullOrEmpty(commaSeparatedIds))
                return null;
            return commaSeparatedIds.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).Where(IsParsableAsInt).Select(int.Parse).ToList();
        }

        public static List<double> GetDoublesFromString(string commaSeparatedIds)
        {
            if (string.IsNullOrEmpty(commaSeparatedIds))
                return null;
            return commaSeparatedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(IsParsableAsDouble).Select(double.Parse).ToList(); ;
        }

        public static List<string> GetQuotedStringsFromString(string commaSeparatedIds)
        {
            if (string.IsNullOrEmpty(commaSeparatedIds))
                return null;
            return commaSeparatedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s=>$"'{s}'").ToList(); ;
        }

        public static List<int?> GetNullableIntsFromString(string commaSeparatedIds)
        {
            if (string.IsNullOrEmpty(commaSeparatedIds))
                return null;
            return commaSeparatedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(IsParsableAsInt).Select(s=>(int?) int.Parse(s)).ToList(); ;
        }

        private static bool IsParsableAsInt(string s)
        {
            int num;
            return int.TryParse(s, out num);
        }

        private static bool IsParsableAsDouble(string s)
        {
            double num;
            return double.TryParse(s, out num);
        }

        public static long? IpToLong(string ipStr)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipStr, out ip))
            {
                byte[] bytes = ip.GetAddressBytes();
                return
                       16777216 * (Int64)bytes[0] +
                       65536 * (Int64)bytes[1] +
                       256 * (Int64)bytes[2] +
                       (Int64)bytes[3];
            }
            return null;
        }

        #region Date Functions 
        public static int GetMonthFromNow(int offset)
            {
                return GetMonthFromDate(DateTime.Today, offset);
            }

            public static int GetMonthFromDate(DateTime date, int offset)
            {
                string sYear = date.AddMonths(offset).Year.ToString();
                return Int32.Parse(sYear.Substring(sYear.Length - 2, 2)) * 100 + date.AddMonths(offset).Month;
            }

            public static int GetMonthFromDate(DateTime date)
            {
                var sYear = date.Year.ToString();
                return Int32.Parse(sYear.Substring(sYear.Length - 2, 2)) * 100 + date.Month;
            }

            public static DateTime GetMonthStart(DateTime date)
            {
                return date.Date.AddDays(-1 * date.Day + 1);
            }

            public static DateTime GetMonthEnd(DateTime date)
            {
                return date.Date.AddMonths(1).AddDays(-1 * date.Day + 1).AddMilliseconds(-1);
            }

            public static DateTime GetDayEnd(DateTime date)
            {
                return date.AddDays(1).AddMilliseconds(-1);
            }

            public static DateTime GetYearStart(DateTime date)
            {
                return new DateTime(date.Year,1,1);
            }

            public static DateTime GetYearEnd(DateTime date)
            {
                return new DateTime(date.Year+1,1,1).AddMilliseconds(-1);
            }

            public static DateTime? GetMonthEnd(DateTime? date)
            {
                return date != null
                           ? (DateTime?) date.Value.Date.AddMonths(1).AddDays(-1*date.Value.Day + 1).AddMilliseconds(-1)
                           : null;
            }

            public static DateTime GetDateFromMonth21(int month21)
            {
                return new DateTime(2000 + (month21 / 100), month21 % 100, 1);
            }

			
        #endregion

        

        /// <summary>
        /// Combines filename and folder and returns first non-existent combination. If file exits, it appends _num where num is 1,2, 3 etc.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folder"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static string GetFilePath(string fileName, string folder, bool overwrite = false)
        {
            string filePath = Path.Combine(folder, fileName);
            if (!overwrite && File.Exists(filePath))
            {
                //If file exists, append _% until no such file exists
                int suffix = 1;
                string fileNoExt = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);
                do
                {
                    fileName = fileNoExt + "_" + suffix.ToString() + extension;
                    suffix++;
                    filePath = Path.Combine(folder, fileName);
                }
                while (File.Exists(filePath));
            }
            return filePath;
        }

        public static string WriteFile(string name, string folder, byte[] contents, bool overWrite = false)
        {
            string filePath = Utilities.GetFilePath(name, folder, overWrite);
            var sw = new StreamWriter(filePath);
            var ms = new MemoryStream(contents);
            ms.WriteTo(sw.BaseStream);
            sw.Close();

            return filePath;
        }

        public static T SafeGetElement<T>(IList<T> list, int index)
        {
            if (list != null && list.Count > index)
                return list[index];
            return default(T);
        }

        public static DateTime Min(DateTime d1, DateTime d2)
        {
            return d1 < d2 ? d1 : d2;
        }

        public static byte[] FileStreamToBytes(Stream s)
        {
            var ms = new MemoryStream();
            s.CopyTo(ms);
            return ms.ToArray();
        }

	    public static double? Min(double? first, double? second)
	    {
		    if (first == null || second == null)
			    return null;
		    return Math.Min(first.Value, second.Value);
	    }

    }

    public class RegexUtilities
    {
        static bool invalid = false;

        public static bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                   RegexOptions.IgnoreCase);
        }


        private static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }

    public static class Extensions
    {
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Contains(source);
        }

		[ExcludeFromCodeCoverage]
        public static TResult IfNotNull<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
            where TInput : class
        {
            if (o == null) return default(TResult);
            return evaluator(o);
        }

	    [ExcludeFromCodeCoverage]
        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        public static string Right(this string str, int length)
        {
	        if (length < 0)
		        return null;
            return str.Substring(Math.Max(0, str.Length - length));
        }

        public static string Left(this string str, int length)
        {
	        if (length < 0)
		        return null;
            return str.Substring(0,Math.Min(str.Length,length));
        }

        public static Month21 ToMonth21(this DateTime date)
        {
            return Month21.FromDate(date);
        }

        public static int ToMonth21Value(this DateTime date)
        {
            return Month21.FromDate(date).Value;
        }

        public static List<T> NotIn<T>(this List<T> list1, List<T> listToCompare, string compareProperty = "id")
        {
            var prop = (typeof(T)).GetProperty(compareProperty);
            if (prop != null)
                return list1.Where(elem => listToCompare.Count(celem => prop.GetValue(celem).Equals(prop.GetValue(elem))) == 0).ToList();
            return null;
        }

		public static string ToIsoDate(this DateTime? date)
		{
			if (date == null)
				return string.Empty;
			return date.Value.ToString("yyyy-MM-dd HH:mm");
		}

    }

}

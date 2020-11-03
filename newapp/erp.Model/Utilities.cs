using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


using System.ComponentModel;


namespace erp.Model
{
    public static class Extensions
    {

        public static string extension(this string filename)
        { 
            if (string.IsNullOrEmpty(filename))
                return string.Empty;
            else
                return System.IO.Path.GetExtension(filename).Substring(1);
        }

        

        public static bool Between<T>(this T source, T from, T to) where T : IComparable
        {
            return source.CompareTo(from) >= 0 && source.CompareTo(to) <= 0;
        }

        public static string ToString(this DateTime? dateTime, string format, IFormatProvider provider = null)
        {
            if (dateTime.HasValue)
            {
                if (provider != null)
                    return dateTime.Value.ToString(format, provider);
                else
                    return dateTime.Value.ToString(format);
            }
            else
                return "";
        }
        public static string ToString(this double? d, string format)
        {
            if (d.HasValue)
                return d.Value.ToString(format);
            return "";
        }

        public static string ToString(this double? d, string format, IFormatProvider provider)
        {
            if (d.HasValue)
                return d.Value.ToString(format,provider);
            return "";
        }

        public static string ToString(this float? d, string format)
        {
            if (d.HasValue)
                return d.Value.ToString(format);
            return "";
        }

        public static string ToString(this int? d, string format)
        {
            if (d.HasValue)
                return d.Value.ToString(format);
            return "";
        }

        public static int? Month(this DateTime? datetime)
        {
            if (datetime == null)
                return null;
            return datetime.Value.Month;
        }

        public static int? Year(this DateTime? datetime)
        {
            if (datetime == null)
                return null;
            return datetime.Value.Year;
        }

        public static DateTime? Date(this DateTime? datetime)
        {
            if (datetime == null)
                return null;
            return datetime.Value.Date;
        }

        public static int? Days(this TimeSpan? span)
        {
            if (span == null)
                return null;
            return span.Value.Days;
        }

        public static DateTime? AddDays(this DateTime? date, double days)
        {
            return date?.AddDays(days);
        }


    }

    public class Utilities
    {
        public static T? FromDbValue<T>(object value) where T: struct
        {
            if (value == null || value == DBNull.Value)
                return null;
            else
            {
                //try
                //{
                    TypeConverter t = TypeDescriptor.GetConverter(typeof(T));
                    if (value.GetType() != typeof(T))
                    {
                        return (T)t.ConvertTo(value, typeof(T));
                    }
                    return (T)value;
                //}
                //catch (Exception ex)
                //{
                //    throw;
                //}
            }
        }

        public static string GetStringOrNull(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            return string.Empty + value;
        }



        public static object GetReaderField(IDataReader dr, string fieldName)
        {
            object value = null;
            try
            {
                value = dr[fieldName];
            }
            catch
            {
                
            }
            return value;
        }


        public static string CheckLocalized(IDataReader dr, string fieldName, string suffix = "_t")
        {
            string text;
            if(ColumnExists(dr, fieldName + suffix))
            {
                text = string.Empty + dr[fieldName + suffix];
                if (string.IsNullOrEmpty(text))
                    text = string.Empty + dr[fieldName];
            }
            else
                text = string.Empty + dr[fieldName];
            
            return text;
        }

        public static Nullable<bool> BoolFromLong(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            else
                return Convert.ToBoolean(value);
        }

        public static bool ColumnExists(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).ToLower() == columnName.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        public static HashSet<string> GetColumnNames(IDataReader reader)
        {
            var result = new HashSet<string>();
            for (int i = 0; i < reader.FieldCount; i++) {
                result.Add(reader.GetName(i));
            }
            return result;
        }

        

        public static DateTime GetFirstDayInWeek(DateTime date)
        {
            return company.Common.Utilities.GetFirstDayInWeek(date);
        }

        public static DateTime GetMonthStart(DateTime date)
        {
            return company.Common.Utilities.GetMonthStart(date);
        }

        public static DateTime GetMonthEnd(DateTime date)
        {
            return company.Common.Utilities.GetMonthEnd(date);
        }
       

        public static List<Dealer> BuildDealersList(List<Dealer> dealers, int maxNum = 8 )
        {
            if (dealers != null)
            {
                //var brandGroups = BrandGroupDAL.GetBrandGroups(Properties.Settings.Default.BrandId);
                var firstGoldDealer = GetDealer(dealers, true, false, 0);
                var firstSilverDealer = GetDealer(dealers, false, true, 0);
                bool hasGold = firstGoldDealer != null, hasSilver = firstSilverDealer != null;
                var startOfOtherDealers = 0;
                var numOfOtherDealers = Math.Min(maxNum, dealers.Count);
                if (!hasGold)
                {
                    if (hasSilver)
                    {
                        firstGoldDealer = firstSilverDealer;
                    }
                    else
                    {
                        firstGoldDealer = GetOtherDealer(dealers, 0);
                        startOfOtherDealers++;
                        numOfOtherDealers--;
                    }
                }
                else
                    numOfOtherDealers--;

                if (!hasSilver)
                {
                    if (hasGold)
                    {
                        firstSilverDealer = firstGoldDealer;
                    }
                    else
                    {
                        firstSilverDealer = GetOtherDealer(dealers, 1);
                        startOfOtherDealers++;
                        numOfOtherDealers--;
                    }
                }
                else
                    numOfOtherDealers--;

                var selectedDealers = new List<Dealer>();
                if (firstGoldDealer != null || firstSilverDealer != null)
                {
                    selectedDealers.Add(firstGoldDealer);
                    if (firstSilverDealer != firstGoldDealer && firstSilverDealer != null)
                        selectedDealers.Add(firstSilverDealer);

                    for (int i = 0; i < numOfOtherDealers; i++)
                    {
                        var dealer = GetOtherDealer(dealers, i + startOfOtherDealers);
                        if (dealer != null)
                            selectedDealers.Add(GetOtherDealer(dealers, i + startOfOtherDealers));
                    }
                }
                return selectedDealers; 
            }
            return null;
        }

        public static Dealer GetOtherDealer(List<Dealer> dealers,int order)
        {
            var firstGold = GetDealer(dealers,true, false, 0);
            var firstSilver = GetDealer(dealers,false, true, 0);
            int[] excludeIds = { 0, 0 };
            if (firstGold != null)
                excludeIds[0] = firstGold.user_id;
            if (firstSilver != null)
                excludeIds[1] = firstSilver.user_id;
            return dealers.Where(d => !excludeIds.Contains(d.user_id)).ElementAtOrDefault(order);
        }

        public static Dealer GetDealer(List<Dealer> dealers,  bool isGold, bool isSilver, int order)
        {
            if (dealers == null)
                return null;
            else
                return dealers.Where(d => d.IsGold == isGold && d.IsSilver == isSilver).ElementAtOrDefault(order);
        }


        public static object ToDBNull(object value)
        {
            return value != null ? (object) value : DBNull.Value;
        }

        public static DateTime Min(DateTime d1, DateTime d2)
        {
            return d1 < d2 ? d1 : d2;
        }

        public static double GetGBPPrice(double price, int currency)
        {
            return currency == 0 ? price/1.6 : currency == 2 ? price/1.2 : price;
        }
       
    }
}

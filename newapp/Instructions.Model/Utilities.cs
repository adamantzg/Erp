using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.ComponentModel;


namespace Instructions.Model
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

        //public static bool In<T>(this T source, params T[] list)
        //{
        //    if (null == source) throw new ArgumentNullException("source");
        //    return list.Contains(source);
        //}

        public static string ToString(this DateTime? dateTime, string format)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToString(format);
            else
                return "";
        }
        public static string ToString(this double? d, string format)
        {
            if (d.HasValue)
                return d.Value.ToString(format);
            return "";
        }
    }

    public class Utilities
    {
        public static Nullable<T> FromDbValue<T>(object value) where T: struct
        {
            if (value == null || value == DBNull.Value)
                return null;
            else
            {
                TypeConverter t = TypeDescriptor.GetConverter(typeof(T));
                if (value.GetType() != typeof(T))
                {
                    return (T)t.ConvertTo(value, typeof(T));
                }
                else
                    //    return (T)value;
                    //if (typeof(T) == typeof(int) && value.GetType() != typeof(int))
                    //{
                    //    TypeConverter t = TypeDescriptor.GetConverter(;
                    //    return (T)t.ConvertTo(value, typeof(T));
                    //}
                    //else
                    return (T)value;
            }
        }

        public static object GetReaderField(MySqlDataReader dr, string fieldName)
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


        public static string CheckLocalized(MySqlDataReader dr, string fieldName, string suffix = "_t")
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
                if (reader.GetName(i) == columnName)
                {
                    return true;
                }
            }

            return false;
        }

        public static string CreateParametersFromIdList(MySqlCommand cmd, List<int> ids)
        {
            List<string> paramList = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                string paramName = "@id" + (i + 1).ToString();
                paramList.Add(paramName);
                MySqlParameter prm = new MySqlParameter(paramName, ids[i]);
                cmd.Parameters.Add(prm);
            }
            return string.Join(",", paramList);
        }

        public static MySqlCommand GetCommand(string sql = "", MySqlConnection conn = null, MySqlTransaction tr = null)
        {
            return new MySqlCommand(sql, conn, tr) { CommandTimeout = Properties.Settings.Default.DefaultTimeout };
        }

    }
}

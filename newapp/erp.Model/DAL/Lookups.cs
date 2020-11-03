using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace asaq2.Model
{
    public class LookupsDal
    {
        public const string LookupTextField = "value";
        public const string LookupIdField = "id";

        /// <summary>
        /// Vraća zapise koji počinju sa zadanim prefiksom. Koristim sqlreader zbog jednostavnosti - sa EF-om bih morao tableName i tableField mapirati u n strong-typed querya ili uvoditi predicatebuilder
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="count"></param>
        /// <param name="tableName"></param>
        /// <param name="tableField"></param>
        /// <param name="idField"></param>
        /// <returns></returns>
        public static IEnumerable<LookupItem> GetLookupItems(string prefix, int count, string tableName, string tableField, string idField = "id")
        {
            List<LookupItem> list = new List<LookupItem>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(string.Format("SELECT {3} {0},{1} FROM {2} WHERE LEFT({1}, LEN(@prefix)) = @prefix ORDER BY {1}", idField, tableField, tableName, (count == 0 ? "" : string.Format(" TOP {0} ", count))), conn);
                cmd.Parameters.Add(new MySqlParameter("@prefix", prefix));
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    LookupItem li = new LookupItem();
                    li.id = (int) dr[idField];
                    li.value = string.Empty + dr[tableField];
                    list.Add(li);
                }
                dr.Close();
                conn.Close();
            }
            return list;
        }

        /// <summary>
        /// returns all records
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableField"></param>
        /// <param name="idField"></param>
        /// <returns></returns>
        public static IEnumerable<LookupItem> GetLookupItems(string tableName, string tableField, string idField = "id", string sortField = "")
        {
            List<LookupItem> list = new List<LookupItem>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(string.Format("SELECT {0},{1} FROM {2} ORDER BY {3}", idField, tableField, tableName, sortField.Length > 0 ? sortField : tableField), conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    LookupItem li = new LookupItem();
                    li.id = (int)dr[idField];
                    li.value = string.Empty + dr[tableField];
                    list.Add(li);
                }
                dr.Close();
                conn.Close();
            }
            return list;
        }

        /// <summary>
        /// Traži lookup po tekstu ili id-u
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableField"></param>
        /// <param name="text"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static LookupItem GetLookupItem(string tableName, string tableField, string text, Guid? id = null, string idField="id")
        {
            LookupItem li = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(string.Format("SELECT {0},{1} FROM {2} WHERE ({1} = @text OR @text IS NULL) AND ({0} = @id OR @id IS NULL)  ORDER BY {1}", idField, tableField, tableName), conn);
                cmd.Parameters.Add(new MySqlParameter("@id", id != null ? (object) id : DBNull.Value));
                cmd.Parameters.Add(new MySqlParameter("@text", text != null ? (object) text : DBNull.Value));
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    li = new LookupItem();
                    li.id = (int)dr[idField];
                    li.value = string.Empty + dr[tableField];
                }
                dr.Close();
                conn.Close();
            }
            return li;
        }
    }

    [Serializable]
    public class LookupItem
    {
        public string value { get; set; }
        public int id { get; set; }
    }   

}

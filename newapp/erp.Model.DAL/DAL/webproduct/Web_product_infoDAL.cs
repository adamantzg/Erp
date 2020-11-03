
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Web_product_infoDAL
	{
	
		public static List<Web_product_info> GetAll()
		{
			var result = new List<Web_product_info>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_info", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Web_product_info> GetForProduct(int web_unique, IDbConnection conn = null,int? language_id = null)
        {
            var result = new List<Web_product_info>();
            
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            try
            {
                var cmd = Utils.GetCommand(GetSelect(@"SELECT web_product_info.* {0} FROM web_product_info {1} WHERE web_unique = @web_unique ORDER BY `order`", language_id != null,commaBeforeFields:true, commaAfterFields:false), (MySqlConnection)conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                if (language_id != null)
                    cmd.Parameters.AddWithValue("@lang", language_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            finally
            {
                if (dispose)
                    conn.Dispose();
            }

            return result;

        }
		
		
		public static Web_product_info GetById(int id)
		{
			Web_product_info result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_info WHERE id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
		
	
		public static Web_product_info GetFromDataReader(MySqlDataReader dr)
		{
			Web_product_info o = new Web_product_info();
		
			o.id =  (int) dr["id"];
			o.web_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"web_unique"));
			o.type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"type"));
			o.value = string.Empty + Utilities.GetReaderField(dr,"value");
		    o.order = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "order"));
			
			return o;

		}
		
		
		public static void Create(Web_product_info o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_product_info (web_unique,type,value,`order`) VALUES(@web_unique,@type,@value,@order)";

		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if(tr == null)
                    conn.Open();

                var cmd = Utils.GetCommand(insertsql, (MySqlConnection)conn, (MySqlTransaction)tr);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM web_product_info WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
            finally
		    {
                if(tr == null)
                    conn.Dispose();
		    }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_info o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@type", o.type);
			cmd.Parameters.AddWithValue("@value", o.value);
            cmd.Parameters.AddWithValue("@order", o.order);
        }
		
		public static void Update(Web_product_info o,IDbTransaction tr = null)
		{
			string updatesql = @"UPDATE web_product_info SET web_unique = @web_unique,type = @type,value = @value, `order`= @order WHERE id = @id";

			var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
                var cmd = Utils.GetCommand(updatesql, (MySqlConnection)conn, (MySqlTransaction)tr);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
		}
		
		public static void Delete(int id, IDbTransaction tr = null)
		{
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM web_product_info WHERE id = @id", (MySqlConnection)conn, (MySqlTransaction)tr);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
		}

        public static void DeleteAll()
        {
            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM web_product_info", conn);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Dispose();
            }
        }

        public static void DeleteMissing(int web_unique, IList<int> ids = null, IDbTransaction tr = null)
        {
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
                var cmd = Utils.GetCommand("", (MySqlConnection)conn, (MySqlTransaction)tr);
                cmd.CommandText =
                    string.Format("DELETE FROM web_product_info WHERE web_unique = @web_unique {0}",
                                  ids!= null && ids.Count > 0 ? string.Format(" AND id NOT IN ({0})", Utils.CreateParametersFromIdList(cmd, ids)) : "");
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
        }

        private static string GetSelect(string initialSql, bool localize = false, bool commaBeforeFields = false, bool commaAfterFields = true)
        {
            List<string> fields = new List<string>();
            string join = string.Empty;
            if (localize)
            {
                fields.Add(GetTranslationFields(false));
                join = GetTranslationJoin();
            }

            return string.Format(initialSql, (commaBeforeFields && fields.Count > 0 ? ", " : "") + string.Join(",", fields.ToArray()) + (commaAfterFields && fields.Count > 0 ? "," : ""), join);
        }

        private static string GetTranslationJoin()
        {
            return @" LEFT OUTER JOIN web_product_info_translate ON (web_product_info.id = web_product_info_translate.info_id AND web_product_info_translate.language_id = @lang)";
        }

        private static string GetTranslationFields(bool productOnly = true)
        {
            return @"web_product_info_translate.`value`";
        }
		
		
	}
}
			
			
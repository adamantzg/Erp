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
    public class Web_option_headersDAL
	{
		public static List<Web_option_headers> GetAll()
		{
			var result = new List<Web_option_headers>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM web_option_headers", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		public static Web_option_headers GetById(int id)
		{
			Web_option_headers result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM web_option_headers WHERE id = @id", conn);
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
		
	
		public static Web_option_headers GetFromDataReader(MySqlDataReader dr)
		{
			Web_option_headers o = new Web_option_headers();
		
			o.id =  (int) dr["id"];
			o.value = string.Empty + Utilities.GetReaderField(dr,"value");
			
			return o;

		}
		
		
		public static void Create(Web_option_headers o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_option_headers (value) VALUES(@value)";

		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if(tr == null)
                    conn.Open();
				
				var cmd = new MySqlCommand(insertsql,(MySqlConnection) conn,(MySqlTransaction) tr);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM web_option_headers WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_option_headers o, bool forInsert = true)
        {
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@value", o.value);
		}

        public static void Update(Web_option_headers o, IDbTransaction tr = null)
		{
			string updatesql = @"UPDATE web_option_headers SET value=@value WHERE id = @id";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();

                var cmd = new MySqlCommand(updatesql, (MySqlConnection)conn,(MySqlTransaction) tr);
                BuildSqlParameters(cmd, o);
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
                if(tr == null)
                    conn.Open();
				var cmd = new MySqlCommand("DELETE FROM web_option_headers WHERE id = @id" ,(MySqlConnection) conn,(MySqlTransaction) tr);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
		}
	}
}
			
			
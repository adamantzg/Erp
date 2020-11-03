
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
    public class Web_product_flowDAL
	{
	
		public static List<Web_product_flow> GetAll()
		{
			var result = new List<Web_product_flow>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_flow", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Web_product_flow> GetForProduct(int web_unique, IDbConnection conn = null)
        {
            var result = new List<Web_product_flow>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            try
            {
                var cmd = Utils.GetCommand("SELECT * FROM web_product_flow WHERE web_unique = @web_unique", (MySqlConnection)conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            finally
            {
                if(dispose)
                    conn.Dispose();
            }
            
            return result;
        }
		
		
		public static Web_product_flow GetById(int id)
		{
			Web_product_flow result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_flow WHERE id = @id", conn);
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
		
	
		public static Web_product_flow GetFromDataReader(MySqlDataReader dr)
		{
			Web_product_flow o = new Web_product_flow();
		
			o.id =  (int) dr["id"];
			o.web_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"web_unique"));
			o.part_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"part_id"));
			o.pressure_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"pressure_id"));
			o.value = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"value"));
			
			return o;

		}
		
		
		public static void Create(Web_product_flow o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_product_flow (web_unique,part_id,pressure_id,value) VALUES(@web_unique,@part_id,@pressure_id,@value)";

		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
                var cmd = Utils.GetCommand(insertsql, (MySqlConnection)conn, (MySqlTransaction)tr);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM web_product_flow WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_flow o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@part_id", o.part_id);
			cmd.Parameters.AddWithValue("@pressure_id", o.pressure_id);
			cmd.Parameters.AddWithValue("@value", o.value);
		}
		
		public static void Update(Web_product_flow o, IDbTransaction tr = null)
		{
			string updatesql = @"UPDATE web_product_flow SET web_unique = @web_unique,part_id = @part_id,pressure_id = @pressure_id,value = @value WHERE id = @id";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
		    try
		    {
                if (tr == null)
                    conn.Open();
                var cmd = Utils.GetCommand(updatesql, (MySqlConnection)conn, (MySqlTransaction)tr);
		        BuildSqlParameters(cmd, o, false);
		        cmd.ExecuteNonQuery();
		    }
		    finally
		    {
                if (tr == null)
                    conn.Dispose();
		    }
		}
		
		public static void Delete(int id,IDbTransaction tr = null)
		{
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM web_product_flow WHERE id = @id" ,(MySqlConnection) conn ,(MySqlTransaction) tr);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
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
                    string.Format("DELETE FROM web_product_flow WHERE web_unique = @web_unique {0}",
                                  ids != null && ids.Count > 0 ? string.Format(" AND id NOT IN ({0})", Utils.CreateParametersFromIdList(cmd, ids)) : "");                
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
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
			
			
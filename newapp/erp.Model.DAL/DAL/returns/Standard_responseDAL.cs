
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Standard_responseDAL
	{
	
		public static List<Standard_response> GetAll()
		{
			var result = new List<Standard_response>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM standard_response", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Standard_response GetById(int id)
		{
			Standard_response result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM standard_response WHERE response_id = @id", conn);
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
		
	
		private static Standard_response GetFromDataReader(MySqlDataReader dr)
		{
			Standard_response o = new Standard_response();
		
			o.response_id =  (int) dr["response_id"];
			o.response_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"response_type"));
			o.response_text = string.Empty + Utilities.GetReaderField(dr,"response_text");
			
			return o;

		}
		
		
		public static void Create(Standard_response o)
        {
            string insertsql = @"INSERT INTO standard_response (response_id,response_type,response_text) VALUES(@response_id,@response_type,@response_text)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(response_id)+1 FROM standard_response", conn);
                o.response_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Standard_response o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@response_id", o.response_id);
			cmd.Parameters.AddWithValue("@response_type", o.response_type);
			cmd.Parameters.AddWithValue("@response_text", o.response_text);
		}
		
		public static void Update(Standard_response o)
		{
			string updatesql = @"UPDATE standard_response SET response_type = @response_type,response_text = @response_text WHERE response_id = @response_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int response_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM standard_response WHERE response_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", response_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
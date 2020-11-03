
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Return_resolutionDAL
	{
	
		public static List<Return_resolution> GetAll()
		{
			var result = new List<Return_resolution>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM return_resolution", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Return_resolution GetById(int id)
		{
			Return_resolution result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM return_resolution WHERE id = @id", conn);
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
		
	
		private static Return_resolution GetFromDataReader(MySqlDataReader dr)
		{
			Return_resolution o = new Return_resolution();
		
			o.id =  (int) dr["id"];
			o.resolution = string.Empty + Utilities.GetReaderField(dr,"resolution");
			
			return o;

		}
		
		
		public static void Create(Return_resolution o)
        {
            string insertsql = @"INSERT INTO return_resolution (id,resolution) VALUES(@id,@resolution)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(id)+1 FROM return_resolution", conn);
                o.id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Return_resolution o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@resolution", o.resolution);
		}
		
		public static void Update(Return_resolution o)
		{
			string updatesql = @"UPDATE return_resolution SET resolution = @resolution WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM return_resolution WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
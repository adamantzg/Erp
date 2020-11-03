
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Instructions.Model
{
    public class FlagDAL
	{
	
		public static List<Flag> GetAll()
		{
			var result = new List<Flag>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM flag", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Flag GetById(int id)
		{
			Flag result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM flag WHERE flag_id = @id", conn);
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
		
	
		public static Flag GetFromDataReader(MySqlDataReader dr)
		{
			Flag o = new Flag();
		
			o.flag_id =  (int) dr["flag_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.image = string.Empty + Utilities.GetReaderField(dr,"image");
			
			return o;

		}
		
		
		public static void Create(Flag o)
        {
            string insertsql = @"INSERT INTO flag (name,image) VALUES(@name,@image)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utilities.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT flag_id FROM flag WHERE flag_id = LAST_INSERT_ID()";
                o.flag_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Flag o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@flag_id", o.flag_id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@image", o.image);
		}
		
		public static void Update(Flag o)
		{
			string updatesql = @"UPDATE flag SET name = @name,image = @image WHERE flag_id = @flag_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utilities.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int flag_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utilities.GetCommand("DELETE FROM flag WHERE flag_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", flag_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
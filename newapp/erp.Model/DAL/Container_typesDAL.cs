
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Container_typesDAL
	{
	
		public static List<Container_types> GetAll()
		{
			List<Container_types> result = new List<Container_types>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM container_types", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Container_types GetById(int id)
		{
			Container_types result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM container_types WHERE container_type_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
	
		private static Container_types GetFromDataReader(MySqlDataReader dr)
		{
			Container_types o = new Container_types();
		
			o.container_type_id =  (int) dr["container_type_id"];
			o.container_type_desc = string.Empty + dr["container_type_desc"];
			o.width = Utilities.FromDbValue<double>(dr["width"]);
			o.length = Utilities.FromDbValue<double>(dr["length"]);
			o.height = Utilities.FromDbValue<double>(dr["height"]);
			
			return o;

		}
		
		public static void Create(Container_types o)
        {
            string insertsql = @"INSERT INTO container_types (container_type_desc,width,length,height) VALUES(@container_type_desc,@width,@length,@height)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT container_type_id FROM container_types WHERE container_type_id = LAST_INSERT_ID()";
                o.container_type_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Container_types o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@container_type_id", o.container_type_id);
			cmd.Parameters.AddWithValue("@container_type_desc", o.container_type_desc);
			cmd.Parameters.AddWithValue("@width", o.width);
			cmd.Parameters.AddWithValue("@length", o.length);
			cmd.Parameters.AddWithValue("@height", o.height);
		}
		
		public static void Update(Container_types o)
		{
			string updatesql = @"UPDATE container_types SET container_type_desc = @container_type_desc,width = @width,length = @length,height = @height WHERE container_type_id = @container_type_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int container_type_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM container_types WHERE container_type_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", container_type_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			
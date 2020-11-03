
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class RoleDAL
	{
	
		public static List<Role> GetAll()
		{
			var result = new List<Role>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM role", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Role GetById(int id)
		{
			Role result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM role WHERE id = @id", conn);
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
		
	
		public static Role GetFromDataReader(MySqlDataReader dr)
		{
			Role o = new Role();
		
			o.id =  (int) dr["id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			
			return o;

		}
		
		
		public static void Create(Role o)
        {
            string insertsql = @"INSERT INTO role (id,name) VALUES(@id,@name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(id)+1 FROM role", conn);
                o.id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Role o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@name", o.name);
		}
		
		public static void Update(Role o)
		{
			string updatesql = @"UPDATE role SET name = @name WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM role WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
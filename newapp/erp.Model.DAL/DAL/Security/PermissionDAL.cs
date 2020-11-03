
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class PermissionDAL
	{
	
		public static List<Permission> GetAll()
		{
			var result = new List<Permission>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM permission", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Permission GetById(int id)
		{
			Permission result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM permission WHERE id = @id", conn);
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
		
	
		public static Permission GetFromDataReader(MySqlDataReader dr)
		{
			Permission o = new Permission();
		
			o.id =  (int) dr["id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			
			return o;

		}
		
		
		public static void Create(Permission o)
        {
            string insertsql = @"INSERT INTO permission (id,name) VALUES(@id,@name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(id)+1 FROM permission", conn);
                o.id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Permission o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@name", o.name);
		}
		
		public static void Update(Permission o)
		{
			string updatesql = @"UPDATE permission SET name = @name WHERE id = @id";

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
				var cmd = Utils.GetCommand("DELETE FROM permission WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
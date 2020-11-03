using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_product_file_renameDAL
	{
		public static List<Web_product_file_rename> GetAll()
		{
            var result = new List<Web_product_file_rename>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file_rename", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static Web_product_file_rename GetById(int id)
		{
            Web_product_file_rename result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file_rename WHERE id = @id", conn);
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

        public static Web_product_file_rename GetFromDataReader(MySqlDataReader dr)
		{
            Web_product_file_rename o = new Web_product_file_rename();
		
			o.id =  (int) dr["id"];
            o.name = string.Empty + Utilities.GetReaderField(dr, "name");
            o.name = string.Empty + Utilities.GetReaderField(dr, "oldname");
			o.web_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"web_unique"));
			o.file_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"file_type"));

			return o;

		}
		
		
		public static void Create(Web_product_file_rename o)
        {
            string insertsql = string.Format(@"INSERT INTO Web_product_file_rename (id,name,oldname,web_unique,file_type) VALUES(@id,@name,@oldname,@web_unique,@file_type)");

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_file_rename o)
        {
		    cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@name", o.name);
            cmd.Parameters.AddWithValue("@oldname", o.oldname);
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@file_type", o.file_type);
		}
		
		public static void Update(Web_product_file_rename o)
		{
            string updatesql = @"UPDATE Web_product_file_rename SET id = @id, name = @name,oldname = @oldname,web_unique = @web_unique,file_type = @file_type WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM Web_product_file_rename WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
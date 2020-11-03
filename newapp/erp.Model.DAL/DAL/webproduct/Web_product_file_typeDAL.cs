
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_product_file_typeDAL
	{
	
		public static List<Web_product_file_type> GetAll()
		{
			var result = new List<Web_product_file_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file_type", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Web_product_file_type GetById(int id)
		{
			Web_product_file_type result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file_type WHERE id = @id", conn);
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
		
	
		public static Web_product_file_type GetFromDataReader(MySqlDataReader dr)
		{
			Web_product_file_type o = new Web_product_file_type();
		
			o.id =  (int) dr["id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.code = string.Empty + Utilities.GetReaderField(dr,"code");
			o.site_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"site_id"));
			o.file_type_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"file_type_id"));
			o.path = string.Empty + Utilities.GetReaderField(dr,"path");
			o.previewpath = string.Empty + Utilities.GetReaderField(dr,"previewpath");
			o.suffix = string.Empty + Utilities.GetReaderField(dr,"suffix");
			
			return o;

		}
		
		
		public static void Create(Web_product_file_type o)
        {
            string insertsql = @"INSERT INTO web_product_file_type (id,name,code,site_id,file_type_id,path,previewpath,suffix) VALUES(@id,@name,@code,@site_id,@file_type_id,@path,@previewpath,@suffix)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(id)+1 FROM web_product_file_type", conn);
                o.id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_file_type o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@code", o.code);
			cmd.Parameters.AddWithValue("@site_id", o.site_id);
			cmd.Parameters.AddWithValue("@file_type_id", o.file_type_id);
			cmd.Parameters.AddWithValue("@path", o.path);
			cmd.Parameters.AddWithValue("@previewpath", o.previewpath);
			cmd.Parameters.AddWithValue("@suffix", o.suffix);
		}
		
		public static void Update(Web_product_file_type o)
		{
			string updatesql = @"UPDATE web_product_file_type SET name = @name,code = @code,site_id = @site_id,file_type_id = @file_type_id,path = @path,previewpath = @previewpath,suffix = @suffix WHERE id = @id";

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
				var cmd = Utils.GetCommand("DELETE FROM web_product_file_type WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
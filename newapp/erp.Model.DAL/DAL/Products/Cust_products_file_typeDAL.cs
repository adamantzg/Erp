
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Cust_products_file_typeDAL
	{
	
		public static List<Cust_products_file_type> GetAll()
		{
			var result = new List<Cust_products_file_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM cust_products_file_type", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Cust_products_file_type GetById(int id)
		{
			Cust_products_file_type result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM cust_products_file_type WHERE id = @id", conn);
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
		
	
		public static Cust_products_file_type GetFromDataReader(MySqlDataReader dr)
		{
			Cust_products_file_type o = new Cust_products_file_type();
		
			o.id =  (int) dr["id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.file_type_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"file_type_id"));
			o.suffix = string.Empty + Utilities.GetReaderField(dr,"suffix");
			o.path = string.Empty + Utilities.GetReaderField(dr,"path");
			//o.fieldname_mast = string.Empty + Utilities.GetReaderField(dr,"fieldname_mast");
			o.fieldname = string.Empty + Utilities.GetReaderField(dr,"fieldname");
			
			return o;

		}
		
		
		public static void Create(Cust_products_file_type o)
        {
            string insertsql = @"INSERT INTO cust_products_file_type (name,file_type_id,suffix,path,fieldname_mast,fieldname) VALUES(@name,@file_type_id,@suffix,@path,@fieldname_mast,@fieldname)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM cust_products_file_type WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Cust_products_file_type o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@file_type_id", o.file_type_id);
			cmd.Parameters.AddWithValue("@suffix", o.suffix);
			cmd.Parameters.AddWithValue("@path", o.path);
			//cmd.Parameters.AddWithValue("@fieldname_mast", o.fieldname_mast);
			cmd.Parameters.AddWithValue("@fieldname", o.fieldname);
		}
		
		public static void Update(Cust_products_file_type o)
		{
			string updatesql = @"UPDATE cust_products_file_type SET name = @name,file_type_id = @file_type_id,suffix = @suffix,path = @path,fieldname_mast = @fieldname_mast,fieldname = @fieldname WHERE id = @id";

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
				var cmd = Utils.GetCommand("DELETE FROM cust_products_file_type WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
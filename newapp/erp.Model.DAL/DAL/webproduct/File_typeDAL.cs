
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
    public partial class File_typeDAL
	{
	
		public static List<File_type> GetAll(IDbConnection conn = null)
		{
			var result = new List<File_type>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            //using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                
                var cmd = Utils.GetCommand("SELECT * FROM file_type", (MySqlConnection) conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
                if(dispose)
                    conn.Dispose();
            }
            return result;
		}
		
		
		public static File_type GetById(int id)
		{
			File_type result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM file_type WHERE id = @id", conn);
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
		
	
		public static File_type GetFromDataReader(MySqlDataReader dr)
		{
			File_type o = new File_type();
		
			o.id =  (int) dr["id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.path = string.Empty + Utilities.GetReaderField(dr,"path");
			o.previewpath = string.Empty + Utilities.GetReaderField(dr,"previewpath");
			
			return o;

		}
		
		
		public static void Create(File_type o)
        {
            string insertsql = @"INSERT INTO file_type (id,name,path,previewpath) VALUES(@id,@name,@path,@previewpath)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(id)+1 FROM file_type", conn);
                o.id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, File_type o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@path", o.path);
			cmd.Parameters.AddWithValue("@previewpath", o.previewpath);
		}
		
		public static void Update(File_type o)
		{
			string updatesql = @"UPDATE file_type SET name = @name,path = @path,previewpath = @previewpath WHERE id = @id";

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
				var cmd = Utils.GetCommand("DELETE FROM file_type WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
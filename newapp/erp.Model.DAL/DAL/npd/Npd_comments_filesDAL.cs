
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Npd_comments_filesDAL
	{
	
		public static List<Npd_comments_files> GetAll()
		{
			var result = new List<Npd_comments_files>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM npd_comments_files", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Npd_comments_files GetById(int id)
		{
			Npd_comments_files result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM npd_comments_files WHERE file_id = @id", conn);
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
		
	
		public static Npd_comments_files GetFromDataReader(MySqlDataReader dr)
		{
			Npd_comments_files o = new Npd_comments_files();
		
			o.file_id =  (int) dr["file_id"];
			o.npd_comment_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"npd_comment_id"));
            o.filename = string.Empty + Utilities.GetReaderField(dr, "filename");
		    o.filetype_id = Utilities.FromDbValue<int>(dr["filetype_id"]);
			return o;

		}
		
		
		public static void Create(Npd_comments_files o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO npd_comments_files (npd_comment_id,filename,filetype_id) VALUES(@npd_comment_id,@filename,@filetype_id)";

            var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
            if (tr == null)
                conn.Open(); 
            var cmd = Utils.GetCommand(insertsql, conn,tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT file_id FROM npd_comments_files WHERE file_id = LAST_INSERT_ID()";
            o.file_id = (int) cmd.ExecuteScalar();
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Npd_comments_files o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@file_id", o.file_id);
			cmd.Parameters.AddWithValue("@npd_comment_id", o.npd_comment_id);
            cmd.Parameters.AddWithValue("@filename", o.filename);
		    cmd.Parameters.AddWithValue("@filetype_id", o.filetype_id);
        }
		
		public static void Update(Npd_comments_files o, MySqlTransaction tr = null)
		{
            string updatesql = @"UPDATE npd_comments_files SET npd_comment_id = @npd_comment_id,filename = @filename,filetype_id = @filetype_id WHERE file_id = @file_id";

			var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
            if (tr == null)
                conn.Open(); 
			MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
            BuildSqlParameters(cmd,o, false);
            cmd.ExecuteNonQuery();
            
		}
		
		public static void Delete(int file_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM npd_comments_files WHERE file_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", file_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
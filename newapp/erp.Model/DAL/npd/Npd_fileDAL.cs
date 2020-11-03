
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Npd_fileDAL
	{
	
		public static List<Npd_file> GetAll()
		{
			var result = new List<Npd_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM npd_file", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Npd_file> GetByNpd(int npd_id)
        {
            var result = new List<Npd_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM npd_file WHERE npd_id = @npd_id", conn);
                cmd.Parameters.AddWithValue("@npd_id", npd_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Npd_file GetById(int id)
		{
			Npd_file result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM npd_file WHERE file_id = @id", conn);
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
		
	
		private static Npd_file GetFromDataReader(MySqlDataReader dr)
		{
			Npd_file o = new Npd_file();
		
			o.file_id =  (int) dr["file_id"];
			o.npd_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"npd_id"));
			o.filename = string.Empty + Utilities.GetReaderField(dr,"filename");
		    o.filetype_id = Utilities.FromDbValue<int>(dr["filetype_id"]);
			
			return o;

		}
		
		
		public static void Create(Npd_file o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO npd_file (npd_id,filename,filetype_id) VALUES(@npd_id,@filename,@filetype_id)";

			var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
            if(tr == null)
                conn.Open();
            var cmd = new MySqlCommand(insertsql, conn,tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT file_id FROM npd_file WHERE file_id = LAST_INSERT_ID()";
            o.file_id = (int) cmd.ExecuteScalar();
			
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Npd_file o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@file_id", o.file_id);
			cmd.Parameters.AddWithValue("@npd_id", o.npd_id);
			cmd.Parameters.AddWithValue("@filename", o.filename);
		    cmd.Parameters.AddWithValue("@filetype_id", o.filetype_id);
        }
		
		public static void Update(Npd_file o)
		{
			string updatesql = @"UPDATE npd_file SET npd_id = @npd_id,filename = @filename,filetype_id = @filetype_id WHERE file_id = @file_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int file_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM npd_file WHERE file_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", file_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
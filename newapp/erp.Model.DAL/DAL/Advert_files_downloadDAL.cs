
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Advert_files_downloadDAL
	{
	
		public static List<Advert_files_download> GetAll()
		{
			List<Advert_files_download> result = new List<Advert_files_download>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM advert_files_download", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Advert_files_download GetById(int id)
		{
			Advert_files_download result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM advert_files_download WHERE id = @id", conn);
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
	
		private static Advert_files_download GetFromDataReader(MySqlDataReader dr)
		{
			Advert_files_download o = new Advert_files_download();
		
			o.id =  (int) dr["id"];
			o.postcode = string.Empty + dr["postcode"];
			o.date = Utilities.FromDbValue<DateTime>(dr["date"]);
            o.ip_address = string.Empty + dr["ip_address"];
            o.filename = string.Empty + dr["filename"];
            o.type = Utilities.FromDbValue<int>(dr["type"]);
			
			return o;

		}
		
		public static void Create(Advert_files_download o)
        {
            string insertsql = @"INSERT INTO advert_files_download (postcode,date,ip_address,filename,type) VALUES(@postcode,@date,@ip_address,@filename,@type)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Advert_files_download o, bool forInsert = true)
        {
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@postcode", o.postcode);
            cmd.Parameters.AddWithValue("@date", o.date);
            cmd.Parameters.AddWithValue("@ip_address", o.ip_address);
            cmd.Parameters.AddWithValue("@filename", o.filename);
            cmd.Parameters.AddWithValue("@type", o.type);
		}
		
		public static void Update(Advert_files_download o)
		{
            string updatesql = @"UPDATE advert_files_download SET postcode = @postcode,date = @date,ip_address = @ip_address,filename = @filename, type=@type WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM advert_files_download WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<Advert_files_download> GetByType(int type)
        {
            List<Advert_files_download> result = new List<Advert_files_download>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM advert_files_download WHERE type=@type", conn);
                cmd.Parameters.AddWithValue("@type",type);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
	}
}
			
			
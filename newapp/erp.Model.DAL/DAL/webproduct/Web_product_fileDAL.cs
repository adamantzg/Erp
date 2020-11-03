
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Web_product_fileDAL
	{
	
		public static List<Web_product_file> GetAll()
		{
			var result = new List<Web_product_file>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Web_product_file GetById(int id)
		{
			Web_product_file result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file WHERE id = @id", conn);
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
		
	
		public static Web_product_file GetFromDataReader(MySqlDataReader dr)
		{
			Web_product_file o = new Web_product_file();
		
			o.id =  (int) dr["id"];
			o.web_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"web_unique"));
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.file_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"file_type"));
			o.width = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"width"));
			o.height = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"height"));
			o.image_site_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"image_site_id"));
            o.hires_tif = string.Empty + Utilities.GetReaderField(dr, "hires_tif");
            o.hires_psd = string.Empty + Utilities.GetReaderField(dr, "hires_psd");
            o.hires_eps = string.Empty + Utilities.GetReaderField(dr, "hires_eps");
            o.approval = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"approval"));
			o.created= Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "created"));
            return o;

		}
		
		
		public static void Create(Web_product_file o, bool identity_insert = false)
        {
            string insertsql = string.Format(@"INSERT INTO web_product_file ({0}web_unique,name,file_type,width,height,image_site_id,hires_tif,hires_psd,hires_eps) VALUES({1}@web_unique,@name,@file_type,@width,@height,@image_site_id,@hires_tif,@hires_psd,@hires_eps)", identity_insert ? "id," : "", identity_insert ? "@id," : "");
            try
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();

                    var cmd = Utils.GetCommand(insertsql, conn);
                    BuildSqlParameters(cmd, o, !identity_insert);
                    cmd.ExecuteNonQuery();
                    if (!identity_insert)
                    {
                        cmd.CommandText = "SELECT id FROM web_product_file WHERE id = LAST_INSERT_ID()";
                        o.id = (int)cmd.ExecuteScalar();
                    }

                }
            }
            catch
            {

            }
			
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_file o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@file_type", o.file_type);
			cmd.Parameters.AddWithValue("@width", o.width);
			cmd.Parameters.AddWithValue("@height", o.height);
			cmd.Parameters.AddWithValue("@image_site_id", o.image_site_id);
            cmd.Parameters.AddWithValue("@hires_tif", o.hires_tif);
            cmd.Parameters.AddWithValue("@hires_psd", o.hires_psd);
            cmd.Parameters.AddWithValue("@hires_eps", o.hires_eps);
            cmd.Parameters.AddWithValue("@approval", o.approval);
            cmd.Parameters.AddWithValue("@created",o.created);
		}
		
        public static void UpdateDate(Web_product_file o)
        {
            string updatesql = @"UPDATE web_product_file
                                SET created=@created 
                               WHERE id = @id";
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o,false);
                cmd.ExecuteNonQuery();
            }
        }
		public static void Update(Web_product_file o)
		{
            string updatesql = @"UPDATE web_product_file 
                                SET web_unique = @web_unique,name = @name,file_type = @file_type,width = @width,height = @height,image_site_id = @image_site_id, hires_tif = @hires_tif, hires_psd = @hires_psd, hires_eps = @hires_eps,@created=@created WHERE id = @id";

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
				var cmd = Utils.GetCommand("DELETE FROM web_product_file WHERE id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
		}

        public static Web_product_file GetByWebUniqueAndFiletype(int web_unique, int filetype)
        {
            var result = new Web_product_file();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_file WHERE web_unique = @web_unique AND file_type = @filetype", conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.Parameters.AddWithValue("@filetype", filetype);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }

            return result;
        }
	}
}
			
			
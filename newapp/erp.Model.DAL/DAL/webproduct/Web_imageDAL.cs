
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using asaq2.Model;
using MySql.Data.MySqlClient;

namespace asaq2.Model.DAL
{
    public class Web_imageDAL
	{
	
		public static List<Web_image> GetAll()
		{
			var result = new List<Web_image>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM web_image", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Web_image> GetForProduct(int web_unique, IDbConnection conn = null)
        {
            var result = new List<Web_image>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            try
            {
                var cmd = new MySqlCommand("SELECT * FROM web_image WHERE web_unique = @web_unique", (MySqlConnection) conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            finally
            {
                if (dispose)
                    conn.Dispose();
            }

            return result;
        }
		
		
		public static Web_image GetById(int id)
		{
			Web_image result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM web_image WHERE id = @id", conn);
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
		
	
		public static Web_image GetFromDataReader(MySqlDataReader dr)
		{
			Web_image o = new Web_image();
		
			o.id =  (int) dr["id"];
			o.web_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"web_unique"));
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.image_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"image_type"));
			o.width = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"width"));
			o.height = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"height"));
			
			return o;

		}
		
		
		public static void Create(Web_image o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_image (web_unique,name,image_type,width,height) VALUES(@web_unique,@name,@image_type,@width,@height)";

		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if(tr == null)
                    conn.Open();
				
				var cmd = new MySqlCommand(insertsql,(MySqlConnection) conn,(MySqlTransaction) tr);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM web_image WHERE id = LAST_INSERT_ID()";
                o.id = (int) cmd.ExecuteScalar();
				
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_image o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@image_type", o.image_type);
			cmd.Parameters.AddWithValue("@width", o.width);
			cmd.Parameters.AddWithValue("@height", o.height);
		}

        public static void Update(Web_image o, IDbTransaction tr = null)
		{
			string updatesql = @"UPDATE web_image SET web_unique = @web_unique,name = @name,image_type = @image_type,width = @width,height = @height WHERE id = @id";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();

                var cmd = new MySqlCommand(updatesql, (MySqlConnection)conn,(MySqlTransaction) tr);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                

            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }

			
		}
		
		public static void Delete(int id, IDbTransaction tr = null)
		{
		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
		    {
                if(tr == null)
                    conn.Open();
				var cmd = new MySqlCommand("DELETE FROM web_image WHERE id = @id" ,(MySqlConnection) conn,(MySqlTransaction) tr);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
		}

        /// <summary>
        /// Deletes images for web product that are not in the list
        /// </summary>
        /// <param name="web_unique"></param>
        /// <param name="ids"></param>
        /// <param name="tr"></param>
        public static void DeleteMissing(int web_unique,IList<int> ids, IDbTransaction tr = null)
        {
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
                var cmd = new MySqlCommand("", (MySqlConnection) conn, (MySqlTransaction) tr);
                cmd.CommandText =
                    string.Format("DELETE FROM web_image WHERE web_unique = @web_unique AND id NOT IN ({0})",
                                  Utilities.CreateParametersFromIdList(cmd, ids));
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (tr == null)
                    conn.Dispose();
            }
        }
		
	}
}
			
			

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Ie_countiesDAL
	{
	
		public static List<Ie_counties> GetAll()
		{
			var result = new List<Ie_counties>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM ie_counties", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Ie_counties GetById(int id)
		{
			Ie_counties result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM ie_counties WHERE ie_id = @id", conn);
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
		
	
		private static Ie_counties GetFromDataReader(MySqlDataReader dr)
		{
			Ie_counties o = new Ie_counties();
		
			o.ie_id =  (int) dr["ie_id"];
			o.county = string.Empty + Utilities.GetReaderField(dr,"county");
			o.longitude = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"long"));
			o.latitude = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"lat"));
			
			return o;

		}
		
		
		public static void Create(Ie_counties o)
        {
            string insertsql = @"INSERT INTO ie_counties (county,long,lat) VALUES(@county,@long,@lat)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT ie_id FROM ie_counties WHERE ie_id = LAST_INSERT_ID()";
                o.ie_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Ie_counties o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@ie_id", o.ie_id);
			cmd.Parameters.AddWithValue("@county", o.county);
			cmd.Parameters.AddWithValue("@long", o.longitude);
			cmd.Parameters.AddWithValue("@lat", o.latitude);
		}
		
		public static void Update(Ie_counties o)
		{
			string updatesql = @"UPDATE ie_counties SET county = @county,long = @long,lat = @lat WHERE ie_id = @ie_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int ie_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM ie_counties WHERE ie_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", ie_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
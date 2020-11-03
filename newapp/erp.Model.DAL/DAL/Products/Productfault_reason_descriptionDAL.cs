
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Productfault_reason_descriptionDAL
	{
	
		public static List<Productfault_reason_description> GetAll()
		{
			var result = new List<Productfault_reason_description>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM productfault_reason_description", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Productfault_reason_description GetById(int id)
		{
			Productfault_reason_description result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM productfault_reason_description WHERE xpgcode = @id", conn);
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
		
	
		public static Productfault_reason_description GetFromDataReader(MySqlDataReader dr)
		{
			Productfault_reason_description o = new Productfault_reason_description();
		
			o.xpgcode = string.Empty + Utilities.GetReaderField(dr,"xpgcode");
			o.description = string.Empty + Utilities.GetReaderField(dr,"description");
			
			return o;

		}
		
		
		public static void Create(Productfault_reason_description o)
        {
            string insertsql = @"INSERT INTO productfault_reason_description (xpgcode,description) VALUES(@xpgcode,@description)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand(insertsql,conn);
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Productfault_reason_description o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@xpgcode", o.xpgcode);
			cmd.Parameters.AddWithValue("@description", o.description);
		}
		
		public static void Update(Productfault_reason_description o)
		{
			string updatesql = @"UPDATE productfault_reason_description SET description = @description WHERE xpgcode = @xpgcode";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int xpgcode)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM productfault_reason_description WHERE xpgcode = @id" , conn);
                cmd.Parameters.AddWithValue("@id", xpgcode);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
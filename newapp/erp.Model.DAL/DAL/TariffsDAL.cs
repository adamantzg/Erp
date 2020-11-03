
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class TariffsDAL
	{
	
		public static List<Tariffs> GetAll()
		{
			List<Tariffs> result = new List<Tariffs>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM tariffs", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Tariffs GetById(int id)
		{
			Tariffs result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand("SELECT * FROM tariffs WHERE tariff_id = @id", conn);
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
	
		private static Tariffs GetFromDataReader(MySqlDataReader dr)
		{
			Tariffs o = new Tariffs();
		
			o.tariff_id =  (int) dr["tariff_id"];
			o.tariff_code = string.Empty + dr["tariff_code"];
			o.tariff_desc = string.Empty + dr["tariff_desc"];
			o.tariff_rate = Utilities.FromDbValue<double>(dr["tariff_rate"]);
			
			return o;

		}
		
		public static void Create(Tariffs o)
        {
            string insertsql = @"INSERT INTO tariffs (tariff_code,tariff_desc,tariff_rate) VALUES(@tariff_code,@tariff_desc,@tariff_rate)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT tariff_id FROM tariffs WHERE tariff_id = LAST_INSERT_ID()";
                o.tariff_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Tariffs o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@tariff_id", o.tariff_id);
			cmd.Parameters.AddWithValue("@tariff_code", o.tariff_code);
			cmd.Parameters.AddWithValue("@tariff_desc", o.tariff_desc);
			cmd.Parameters.AddWithValue("@tariff_rate", o.tariff_rate);
		}
		
		public static void Update(Tariffs o)
		{
			string updatesql = @"UPDATE tariffs SET tariff_code = @tariff_code,tariff_desc = @tariff_desc,tariff_rate = @tariff_rate WHERE tariff_id = @tariff_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int tariff_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM tariffs WHERE tariff_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", tariff_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Mastproduct_characteristicsDAL
	{
	
		public static List<Mastproduct_characteristics> GetAll()
		{
			var result = new List<Mastproduct_characteristics>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM mastproduct_characteristics", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}


        public static List<Mastproduct_characteristics> GetByProductId(int id)
        {
            var result = new List<Mastproduct_characteristics>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT mastproduct_characteristics.*, catdop_characteristics.* 
                            FROM mastproduct_characteristics INNER JOIN catdop_characteristics ON catdop_characteristics.characteristic_id = mastproduct_characteristics.characteristics_id
                            WHERE mast_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var mastprod_char = GetFromDataReader(dr);
                    mastprod_char.Characteristic = Catdop_characteristicsDAL.GetFromDataReader(dr);
                    result.Add(mastprod_char);
                    
                }
                dr.Close();
            }
            return result;
        }
		
		
	
		private static Mastproduct_characteristics GetFromDataReader(MySqlDataReader dr)
		{
			Mastproduct_characteristics o = new Mastproduct_characteristics();
		
			o.mast_id =  (int) dr["mast_id"];
			o.characteristics_id =  (int) dr["characteristics_id"];
			o.value = string.Empty + Utilities.GetReaderField(dr,"value");
			
			return o;

		}
		
		
		public static void Create(Mastproduct_characteristics o)
        {
            string insertsql = @"INSERT INTO mastproduct_characteristics (mast_id,characteristics_id,value) VALUES(@mast_id,@characteristics_id,@value)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = Utils.GetCommand("SELECT MAX(characteristics_id)+1 FROM mastproduct_characteristics", conn);
                o.characteristics_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Mastproduct_characteristics o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@mast_id", o.mast_id);
			cmd.Parameters.AddWithValue("@characteristics_id", o.characteristics_id);
			cmd.Parameters.AddWithValue("@value", o.value);
		}
		
		public static void Update(Mastproduct_characteristics o)
		{
			string updatesql = @"UPDATE mastproduct_characteristics SET value = @value WHERE characteristics_id = @characteristics_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int characteristics_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM mastproduct_characteristics WHERE characteristics_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", characteristics_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
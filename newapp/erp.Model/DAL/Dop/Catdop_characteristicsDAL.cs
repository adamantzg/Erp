
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Catdop_characteristicsDAL
	{
	
		public static List<Catdop_characteristics> GetAll()
		{
			var result = new List<Catdop_characteristics>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM catdop_characteristics", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}


        public static List<Catdop_characteristics> GetForCategory(int categorydop_id)
        {
            var result = new List<Catdop_characteristics>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM catdop_characteristics WHERE categorydop_id = @category_id", conn);
                cmd.Parameters.AddWithValue("@category_id", categorydop_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		public static Catdop_characteristics GetById(int id)
		{
			Catdop_characteristics result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM catdop_characteristics WHERE characteristic_id = @id", conn);
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


		
	
		public  static Catdop_characteristics GetFromDataReader(MySqlDataReader dr)
		{
			Catdop_characteristics o = new Catdop_characteristics();
		
			o.characteristic_id =  (int) dr["characteristic_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			o.categorydop_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"categorydop_id"));
			o.defaultvalue = string.Empty + Utilities.GetReaderField(dr,"defaultvalue");
			
			return o;

		}
		
		
		public static void Create(Catdop_characteristics o)
        {
            string insertsql = @"INSERT INTO catdop_characteristics (name,categorydop_id,defaultvalue) VALUES(@name,@categorydop_id,@defaultvalue)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT characteristic_id FROM catdop_characteristics WHERE characteristic_id = LAST_INSERT_ID()";
                o.characteristic_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Catdop_characteristics o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@characteristic_id", o.characteristic_id);
			cmd.Parameters.AddWithValue("@name", o.name);
			cmd.Parameters.AddWithValue("@categorydop_id", o.categorydop_id);
			cmd.Parameters.AddWithValue("@defaultvalue", o.defaultvalue);
		}
		
		public static void Update(Catdop_characteristics o)
		{
			string updatesql = @"UPDATE catdop_characteristics SET name = @name,categorydop_id = @categorydop_id,defaultvalue = @defaultvalue WHERE characteristic_id = @characteristic_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int characteristic_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM catdop_characteristics WHERE characteristic_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", characteristic_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
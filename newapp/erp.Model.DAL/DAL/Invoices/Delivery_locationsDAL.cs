
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Delivery_locationsDAL
	{
	
		public static List<Delivery_locations> GetAll()
		{
			var result = new List<Delivery_locations>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM delivery_locations", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Delivery_locations> GetForClient(int client_id)
        {
            var result = new List<Delivery_locations>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM delivery_locations WHERE cus_id = @client_id", conn);
                cmd.Parameters.AddWithValue("@client_id", client_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Delivery_locations GetById(int id)
		{
			Delivery_locations result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM delivery_locations WHERE unique_id = @id", conn);
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
		
	
		private static Delivery_locations GetFromDataReader(MySqlDataReader dr)
		{
			Delivery_locations o = new Delivery_locations();
		
			o.unique_id =  (int) dr["unique_id"];
			o.cus_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cus_id"));
			o.default_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"default_flag"));
			o.del1 = string.Empty + Utilities.GetReaderField(dr,"del1");
			o.del2 = string.Empty + Utilities.GetReaderField(dr,"del2");
			o.del3 = string.Empty + Utilities.GetReaderField(dr,"del3");
			o.del4 = string.Empty + Utilities.GetReaderField(dr,"del4");
			o.del5 = string.Empty + Utilities.GetReaderField(dr,"del5");
			o.del6 = string.Empty + Utilities.GetReaderField(dr,"del6");
			o.del7 = string.Empty + Utilities.GetReaderField(dr,"del7");
			o.delport = string.Empty + Utilities.GetReaderField(dr,"delport");
			o.inv_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"inv_flag"));
			
			return o;

		}
		
		
		public static void Create(Delivery_locations o)
        {
            string insertsql = @"INSERT INTO delivery_locations (cus_id,default_flag,del1,del2,del3,del4,del5,del6,del7,delport,inv_flag) VALUES(@cus_id,@default_flag,@del1,@del2,@del3,@del4,@del5,@del6,@del7,@delport,@inv_flag)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT unique_id FROM delivery_locations WHERE unique_id = LAST_INSERT_ID()";
                o.unique_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Delivery_locations o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@unique_id", o.unique_id);
			cmd.Parameters.AddWithValue("@cus_id", o.cus_id);
			cmd.Parameters.AddWithValue("@default_flag", o.default_flag);
			cmd.Parameters.AddWithValue("@del1", o.del1);
			cmd.Parameters.AddWithValue("@del2", o.del2);
			cmd.Parameters.AddWithValue("@del3", o.del3);
			cmd.Parameters.AddWithValue("@del4", o.del4);
			cmd.Parameters.AddWithValue("@del5", o.del5);
			cmd.Parameters.AddWithValue("@del6", o.del6);
			cmd.Parameters.AddWithValue("@del7", o.del7);
			cmd.Parameters.AddWithValue("@delport", o.delport);
			cmd.Parameters.AddWithValue("@inv_flag", o.inv_flag);
		}
		
		public static void Update(Delivery_locations o)
		{
			string updatesql = @"UPDATE delivery_locations SET cus_id = @cus_id,default_flag = @default_flag,del1 = @del1,del2 = @del2,del3 = @del3,del4 = @del4,del5 = @del5,del6 = @del6,del7 = @del7,delport = @delport,inv_flag = @inv_flag WHERE unique_id = @unique_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int unique_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM delivery_locations WHERE unique_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", unique_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
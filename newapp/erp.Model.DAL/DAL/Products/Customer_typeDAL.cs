
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Customer_typeDAL
	{
	
		public static List<Customer_type> GetAll()
		{
			var result = new List<Customer_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM customer_type", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Customer_type GetById(int id)
		{
			Customer_type result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM customer_type WHERE type_id = @id", conn);
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
		
	
		public static Customer_type GetFromDataReader(MySqlDataReader dr)
		{
			Customer_type o = new Customer_type();
		
			o.type_id =  (int) dr["type_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			
			return o;

		}
		
		
		public static void Create(Customer_type o)
        {
            string insertsql = @"INSERT INTO customer_type (name) VALUES(@name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT type_id FROM customer_type WHERE type_id = LAST_INSERT_ID()";
                o.type_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Customer_type o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@type_id", o.type_id);
			cmd.Parameters.AddWithValue("@name", o.name);
		}
		
		public static void Update(Customer_type o)
		{
			string updatesql = @"UPDATE customer_type SET name = @name WHERE type_id = @type_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int type_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM customer_type WHERE type_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", type_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
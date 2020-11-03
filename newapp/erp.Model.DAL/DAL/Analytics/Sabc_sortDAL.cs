
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Sabc_sortDAL
	{
	
		public static List<Sabc_sort> GetAll()
		{
			var result = new List<Sabc_sort>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM sabc_sort", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Sabc_sort GetById(int id)
		{
			Sabc_sort result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM sabc_sort WHERE SABC = @id", conn);
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
		
	
		public static Sabc_sort GetFromDataReader(MySqlDataReader dr)
		{
			Sabc_sort o = new Sabc_sort();
		
			o.SABC = string.Empty + Utilities.GetReaderField(dr,"SABC");
			o.sort_code = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"sort_code"));
			o.prod_days = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"prod_days"));
			o.prod_days_reporting = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"prod_days_reporting"));
			
			return o;

		}
		
		
		public static void Create(Sabc_sort o)
        {
            string insertsql = @"INSERT INTO sabc_sort (SABC,sort_code,prod_days,prod_days_reporting) VALUES(@SABC,@sort_code,@prod_days,@prod_days_reporting)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand(insertsql,conn);
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Sabc_sort o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@SABC", o.SABC);
			cmd.Parameters.AddWithValue("@sort_code", o.sort_code);
			cmd.Parameters.AddWithValue("@prod_days", o.prod_days);
			cmd.Parameters.AddWithValue("@prod_days_reporting", o.prod_days_reporting);
		}
		
		public static void Update(Sabc_sort o)
		{
			string updatesql = @"UPDATE sabc_sort SET sort_code = @sort_code,prod_days = @prod_days,prod_days_reporting = @prod_days_reporting WHERE SABC = @SABC";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int SABC)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM sabc_sort WHERE SABC = @id" , conn);
                cmd.Parameters.AddWithValue("@id", SABC);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
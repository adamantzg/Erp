
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Quotation_companiesDAL
	{
	
		public static List<Quotation_companies> GetAll()
		{
			List<Quotation_companies> result = new List<Quotation_companies>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql(), conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Quotation_companies GetById(int id)
		{
			Quotation_companies result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(GetSelectSql() + " WHERE company_id = @id", conn);
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

        private static string GetSelectSql()
        {
            return @"SELECT quotation_companies.*, currencies.curr_desc, countries.CountryName as country_name FROM quotation_companies LEFT OUTER JOIN countries ON quotation_companies.country_id= countries.country_id 
                                                      LEFT OUTER JOIN currencies ON quotation_companies.currency_id = currencies.curr_code";
        }
	
		private static Quotation_companies GetFromDataReader(MySqlDataReader dr)
		{
			Quotation_companies o = new Quotation_companies();
		
			o.company_id =  (int) dr["company_id"];
			o.company_name = string.Empty + dr["company_name"];
			o.address = string.Empty + dr["address"];
			o.contact = string.Empty + dr["contact"];
			o.email = string.Empty + dr["email"];
			o.phone = string.Empty + dr["phone"];
			o.country_id = Utilities.FromDbValue<int>(dr["country_id"]);
			o.currency_id = Utilities.FromDbValue<int>(dr["currency_id"]);
            if (Utilities.ColumnExists(dr, "curr_desc"))
                o.curr_desc = string.Empty + dr["curr_desc"];
            if (Utilities.ColumnExists(dr, "country_name"))
                o.country_name = string.Empty + dr["country_name"];
			
			return o;

		}
		
		public static void Create(Quotation_companies o)
        {
            string insertsql = @"INSERT INTO quotation_companies (company_name,address,contact,email,phone,country_id,currency_id) VALUES(@company_name,@address,@contact,@email,@phone,@country_id,@currency_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT company_id FROM quotation_companies WHERE company_id = LAST_INSERT_ID()";
                o.company_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Quotation_companies o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@company_id", o.company_id);
			cmd.Parameters.AddWithValue("@company_name", o.company_name);
			cmd.Parameters.AddWithValue("@address", o.address);
			cmd.Parameters.AddWithValue("@contact", o.contact);
			cmd.Parameters.AddWithValue("@email", o.email);
			cmd.Parameters.AddWithValue("@phone", o.phone);
			cmd.Parameters.AddWithValue("@country_id", o.country_id);
			cmd.Parameters.AddWithValue("@currency_id", o.currency_id);
		}
		
		public static void Update(Quotation_companies o)
		{
			string updatesql = @"UPDATE quotation_companies SET company_name = @company_name,address = @address,contact = @contact,email = @email,phone = @phone,country_id = @country_id,currency_id = @currency_id WHERE company_id = @company_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int company_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM quotation_companies WHERE company_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", company_id);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			
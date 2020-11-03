
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Payment_detailsDAL
	{
	
		public static List<Payment_details> GetAll()
		{
			var result = new List<Payment_details>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM payment_details", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Payment_details> GetForCompany(int company_id)
        {
            var result = new List<Payment_details>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM payment_details WHERE company_id = @company_id", conn);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Payment_details GetById(int id)
		{
			Payment_details result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM payment_details WHERE payment_details_id = @id", conn);
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
		
	
		private static Payment_details GetFromDataReader(MySqlDataReader dr)
		{
			Payment_details o = new Payment_details();
		
			o.payment_details_id =  (int) dr["payment_details_id"];
			o.bank_name = string.Empty + Utilities.GetReaderField(dr,"bank_name");
			o.address = string.Empty + Utilities.GetReaderField(dr,"address");
			o.sort_code = string.Empty + Utilities.GetReaderField(dr,"sort_code");
			o.beneficiary_name = string.Empty + Utilities.GetReaderField(dr,"beneficiary_name");
			o.beneficiary_accnumber = string.Empty + Utilities.GetReaderField(dr,"beneficiary_accnumber");
			o.company_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"company_id"));
			
			return o;

		}
		
		
		public static void Create(Payment_details o)
        {
            string insertsql = @"INSERT INTO payment_details (bank_name,address,sort_code,beneficiary_name,beneficiary_accnumber,company_id) VALUES(@bank_name,@address,@sort_code,@beneficiary_name,@beneficiary_accnumber,@company_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT payment_details_id FROM payment_details WHERE payment_details_id = LAST_INSERT_ID()";
                o.payment_details_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Payment_details o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@payment_details_id", o.payment_details_id);
			cmd.Parameters.AddWithValue("@bank_name", o.bank_name);
			cmd.Parameters.AddWithValue("@address", o.address);
			cmd.Parameters.AddWithValue("@sort_code", o.sort_code);
			cmd.Parameters.AddWithValue("@beneficiary_name", o.beneficiary_name);
			cmd.Parameters.AddWithValue("@beneficiary_accnumber", o.beneficiary_accnumber);
			cmd.Parameters.AddWithValue("@company_id", o.company_id);
		}
		
		public static void Update(Payment_details o)
		{
			string updatesql = @"UPDATE payment_details SET bank_name = @bank_name,address = @address,sort_code = @sort_code,beneficiary_name = @beneficiary_name,beneficiary_accnumber = @beneficiary_accnumber,company_id = @company_id WHERE payment_details_id = @payment_details_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int payment_details_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM payment_details WHERE payment_details_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", payment_details_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Quotation_headerDAL
	{
	
		public static List<Quotation_header> GetAll()
		{
			List<Quotation_header> result = new List<Quotation_header>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelectSql(), conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        private static string GetSelectSql()
        {
            return @"SELECT quotation_header.*, quotation_companies.company_name, currencies.curr_desc, container_types.container_type_desc AS container_name FROM quotation_header
                    LEFT OUTER JOIN quotation_companies ON quotation_header.company_id = quotation_companies.company_id LEFT OUTER JOIN currencies ON quotation_header.currency_id = currencies.curr_code
                    LEFT OUTER JOIN container_types ON quotation_header.container_type = container_types.container_type_id";
        }
		
		
		public static Quotation_header GetById(int id)
		{
			Quotation_header result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(GetSelectSql() + " WHERE quotation_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
                result.Lines = GetLines(id);
                foreach (var line in result.Lines)
                {
                    line.Header = result;
                }
            }
			return result;
		}
	
		private static Quotation_header GetFromDataReader(MySqlDataReader dr)
		{
			Quotation_header o = new Quotation_header();
		
			o.quotation_id =  (int) dr["quotation_id"];
			o.company_id = Utilities.FromDbValue<int>(dr["company_id"]);
			o.date_created = Utilities.FromDbValue<DateTime>(dr["date_created"]);
			o.container_price = Utilities.FromDbValue<double>(dr["container_price"]);
			o.margin = Utilities.FromDbValue<double>(dr["margin"]);
			o.exchange_rate = Utilities.FromDbValue<double>(dr["exchange_rate"]);
			o.currency_id = Utilities.FromDbValue<int>(dr["currency_id"]);
			o.container_type = Utilities.FromDbValue<int>(dr["container_type"]);
            o.agent_commission = Utilities.FromDbValue<double>(dr["agent_commission"]);

            if (Utilities.ColumnExists(dr, "container_name"))
                o.container_name = string.Empty + dr["container_name"];
            if (Utilities.ColumnExists(dr, "curr_desc"))
                o.currency_name = string.Empty + dr["curr_desc"];
            if (Utilities.ColumnExists(dr, "company_name"))
                o.company_name = string.Empty + dr["company_name"];
			
			return o;

		}

        public static List<Brand> GetBrandsForHeader(int quotation_id)
        {
            var result = new List<Brand>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT DISTINCT brands.* FROM quotation_lines INNER JOIN cust_products ON cust_products.cprod_id = quotation_lines.cprod_id
                                                      INNER JOIN brand_categories ON cust_products.cprod_brand_cat = brand_categories.brand_cat_id
                                                      INNER JOIN brands ON brands.user_id = brand_categories.brand
                                                      WHERE quotation_lines.header_id = @header_id ", conn);
                cmd.Parameters.AddWithValue("@header_id", quotation_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(BrandsDAL.GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;   
        }

        private static List<Quotation_lines> GetLines(int header_id)
        {
            List<Quotation_lines> result = new List<Quotation_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT cust_products.*, mast_products.*, users.*, brand_categories.brand FROM quotation_lines INNER JOIN cust_products ON quotation_lines.cprod_id = cust_products.cprod_id 
                                                      INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id 
                                                      INNER JOIN users ON mast_products.factory_id = users.user_id
                                                      LEFT OUTER JOIN brand_categories ON cust_products.cprod_brand_cat = brand_categories.brand_cat_id
                                                        WHERE header_id = @header_id ", conn);
                cmd.Parameters.AddWithValue("@header_id", header_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetLinesFromDataReader(dr));
                }
                dr.Close();
            }
            return result;   
        }

        private static Quotation_lines GetLinesFromDataReader(MySqlDataReader dr)
        {
            Quotation_lines line = new Quotation_lines();
            line.Product = Cust_productsDAL.GetFromDataReader(dr);
            if (Utilities.ColumnExists(dr, "brand"))
                line.Product.brand_userid = Utilities.FromDbValue<int>(dr["brand"]);
            line.Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
            line.Product.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);
            return line;
        }
		
		public static void Create(Quotation_header o)
        {
            string insertsql = @"INSERT INTO quotation_header (company_id,date_created,container_price,margin,exchange_rate,currency_id,container_type,agent_commission) VALUES(@company_id,@date_created,@container_price,@margin,@exchange_rate,@currency_id,@container_type,@agent_commission)";
            
            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            var trn = conn.BeginTransaction();
			try
            {
                var cmd = new MySqlCommand(insertsql, conn,trn);
				BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT quotation_id FROM quotation_header WHERE quotation_id = LAST_INSERT_ID()";
                o.quotation_id = (int) cmd.ExecuteScalar();
                if (o.Lines != null)
                {
                    cmd.CommandText = "INSERT INTO quotation_lines(header_id, cprod_id) VALUES(@header_id, @cprod_id)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@header_id", o.quotation_id);
                    cmd.Parameters.AddWithValue("@cprod_id", 0);
                    foreach (var line in o.Lines)
                    {
                        cmd.Parameters[1].Value = line.cprod_id;
                        cmd.ExecuteNonQuery();
                    }
                }
                trn.Commit();
            }
            catch
            {
                if (trn != null)
                    trn.Rollback();
            }
            finally
            {
                conn = null;
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Quotation_header o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@quotation_id", o.quotation_id);
			cmd.Parameters.AddWithValue("@company_id", o.company_id);
			cmd.Parameters.AddWithValue("@date_created", o.date_created);
			cmd.Parameters.AddWithValue("@container_price", o.container_price);
			cmd.Parameters.AddWithValue("@margin", o.margin);
			cmd.Parameters.AddWithValue("@exchange_rate", o.exchange_rate);
			cmd.Parameters.AddWithValue("@currency_id", o.currency_id);
			cmd.Parameters.AddWithValue("@container_type", o.container_type);
            cmd.Parameters.AddWithValue("@agent_commission", o.agent_commission);
		}
		
		public static void Update(Quotation_header o)
		{
            string updatesql = @"UPDATE quotation_header SET company_id = @company_id,date_created = @date_created,container_price = @container_price,margin = @margin,exchange_rate = @exchange_rate,currency_id = @currency_id,container_type = @container_type, agent_commission = @agent_commission WHERE quotation_id = @quotation_id";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            var trn = conn.BeginTransaction();
            try
            {
                var cmd = new MySqlCommand(updatesql, conn, trn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                
                if (o.Lines != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@header_id", o.quotation_id);
                    cmd.CommandText = "DELETE FROM quotation_lines WHERE header_id = @header_id";
                    cmd.ExecuteNonQuery();
                    
                    cmd.CommandText = "INSERT INTO quotation_lines(header_id, cprod_id) VALUES(@header_id, @cprod_id)";
                    cmd.Parameters.AddWithValue("@cprod_id", 0);
                    foreach (var line in o.Lines)
                    {
                        cmd.Parameters[1].Value = line.cprod_id;
                        cmd.ExecuteNonQuery();
                    }
                }
                trn.Commit();
            }
            catch
            {
                if (trn != null)
                    trn.Rollback();
            }
            finally
            {
                conn = null;
            }
		}
		
		public static void Delete(int quotation_id)
		{
            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            var trn = conn.BeginTransaction();
            try
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM quotation_lines WHERE header_id = @header_id", conn, trn);
                cmd.Parameters.AddWithValue("@header_id", quotation_id);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM quotation_header WHERE quotation_id = @header_id";
                cmd.ExecuteNonQuery();
                trn.Commit();
            }
            catch
            {
                if (trn != null)
                    trn.Rollback();
            }
            finally
            {
                conn = null;
            }
		}
	}
}
			
			
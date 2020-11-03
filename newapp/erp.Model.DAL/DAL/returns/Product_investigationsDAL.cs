
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public partial class Product_investigationsDAL
	{
	
		public static List<Product_investigations> GetAll()
		{
			var result = new List<Product_investigations>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM product_investigations", conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
				foreach (var r in result)
				{
					r.Product = Cust_productsDAL.GetById(r.cprod_id);
				}
			}
			return result;
		}
		
		
		public static Product_investigations GetById(int id)
		{
			Product_investigations result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM product_investigations WHERE id = @id", conn);
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
		
	
		public static Product_investigations GetFromDataReader(MySqlDataReader dr)
		{
			Product_investigations o = new Product_investigations();
		
			o.id =  (int) dr["id"];
			o.cprod_id =  (int) dr["cprod_id"];
			o.mast_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"mast_id"));
			o.date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"date"));
			o.monitored_by = string.Empty + Utilities.GetReaderField(dr,"monitored_by");
			o.status =  (int) dr["status"];
			o.comments = string.Empty + Utilities.GetReaderField(dr,"comments");
			return o;

		}
		
		
		public static void Create(Product_investigations o)
		{
			string insertsql = @"INSERT INTO product_investigations (cprod_id,mast_id,date,monitored_by,status,comments) VALUES(@cprod_id,@mast_id,@date,@monitored_by,@status,@comments)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
				BuildSqlParameters(cmd,o);
				cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM product_investigations WHERE id = LAST_INSERT_ID()";
				o.id = (int) cmd.ExecuteScalar();
				
			}
		}

        public static void Edit()
        {

        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Product_investigations o, bool forInsert = true)
		{
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@mast_id", o.mast_id);
			cmd.Parameters.AddWithValue("@date", o.date);
			cmd.Parameters.AddWithValue("@monitored_by", o.monitored_by);
			cmd.Parameters.AddWithValue("@status", o.status);
			cmd.Parameters.AddWithValue("@comments", o.comments);
		}
		
		public static void Update(Product_investigations o)
		{
			string updatesql = @"UPDATE product_investigations SET cprod_id = @cprod_id,mast_id = @mast_id,date = @date,monitored_by = @monitored_by,status = @status,comments = @comments WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
				BuildSqlParameters(cmd,o, false);
				cmd.ExecuteNonQuery();
			}
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM product_investigations WHERE id = @id" , conn);
				cmd.Parameters.AddWithValue("@id", id);
				cmd.ExecuteNonQuery();
			}
		}

		public static List<Product_investigations> GetClaimInvestigationForProduct(int cprodId)
		{
			var productInvestigations = new List<Product_investigations>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(
                        @"SELECT product_investigations.*,cust_products.* FROM product_investigations INNER JOIN cust_products ON product_investigations.cprod_id = cust_products.cprod_id
                            WHERE product_investigations.cprod_id = @cprod_id",
						conn);
				cmd.Parameters.AddWithValue("@cprod_id", cprodId);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
				    var pi = GetFromDataReader(dr);
				    pi.Product = Cust_productsDAL.GetFromDataReader(dr);
					productInvestigations.Add(pi);
                }
				dr.Close();

			}
			return productInvestigations;

		}



		public static void CreateStatus(Product_investigations o)
		{
			string insertsql = @"INSERT INTO product_investigations (cprod_id,mast_id,date,status,comments,monitored_by) VALUES(@CprodId,@MastId,@Date,@Status,@Comments,@MonitoredBy)";
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(insertsql, conn);
				BuildSqlParameters(cmd, o);
				cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				cmd.CommandText = "SELECT id FROM product_investigations WHERE id = LAST_INSERT_ID()";
				o.id = (int)cmd.ExecuteScalar();
			}
		}
		
		
	}
}
			
			

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class SaleDAL
	{
	
		public static List<Sale> GetAll()
		{
			var result = new List<Sale>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM sale", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Sale> GetForSite(int site_id)
        {
            var result = new List<Sale>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM sale WHERE WebSiteId = @site_id", conn);
                cmd.Parameters.AddWithValue("@site_id", site_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Sale GetById(int id)
		{
			Sale result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM sale WHERE IdSale = @id", conn);
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
		
	
		public static Sale GetFromDataReader(MySqlDataReader dr)
		{
			Sale o = new Sale();
		
			o.IdSale =  (int) dr["IdSale"];
			o.WebSiteId =  (int) dr["WebSiteId"];
			o.SaleStart =  (DateTime) dr["SaleStart"];
			o.SaleEnd =  (DateTime) dr["SaleEnd"];
			
			return o;

		}
		
		
		public static void Create(Sale o)
        {
            string insertsql = @"INSERT INTO sale (IdSale,WebSiteId,SaleStart,SaleEnd) VALUES(@IdSale,@WebSiteId,@SaleStart,@SaleEnd)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(IdSale)+1 FROM sale", conn);
                o.IdSale = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Sale o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@IdSale", o.IdSale);
			cmd.Parameters.AddWithValue("@WebSiteId", o.WebSiteId);
			cmd.Parameters.AddWithValue("@SaleStart", o.SaleStart);
			cmd.Parameters.AddWithValue("@SaleEnd", o.SaleEnd);
		}
		
		public static void Update(Sale o)
		{
			string updatesql = @"UPDATE sale SET WebSiteId = @WebSiteId,SaleStart = @SaleStart,SaleEnd = @SaleEnd WHERE IdSale = @IdSale";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int IdSale)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM sale WHERE IdSale = @id" , conn);
                cmd.Parameters.AddWithValue("@id", IdSale);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<Sale> GetForProduct(int web_unique, IDbConnection conn = null)
        {
            var result = new List<Sale>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            try
            {
                var cmd = Utils.GetCommand("SELECT sale.* FROM sale INNER JOIN web_product_new_sale ON sale.idsale = web_product_new_sale.sale_id WHERE web_unique = @web_unique",(MySqlConnection) conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            finally
            {
                if (dispose)
                    conn.Dispose();
            }
            return result;
        }
		

		
		
	}
}
			
			
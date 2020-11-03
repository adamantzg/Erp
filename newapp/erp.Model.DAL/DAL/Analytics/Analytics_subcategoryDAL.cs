
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Analytics_subcategoryDAL
	{
	
		public static List<Analytics_subcategory> GetAll()
		{
			var result = new List<Analytics_subcategory>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT analytics_subcategory.*,analytics_categories.* 
                                           FROM analytics_subcategory INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id ", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var subcat = GetFromDataReader(dr);
                    subcat.Category = Analytics_categoriesDAL.GetFromDataReader(dr);
                    result.Add(subcat);
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Analytics_subcategory GetById(int id)
		{
			Analytics_subcategory result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT analytics_subcategory.*                                                                                   
                                        FROM analytics_subcategory WHERE subcat_id = @id", conn);
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


        public static Analytics_subcategory GetFromDataReader(MySqlDataReader dr)
		{
			Analytics_subcategory o = new Analytics_subcategory();
		
			o.subcat_id =  (int) dr["subcat_id"];
			o.subcategory_name = string.Empty + Utilities.GetReaderField(dr,"subcategory_name");
			o.category_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"category_id"));
            o.seq = Utilities.FromDbValue<int>(dr["seq"]);
            if (Utilities.ColumnExists(dr, "DisplayQty"))
                o.DisplayQty = Utilities.FromDbValue<int>(dr["DisplayQty"]);
            if (Utilities.ColumnExists(dr, "FirstShipDate"))
                o.FirstShipDate = Utilities.FromDbValue<DateTime>(dr["FirstShipDate"]);
            return o;

		}
		
		
		public static void Create(Analytics_subcategory o)
        {
            string insertsql = @"INSERT INTO analytics_subcategory (subcategory_name,category_id) VALUES(@subcategory_name,@category_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT subcat_id FROM analytics_subcategory WHERE subcat_id = LAST_INSERT_ID()";
                o.subcat_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Analytics_subcategory o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@subcat_id", o.subcat_id);
			cmd.Parameters.AddWithValue("@subcategory_name", o.subcategory_name);
			cmd.Parameters.AddWithValue("@category_id", o.category_id);
		}
		
		public static void Update(Analytics_subcategory o)
		{
			string updatesql = @"UPDATE analytics_subcategory SET subcategory_name = @subcategory_name,category_id = @category_id WHERE subcat_id = @subcat_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int subcat_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM analytics_subcategory WHERE subcat_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", subcat_id);
                cmd.ExecuteNonQuery();
            }
		}


        public static List<Analytics_subcategory> GetForBrand(int? brandId=null, bool nullBrandOnly = false)
        {
            var result = new List<Analytics_subcategory>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format(@"SELECT analytics_subcategory.*,analytics_categories.*  FROM analytics_subcategory
                        INNER JOIN analytics_categories ON analytics_subcategory.category_id = analytics_categories.category_id
                        WHERE {0}", nullBrandOnly ? "analytics_categories.category_type IS NULL" : 
                        @"analytics_subcategory.subcat_id IN (SELECT cust_products.analytics_category FROM cust_products
                        INNER JOIN brands ON cust_products.brand_id = brands.brand_id
                        WHERE brands.brand_id = @id OR @id IS NULL)"), conn);
                if(!nullBrandOnly)
                    cmd.Parameters.AddWithValue("@id",Utilities.ToDBNull(brandId));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var subcat = GetFromDataReader(dr);
                    subcat.Category = Analytics_categoriesDAL.GetFromDataReader(dr);
                    result.Add(subcat);
                }
                dr.Close();
            }
            return result;
        }
	}
}
			
			
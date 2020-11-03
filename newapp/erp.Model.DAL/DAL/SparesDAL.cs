
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public class SparessDAL
	{
	
		public static List<Spares> GetAll(string factory_code = null)
		{
			var result = new List<Spares>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand(string.Format(@"SELECT spares.spare_id as id,users.factory_code,spares_cprod.cprod_code1 AS spare_code,spares_cprod.cprod_name AS spare_description,
                                            product_cprod.cprod_code1 AS related_code,product_cprod.cprod_name AS related_description,mast_products.prod_image1
                                            FROM spares INNER JOIN cust_products AS spares_cprod ON spares.product_cprod = spares_cprod.cprod_id
                                            INNER JOIN cust_products AS product_cprod ON spares.spare_cprod = product_cprod.cprod_id
                                            INNER JOIN mast_products ON product_cprod.cprod_mast = mast_products.mast_id
                                            INNER JOIN users ON mast_products.factory_id = users.user_id {0}",!string.IsNullOrEmpty(factory_code) ? "WHERE users.factory_code = @factory_code" : ""), conn);
                if (!string.IsNullOrEmpty(factory_code))
                    cmd.Parameters.AddWithValue("@factory_code", factory_code);
                var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}
        public static List<SparesProducts> GetReportSpareProduct()
        {
            var result = new List<SparesProducts>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(
                @"SELECT spares.spare_id,spares.product_cprod,spares.spare_cprod,spares.spare_desc,cust_products.cprod_code1,cust_products.brand_user_id,cust_products.cprod_status,cust_products.cprod_name,
                        product_cust_products.cprod_id AS product_cprod_id, 
                        product_cust_products.cprod_name AS product_cprod_name,
                        product_cust_products.cprod_code1 AS product_cprod_code1,
                        product_cust_products.cprod_status AS product_cprod_status,
                        (SELECT SUM(order_lines.orderqty) FROM order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid WHERE order_header.status NOT IN ('X','Y') AND order_lines.cprod_id = spares.spare_cprod) AS order_qty                                                            
                        FROM spares
                        INNER JOIN cust_products ON spares.spare_cprod=cust_products.cprod_id
                        INNER JOIN cust_products AS product_cust_products ON spares.product_cprod = product_cust_products.cprod_id
", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var tempresult = GetFromDataReaderSpareProduct(dr);
                    //tempresult.order_qty = Order_line_detail_exportDAL.GetShippingForProduct_V6(tempresult.spare_cprod).Sum(c=>c.orderqty);
                    result.Add(tempresult);
                }
            }
           
            return result;
        }

        public static SparesProducts GetFromDataReaderSpareProduct(MySqlDataReader dr)
        {
            var o =new  SparesProducts();
            o.id = (int)dr["spare_id"];
            o.product_cprod = (int)dr["product_cprod"];
            o.spare_cprod = (int)dr["spare_cprod"];
            o.cprod_code = string.Empty + dr["cprod_code1"];
            o.desc = string.Empty + dr["cprod_name"];
            o.status = string.Empty + dr["cprod_status"];
            o.product_cprod_id = (int)dr["product_cprod_id"];
            o.brand_user_id=string.Empty+dr["brand_user_id"];
            
            o.product_cprod_name = string.Empty + dr["product_cprod_name"];
            o.product_cprod_code1 = string.Empty + dr["product_cprod_code1"];
            o.product_cprod_status = string.Empty + dr["product_cprod_status"];
            o.order_qty = Utilities.FromDbValue<double>(dr["order_qty"]);
            return o;
        }

        
	
		public static Spares GetFromDataReader(MySqlDataReader dr)
		{
			var o = new Spares();
		
			o.id =  (int) dr["id"];
            o.factory_code = string.Empty + dr["factory_code"];
            o.spare_code = string.Empty + dr["spare_code"];
            o.spare_description = string.Empty + dr["spare_description"];
            o.related_code = string.Empty + dr["related_code"];
            o.related_description = string.Empty + dr["related_description"];
            o.prod_image1 = string.Empty + dr["prod_image1"];
			
			return o;
		}
		
		
		
		private static void BuildSqlParameters(MySqlCommand cmd, Spares o)
		{
            cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@factory_code", o.factory_code);
            cmd.Parameters.AddWithValue("@spare_code", o.spare_code);
            cmd.Parameters.AddWithValue("@spare_description", o.spare_description);
            cmd.Parameters.AddWithValue("@related_code", o.related_code);
            cmd.Parameters.AddWithValue("@related_description", o.related_description);
            cmd.Parameters.AddWithValue("@prod_image1", o.prod_image1);
		}
		
	}
}
			
			
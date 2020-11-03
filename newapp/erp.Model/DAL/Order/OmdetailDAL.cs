
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public enum SearchCategory
    {
        NotSet,
        New,
        Changed
    }

    public class OrderMgmtDetailDAL
	{
	
		public static List<OrderMgtmDetail> GetAll()
		{
			List<OrderMgtmDetail> result = new List<OrderMgtmDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM om_detail2", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
        
        public static List<OrderMgtmDetail> GetByProduct(int cprod_id, DateTime eta_from, DateTime eta_to)
        {
            var result = new List<OrderMgtmDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM om_detail2 WHERE cprod_id = @cprod_id AND stock_order <> 1 
                                           AND ( (booked_in_date IS NOT NULL AND (booked_in_date BETWEEN @from AND @to)) 
                                                 OR (booked_in_date IS NULL AND (CASE WHEN req_eta < CURDATE() THEN CURDATE() ELSE req_eta END) BETWEEN @from AND @to) )", conn);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@from", eta_from);
                cmd.Parameters.AddWithValue("@to", eta_to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<OrderMgtmDetail> GetByMastProduct(int mast_id, DateTime eta_from, DateTime eta_to)
        {
            var result = new List<OrderMgtmDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM om_detail2 INNER JOIN cust_products ON om_detail2.cprod_id = cust_products.cprod_id WHERE cprod_mast = @mast_id AND stock_order <> 1 AND (req_eta BETWEEN @from AND @to OR booked_in_date BETWEEN @from AND @to)", conn);
                cmd.Parameters.AddWithValue("@cprod_id", mast_id);
                cmd.Parameters.AddWithValue("@from", eta_from);
                cmd.Parameters.AddWithValue("@to", eta_to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductSaleSummary> GetSaleByProductIds(List<int> ids, DateTime? etd_from=null, DateTime? etd_to=null)
        {
            var result = new List<ProductSaleSummary>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(
                        @"SELECT cprod_id, SUM(orderqty) AS qtysum, SUM(orderqty*(CASE unitcurrency WHEN 0 THEN unitprice/1.6 ELSE unitprice END)) AS amount FROM om_detail1 
                                        WHERE cprod_id IN ({0}) AND (po_req_etd >= @from OR @from IS NULL) AND (po_req_etd <= @to OR @to IS NULL)
                                        GROUP BY cprod_id",
                        Utilities.CreateParametersFromIdList(cmd, ids));
                cmd.Parameters.AddWithValue("@from", etd_from);
                cmd.Parameters.AddWithValue("@to", etd_to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductSaleSummary{cprod_id = (int) dr["cprod_id"],QtySold = Utilities.FromDbValue<int>(dr["qtysum"]),Amount = Utilities.FromDbValue<double>(dr["amount"])});
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductSaleMonthSummary> GetMonthSaleByFactory(int? factory_Id, int? month21_from = null, int? month21_to = null)
        {
            var result = new List<ProductSaleMonthSummary>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                        @"SELECT cprod_id, cprod_name, cprod_code1,month21, SUM(orderqty) AS qtysum, SUM(orderqty*(CASE unitcurrency WHEN 0 THEN unitprice/1.6 ELSE unitprice END)) AS amount FROM om_detail1 
                                        WHERE (factory_id = @factory_id OR @factory_id IS NULL) AND (month21 >= @from OR @from IS NULL) AND (month21 <= @to OR @to IS NULL)
                                        GROUP BY cprod_id,cprod_name, cprod_code1, month21 ORDER BY  month21";
                cmd.Parameters.AddWithValue("@from",month21_from);
                cmd.Parameters.AddWithValue("@to", month21_to);
                cmd.Parameters.AddWithValue("@factory_id", factory_Id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductSaleMonthSummary { cprod_id = (int)dr["cprod_id"],month21  = Convert.ToInt32(dr["month21"]),  QtySold = Utilities.FromDbValue<int>(dr["qtysum"]), Amount = Utilities.FromDbValue<double>(dr["amount"]) });
                }
                dr.Close();
            }
            return result;
        }

        public static List<ProductOrderSummary> GetOrdersTotal(DateTime? from, DateTime? to)
        {
            var result = new List<ProductOrderSummary>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT cprod_id, COALESCE(SUM(orderqty),0) AS TotalQty FROM om_detail1 WHERE (po_req_etd >= @from OR @from IS NULL) AND (po_req_etd <= @to OR @to IS NULL) GROUP BY cprod_id", conn);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new ProductOrderSummary { cprod_id = (int)dr["cprod_id"], TotalQty = Convert.ToInt32(dr["TotalQty"])});
                }
                dr.Close();
            }
            return result;
        }

        public static List<OrderMgtmDetail> SearchLines(string product, string po, int? factory_id, int? client_id,
                                                        DateTime? etd_from, DateTime? etd_to,SearchCategory category ,
                                                        int orderby)
        {
            var result = new List<OrderMgtmDetail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT om_detail1.* FROM om_detail1 WHERE category1 <> 13
                                        AND (factory_ref like @product or cprod_code1 like @product OR cprod_name LIKE @product OR @product IS NULL) 
                                        AND (po_req_etd >= @etd_from OR @etd_from IS NULL) AND (po_req_etd <= @etd_to OR @etd_to IS NULL)
                                        AND (factory_id = @factory_id OR @factory_id IS NULL) AND (userid1 = @client_id OR @client_id IS NULL) 
                                        AND (custpo LIKE @po OR @po IS NULL) ",
                                           conn);
                if (category == SearchCategory.New)
                {
                    cmd.CommandText +=
                        @" AND NOT EXISTS (SELECT om_detail1.linenum FROM om_detail1 oldlines WHERE oldlines.cprod_mast = om_detail1.cprod_mast AND category1 <> 13 
                                            AND orderqty > 0 AND po_req_etd < om_detail1.po_req_etd)";
                }
                else
                {
                    cmd.CommandText +=
                        @" AND EXISTS (SELECT * FROM asaq.2011_change_notice_product_table WHERE mastid = om_detail1.cprod_mast AND product_po = om_detail1.custpo)";
                }
                if (orderby == 1)
                    cmd.CommandText += " ORDER BY po_req_etd";
                else if (orderby == 2)
                    cmd.CommandText += " ORDER BY custpo";
                else
                {
                    cmd.CommandText += " ORDER BY factory_code";
                }
                cmd.Parameters.AddWithValue("@product", !string.IsNullOrEmpty(product) ? (object) ("%" + product + "%") : DBNull.Value);
                cmd.Parameters.AddWithValue("@po", !string.IsNullOrEmpty(po) ? (object) ("%" + po + "%") : DBNull.Value);
                cmd.Parameters.AddWithValue("@factory_id", factory_id != null ? (object) factory_id : DBNull.Value);
                cmd.Parameters.AddWithValue("@client_id", client_id != null ? (object) client_id : DBNull.Value);
                cmd.Parameters.AddWithValue("@etd_from", etd_from != null ? (object) etd_from : DBNull.Value);
                cmd.Parameters.AddWithValue("@etd_to", etd_to != null ? (object)etd_to : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }


        private static OrderMgtmDetail GetFromDataReader(MySqlDataReader dr)
		{
			OrderMgtmDetail o = new OrderMgtmDetail();
		
			o.orderid = Utilities.FromDbValue<int>(dr["orderid"]);
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
			o.container_type = Utilities.FromDbValue<int>(dr["container_type"]);
			o.linedate = Utilities.FromDbValue<DateTime>(dr["linedate"]);
			o.description = string.Empty + dr["description"];
			o.orderqty = Utilities.FromDbValue<double>(dr["orderqty"]);
			o.unitprice = Utilities.FromDbValue<double>(dr["unitprice"]);
			o.unitcurrency = Utilities.FromDbValue<int>(dr["unitcurrency"]);
			o.cprod_code1 = string.Empty + dr["cprod_code1"];
			o.cprod_name = string.Empty + dr["cprod_name"];
			o.cprod_mast = Utilities.FromDbValue<int>(dr["cprod_mast"]);
			o.factory_id = Utilities.FromDbValue<int>(dr["factory_id"]);
			o.factory_ref = string.Empty + dr["factory_ref"];
			o.factory_name = string.Empty + dr["factory_name"];
            o.factory_code = string.Empty + dr["factory_code"];
			o.stock_order = Utilities.FromDbValue<int>(dr["stock_order"]);
			o.cprod_stock_date = Utilities.FromDbValue<DateTime>(dr["cprod_stock_date"]);
			o.asaq_ref = string.Empty + dr["asaq_ref"];
			o.asaq_name = string.Empty + dr["asaq_name"];
			o.original_eta = Utilities.FromDbValue<DateTime>(dr["original_eta"]);
			o.special_comments = string.Empty + dr["special_comments"];
			o.user_name = string.Empty + dr["user_name"];
			o.mc_qty = Utilities.FromDbValue<int>(dr["mc_qty"]);
			o.pallet_qty = Utilities.FromDbValue<int>(dr["pallet_qty"]);
			o.unit_qty = Utilities.FromDbValue<int>(dr["unit_qty"]);
			o.cprod_user = Utilities.FromDbValue<int>(dr["cprod_user"]);
			o.cprod_brand_cat = Utilities.FromDbValue<int>(dr["cprod_brand_cat"]);
			o.status = string.Empty + dr["status"];
			o.po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]);
			o.original_po_req_etd = Utilities.FromDbValue<DateTime>(dr["original_po_req_etd"]);
			o.om_seq_number = Utilities.FromDbValue<int>(dr["om_seq_number"]);
			o.month21 = string.Empty + dr["month21"];
			//o.month22 = string.Empty + dr["month22"];
			//o.week22 = string.Empty + dr["week22"];
			o.userid1 = Utilities.FromDbValue<int>(dr["userid1"]);
			o.custpo = string.Empty + dr["custpo"];
			o.req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]);
			o.orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]);
			o.consolidated_port = Utilities.FromDbValue<int>(dr["consolidated_port"]);
			o.customer_code = string.Empty + dr["customer_code"];
			o.combined_factory = Utilities.FromDbValue<int>(dr["combined_factory"]);
			o.distributor = Utilities.FromDbValue<int>(dr["distributor"]);
			o.po_ready_date = Utilities.FromDbValue<DateTime>(dr["po_ready_date"]);
			o.booked_in_date = Utilities.FromDbValue<DateTime>(dr["booked_in_date"]);
            o.porder_id = Utilities.FromDbValue<int>(dr["porderid"]);
			return o;

		}
		
		
	}
}
			
			
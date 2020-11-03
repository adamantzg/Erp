
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
	public class Order_line_detail_exportDAL
	{
	
		public static List<Order_line_export> GetAll()
		{
			List<Order_line_export> result = new List<Order_line_export>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM order_line_detail2_v7", conn);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}

	    public static List<Order_line_Summary> GetCustomerSummaryForPeriod(DateTime? from, DateTime? to,int? brand_user_id = null)
	    {
            var result = new List<Order_line_Summary>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT customer_code, SUM(orderqty * unitprice * (CASE unitcurrency WHEN 0 THEN 0.625 ELSE 1 END )) AS Total 
                                                FROM order_line_detail2_v7 WHERE distributor > 0 AND hide_1 = 0 
                                                      AND (req_eta >= @from OR @from IS NULL) AND (req_eta <= @to OR @to IS NULL) AND (brand_user_id = @userid OR @userid IS NULL)
                                                      GROUP BY customer_code", conn);
                cmd.Parameters.AddWithValue("@from", (from != null ? (object) from.Value : DBNull.Value));
                cmd.Parameters.AddWithValue("@to", (to != null ? (object)to.Value : DBNull.Value));
                cmd.Parameters.AddWithValue("@userid", brand_user_id != null ? (object) brand_user_id : DBNull.Value);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Order_line_Summary
                        {
                            customer_code = string.Empty + dr["customer_code"],
                            total = (double) dr["Total"]
                        });
                }
                dr.Close();
            }
            return result;
	    }


	    public static List<Order_line_export> GetForFactory(List<int> factory_ids)
        {
            var result = new List<Order_line_export>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format("SELECT * FROM order_line_detail2_v7 WHERE cprod_status <> 'D' AND factory_id IN ({0}) AND stock_order IN (1,8) ",Utilities.CreateParametersFromIdList(cmd, factory_ids));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

		public static List<Company> GetFactories()
		{
			var result = new List<Company>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT factory_id,factory_code, combined_factory FROM order_line_detail2_v7 WHERE stock_order IN (1,8)", conn);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(new Company{user_id = (int) dr["factory_id"],factory_code = string.Empty + dr["factory_code"],combined_factory = Utilities.FromDbValue<int>(dr["combined_factory"])});
				}
				dr.Close();

			}
			return result;
		}



		private static Order_line_export GetFromDataReader(MySqlDataReader dr)
		{
			var o = new Order_line_export();
		
			o.linenum =  (int) dr["linenum"];
			o.orderid = Utilities.FromDbValue<int>(dr["orderid"]);
			o.linedate = Utilities.FromDbValue<DateTime>(dr["linedate"]);
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
			o.description = string.Empty + dr["description"];
			o.orderqty = Utilities.FromDbValue<double>(dr["orderqty"]);
			o.unitprice = Utilities.FromDbValue<double>(dr["unitprice"]);
			o.unitcurrency = Utilities.FromDbValue<int>(dr["unitcurrency"]);
			o.linestatus = Utilities.FromDbValue<int>(dr["linestatus"]);
			o.cprod_code1 = string.Empty + dr["cprod_code1"];
			o.cprod_name = string.Empty + dr["cprod_name"];
			o.lineunit = Utilities.FromDbValue<int>(dr["lineunit"]);
			o.cprod_mast = Utilities.FromDbValue<int>(dr["cprod_mast"]);
			o.cprod_status = string.Empty + dr["cprod_status"];
			o.factory_id = Utilities.FromDbValue<int>(dr["factory_id"]);
			o.factory_ref = string.Empty + dr["factory_ref"];
			o.asaq_ref = string.Empty + dr["asaq_ref"];
			o.asaq_name = string.Empty + dr["asaq_name"];
			o.soline = Utilities.FromDbValue<int>(dr["soline"]);
			o.porderid = Utilities.FromDbValue<int>(dr["porderid"]);
			o.poqty = Utilities.FromDbValue<double>(dr["poqty"]);
			o.polinenum = Utilities.FromDbValue<int>(dr["polinenum"]);
			o.original_po_req_etd = Utilities.FromDbValue<DateTime>(dr["original_po_req_etd"]);
			o.mc_qty = Utilities.FromDbValue<int>(dr["mc_qty"]);
			o.pallet_qty = Utilities.FromDbValue<int>(dr["pallet_qty"]);
			o.cprod_lme = Utilities.FromDbValue<int>(dr["cprod_lme"]);
			o.cprod_cgflag = Utilities.FromDbValue<int>(dr["cprod_cgflag"]);
			o.poprice = Utilities.FromDbValue<double>(dr["poprice"]);
			o.unit_qty = Utilities.FromDbValue<int>(dr["unit_qty"]);
			o.cprod_user = Utilities.FromDbValue<int>(dr["cprod_user"]);
			o.cprod_brand_cat = Utilities.FromDbValue<int>(dr["cprod_brand_cat"]);
			o.pocurrency = Utilities.FromDbValue<int>(dr["pocurrency"]);
			o.allow_change = Utilities.FromDbValue<int>(dr["allow_change"]);
			o.allow_change_down = Utilities.FromDbValue<int>(dr["allow_change_down"]);
			o.cprod_loading = Utilities.FromDbValue<int>(dr["cprod_loading"]);
			o.moq = Utilities.FromDbValue<int>(dr["moq"]);
			o.cprod_disc = Utilities.FromDbValue<int>(dr["cprod_disc"]);
			o.pack_qty = Utilities.FromDbValue<double>(dr["pack_qty"]);
			o.orig_orderqty = Utilities.FromDbValue<double>(dr["orig_orderqty"]);
			o.pending_orderqty = Utilities.FromDbValue<double>(dr["pending_orderqty"]);
			o.pending_unitprice = Utilities.FromDbValue<double>(dr["pending_unitprice"]);
			o.category1 = Utilities.FromDbValue<int>(dr["category1"]);
			o.units_per_carton = Utilities.FromDbValue<int>(dr["units_per_carton"]);
			o.carton_height = Utilities.FromDbValue<double>(dr["carton_height"]);
			o.carton_GW = Utilities.FromDbValue<double>(dr["carton_GW"]);
			o.units_per_40nopallet_hc = Utilities.FromDbValue<int>(dr["units_per_40nopallet_hc"]);
			o.units_per_40pallet_hc = Utilities.FromDbValue<int>(dr["units_per_40pallet_hc"]);
			o.units_per_40nopallet_gp = Utilities.FromDbValue<int>(dr["units_per_40nopallet_gp"]);
			o.units_per_40pallet_gp = Utilities.FromDbValue<int>(dr["units_per_40pallet_gp"]);
			o.pallet_width = Utilities.FromDbValue<double>(dr["pallet_width"]);
			o.pallet_length = Utilities.FromDbValue<double>(dr["pallet_length"]);
			o.pallet_height = Utilities.FromDbValue<double>(dr["pallet_height"]);
			o.pallet_height_upper = Utilities.FromDbValue<double>(dr["pallet_height_upper"]);
			o.pallet_height_lower = Utilities.FromDbValue<double>(dr["pallet_height_lower"]);
			o.units_per_pallet_single = Utilities.FromDbValue<int>(dr["units_per_pallet_single"]);
			o.units_per_pallet_lower = Utilities.FromDbValue<int>(dr["units_per_pallet_lower"]);
			o.units_per_pallet_upper = Utilities.FromDbValue<int>(dr["units_per_pallet_upper"]);
			o.pallets_per_20 = Utilities.FromDbValue<int>(dr["pallets_per_20"]);
			o.pallets_per_40 = Utilities.FromDbValue<int>(dr["pallets_per_40"]);
			o.units_per_20pallet = Utilities.FromDbValue<int>(dr["units_per_20pallet"]);
			o.units_per_20nopallet = Utilities.FromDbValue<int>(dr["units_per_20nopallet"]);
			o.min_ord_qty = Utilities.FromDbValue<int>(dr["min_ord_qty"]);
			o.pack2_gw = Utilities.FromDbValue<double>(dr["pack2_gw"]);
			o.po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]);
			o.month21 = string.Empty + dr["month21"];
			o.custpo = string.Empty + dr["custpo"];
			o.req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]);
			o.container_type = Utilities.FromDbValue<int>(dr["container_type"]);
			o.customer_code = string.Empty + dr["customer_code"];
			o.price_dollar = Utilities.FromDbValue<double>(dr["price_dollar"]);
			o.price_pound = Utilities.FromDbValue<double>(dr["price_pound"]);
			o.original_orderid = Utilities.FromDbValue<int>(dr["original_orderid"]);
			o.pack_loading_ratio = Utilities.FromDbValue<double>(dr["pack_loading_ratio"]);
			o.userid1 = Utilities.FromDbValue<int>(dr["userid1"]);
			o.distributor = Utilities.FromDbValue<int>(dr["distributor"]);
			o.pack_length = Utilities.FromDbValue<double>(dr["pack_length"]);
			o.pack_width = Utilities.FromDbValue<double>(dr["pack_width"]);
			o.pack_height = Utilities.FromDbValue<double>(dr["pack_height"]);
			o.carton_width = Utilities.FromDbValue<double>(dr["carton_width"]);
			o.carton_length = Utilities.FromDbValue<double>(dr["carton_length"]);
			o.stock_order = Utilities.FromDbValue<int>(dr["stock_order"]);
			o.factory_code = string.Empty + dr["factory_code"];
		    o.orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]);
		    o.cprod_stock_code = Utilities.FromDbValue<int>(dr["cprod_stock_code"]);
		    o.cprod_stock = Utilities.FromDbValue<int>(dr["cprod_stock"]);
		    o.combined_factory = Utilities.FromDbValue<int>(dr["combined_factory"]);
		    o.uk_production = Utilities.FromDbValue<int>(dr["uk_production"]);
		    o.po_ready_date = Utilities.FromDbValue<DateTime>(dr["po_ready_date"]);
			return o;

		}
		
		
	}

    public class Order_line_Summary
    {
        public string customer_code { get; set; }
        public double total { get; set; }
    }
}
			
			
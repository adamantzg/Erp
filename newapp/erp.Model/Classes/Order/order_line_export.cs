
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Order_line_export
	{
		public int linenum { get; set; }
		public int? orderid { get; set; }
		public DateTime? linedate { get; set; }
		public int? cprod_id { get; set; }
		public string description { get; set; }
		public double? orderqty { get; set; }
		public double? unitprice { get; set; }
		public int? unitcurrency { get; set; }
		public int? linestatus { get; set; }
		public string cprod_code1 { get; set; }
		public string cprod_name { get; set; }
		public int? lineunit { get; set; }
		public int? cprod_mast { get; set; }
		public string cprod_status { get; set; }
		public int? factory_id { get; set; }
		public string factory_ref { get; set; }
		public string asaq_ref { get; set; }
		public string asaq_name { get; set; }
		public int? soline { get; set; }
		public int? porderid { get; set; }
		public double? poqty { get; set; }
		public int? polinenum { get; set; }
		public DateTime? original_po_req_etd { get; set; }
		public int? mc_qty { get; set; }
		public int? pallet_qty { get; set; }
		public int? cprod_lme { get; set; }
		public int? cprod_cgflag { get; set; }
		public double? poprice { get; set; }
		public int? unit_qty { get; set; }
		public int? cprod_user { get; set; }
		public int? cprod_brand_cat { get; set; }
		public int? pocurrency { get; set; }
		public int? allow_change { get; set; }
		public int? allow_change_down { get; set; }
		public int? cprod_loading { get; set; }
		public int? moq { get; set; }
		public int? cprod_disc { get; set; }
		public double? pack_qty { get; set; }
		public double? orig_orderqty { get; set; }
		public double? pending_orderqty { get; set; }
		public double? pending_unitprice { get; set; }
		public int? category1 { get; set; }
		public int? units_per_carton { get; set; }
		public double? carton_height { get; set; }
		public double? carton_GW { get; set; }
		public int? units_per_40nopallet_hc { get; set; }
		public int? units_per_40pallet_hc { get; set; }
		public int? units_per_40nopallet_gp { get; set; }
		public int? units_per_40pallet_gp { get; set; }
		public double? pallet_width { get; set; }
		public double? pallet_length { get; set; }
		public double? pallet_height { get; set; }
		public double? pallet_height_upper { get; set; }
		public double? pallet_height_lower { get; set; }
		public int? units_per_pallet_single { get; set; }
		public int? units_per_pallet_lower { get; set; }
		public int? units_per_pallet_upper { get; set; }
		public int? pallets_per_20 { get; set; }
		public int? pallets_per_40 { get; set; }
		public int? units_per_20pallet { get; set; }
		public int? units_per_20nopallet { get; set; }
		public int? min_ord_qty { get; set; }
		public double? pack2_gw { get; set; }
		public DateTime? po_req_etd { get; set; }
		public string month21 { get; set; }
		public string custpo { get; set; }
		public DateTime? req_eta { get; set; }
		public int? container_type { get; set; }
		public string customer_code { get; set; }
		public double? price_dollar { get; set; }
		public double? price_pound { get; set; }
		public int? original_orderid { get; set; }
		public double? pack_loading_ratio { get; set; }
		public int? userid1 { get; set; }
		public int? distributor { get; set; }
		public double? pack_length { get; set; }
		public double? pack_width { get; set; }
		public double? pack_height { get; set; }
		public double? carton_width { get; set; }
		public double? carton_length { get; set; }
		public int? stock_order { get; set; }
		public string factory_code { get; set; }
        public DateTime? orderdate { get; set; }
        public int? cprod_stock_code { get; set; }
        public int? cprod_stock { get; set; }
        public int? combined_factory { get; set; }
        public int? uk_production { get; set; }
        public DateTime? po_ready_date { get; set; }

        public List<Order_lines> AllocatedLines { get; set; }

        public int? alloc_qty { get; set; }
        public int? allocation_id { get; set; }
        //public int? so_line { get; set; }

        public int? insp_id_FI { get; set; }

        public int? insp_id_LI { get; set; }
    }
}	
	
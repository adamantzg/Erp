
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Order_lines
	{
		public int linenum { get; set; }
		public int? orderid { get; set; }
		public int? original_orderid { get; set; }
		public DateTime? linedate { get; set; }
		public int? cprod_id { get; set; }
		public string description { get; set; }
		public string spec_code { get; set; }
		public double? orderqty { get; set; }
		public double? orig_orderqty { get; set; }
		public double? unitprice { get; set; }
		public int? unitcurrency { get; set; }
		public double? override_nw { get; set; }
		public double? override_gw { get; set; }
		public int? linestatus { get; set; }
		public int? lineunit { get; set; }
		public int? factory_group { get; set; }
		public int? mc_qty { get; set; }
		public int? pallet_qty { get; set; }
		public int? unit_qty { get; set; }
		public double? lme { get; set; }
		public int? allow_change { get; set; }
		public int? allow_change_down { get; set; }
		public int? fi_line { get; set; }
		public double? pack_qty { get; set; }
		public int? li_line { get; set; }
        public double? received_qty { get; set; }


        [NotMapped]
		public int? AllocQty { get; set; }
        [NotMapped]
		public int? allocation_id { get; set; }
        [NotMapped]
        public int? special_terms { get; set; }

        [NotMapped]
		public virtual List<Order_lines> AllocatedLines { get; set; }

		public virtual Cust_products Cust_Product { get; set; }
        public virtual  Order_header Header { get; set; }

        [NotMapped]
        public virtual  Porder_header POHeader { get; set; }

        
        public virtual List<Porder_lines> PorderLines { get; set; }
        public virtual List<Stock_order_allocation> Allocations { get; set; }
        public virtual List<Stock_order_allocation> SOAllocations { get; set; }
        public virtual List<Inspection_v2_line> InspectionV2Lines { get; set; }

        public virtual Currencies Currency { get; set; }

        [NotMapped]
        public virtual Order_header OriginalOrder { get; set; }

        public double? RowPriceGBP
		{
			get { return orderqty*(unitcurrency == 0 ? unitprice/1.6 : unitcurrency == 2 ? unitprice / 1.2 : unitprice); }
		}

		public double? RowPrice
		{
			get { return orderqty*unitprice; }
		}

        [NotMapped]
        public double? PORowPrice
        {
            get;set;            
        }
        [NotMapped]
        public int? POCurrency { get; set; }

        [NotMapped]
        public double? po_unitprice
        {
            get; set;
        }

        [NotMapped]
        public int? po_unitcurrency { get; set; }
        [NotMapped]
        public double? po_orderqty { get; set; }


        [NotMapped]
        public double? usd_gbp
        {
            get; set;
        }

        [NotMapped]
        public double? PO_USD
        {
            get; set;
        }
        
        [NotMapped]
        public double? commission_rate
        { get; set; }
    }

    public class OrderLineComparer : IEqualityComparer<Order_lines>
    {
        public bool Equals(Order_lines x, Order_lines y)
        {
            return x.linenum == y.linenum;
        }

        public int GetHashCode(Order_lines obj)
        {
            return obj.linenum;
        }
    }

    public class OrderLineByProductComparer : IEqualityComparer<Order_lines>
    {
        public bool Equals(Order_lines x, Order_lines y)
        {
            return x.cprod_id == y.cprod_id;
        }

        public int GetHashCode(Order_lines obj)
        {
            return obj.cprod_id ?? 0;
        }
    }

    public class OrderLineByMastProductComparer : IEqualityComparer<Order_lines>
    {
        public bool Equals(Order_lines x, Order_lines y)
        {
            return x.Cust_Product.cprod_mast == y.Cust_Product.cprod_mast;
        }

        public int GetHashCode(Order_lines obj)
        {
            return obj.Cust_Product.cprod_mast ?? 0;
        }
    }

    public class OrderLineByHeaderComparer : IEqualityComparer<Order_lines>
    {
        public bool Equals(Order_lines x, Order_lines y)
        {
            return x.orderid == y.orderid;
        }

        public int GetHashCode(Order_lines obj)
        {
            return obj.orderid ?? 0;
        }
    }

    /// <summary>
    /// refactor this! (jkricka 24.06.2016)
    /// </summary>
    public class OrderLinesLight
    {
        public int? cprod_id { get; set; }
        public int? orderid { get; set; }
        public string custpo { get; set; }
        public int? orderqty { get; set; }
        public string calloff_custpo { get; set; }
        public int? alloc_qty { get; set; }
    }
}	
	
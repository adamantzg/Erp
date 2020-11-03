
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Client_sales_data
	{
		public int id { get; set; }
		public int? cprod_id { get; set; }
		public DateTime? invoice_date { get; set; }
		public double? qty { get; set; }
		public double? value { get; set; }
        public string customer { get; set; }
        public virtual Dealer Customer { get; set; }
        public string cprod_code1 { get; set; }

        public virtual Cust_products Product { get; set; }
	}

    public class ClientSalesAggregate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? Qty { get; set; }
        public double? Sum { get; set; }
    }
}	
	
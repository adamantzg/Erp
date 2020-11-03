
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace erp.Model
{
	
	public class Brand
	{
	    public const int ClearBrand = 1;
	    public const int BrandingClosets = 3;        
	    public const int Brandigton = 2;
		public const int Brandtton = 4;
		public const int AutomateBrand = 11;

        //public int? SelectBrandId { get; set; }

		public int brand_id { get; set; }
		public string brandname { get; set; }
		public int? user_id { get; set; }
		public string dealerstatus_view { get; set; }
		public string code { get; set; }
        public string image { get; set; }
        public int? eb_brand { get; set; }
        public int? category_flag { get; set; }
        public bool? dealerstatus_manual { get; set; }
        public bool? show_as_eb_default { get; set; }
        public bool? show_as_eb_products { get; set; }
        public int? brand_group { get; set; }
		public string dealersearch_view { get; set; }

        public virtual List<Web_site> Sites { get; set; }
        public virtual List<Dealer_images> Images { get; set; }
        public virtual List<Cust_products> CustProducts { get; set; }

	}
}	
	
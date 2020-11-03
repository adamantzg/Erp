
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Claims_report
	{
		public int cprod_id { get; set; }
		public string cprod_code1 { get; set; }
		public string brandname { get; set; }
		public string brand_cat_desc { get; set; }
		public string BW_qty { get; set; }
		public string FR_qty { get; set; }
		public string CG_qty { get; set; }
		public string SM_qty { get; set; }
		public string FA_qty { get; set; }
		public string CM_qty { get; set; }
		public string total_claims { get; set; }
		public string Appearance { get; set; }
		public string Dimension { get; set; }
		public string Function { get; set; }
		public string Material { get; set; }
		public string Packaging { get; set; }
		public string pending_qty { get; set; }
		public string accepted_qty { get; set; }
		public string declined_qty { get; set; }
		public int brand_cat_id { get; set; }
		public string RETURN_NO { get; set; }
		public string client_comments { get; set; }
		public string cprod_name { get; set; }
		public int? factory_id { get; set; }
	
	}
}	
	
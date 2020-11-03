
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Factory_client_settings
	{
        [Key]
		public int uniqueid { get; set; }
		public int? factoryid { get; set; }
		public int? custid { get; set; }
		public string loading_pref { get; set; }
		public string display_price { get; set; }
		public int? deposits { get; set; }
		public int? payments { get; set; }
		public string shipping_mark { get; set; }
		public DateTime? last_update { get; set; }
		public double? qc_show { get; set; }
		public double? lcl_surcharge { get; set; }
		public int? alternative_codes { get; set; }
		public double? customs { get; set; }
		public string supplier_code { get; set; }
		public string supplier_code_bs { get; set; }
	
	}
}	
	
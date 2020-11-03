using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class nr_line_legacy
	{
		public string insp_document{get;set;}
		public string insp_container_number{get;set;}
		public string insp_custpo{get;set;}
		public string insp_factory{get;set;}
		public string insp_client{get;set;}
		public string insp_mfg_code{get;set;}
		public string insp_cust_code{get;set;}
		public string insp_description{get;set;}
		public string insp_comments{get;set;}
		public string changed_detail{get;set;}
		public string insp_reason{get;set;}
		public string insp_summary_comments{get;set;}
		public string insp_summary_reason{get;set;}
		public string insp_summary_changedetails{get;set;}
		public int? insp_line_unique{get;set;}
		public int? insp_line_type{get;set;}
		public int? insp_qty2{get;set;}
		public int? insp_qty3{get;set;}
		public int? insp_qty{get;set;}
		public int? insp_nr_unique{get;set;}
		public int? insp_line_show{get;set;}
		public int? insp_seq{get;set;}
		public int? insp_nr_status{get;set;}
		public DateTime? etd{get;set;}
		public DateTime? eta{get;set;}
	}
}

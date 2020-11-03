using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class inspection_lines_notified{
		public string insp_line_type{get;set;}
		public string insp_line_rejection{get;set;}
		public string insp_line_action{get;set;}
		public string insp_comments{get;set;}
		public string insp_reason{get;set;}
		public string insp_permanent_action{get;set;}
		public string insp_document{get;set;}
		public string insp_pdf{get;set;}
		public string changed_details{get;set;}
		public string insp_container_number{get;set;}
		public DateTime? etd{get;set;}
		public DateTime? eta{get;set;}
		public int? insp_line_unique{get;set;}
		public int? insp_unique{get;set;}
		public int? insp_line_id{get;set;}
		public int? insp_po_linenum{get;set;}
		public int? insp_qty2{get;set;}
		public int? insp_qty3{get;set;}
		public int? insp_ca2{get;set;}
		public int? master_line{get;set;}
	}
}

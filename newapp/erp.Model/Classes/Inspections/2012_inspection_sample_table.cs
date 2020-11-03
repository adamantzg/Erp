using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
	public class _2012_inspection_sample_table
	{
		public string si_number{get;set;}
		public string si_reason{get;set;}
		public string si_comments{get;set;}
		public int si_id{get;set;}
		public int? si_factory{get;set;}
		public int? si_client{get;set;}
		public int? si_userid{get;set;}
		public int? si_qc{get;set;}
		public int? si_qc2{get;set;}
		public int? si_qc3{get;set;}
		public int? si_qc4{get;set;}
		public int? si_qc5{get;set;}
		public int? si_status{get;set;}
		public int? si_subject{get;set;}
		public DateTime? si_date{get;set;}
		public DateTime? si_insp_date{get;set;}

		public Company Factory { get; set; }
		public Company Client { get; set; }
	}
}

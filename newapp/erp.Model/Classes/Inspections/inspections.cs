
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{

	public enum InspectionStatus
	{
		Undefined = 0,
		Todo,
		AwaitingReview,
		AwaitingReport,
		Rejected,
		Accepted,
		Cancelled
	}
	public class Inspections
	{
		[Key]
		public int insp_unique { get; set; }
		public string insp_id { get; set; }
		public string insp_type { get; set; }
		public DateTime? insp_start { get; set; }
		public DateTime? insp_end { get; set; }
		public int? insp_version { get; set; }
		public int? insp_days { get; set; }
		public int? insp_porderid { get; set; }
		public int? insp_qc1 { get; set; }
		public int? insp_qc2 { get; set; }
		public int? insp_qc3 { get; set; }
		public int? insp_qc6 { get; set; }
		public int? insp_fc { get; set; }
		public string customer_code { get; set; }
		public string custpo { get; set; }
		public string batch_no { get; set; }
		public string factory_code { get; set; }
		public string insp_comments { get; set; }
		public string insp_comments_admin { get; set; }
		public int? insp_status { get; set; }
		public int? qc_required { get; set; }
		public string upload { get; set; }
		public int? upload_flag { get; set; }
		public int? lcl { get; set; }
		public int? gp20 { get; set; }
		public int? gp40 { get; set; }
		public int? hc40 { get; set; }
		public int? adjustment { get; set; }
		public int? LO_id { get; set; }
		public int? acceptance_qc1 { get; set; }
		public int? acceptance_qc2 { get; set; }
		public int? acceptance_qc3 { get; set; }
		public int? acceptance_qc4 { get; set; }
		public int? acceptance_fc { get; set; }
		public int? acceptance_cc { get; set; }
		public int? insp_qc5 { get; set; }
		public int? insp_qc4 { get; set; }
		public DateTime? etd { get; set; }
		public DateTime? eta { get; set; }
		public int? insp_batch_inspection { get; set; }
		public int? insp_executor { get; set; }
		public int? new_insp_id { get; set; }

		[NotMapped]
		public int? customer_id { get; set; }

		public string[] CustPos { get; set; }

		public List<Inspection_controller> Controllers { get; set; }
		public List<Inspection_lines_tested> LinesTested { get; set; }
		public List<Inspection_lines_accepted> LinesAccepted { get; set; }
		public List<Inspection_lines_rejected> LinesRejected { get; set; }

		[NotMapped]
		public Company Factory { get; set; }

		[NotMapped]
		public int? orderid { get; set; }

		[NotMapped]
		public bool IsV2
		{
			get
			{
				return new_insp_id != null;
			}
		}


		public Inspection_v2 Inspection_V2 { get; set; }
		public List<Returns> Returns { get; set; }
		public List<Nr_header> NrHeaders { get; set; }

	}
}	
	

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Nr_header
	{
        public const int TypeNS = 1;
        public const int TypeNT = 2;

		public int id { get; set; }
		public string NR_document_no { get; set; }
		public string NR_comment1 { get; set; }
		public string NR_comment2 { get; set; }
		public string NR_comment3 { get; set; }
		public DateTime? NR_datecreated { get; set; }
        public int? factory_id { get; set; }
        public int? insp_id { get; set; }
        public int? insp_v2_id { get; set; }
        public int? nr_type_id { get; set; }
        public int? no_of_cartons { get; set; }
        public int? status { get; set; }
        public int? submitted_by { get; set; }
        public DateTime? submitted_date { get; set; }
        public int? change_notice_id { get; set; }


        public List<Nr_lines> Lines { get; set; }
        public List<Nr_line_images> Images { get; set; }
        public Inspections Inspection { get; set; }
        public Inspection_v2 InspectionV2 { get; set; }
	
	}
}	
	

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
    
	
	public partial class Inspection_v2
	{
		public int id { get; set; }
		public DateTime? startdate { get; set; }
		public int? type { get; set; }
		public string custpo { get; set; }
		public int? factory_id { get; set; }
		public string code { get; set; }
		public int? client_id { get; set; }
		public int? duration { get; set; }
		public string comments { get; set; }
		public int? qc_required { get; set; }
        public string comments_admin { get; set; }
        public InspectionV2Status? insp_status { get; set; }
        //public int? acceptance_fc { get; set; }
        public int? insp_batch_inspection { get; set; }
        public int? si_subject_id { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? dateCreated { get; set; }
        public string drawingFile { get; set; }
		public bool? factory_loading { get; set; }
		public bool? show_all_images { get; set; }


    }
}	
	
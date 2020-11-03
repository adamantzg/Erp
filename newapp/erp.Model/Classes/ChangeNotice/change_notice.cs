
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Change_notice
	{
		public const int status_pending = 0;
		public const int status_resolved = 1;

        [Key]
		public int id { get; set; }
		public string filename { get; set; }
		public string description { get; set; }
		public DateTime? datecreated { get; set; }
		public int? createdById { get; set; }
		public DateTime? dateModified { get; set; }
		public int? modifiedById { get; set; }
        public int? reason_id { get; set; } 
        public DateTime? expectedReadyDate { get; set; }
        public int? status { get; set; }
        public int? categoryId { get; set; }
        public int? cnid { get; set; }
        public List<Change_notice_allocation> Allocations { get; set; }
        public List<change_notice_image> Images { get; set; }

        public change_notice_document Document { get; set; }

        public Return_category Category { get; set; }
        public change_notice_reasons Reason { get; set; }
	
	}

    
}	
	
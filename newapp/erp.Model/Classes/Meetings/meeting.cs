
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Meeting : TrackableObject
	{
        [Key]
		public int meeting_id { get; set; }
		public DateTime? meetingDate { get; set; }
		public string description { get; set; }

        public virtual List<User> Members { get; set; }
        public virtual List<Meeting_detail> Details { get; set; }
	
	}
}	
	
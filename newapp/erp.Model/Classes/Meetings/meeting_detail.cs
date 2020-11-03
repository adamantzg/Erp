
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Meeting_detail
	{
        [Key]
		public int meeting_detail_id { get; set; }
		public int? meeting_id { get; set; }
		public int? orderid { get; set; }
		public string issue { get; set; }
		public string resolution { get; set; }
        public int? status { get; set; }
        public string heading { get; set; }

        public virtual Meeting Meeting { get; set; }
        public virtual List<Meeting_detail_responsibility> Responsibilities { get; set; }
        public virtual List<Meeting_detail_image> Images { get; set; }
	}
}	
	
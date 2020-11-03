
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Client_page //: IPage
	{
        [Key]
		public int page_id { get; set; }
		public int? page_type { get; set; }
		public string top_level { get; set; }
		public string sub_level { get; set; }
		public string sub_sub_level { get; set; }
		public string notes { get; set; }
		public string page_URL { get; set; }
		public string parameter1 { get; set; }
		public string parameter1_value { get; set; }
		public string URL_value { get; set; }
        //public string icon { get; set; }
	
	}
}	
	
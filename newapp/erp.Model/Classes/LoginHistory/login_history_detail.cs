
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Login_history_detail
	{
        [Key]
		public int detail_unique { get; set; }
		public int? history_id { get; set; }
		public string visit_page { get; set; }
		public string visit_URL { get; set; }
		public DateTime? visit_time { get; set; }
		public int? cprod_id { get; set; }

        [NotMapped]
        public Login_history_page Page { get; set; }
        public Login_history Header { get; set; }
	
	}
}	
	
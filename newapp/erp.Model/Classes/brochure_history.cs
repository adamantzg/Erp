using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
    [NotMapped]
	public class Brochure_history
	{
		public int id { get; set; }
		public int? userid { get; set; }
		public string page { get; set; }
		public string pageURL { get; set; }
		public DateTime? visit_date { get; set; }	
	}
}	
	
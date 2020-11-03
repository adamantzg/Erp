
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Survey_result
	{
		public int result_id { get; set; }
		public int? surveydef_id { get; set; }
		public DateTime? datecreated { get; set; }
		public int? dealer_id { get; set; }
		public int? user_id { get; set; }
		public string ipaddress { get; set; }

        public Dealer Dealer { get; set; }
        public List<Survey_result_answer> Answers { get; set; }
	
	}
}	
	
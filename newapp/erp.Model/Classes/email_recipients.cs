
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Email_recipients
	{
		public int id { get; set; }
		public int? company_id { get; set; }
		public string area { get; set; }
		public string to { get; set; }
		public string cc { get; set; }
		public string bcc { get; set; }
        public string param1 { get; set; }
        public string param2 { get; set; }
	
	}
}	
	
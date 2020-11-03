
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Category_dop
	{
		public int category_id { get; set; }
		public string category_name { get; set; }
		public string en_standard { get; set; }
		public string avcp_system { get; set; }
		public string intended_use { get; set; }

        public List<Catdop_characteristics> Characteristics { get; set; }
	
	}
}	
	
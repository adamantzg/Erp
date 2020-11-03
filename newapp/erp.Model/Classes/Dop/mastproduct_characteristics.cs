using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Mastproduct_characteristics
	{
		public int mast_id { get; set; }
		public int characteristics_id { get; set; }
		public string value { get; set; }
        [NotMapped]
        public string characteristics_name { get; set; }
        public virtual Mast_products MastProduct { get; set; }

        public virtual Catdop_characteristics Characteristic { get; set; }
	
	}
}	
	
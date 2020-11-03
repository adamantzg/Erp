
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Catdop_characteristics
	{
		public int characteristic_id { get; set; }
		public string name { get; set; }
		public int? categorydop_id { get; set; }
		public string defaultvalue { get; set; }

        public virtual List<Mastproduct_characteristics> MastproductCharacteristics { get; set; }
	
	}
}	
	
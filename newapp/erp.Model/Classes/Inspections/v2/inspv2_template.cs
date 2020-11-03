
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspv2_template 
	{
		public int id { get; set; }
		public string name { get; set; }

        public List<Inspv2_criteria> Criteria { get; set; }
        
        public List<Cust_products> Products {  get;set; }
	
	}
}	
	
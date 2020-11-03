
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public partial class Inspv2_customcriteria : ITrackable
	{
		public int id { get; set; }
		public int? criteria_id { get; set; }
		public int cprod_id { get; set; }
		public int? category_id { get; set; }
		public int? point_id { get; set; }
		public string requirements { get; set; }
		public string requirements_cn { get; set; }
		public int? importance { get; set; }
		public int? number { get; set; }
        

        public Inspv2_criteria Criteria { get; set; }
        public Cust_products Product { get; set; }
        public Inspv2point Point { get; set; }

        [NotMapped]
	    public bool IsModified { get; set; }
      
	    public bool IsDeleted { get; set; }
        [NotMapped]
	    public bool IsNew { get; set; }
	}
}	
	
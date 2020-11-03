
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Analytics_subcategory
	{
        [Key]
		public int subcat_id { get; set; }
		public string subcategory_name { get; set; }
		public int? category_id { get; set; }
        public int? seq { get; set; }
        public Analytics_categories Category { get; set; }
        [NotMapped]
        public int? DisplayQty { get; set; }
        [NotMapped]
        public DateTime? FirstShipDate { get; set; }
    }
}	
	
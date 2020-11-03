
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class Dealers_all_brands_new_export
	{
        [Key]
		public int user_id { get; set; }
        [NotMapped]
        public string customer_code{ get; set; }
		public string user_name { get; set; }
        public DateTime? user_created { get; set; }
		public DateTime? user_modified { get; set; }
		[NotMapped]
        public int sales_registered { get; set; }
		public string postcode { get; set; }
		public string user_email { get; set; }
		public string user_email2 { get; set; }
		public string user_tel { get; set; }
		public string user_mobile { get; set; }
		public string user_website { get; set; }
		public string user_address1 { get; set; }
		public string user_address2 { get; set; }
		public string user_address3 { get; set; }
		public string user_address4 { get; set; }
		public int? sales_registered_2014 { get; set; }
		public string clearwater_STATUS { get; set; }
		public string burlington_STATUS { get; set; }
		public string BRITTON_STATUS { get; set; }
		public string ARCADE_STATUS { get; set; }
		public int? hide_1 { get; set; }
		public string user_contact { get; set; }
		public double? longitude { get; set; }
		public double? latitude { get; set; }
        public string distributors { get; set; }
        [NotMapped]
        public string user_country_code { get; set; }
        [NotMapped]
        public string user_country { get; set; }
	
	}
}	
	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using erp.Model.Properties;

namespace erp.Model
{
	
	public class After_sales_enquiry
	{
        public const int response_type_technicalresponse = 1;
        public const int response_type_sitevisit = 2;

        public const int charge_type_customer = 1;
        public const int charge_type_dealer = 2;
        public const int charge_type_distributor = 3;

		public int enquiry_id { get; set; }
        [Required(ErrorMessageResourceName = "After_sales_enquiry_Validation_reference", ErrorMessageResourceType = typeof(Resources))]
        public string reference_number { get; set; }
		public int? dealer_id { get; set; }
        [Required(ErrorMessageResourceName = "After_sales_enquiry_Validation_customername", ErrorMessageResourceType = typeof(Resources))]
        public string cust_name { get; set; }
		public string cust_postcode { get; set; }
		public string cust_address1 { get; set; }
		public string cust_address2 { get; set; }
		public string cust_address3 { get; set; }
		public string cust_address4 { get; set; }
		public string cust_address5 { get; set; }
		public string cust_tel { get; set; }
		public string cust_mobile { get; set; }
		public string cust_email { get; set; }
		public string cust_web { get; set; }
		public string cust_country { get; set; }
		public string details { get; set; }
        [Required(ErrorMessageResourceName = "After_sales_enquiry_Validation_classification", ErrorMessageResourceType = typeof(Resources))]
		public int? clasification_id { get; set; }
		public string po_reference { get; set; }
        public DateTime? datecreated { get; set; }
        public int? created_userid { get; set; }
        public DateTime? datemodified { get; set; }
        public int? modified_userid { get; set; }
		public int? response_type { get; set; }
		public int? charge_type { get; set; }
		public int status_id { get; set; }
        //public int? contractor_id { get; set; }
        //public string contractor_message { get; set; }
        [Required(ErrorMessageResourceName = "After_sales_enquiry_Validation_cprod", ErrorMessageResourceType = typeof(Resources))]
        public int cprod_id { get; set; }
        public DateTime? dateclosed { get; set; }

        public string dealer_name { get; set; }
        [Required(ErrorMessageResourceName = "After_sales_enquiry_Validation_cprod", ErrorMessageResourceType = typeof(Resources))]
        public string cprod_name { get; set; }
        public string cprod_code { get; set; }
        public string classification_name { get; set; }
        public string creator { get; set; }
        public string editor { get; set; }
        public string status_name { get; set; }
        public int distributor_id { get; set; }
        public string distributor_name { get; set; }

        public CompanyType? lastcommentrecipient_role { get; set; }
        public int? lastsitevisit_status { get; set; }
        
        public List<After_sales_enquiry_files> Files { get; set; }
        public List<After_sales_enquiry_comment> Comments { get; set; }
        public List<After_sales_enquiry_sitevisit> SiteVisits { get; set; }

        public After_sales_enquiry()
        {
            status_id = After_sales_enquiry_status.New;
        }
	
	}
}	
	
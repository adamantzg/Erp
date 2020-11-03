
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace erp.Model
{
	
	public class After_sales_enquiry_status
	{
        public const int New = 1;
        public const int AwaitingManufacturerResponse = 2;
        public const int AwaitingDistributorResponse = 3;
        public const int SiteVisitRequested = 4;
        public const int SiteVisitReportSubmitted = 5;
        public const int Processed = 6;

		public int status_id { get; set; }
		public string status_name { get; set; }
	
	}
}	
	
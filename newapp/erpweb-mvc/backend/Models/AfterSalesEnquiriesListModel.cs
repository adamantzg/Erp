using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class AfterSalesEnquiriesListModel
    {
        public List<After_sales_enquiry> Enquiries { get; set; }
        public bool EnableAddNew { get; set; }
        public bool ShowContractorsLink { get; set; }
        public DateTime Month { get; set; }
        public bool IsMasterDistributor { get; set; }
        public List<EnquiryStatus> Statuses { get; set; }
    }

    public class EnquiryStatus
    {
        public const int New = 1;
        public const int AwaitingResponse = 2;
        public const int Responded = 3;
        public const int SiteVisitRequested = 4;
        public const int SiteVisitSubmitted = 5;
        public const int Processed = 6;

        public int status_id { get; set; }
        public string status_name { get; set; }

    }
}
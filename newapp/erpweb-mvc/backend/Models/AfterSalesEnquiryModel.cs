using System;
using System.Collections.Generic;
using erp.Model;
using System.Web.Mvc;

namespace backend.Models
{
    public class AfterSalesEnquiryModel
    {
        public After_sales_enquiry Enquiry { get; set; }
        public List<Clasification> Classifications { get; set; }
        public List<SelectListItem> ResponseTypes { get; set; }
        public List<SelectListItem> ChargeTypes { get; set; }
        public List<External_user> Contractors { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Company> CommentRecipients { get; set; }
        public Dealer Dealer { get; set; }
        public int? distributor_id { get; set; }
        public User CurrentUser { get; set; }
        public int? contractor_id { get; set; }
        public DateTime? datevisited { get; set; }
        public string siteVisitMessage { get; set; }
    }
}
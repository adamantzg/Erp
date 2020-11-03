using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class AfterSalesSiteVisitsModel
    {
        public List<After_sales_enquiry_sitevisit> Visits { get; set; }
        [Required(ErrorMessage="Search term should be supplied")]
        public string SearchTerm { get; set; }
        public string Message { get; set; }
    }
}
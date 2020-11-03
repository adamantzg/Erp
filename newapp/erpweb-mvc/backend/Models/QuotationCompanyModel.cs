using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class QuotationCompanyModel
    {
        public Quotation_companies Company { get; set; }
        public List<Countries> Countries { get; set; }
        public List<Currencies> Currencies { get; set; }
    }
}
using System.Collections.Generic;
using erp.Model;

namespace backend.Models
{
    public class ProductGroupPartialModel
    {
        public List<Cust_products> Products { get; set; }
        public List<BrandCategory> Categories { get; set; }
        public List<CatGroup> CategoryGroups { get; set; }
        public string ProductGroup { get; set; }
        public string Description { get; set; }
    }
}
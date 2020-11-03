using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class QuotationModel
    {
        public Quotation_header Header { get; set; }
        public List<Quotation_companies> Companies { get; set; }
        public List<Currencies> Currencies { get; set; }
        public List<Container_types> ContainerTypes { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Tariffs> Tariffs { get; set; }
    }
}
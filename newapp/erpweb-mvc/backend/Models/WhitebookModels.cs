using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class WhitebookComponentExportModel
    {
        public List<Web_product_pressure> Pressures { get; set; }
        public List<Web_product_part> Parts { get; set; }
        public Dictionary<string,List<Web_product_component_details>> ComponentDetails { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class Inspections_documents
    {
        public string insp_factory_ref { get; set; }
        public string insp_client_ref { get; set; }
        public string insp_client_desc { get; set; }
        public string cprod_instructions { get; set; }
        public string cprod_label { get; set; }
        public string cprod_packaging { get; set; }
        public string cprod_dwg { get; set; }
        public string asaq_ref { get; set; }
        public int prod_length { get; set; }
        public int prod_height { get; set; }
        public int prod_width { get; set; }
        public double prod_nw { get; set; }
        public int pack_width{ get; set; }
        public int pack_depth { get; set; }
        public int pack_hight { get; set; }
        public double pack_GW { get; set; }
        public string prod_image3 { get; set; }
        public string prod_instructions { get; set; }
        public string prod_image4 { get; set; }
        public string prod_image5 { get; set; }
        public string prod_image2 { get; set; }
        public string prod_image1 { get; set; }
        public int insp_line_unique { get; set; }
        public int insp_id { get; set; }
        public double? insp_qty { get; set; }
        public int insp_override_qty { get; set; }
        public int order_linenum { get; set; }
        public int mast_id { get; set; }
        public int cprod_id { get; set; }

        public string inspection_id { get; set; }
    }
}

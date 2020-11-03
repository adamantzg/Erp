using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public partial class inspections_list2
    {
        [Key]
        public int insp_unique { get; set; }           
        public string insp_id { get; set; }
        public string insp_type { get; set; }
        public DateTime? insp_start { get; set; }
        public DateTime? insp_end { get; set; }
        public int? insp_porderid { get; set; }
        public int? insp_qc1 { get; set; }
        public int? insp_qc2 { get; set; }
        public int? insp_qc3 { get; set; }
        public int? insp_qc4 { get; set; }
        public string customer_code { get; set; }
        public string custpo { get; set; }
        public string factory_code { get; set; }
        public string qc_name{ get; set; }
        public string qc_name2{ get; set; }
        public string qc_name3{ get; set; }
        public int? insp_fc { get; set; }
        //public string fc_user { get; set; }
        public string fc_name{ get; set; }
        public int? consolidated_port { get; set; }
        public int? insp_days { get; set; }
        public string insp_comments { get; set; }
        public int? insp_status { get; set; }
        public int? qc_required { get; set; }
        public int? lcl { get; set; }
        public int? gp20 { get; set; }
        public int? gp40 { get; set; }
        public int? hc40 { get; set; }
        public string insp_comments_admin { get; set; }
        public int? adjustment { get; set; }
        public int? LO_id { get; set; }
        public int? LO_qc1 { get; set; }
        public int? LO_qc2 { get; set; }
        public int? LO_qc3 { get; set; }
        public int? acceptance_fc { get; set; }
        public int? insp_batch_inspection { get; set; }
        public int? fc_status { get; set; }
        public int? new_insp_id { get; set; }
        public int? insp_qc5 { get; set; }
        public int? v2_status { get; set; }
        public int? lineCount { get; set; }
        public int? orderid { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using company.Common;

namespace erp.Model
{
    public enum InspectionV2Status
    {
        New = 0,
        ListReady = 1,
        ReportSubmitted = 2,
        Rejected = 3,
        Accepted = 4,
        AwaitingApproval = 5,
        Cancelled = 9
    }

    public partial class Inspection_v2
    {
        
        public virtual Inspection_v2_type InspectionType { get; set; }
        public virtual Company Factory { get; set; }
        public virtual Company Client { get; set; }
        public virtual List<Inspection_v2_line> Lines { get; set; }
        public virtual List<Inspection_v2_controller> Controllers { get; set; }
        public virtual List<Inspection_v2_container> Containers { get; set; }
        public virtual List<Inspection_v2_mixedpallet> MixedPallets { get; set; }
        public virtual Inspection_v2_si_subject Subject { get; set; }

        [NotMapped]
        public string file_id { get; set; }

        public Inspection_v2()
        {
            //acceptance_fc = 0;
            insp_status = 0;
            //dateCreated = DateTime.Today;
        }

        [NotMapped]
        public List<Inspection_v2_loading> AllLoadings
        {
            get
            {
                var result = new List<Inspection_v2_loading>();
                if (Lines != null)
                {
                    foreach (var l in Lines) {
                        if (l.Loadings != null)
                            result.AddRange(l.Loadings);
                    }    
                }
                return result;
            }
        }

        [NotMapped]
        public List<Inspection_v2_image> AllImages
        {
            get
            {
                var result = new List<Inspection_v2_image>();
                if (Lines != null)
                {
                    foreach (var l in Lines) {
                        if (l.Images != null)
                            result.AddRange(l.Images);
                    }
                }
                return result;    
            }
        }

        [NotMapped]
        public List<Inspection_v2_line_rejection> AllRejections
        {
            get
            {
                var result = new List<Inspection_v2_line_rejection>();
                if (Lines != null) {
                    foreach (var l in Lines) {
                        if (l.Rejections != null)
                            result.AddRange(l.Rejections);
                    }
                }
                return result;
            }
        }

        public string ComputedCode
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}", Factory?.factory_code,
                    InspectionType?.name, startdate.ToString("yyMMdd"),
                    Client.IfNotNull(c => c.customer_code));
            }
        }

        public string CustPos
        {
            get
            {
                return Lines != null
                    ? string.Join(",",
                        Lines.Select(l => l.OrderLine.IfNotNull(ol => ol.Header.IfNotNull(h => h.custpo))).Distinct())
                    : string.Empty;
            }
        }

        //public InspectionStatus Status
        //{
        //    get
        //    {
        //        var result = InspectionStatus.Undefined;
        //        if (acceptance_fc == 0 && insp_status == 1)
        //            result = InspectionStatus.Todo;
        //        else if (insp_status == 0)
        //            result = InspectionStatus.Awaiting;
        //        else if (acceptance_fc == 2)
        //            result = InspectionStatus.Rejected;
        //        else if (acceptance_fc == 1 && insp_status == 1)
        //            result = InspectionStatus.Accepted;
        //        return result;
        //    }
        //}
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public interface IInspectionController
    {
        int id { get; set; }
        int inspection_id { get; set; }
        int controller_id { get; set; }
        DateTime startdate { get; set; }
        int duration { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class TrackableObject
    {
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int? CreatedById { get; set; }
        public int? ModifiedById { get; set; }

        public virtual User CreatedBy { get; set; }
        public virtual User ModifiedBy { get; set; }
    }
}

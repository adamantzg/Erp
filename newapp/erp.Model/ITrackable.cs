using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public interface ITrackable
    {
        bool IsModified { get; set; }
        bool IsDeleted { get; set; }
        bool IsNew { get; set; }
    }
}

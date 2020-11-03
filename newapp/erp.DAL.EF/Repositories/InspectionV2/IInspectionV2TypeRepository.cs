using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories.InspectionV2
{
    public interface IInspectionV2TypeRepository
    {
        List<Inspection_v2_type> GetTypes();
    }

   
}

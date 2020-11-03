using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories.InspectionV2
{
    public class InspectionV2TypeRepository : IInspectionV2TypeRepository
    {
        public List<Inspection_v2_type> GetTypes()
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionV2Types.ToList();
            }
        }
    }
}

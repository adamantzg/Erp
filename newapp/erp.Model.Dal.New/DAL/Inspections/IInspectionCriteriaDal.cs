using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
    public interface IInspectionCriteriaDal : IGenericDal<Inspection_criteria>
    {
        List<Inspection_criteria> GetForCategory1(int category1_id);
    }
}

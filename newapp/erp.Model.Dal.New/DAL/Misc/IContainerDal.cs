using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
    public interface IContainerDal : IGenericDal<Containers>
    {
        List<Containers> GetForInspection(int insp_id);
    }
}

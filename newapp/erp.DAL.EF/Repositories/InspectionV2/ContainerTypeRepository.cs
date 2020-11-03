using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories.InspectionV2
{
    public class ContainerTypeRepository : IContainerTypeRepository
    {
        public List<Container_types> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.ContainerTypes.ToList();
            }
        }
    }
}

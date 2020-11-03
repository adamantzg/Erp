using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories
{
    public interface IReturnRepository
    {
        List<Returns> GetForFactories(IList<int> factoryIds,bool commentsOnly = false, DateTime? fromDate = null );
        
    }
}

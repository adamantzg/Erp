using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
    public interface IAqlDal : IGenericDal<Inspv2_aql>
    {
        List<aql_new_category1_returncategory_level> GetCategory1ReturnCategoryLevels();
        List<aql_new_range> GetRanges();
        List<aql_new_range_level_sample> GetRangeLevelSamples();
    }
}

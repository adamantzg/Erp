using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
    public interface IReturnRepository : IGenericRepository<Returns>
    {
		List<ClaimSimple> GetClaimsSimple(int type, int? status1 = null);
		List<ClaimSimple> GetClaimsSimpleAll(User user, int month, bool closedOnly = false, int? qc_id = null, bool loadProducts = false,DateTime? dateFrom = null, DateTime? dateTo = null);
		List<ClaimsStatsOtherProductRow> GetOtherProductsSales(DateTime? etaFrom, DateTime? etaTo, IList<int?> factory_Ids = null, IList<int?> cprod_ids = null,IList<int?> brand_ids = null);
    }
}

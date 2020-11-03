using System;
using System.Collections.Generic;
using erp.Model;

namespace erp.DAL.EF.Repositories.Order
{
    public interface IOrderRepository
    {
        Order_header GetById(int id);

        Dictionary<int, List<Order_lines>> GetLinesInPeriod(IEnumerable<int> cprod_ids,DateTime? from = null, DateTime? to = null,
            int? company_id = null);

        List<Order_line_Summary> GetBrandSummaryForPeriod(DateTime? from, DateTime? to, int? brand_user_id = null,
            string cprod_code = null, bool brands_only = false);

        List<Order_lines> GetLinesInPeriodETD(DateTime? from = null, DateTime? to = null, int? stock_order = null);
        List<Order_lines> GetLinesForOrdersAnalysis(DateTime from, CountryFilter countryFilter = CountryFilter.UKOnly, int? client_id = null);
        List<Order_lines> GetOverDueLines(List<int> clientIds, Dictionary<int, int> rules, DateTime overdueFrom,DateTime overdueTo, bool limitToStock = true);
        List<Order_header> UpdateEtaCombinedOrders(DateTime from);

        void Create(Order_header o);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using LinqKit;

namespace erp.DAL.EF
{
    public class ReportingRepository
    {
        public static List<Stocksummary_factoryvalue> GetFactoryValues(int brand_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.StocksummaryFactoryvalues.Include("Factory").Where(s => s.brand_id == brand_id).ToList();
            }
        }
        
        public static List<DownloadLogTotal> GetDownloadLogTotals(DateTime from, DateTime to)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.DownloadLogs.Where(d => d.log_date >= from && d.log_date <= to)
                        .GroupBy(d => d.log_useruserid)
                        .Select(g => new DownloadLogTotal {user_id = g.Key, Count = g.Count()})
                        .ToList();

            }
        }

        public static List<Factory_client_settings> GetFactoryClientSettings(int custId)
        {
            using (var m = Model.CreateModel())
            {
                return m.FactoryClientSettings.Where(fcs => fcs.custid == custId).ToList();
            }
        }

        public static List<Claims_monthly_summary> GetClaimsMonthlySummary(int month, bool showRefit = true)
        {
            using (var m = Model.CreateModel())
            {
                var sMonth = month.ToString();
                var criteria = PredicateBuilder.True<Claims_monthly_summary>();
                criteria = criteria.And(c => c.request_month == sMonth);

                if (!showRefit)
                    criteria = criteria.And(c => c.claim_type != 2);

                return m.ClaimsMonthlySummaries.AsExpandable().Where(criteria).ToList();

            }
        }

        public static List<tp2_sales_export_all> GetSalesExport(int month)
        {
            using (var unit = new UnitOfWork())
            {
                return unit.Tp2SalesExportAll.Get(m => m.month22 == month).ToList();

            }
        }



    }
}

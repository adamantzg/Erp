using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF
{
    public class SalesForecastRepository
    {
        public static void Create(Sales_forecast_amendments s)
        {
            using (var m = Model.CreateModel())
            {
                m.SalesForecastAmendments.Add(s);
                m.SaveChanges();
            }
        }

        public static List<Sales_forecast_amendments> GetAmendmentsInPeriod(DateTime? from, DateTime? to, IList<int> userIds )
        {
            using (var m = Model.CreateModel())
            {
                
                return
                    m.SalesForecastAmendments.Include("Product").Include("Product.MastProduct").Where(
                        s => (s.dateModified >= from || from == null) && (s.dateModified <= to || to == null) && (s.Product != null && s.Product.cprod_user != null && userIds.Contains(s.Product.cprod_user.Value))).ToList();
            }
        }

        public static List<Sales_forecast> GetSalesInPeriod(int cprod_id,int? from, int? to)
        {
            using (var m = Model.CreateModel())
            {
                return m.SalesForecasts.Where(s => s.cprod_id == cprod_id && (s.month21 >= from || from == null) && (s.month21 <= to || to == null))
                    .ToList();
            }
        }


    }
}

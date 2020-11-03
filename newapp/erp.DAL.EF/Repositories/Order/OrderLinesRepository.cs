using erp.DAL.EF.Repositories;
using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF
{
    public class OrderLinesRepository : GenericRepository<Order_lines>
    {
        public OrderLinesRepository(Model context) : base(context)
        {
        }

        //public List<Company> GetClientsOnOrders(IList<int> factoryIds = null, bool combined = true)
        //{
        //    var useFactories = factoryIds != null;
        //    if (factoryIds == null)
        //        factoryIds = new List<int>();
        //    var statuses = new[] { "X", "Y" };

        //    var result = GetQuery(l => !statuses.Contains(l.Header.status) && (!useFactories || factoryIds.Contains(l.Cust_Product.MastProduct.factory_id.Value)), 
        //                                    includeProperties: "Header.Client, Cust_Product.MastProduct")
        //                .GroupBy(l => l.Header.userid1)
        //                .Select(g => new { user_id = g.FirstOrDefault().Header.Client.user_id, user_name = g.FirstOrDefault().Header.Client.user_name, customer_code = g.FirstOrDefault().Header.Client.customer_code,combined_factory = g.FirstOrDefault().Header.Client.combined_factory }).ToList()
        //                .Select(r=> new Company {user_id = r.user_id, user_name = r.user_name, combined_factory = r.combined_factory, customer_code = r.customer_code }).ToList();
        //    if (combined)
        //        result.AddRange(result.GroupBy(c => c.combined_factory).Select(g => new Company { user_id = -1* g.Key.Value, customer_code = string.Join("/",g.Select(c=>c.customer_code))  }));
        //    return result.OrderBy(r => r.customer_code).ToList();
        //}

         
    }
}

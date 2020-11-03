using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using LinqKit;
using erp.DAL.EF.Repositories;

namespace erp.DAL.EF
{
    public class CompanyRepository : GenericRepository<Company>
    {
        public CompanyRepository(Model context) : base(context)
        {
        }

        public static List<Company> GetFactories(bool combined = false, int? location_id = null)
        {
            using (var m = Model.CreateModel())
            {
                var result = m.Companies.Where(c => c.user_type == (int) Company_User_Type.Factory && (c.consolidated_port == location_id || location_id == null)).ToList();
                if(combined)
                    result.AddRange(m.Companies.Where(c => c.combined_factory > 0 && (c.consolidated_port == location_id || location_id == null)).GroupBy(c => c.combined_factory).ToList()
                        .Select(g=>new Company{user_id = -1*g.Key.Value,factory_code = GetCombinedFactoryCode(g.First().factory_code)}));

                return result;
            }
        }
        
        public static List<Company> GetByType(int type)
        {
            using (var m = Model.CreateModel())
            {
                return m.Companies.Where(c => c.user_type == type).ToList();
            }
        }

        private static string GetCombinedFactoryCode(string factoryCode)
        {
            if (string.IsNullOrEmpty(factoryCode))
                return string.Empty;
            return factoryCode.Substring(0, factoryCode.Length > 1 ? factoryCode.Length - 1 : 1);
        }

        public static List<Company> GetCombinedFactories(int combined_value)
        {
            using (var m = Model.CreateModel())
            {
                return m.Companies.Where(c => c.combined_factory == combined_value).ToList();
            }
        }

        public static List<Company> GetDistributors(bool uk = true)
        {
            var ukCountryCodes = new[]{"GB", "IE"};
            using (var m = Model.CreateModel())
            {
                var builder = PredicateBuilder.True<Company>();
                builder = builder.And(c => c.distributor > 0 && c.hide_1 == 0);
                if (uk)
                    builder = builder.And(c => ukCountryCodes.Contains(c.user_country));
                else
                {
                    builder = builder.And(c => !ukCountryCodes.Contains(c.user_country));
                }
                return m.Companies.AsExpandable().Where(builder).ToList();
            }
        }

        public static List<Company> GetByIds(List<int> ids)
        {
            using (var m = Model.CreateModel())
            {
                return m.Companies.Where(c => ids.Contains(c.user_id)).ToList();
            }
        }

        public List<Company> GetClientsWithOrders(bool combined = false)
        {
            var result = Get(c => c.user_type == (int)Company_User_Type.Client && c.Orders.Count() > 0).ToList();
            if (combined)
                result.AddRange(GetQuery(c=> c.user_type == (int)Company_User_Type.Client && c.Orders.Count() > 0 && c.combined_factory > 0).GroupBy(c => c.combined_factory).ToList()
                    .Select(g => new Company { user_id = -1 * g.Key.Value, customer_code = string.Join("/", g.Select(c => c.customer_code)) }));
            return result;
        }

        public List<Company> GetFactories()
        {
            return Get(c => c.user_type == (int)Company_User_Type.Factory).ToList();
        }
    }
}

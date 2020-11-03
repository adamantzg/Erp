using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using erp.DAL.EF.Repositories;

namespace erp.DAL.EF
{
    public class BudgetRepository : GenericRepository<BudgetActualData>, IRepository<Budget>
    {
        public BudgetRepository(Model context) : base(context)
        {
        }

        public void Create(Budget obj)
        {
            using (var m = Model.CreateModel())
            {
                m.Budgets.Add(obj);
                m.SaveChanges();
            }
        }

        public void Update(Budget obj)
        {
            using (var m = Model.CreateModel())
            {
                m.Budgets.Attach(obj);
                m.SaveChanges();
            }
        }

        public IList<Budget> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.Budgets.ToList();
            }
        }

        public Budget GetById(int id)
        {
            using (var m = Model.CreateModel())
            {
                return m.Budgets.FirstOrDefault(b => b.id == id);
            }
        }

        public static List<Budget> GetForPeriod(int? from, int? to, int? type)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.Budgets.Where(b => (b.month21 >= from || from == null) && (b.month21 <= to || to == null) && (b.data_type == type || type == null))
                     .ToList();
            }
        }

        public void Delete(int id)
        {
            var m = Model.CreateModel();
            m.Database.ExecuteSqlCommand("DELETE FROM Budget WHERE id = @p0", id);
        }

        /*public static List<BudgetActualData> GetBudgetActualDataByCriteria(int? from = null, int? to = null,
            CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2,CWB,LK")
        {
            using (var m = Model.CreateModel())
            {
                var ukCountries = new[] {"GB", "IE", "UK"};

                var exCusts = excludedCustomers.Split(',').ToList();

                return m.BudgetActualData.Include("Distributor").Include("Brand").Where(b =>
                    (b.month21 >= from || from == null) &&
                    (b.month21 <= to || to == null) &&
                    (!exCusts.Contains(b.Distributor.customer_code)) &&
                    (b.Distributor == null
                     || (countryFilter == CountryFilter.UKOnly && ukCountries.Contains(b.Distributor.user_country))
                     || (countryFilter == CountryFilter.NonUK && !ukCountries.Contains(b.Distributor.user_country))))
                    .ToList();
            }
        }*/
        public static List<BudgetActualData> GetBudgetActualDataByCriteria(int? from = null, int? to = null,
            CountryFilter countryFilter = CountryFilter.UKOnly, string excludedCustomers = "NK2,CWB,LK", string includedCustomers = null, string budgetBrands = null)
        {
            using (var m = Model.CreateModel())
            {
                var ukCountries = new[] { "GB", "IE", "UK" };
                var asianCountries = m.Set<Countries>().Where(c => c.continent_code == "AS").Select(c => c.ISO2).ToList();

                var exCusts = excludedCustomers != null ? excludedCustomers.Split(',').ToList() : new List<string>();
                var incCusts = includedCustomers != null ? includedCustomers.Split(',').ToList() : new List<string>();

                var budgetBrandsIds = !string.IsNullOrEmpty(budgetBrands) ? budgetBrands.Split(',').Select(int.Parse).ToList() : new List<int>();

                return m.BudgetActualData.Include("Distributor").Include("Brand").Where(b =>
                    (b.month21 >= from || from == null) &&
                    (b.month21 <= to || to == null) &&
                    (exCusts.Count == 0 || !exCusts.Contains(b.Distributor.customer_code)) &&
                    (incCusts.Count == 0 || incCusts.Contains(b.Distributor.customer_code)) &&
                   
                    ((b.brand_id == null) || (budgetBrands == null || budgetBrands == "" || budgetBrandsIds.Contains(b.brand_id.Value))) &&
                    (b.Distributor == null
					 || countryFilter == CountryFilter.All
                     || (countryFilter == CountryFilter.UKOnly && ukCountries.Contains(b.Distributor.user_country))
                     || (countryFilter == CountryFilter.NonUK && !ukCountries.Contains(b.Distributor.user_country))
                     || (countryFilter == CountryFilter.NonUKExcludingAsia && !ukCountries.Contains(b.Distributor.user_country) && !asianCountries.Contains(b.Distributor.user_country))
                     ))
                    .ToList();
            }
        }

		

    }
}

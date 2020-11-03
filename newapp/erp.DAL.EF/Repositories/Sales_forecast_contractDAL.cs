using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.DAL
{
    public class Sales_forecast_contractDAL
    {
        
        public static void Create(sales_forecast_contract sfc)
        {
            var m = new Model();
            m.sales_forecast_contracts.Add(sfc);
            m.SaveChanges();
        }

        public static List<sales_forecast_contract> GetByCriteria(int cprod_id, int month21From, int month21To)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.sales_forecast_contracts.Where(
                        s => s.cprod_id == cprod_id && s.month21 >= month21From && s.month21 <= month21To).ToList();
            }
        }

        public static void BulkCreate(List<sales_forecast_contract> list)
        {
            using (var m = new Model())
            {
                var existingRecords = m.sales_forecast_contracts.ToList();

                foreach (var salesForecastContract in list)
                {
                    var old =
                        existingRecords.FirstOrDefault(
                            s =>
                                s.cprod_id == salesForecastContract.cprod_id &&
                                s.month21 == salesForecastContract.month21);
                    if (old == null || old.sales_qty != salesForecastContract.sales_qty)
                    {
                        var am = new Sales_forecast_amendments
                        {
                            cprod_id = salesForecastContract.cprod_id,
                            month21 = salesForecastContract.month21,
                            new_qty = salesForecastContract.sales_qty,
                            dateModified = DateTime.Now
                        };
                        if (old != null)
                            am.old_qty = old.sales_qty;
                        m.SalesForecastAmendments.Add(am);
                    }

                }
                
                foreach (var sfc in existingRecords)
                {
                    if (list.Count(sf => sf.cprod_id == sfc.cprod_id && sf.month21 == sfc.month21) == 0)
                    {
                        //missing from import
                        var am = new Sales_forecast_amendments
                        {
                            cprod_id = sfc.cprod_id,
                            month21 = sfc.month21,
                            old_qty = sfc.sales_qty,
                            new_qty = null,
                            dateModified = DateTime.Now
                        };
                        m.SalesForecastAmendments.Add(am);
                    }
                }

                m.SaveChanges();

                m.Database.ExecuteSqlCommand("DELETE FROM `sales_forecast_contracts`");

            }
            //DeleteAll();
            
            using (var m = new Model())
            {
                m.Configuration.AutoDetectChangesEnabled = false;
                m.Configuration.ValidateOnSaveEnabled = false;
                m.sales_forecast_contracts.AddRange(list);                
                m.SaveChanges();
            }
        }

        

        public static List<sales_forecast_contract> GetAll()
        {
            var m = new Model();
            return m.sales_forecast_contracts.ToList();
        }
    }
}

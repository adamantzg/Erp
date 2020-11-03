using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using System.Data.Entity;

namespace erp.DAL.EF.Repositories
{
    public class TechnicalDataRepository
    {
        public static List<Technical_subcategory_template> GetTemplateForSubCat(int subcat_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.TechnicalSubcatTemplates.Include(t=>t.TechnicalDataType).Where(t => t.category1_sub == subcat_id).ToList();
            }
        }

        public static List<Cust_products> GetCustProductsByCriteria(int subcat_id, List<int?> clientIds = null)
        {
            using (var m = Model.CreateModel()) {
                if(clientIds == null)
                    return m.CustProducts.Include(p => p.MastProduct).Include(p => p.MastProduct.Factory).Where(p => p.MastProduct.category1_sub == subcat_id && p.cprod_status != "D").ToList();
                return m.CustProducts.Include(p=>p.MastProduct).Include(p=>p.MastProduct.Factory).Where(p => p.MastProduct.category1_sub == subcat_id && p.cprod_status != "D" && (clientIds.Contains(p.cprod_user))).ToList();
            }
        }

        public static List<Technical_data_type> GetTechnicalDataTypes()
        {
            using (var m = Model.CreateModel())
            {
                return m.TechnicalDataTypes.ToList();
            }
        }

        public static List<Technical_product_data> GeTechnicalProductDataForMastProduct(int mast_id)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.TechnicalProductData.Include(t => t.TechnicalDataType).Where(t => t.mast_id == mast_id).ToList();
            }
        }

        public static List<Technical_product_data> GetTechnicalProductDataForSubCat(int subcat_id)
        {
            using (var m = Model.CreateModel()) {
                return
                    m.TechnicalProductData.Include(t => t.TechnicalDataType).Include(t => t.MastProduct).
                    Where(t =>t.MastProduct.category1_sub == subcat_id).ToList();
            }
        }

        public static void UpdateTechnicalData(List<Technical_product_data> dataToUpdate, List<Technical_product_data> dataToDelete )
        {
            using (var m = Model.CreateModel())
            {
                foreach (var item in dataToUpdate)
                {
                    if (item.unique_id <= 0)
                        m.TechnicalProductData.Add(item);
                    else
                    {
                        m.TechnicalProductData.Attach(item);
                        m.Entry(item).State = EntityState.Modified;
                    }
                }
                m.SaveChanges();
                if (dataToDelete.Count > 0)
                {
                    m.Database.ExecuteSqlCommand(
                        string.Format(
                            "DELETE FROM Technical_product_data WHERE mast_id = {0} AND technical_data_type IN ({1})",dataToDelete[0].mast_id,
                            string.Join(",", dataToDelete.Select(d => d.technical_data_type.ToString()))));
                }
                
            }
        }
    }
}

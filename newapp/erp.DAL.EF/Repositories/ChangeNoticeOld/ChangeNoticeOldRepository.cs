using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories.ChangeNoticeOld
{
    public class ChangeNoticeOldRepository : IChangeNoticeOldRepository
    {
        public List<change_notice_product_table> GetByCriteria(string factory_ref = null, string product_po = null)
        {
            using (var m = Model.CreateModel())
            {
                return
                    m.ChangeNoticeProducts.Include("MastProduct").Include("ChangeNotice")
                        .Where(
                            c =>
                                (factory_ref == null ||
                                 (c.MastProduct != null && c.MastProduct.factory_ref == factory_ref)) &&
                                (product_po == null || c.product_po == product_po)).ToList();
            }
        }

        public List<change_notice_product_table> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.ChangeNoticeProducts.Include("MastProduct").Include("ChangeNotice").ToList();
            }
        }
    }
}

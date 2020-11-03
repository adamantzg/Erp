using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Repositories.ChangeNoticeOld
{
    public interface IChangeNoticeOldRepository
    {
        List<change_notice_product_table> GetByCriteria(string factory_ref = null, string product_po = null);
        List<change_notice_product_table> GetAll();
    }

    
}

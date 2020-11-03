using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF
{
    public class ExternalBrandRepository
    {
        public static List<Brand_external> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.ExternalBrands.ToList();
            }
        }
    }
}

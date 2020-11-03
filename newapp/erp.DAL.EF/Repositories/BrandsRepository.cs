using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF
{
    public class BrandsRepository
    {
        public Brand GetByCriteria(string code)
        {
            using (var m = Model.CreateModel())
            {
                return m.Brands.FirstOrDefault(b => b.code == code);
            }
        }

        public static List<Brand> GetEBBrands(Model m = null)
        {
            var dispose = m == null;
            if (m == null)
                m = Model.CreateModel();
            var result = m.Brands.Where(b => b.eb_brand == 1).ToList();
            if(dispose)
                m.Dispose();
            return result;

        }

        public static List<Brand> GetByIds(IList<int> ids)
        {
            using (var m = Model.CreateModel())
            {
                return m.Brands.Where(b => ids.Contains(b.brand_id)).ToList();
            }
        }

        public static List<Brand> GetActiveBrands()
        {
            using (var m = Model.CreateModel())
            {
                return m.Brands.Where(b => b.Sites.Any(s => s.Url != null)).ToList();
            }
            
        }
    }
}

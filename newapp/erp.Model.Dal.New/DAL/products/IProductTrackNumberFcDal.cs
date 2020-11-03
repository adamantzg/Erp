using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
    public interface IProductTrackNumberFcDal : IGenericDal<products_track_number_fc>
    {
        List<products_track_number_fc> GetByCriteria(int? orderid = null, int? mastid = null);
    }
}

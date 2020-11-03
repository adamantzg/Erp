using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asaq2.Model;

namespace asaq2.WebSites.Common
{
    public class DealerSorter
    {
        public static List<Dealer> FirstGoldSilver(IEnumerable<Dealer> list, int numOfItems = 6)
        {
            return Utilities.BuildDealersList(list.ToList(), numOfItems);
        }

        public static List<Dealer> StatusDesc(IEnumerable<Dealer> list)
        {
            return list.OrderByDescending(d => d.BrandStatus).ToList();
        }
    }
}

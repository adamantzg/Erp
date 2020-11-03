using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF
{
    public class ExternalDealerDisplayRepository
    {
        public static void Create(Dealer_external_display disp)
        {
            using (var m = Model.CreateModel())
            {
                var dealer = m.ExternalDealers.Include("Displays").FirstOrDefault(d => d.id == disp.dealer_id);
                if (dealer != null)
                {
                    dealer.Displays.Add(disp);
                }
                m.SaveChanges();
            }
        }
        

        public static void Delete(int dealer_id, int webproduct_id)
        {
            using (var m = Model.CreateModel())
            {
                var dealer = m.ExternalDealers.Include("Displays").FirstOrDefault(d => d.id == dealer_id);
                if (dealer != null)
                {
                    var disp = dealer.Displays.FirstOrDefault(di => di.webproduct_id == webproduct_id);
                    if (disp != null)
                    {
                        dealer.Displays.Remove(disp);
                        m.SaveChanges();
                    }
                }
                
            }
        }
    }
}

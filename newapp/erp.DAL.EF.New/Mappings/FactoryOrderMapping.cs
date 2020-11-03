using erp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.Mappings
{
    public class FactoryOrderMapping : EntityTypeConfiguration<Factory_stock_order>
    {
        public FactoryOrderMapping()
        {
            HasMany(o => o.Lines).WithOptional(l => l.Order).HasForeignKey(l => l.orderid);
            HasOptional(o => o.Factory).WithMany().HasForeignKey(o => o.factory_id);
            HasOptional(o => o.Creator).WithMany().HasForeignKey(o => o.creator_id);
        }
    }

    public class FactoryOrderLinesMapping : EntityTypeConfiguration<Factory_stock_order_lines>
    {
        public FactoryOrderLinesMapping()
        {
            HasOptional(o => o.MastProduct).WithMany().HasForeignKey(o => o.mast_id);            
        }
    }

    public class StockMovementMapping : EntityTypeConfiguration<Stock_movements>
    {
        public StockMovementMapping()
        {
            
        }
    }
}

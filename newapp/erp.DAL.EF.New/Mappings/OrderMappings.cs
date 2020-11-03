using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class OrderLinesMappings : EntityTypeConfiguration<Order_lines>
    {
        public OrderLinesMappings()
        {
            HasKey(l => l.linenum);
            HasOptional(l => l.Header).WithMany(h => h.Lines).HasForeignKey(l => l.orderid);
            HasMany(l => l.Allocations).WithOptional(a => a.StockLine).HasForeignKey(a => a.st_line);
            HasMany(l => l.SOAllocations).WithOptional(a => a.CalloffLine).HasForeignKey(a => a.so_line);
            HasOptional(l => l.Cust_Product).WithMany(c => c.OrderLines).HasForeignKey(l => l.cprod_id);
            

            HasOptional(l => l.Currency).WithMany(c => c.OrderLines).HasForeignKey(l => l.unitcurrency);
        }
    }

    public class OrderHeaderMappings : EntityTypeConfiguration<Order_header>
    {
        public OrderHeaderMappings()
        {
            HasKey(o => o.orderid);
            Property(o => o.orderid).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasOptional(o => o.Client).WithMany(h => h.Orders).HasForeignKey(h => h.userid1);
            HasMany(o => o.Invoices).WithOptional(i => i.OrderHeader).HasForeignKey(i => i.orderid);
            HasOptional(o => o.Parent).WithMany().HasForeignKey(o => o.combined_order);
            HasMany(o => o.CombinedOrders).WithOptional().HasForeignKey(o => o.combined_order);
            HasMany(o => o.PorderHeaders).WithOptional(po => po.OrderHeader).HasForeignKey(po => po.soorderid);
            HasOptional(o => o.LoadingFactory).WithMany().HasForeignKey(o => o.loading_factory);
            HasMany(o => o.Shipments).WithOptional().HasForeignKey(s => s.orderid);
        }
    }

    public class POrderHeaderMappings : EntityTypeConfiguration<Porder_header>
    {
        public POrderHeaderMappings()
        {
            HasKey(p => p.porderid);
            HasMany(p => p.Lines).WithOptional(l => l.Header).HasForeignKey(l => l.porderid);
            HasOptional(p => p.Factory).WithMany(f=>f.POrders).HasForeignKey(p => p.userid);
            //HasOptional(p => p.OrderHeader).WithMany(o => o.PorderHeaders).HasForeignKey(p => p.soorderid);
        }
    }

    public class POrderLineMappings : EntityTypeConfiguration<Porder_lines>
    {
        public POrderLineMappings()
        {
            HasKey(l => l.linenum);
            HasOptional(l => l.MastProduct).WithMany(m => m.PorderLines).HasForeignKey(l => l.mast_id);
            HasOptional(l => l.Currency).WithMany(c => c.PorderLines).HasForeignKey(l => l.unitcurrency);
            HasOptional(l => l.OrderLine).WithMany(p => p.PorderLines).HasForeignKey(p => p.soline);
        }
    }

    public class StockAllocationMappings : EntityTypeConfiguration<Stock_order_allocation>
    {
        public StockAllocationMappings()
        {
            HasKey(l => l.unique_link_ref);
            HasOptional(l => l.StockLine).WithMany().HasForeignKey(l => l.st_line);
            HasOptional(l => l.CalloffLine).WithMany().HasForeignKey(l => l.so_line);
        }
    }

    public class ContainerCalculationMappings : EntityTypeConfiguration<Containercalculation_order>
    {
        public ContainerCalculationMappings()
        {
            HasMany(c => c.Products).WithOptional(p=>p.Calculation).HasForeignKey(p => p.calculation_id);
            HasOptional(c => c.Order).WithMany().HasForeignKey(c => c.orderid);
        }
    }

    public class ContainerCalculationProductMappings : EntityTypeConfiguration<Containercalculation_order_product>
    {
        public ContainerCalculationProductMappings()
        {
            HasOptional(c => c.MastProduct).WithMany().HasForeignKey(c => c.mast_id);
        }
    }
}

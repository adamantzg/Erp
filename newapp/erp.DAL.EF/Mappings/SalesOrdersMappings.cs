
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using erp.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.Mappings
{
    public class SalesOrdersMappings : EntityTypeConfiguration<Sales_orders>
    {
        public SalesOrdersMappings()
       { 
            ToTable("sales_orders");
           
            HasOptional(s => s.Product).WithMany().HasForeignKey(s => s.cprod_id);
            HasOptional(s => s.Bundle).WithMany().HasForeignKey(s => s.bundle_id);
            

        }
    }

    public class BackOrdersMappings: EntityTypeConfiguration<Us_backorders>
    {
        public BackOrdersMappings()
        {
            ToTable("Us_backorders");
            HasOptional(o => o.Dealer).WithMany().HasForeignKey(o => o.customer);
            HasOptional(s => s.CustProduct).WithMany().HasForeignKey(s => s.cprod_id);
            HasOptional(s => s.Bundle).WithMany().HasForeignKey(s => s.bundle_id);
        }
    }

    public class SalesOrdersHeadersMappings: EntityTypeConfiguration<Sales_orders_headers>
    {
        public SalesOrdersHeadersMappings()
        {
            ToTable("sales_orders_headers");
            HasMany(o => o.Shippings).WithOptional().HasForeignKey(s => s.header_id);
        }
        
    }
    public class SalesOrdersHeadersShippingMappings : EntityTypeConfiguration<Sales_orders_headers_shipping>
    {
        public SalesOrdersHeadersShippingMappings()
        {
            ToTable("sales_orders_headers_shipping");
            //HasMany(o => o.Shippings).WithOptional().HasForeignKey(s => s.header_id);
        }

    }
}

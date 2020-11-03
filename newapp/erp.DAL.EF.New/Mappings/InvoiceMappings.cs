using erp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF.Mappings
{
    public class InvoiceMappings : EntityTypeConfiguration<Invoices>
    {
        public InvoiceMappings()
        {
            HasOptional(i => i.Client).WithMany().HasForeignKey(i => i.userid1);
            HasOptional(i => i.From).WithMany().HasForeignKey(i => i.invoice_from);
            HasOptional(i => i.OrderHeader).WithMany().HasForeignKey(i => i.orderid);
            HasOptional(i => i.Payment).WithMany().HasForeignKey(i => i.payment_details_id);
            HasMany(i => i.Lines).WithOptional().HasForeignKey(l => l.invoice_id);
            HasMany(i => i.CreditnoteLines).WithRequired().HasForeignKey(l => l.invoice_id);
            HasMany(i => i.Sequences).WithOptional().HasForeignKey(s => s.invoiceid);
        }
    }

    //public class InvoiceLinesMappings: EntityTypeConfiguration<Invoice_lines>
    //{
    //    public InvoiceLinesMappings()
    //    {
    //        HasRequired(l => l.CustProduct).WithMany().HasForeignKey(l => l.cprod_id);
    //    }
    //}
}

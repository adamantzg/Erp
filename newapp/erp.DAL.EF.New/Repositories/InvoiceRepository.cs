﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.New
{
    public class InvoiceRepository : GenericRepository<Invoices>, IInvoiceRepository
    {
        public InvoiceRepository(DbContext context) : base(context)
        {
        }

        public Invoice_export_settings GetExportSettingsByClient(int client_id)
        {
            return context.Set<Invoice_export_settings>().FirstOrDefault(i => i.client_id == client_id);            
        }

        public void UpdateExportSettings(Invoice_export_settings i)
        {
            context.Set<Invoice_export_settings>().Attach(i);
            context.Entry(i).State = EntityState.Modified;
            context.SaveChanges();            
        }

        public void CreateExportSettings(Invoice_export_settings i)
        {
            context.Set<Invoice_export_settings>().Add(i);
            context.SaveChanges();
        }

        public List<Shipments> GetShipmentsForOrderIds(IList<int?> orderids)
        {
            return context.Set<Shipments>().Where(s => orderids.Contains(s.orderid)).ToList();            
        }

        public void Insert(Invoices inv, bool createEBinvoice = true, int? invoice_sequence_type = null)
        {
            inv.invoice = GetQuery().Max(i => i.invoice)  + 1;

            if(createEBinvoice) {
                if(invoice_sequence_type == null) {
                    inv.eb_invoice = Math.Max(context.Set<Order_header>().Max(o => o.order_eb_invoice) ?? 0, context.Set<Invoices>().Max(i => i.eb_invoice) ?? 0) + 1;
                }
            }
            if(inv.Lines != null)

            if(invoice_sequence_type != null) {
                if (inv.Sequences == null)
                    inv.Sequences = new List<order_invoice_sequence>();
                    var max_sequence = (context.Set<order_invoice_sequence>().Where(o => o.type == invoice_sequence_type).Max(o => o.sequence) ?? 0) + 1;
                inv.Sequences.Add(new order_invoice_sequence { type = invoice_sequence_type,invoiceid = inv.invoice,sequence = max_sequence });
            }
            base.Insert(inv);
        }

        public override void Update(Invoices entityToUpdate)
        {
            context.UpdateGraph(entityToUpdate, map => map.OwnedCollection(i=>i.Lines));
        }


    }
}

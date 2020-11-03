
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class InvoiceTypeDAL : GenericDal<Invoice_type>, IInvoiceTypeDAL
    {
		protected override string GetAllSql()
		{
			return "SELECT * FROM invoice_type";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM invoice_type WHERE invoice_type_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO invoice_type (invoice_type_id,invoice_type_name) VALUES(@invoice_type_id,@invoice_type_name)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE invoice_type SET invoice_type_name = @invoice_type_name WHERE invoice_type_id = @invoice_type_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM invoice_type WHERE invoice_type_id = @id";
		}

		protected override bool IsAutoKey => false;
		protected override string IdField => "invoice_type_id";
		
		public InvoiceTypeDAL(IDbConnection conn) : base(conn)
		{
		}
	}
}
			
			
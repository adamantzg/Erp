
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace erp.Model.Dal.New
{
    public class InvoiceLinesDAL : GenericDal<Invoice_lines>, IInvoiceLinesDAL
	{
		public InvoiceLinesDAL(IDbConnection conn) : base(conn)
		{
		}

		protected override string IdField => "linenum";
		
        public List<Invoice_lines> GetByInvoice(int id)
        {
            return conn.Query<Invoice_lines>("SELECT * FROM invoice_lines WHERE invoice_id = @id", new {id}).ToList();
        }

		protected override string GetAllSql()
		{
			return "SELECT * FROM invoice_lines";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM invoice_lines WHERE linenum = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO invoice_lines (invoice_id,linedate,cprod_id,description,orderqty,unitprice,unitcurrency,linestatus,record_type,qty_type,image_id) 
                    VALUES(@invoice_id,@linedate,@cprod_id,@description,@orderqty,@unitprice,@unitcurrency,@linestatus,@record_type,@qty_type,@image_id)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE invoice_lines SET invoice_id = @invoice_id,linedate = @linedate,cprod_id = @cprod_id,
					description = @description,orderqty = @orderqty,unitprice = @unitprice, unitcurrency = @unitcurrency,linestatus = @linestatus,
					record_type = @record_type,qty_type = @qty_type, image_id=@image_id WHERE linenum = @linenum";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM invoice_lines WHERE linenum = @id";
		}
	}
}
			
			
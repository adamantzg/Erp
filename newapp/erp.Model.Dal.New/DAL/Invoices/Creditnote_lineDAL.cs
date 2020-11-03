
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class CreditNoteLinesDAL : GenericDal<Creditnote_line>, ICreditNoteLinesDAL
	{
		public CreditNoteLinesDAL(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM creditnote_line";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM creditnote_line WHERE line_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO creditnote_line (invoice_id,return_no,cprod_code,client_ref,cprod_name,unitprice,quantity,overridden) 
					VALUES(@invoice_id,@return_no,@cprod_code,@client_ref,@cprod_name,@unitprice,@quantity,@overridden)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE creditnote_line SET invoice_id = @invoice_id,return_no = @return_no,cprod_code = @cprod_code,client_ref = @client_ref,
					cprod_name = @cprod_name,unitprice = @unitprice,quantity = @quantity,overridden = @overridden 
                    WHERE line_id = @line_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM creditnote_line WHERE line_id = @id";
		}

		protected override string IdField => "line_id";
	}
}
			
			
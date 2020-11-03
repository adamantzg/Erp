using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class PorderLinesDal : GenericDal<Porder_lines>, IPorderLinesDal
	{
		public PorderLinesDal(IDbConnection conn) : base(conn)
		{
		}

		public List<Porder_lines> GetLinesByCriteria(int? mast_id = null, DateTime? dateFrom = null)
		{
			return conn.Query<Porder_lines>(
				@"SELECT porder_lines.* FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid
				 INNER JOIN cust_products ON porder_lines.cprod_id = cust_products.cprod_id
				 INNER JOIN order_lines ON porder_lines.soline = order_lines.linenum
				 INNER JOIN order_header ON order_lines.orderid = order_header.orderid
				WHERE order_header.status NOT IN ('X', 'Y') AND
					  (cust_products.cprod_mast = @mast_id OR @mast_id IS NULL) AND 
					  (porder_header.po_req_etd >= @dateFrom OR @dateFrom IS NULL)", new { mast_id, dateFrom }).ToList();
		}

		protected override string GetAllSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}
	}
}

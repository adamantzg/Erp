using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class StockMovementsDal : GenericDal<stock_movements>, IStockMovementsDal
	{
		public StockMovementsDal(IDbConnection conn) : base(conn)
		{
		}

		public List<stock_movements> GetByCriteria(int? mast_id = null, DateTime? dateFrom = null)
		{
			return conn.Query<stock_movements>(
				@"SELECT stock_movements.* FROM stock_movements 
				WHERE (mast_id = @mast_id OR @mast_id IS NULL) AND 
					  (date >= @dateFrom OR @dateFrom IS NULL)", new { mast_id, dateFrom }).ToList();
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

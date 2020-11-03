using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class StockCodeDal : GenericDal<Stock_code>, IStockCodeDal
	{
		public StockCodeDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM stock_code";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM stock_code WHERE stock_code_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO stock_code (stock_code_id,stock_code_name,target_weeks) VALUES(@stock_code_id,@stock_code_name,@target_weeks)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM stock_code WHERE stock_code_id = @id";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE stock_code SET stock_code_name = @stock_code_name,target_weeks = @target_weeks WHERE stock_code_id = @stock_code_id";
		}

		protected override string IdField => "stock_code_id";
		protected override bool IsAutoKey => false;
	}
}

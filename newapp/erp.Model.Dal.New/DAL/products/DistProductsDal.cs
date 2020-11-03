using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class DistProductsDal : GenericDal<Dist_products>, IDistProductsDal
	{
		public DistProductsDal(IDbConnection conn) : base(conn)
		{

		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM dist_products";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM dist_products WHERE distprod_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO dist_products (client_id,dist_cprod_id,dist_opening_stock,dist_special_code,dist_special_desc,dist_special_price,dist_special_curr,dist_special_moq,
                                dist_seq,dist_spec_disc,dist_stock,dist_stock_date,client_system_code,dist_onorder) VALUES(@client_id,@dist_cprod_id,@dist_opening_stock,@dist_special_code,
                                @dist_special_desc,@dist_special_price,@dist_special_curr,@dist_special_moq,@dist_seq,@dist_spec_disc,@dist_stock,@dist_stock_date,@client_system_code,@dist_onorder)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM dist_products WHERE distprod_id = @id";
		}

		
		protected override string GetUpdateSql()
		{
			return @"UPDATE dist_products SET client_id=@client_id,dist_cprod_id=@dist_cprod_id,dist_opening_stock=@dist_opening_stock,dist_special_code=@dist_special_code,
                                dist_special_desc=@dist_special_desc,dist_special_price=@dist_special_price,dist_special_curr=@dist_special_curr,dist_special_moq=@dist_special_moq,
                                dist_seq=@dist_seq,dist_spec_disc=@dist_spec_disc,dist_stock=@dist_stock,dist_stock_date=@dist_stock_date,client_system_code=@client_system_code,
                                dist_onorder=@dist_onorder  WHERE distprod_id = @distprod_id";
		}

		public List<Dist_products> GetByUser(int userid)
		{
			return conn.Query<Dist_products>("SELECT * FROM dist_products WHERE client_id = @userid", new {userid}).ToList();
		}

		public List<Dist_products> GetByIds(IList<int> ids)
		{
			return conn.Query<Dist_products>("SELECT * FROM dist_products WHERE dist_cprod_id IN @ids", new {ids})
		            .ToList();
		}

		protected override string IdField => "dist_cprod_id";
		protected override bool IsAutoKey => true;
	}
}

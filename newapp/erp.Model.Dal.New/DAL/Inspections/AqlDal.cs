using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
    public class AqlDal : GenericDal<Inspv2_aql>, IAqlDal
    {
        public AqlDal(IDbConnection conn) : base(conn)
        {
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

		public List<aql_new_range> GetRanges()
		{
			return conn.Query<aql_new_range>("SELECT * FROM aql_new_range").ToList();
		}

		
		public List<aql_new_range_level_sample> GetRangeLevelSamples()
		{
			
			return conn.Query<aql_new_range_level_sample, aql_new_sample_size, aql_new_range_level_sample>(
					@"SELECT * FROM aql_new_range_level_sample 
					INNER JOIN aql_new_sample_size ON aql_new_range_level_sample.sample_size_id = aql_new_sample_size.id",
					(rls, sample) =>
					{
						rls.SampleSize = sample;
						return rls;
					}).ToList();
			
		}

		public  List<aql_new_category1_returncategory_level> GetCategory1ReturnCategoryLevels()
		{
			return conn.Query<aql_new_category1_returncategory_level>("SELECT * FROM aql_new_category1_returncategory_level").ToList();
			
		}
	}
}

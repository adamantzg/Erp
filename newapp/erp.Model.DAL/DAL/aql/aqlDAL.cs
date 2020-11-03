using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public class aqlDAL
	{
		public static List<aql_new_range> GetRanges()
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				return conn.Query<aql_new_range>("SELECT * FROM aql_new_range").ToList();
			}
		}

		/*public static List<aql_new_sample_size> GetSampleSizes()
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				return conn.Query<aql_new_sample_size>("SELECT * FROM aql_new_sample_size").ToList();
			}
		}*/

		public static List<aql_new_range_level_sample> GetRangeLevelSamples()
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				return conn.Query<aql_new_range_level_sample, aql_new_sample_size, aql_new_range_level_sample>(
					 @"SELECT * FROM aql_new_range_level_sample 
						INNER JOIN aql_new_sample_size ON aql_new_range_level_sample.sample_size_id = aql_new_sample_size.id",
					 (rls, sample) =>
					 {
						 rls.SampleSize = sample;
						 return rls;
					 }).ToList();
			}
		}

		public static List<aql_new_category1_returncategory_level> GetCategory1ReturnCategoryLevels()
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				return conn.Query<aql_new_category1_returncategory_level>("SELECT * FROM aql_new_category1_returncategory_level").ToList();
			}
		}
	}
}

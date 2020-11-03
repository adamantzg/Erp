using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class SchemaInfoDal : ISchemaInfoDal
	{
		private readonly MySqlConnection conn;

		public SchemaInfoDal(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}

		public List<DbColumnInfo> GetColumnInfo(IList<string> tables = null, string pattern = null)
		{
			var criteria = new List<string>();
			if(tables != null)
			{
				criteria.Add("TABLE_NAME IN @tables");
			}
			if(pattern != null)
			{
				pattern = "%" + pattern + "%";
				criteria.Add("TABLE_NAME LIKE @pattern");
			}
			//get tables only -skip views
			var tableDefs = conn.Query($@"SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_TYPE = 'BASE TABLE' 
			AND {string.Join(" AND ", criteria)} ", new { tables, pattern }).ToList();
			if(tableDefs.Count > 0)
			{
				var table_names = tableDefs.Select(d=>d.TABLE_NAME.ToString()).ToList();
				return conn.Query<DbColumnInfo>(
				$@"SELECT TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, DATA_TYPE, COLUMN_TYPE, COLUMN_KEY FROM information_schema.columns 
				WHERE TABLE_NAME IN @table_names", new { table_names }).ToList();
			}
			return null;
		}
	}
}

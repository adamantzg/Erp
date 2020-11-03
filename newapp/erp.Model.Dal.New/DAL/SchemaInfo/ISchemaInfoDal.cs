using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface ISchemaInfoDal
	{
		List<DbColumnInfo> GetColumnInfo(IList<string> tables, string pattern = null);
	}
}
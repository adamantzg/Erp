using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IRangeDAL
	{
		List<Range> GetAll();
		Range GetById(int id);
		List<Range> GetByCompanyId(int id, bool combined = true);
		void Create(Range o);
		void Update(Range o);
		void Delete(int rangeid);
	}
}
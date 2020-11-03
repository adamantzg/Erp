using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IReturnsCommentsDAL : IGenericDal<Returns_comments>
	{
		List<Returns_comments> GetByReturn(int return_id);
	}
}
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IReturnsImportanceDAL : IGenericDal<Returns_importance>
	{
		List<Returns_importance> GetForType(int type);
	}
}
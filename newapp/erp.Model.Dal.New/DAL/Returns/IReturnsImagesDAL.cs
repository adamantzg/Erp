using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IReturnsImagesDAL : IGenericDal<Returns_images>
	{
		List<Returns_images> GetByReturn(int return_id, int file_category=0);
	}
}
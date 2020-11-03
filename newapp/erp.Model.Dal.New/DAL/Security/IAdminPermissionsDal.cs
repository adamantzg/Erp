using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IAdminPermissionsDal : IGenericDal<Admin_permissions>
	{
		List<Admin_permissions> GetByCompany(int company_id);
	}
}
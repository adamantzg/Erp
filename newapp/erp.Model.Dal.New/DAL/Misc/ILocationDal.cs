using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface ILocationDAL
	{
		List<Location> GetAll();
		List<Location> GetForUser(int userid);
	}
}

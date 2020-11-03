using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IDeliveryLocationsDAL
	{
		List<Delivery_locations> GetForClient(int client_id);
	}
}
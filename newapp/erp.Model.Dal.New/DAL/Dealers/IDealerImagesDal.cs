using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IDealerImagesDal
	{
		List<Dealer_images> GetByDealer(int dealer_id);
		List<Dealer_images> GetUnallocatedImages();
		List<Dealer_images> GetForSlaveHost(int host_id);
		void DeleteTransferData(int host_id, int image_id);
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IInstructionsDal : IGenericDal<instructions_new>
	{
		List<string> GetForSlaveHost(int host_id);
		void DeleteTransferData(int host_id, string filename);
	}
}

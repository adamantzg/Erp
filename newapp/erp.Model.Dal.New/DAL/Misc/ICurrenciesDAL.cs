using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface ICurrenciesDAL
	{
		List<Currencies> GetAll();
		Currencies GetById(int id);
		void Create(Currencies o);
		void Update(Currencies o);
		void Delete(int curr_code);
	}
}
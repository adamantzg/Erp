using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IContinentsDAL
	{
		List<Continents> GetAll(bool all = false);
		Continents GetById(int id);
		Continents GetByCode(string code);
		void Create(Continents o);
		void Update(Continents o);
		void Delete(int country_id);
		List<Continents> GetForBrand(int brandid);
	}
}
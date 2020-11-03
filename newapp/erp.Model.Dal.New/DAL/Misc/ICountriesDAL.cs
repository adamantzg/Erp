using System.Collections.Generic;
using System.Data;

namespace erp.Model.Dal.New
{
	public interface ICountriesDAL
	{
		List<Countries> GetAll();
		Countries GetById(int id);
		Countries GetById_ISO2(string iso2);
		List<Countries> GetForExistingDealers();
		void Create(Countries o);
		void Update(Countries o);
		void Delete(int country_id);
		List<Countries> GetForDealersByBrand(string brand_code);
		List<Countries> GetCountriesByContinentAndBrand(string code, string brand_code);
		List<Countries> GetByLanguage(int language_id, IDbConnection conn = null);
		string GetCountryCondition(CountryFilter countryFilter, string prefix = "", bool useAnd = true, string countryField = "user_country");
		List<string> GetUkCountryCodes();
		List<Countries> GetForContinent(string code);
	}
}
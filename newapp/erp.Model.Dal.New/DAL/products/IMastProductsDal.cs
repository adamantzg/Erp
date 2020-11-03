using System.Collections.Generic;
using System;

namespace erp.Model.Dal.New
{
	public interface IMastProductsDal
	{
		List<Mast_products> GetAll();
		Mast_products GetById(int id, bool loadCharacteristics = false);
		void Create(Mast_products o);
		void Update(Mast_products o);
		Mast_products GetByRefAndCode(string factoryRef, string factoryCode);
		List<ProductDate> ProductFirstShipmentDates(IList<int> ids);
		void DeletePackaging(int id);
		List<Mast_products> GetProductsWithSalesHistory(int? factoryId, int? categoryId, DateTime fromDate);
		void RemoveFile(int mastProductid, int fileId);
		void UpdateOtherFiles(Mast_products mp, int? fileType = null);
	}
}
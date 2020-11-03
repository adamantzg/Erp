using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.DAL.EF.New;
using erp.Model.Dal.New;
using erp.Model;

namespace backend.ApiServices
{
	public class TechnicalDataService : ITechnicalDataService
	{
		private readonly IUnitOfWork unitOfWork;

		public TechnicalDataService(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public List<Technical_subcategory_template> GetTemplateForSubCat(int subcat_id)
        {
			return unitOfWork.TechnicalSubcatTemplatesRepository.Get(t => t.category1_sub == subcat_id, includeProperties: "TechnicalDataType").ToList();            
        }

        public List<Cust_products> GetCustProductsByCriteria(int subcat_id, List<int?> clientIds = null)
        {
			var useIds = clientIds != null;
			return unitOfWork.CustProductRepository.Get(p => p.MastProduct.category1_sub == subcat_id 
			&& p.cprod_status != "D" && (!useIds || clientIds.Contains(p.cprod_user)), includeProperties: "MastProduct.Factory").ToList();

        }

        public List<Technical_data_type> GetTechnicalDataTypes()
        {
            return unitOfWork.TechnicalDataTypeRepository.Get().ToList();
        }

        public List<Technical_product_data> GeTechnicalProductDataForMastProduct(int mast_id)
        {
            return unitOfWork.TechnicalProductDataRepository.Get(p=>p.mast_id == mast_id).ToList();
        }

        public List<Technical_product_data> GetTechnicalProductDataForSubCat(int subcat_id)
        {
			return unitOfWork.TechnicalProductDataRepository.Get(t=>t.MastProduct.category1_sub == subcat_id, 
				includeProperties: "TechnicalDataType,MastProduct").ToList();            
        }

        public void UpdateTechnicalData(List<Technical_product_data> dataToUpdate, List<Technical_product_data> dataToDelete )
        {
            
            foreach (var item in dataToUpdate)
            {
                if (item.unique_id <= 0)
                    unitOfWork.TechnicalProductDataRepository.Insert(item);
                else
                {
                    unitOfWork.TechnicalProductDataRepository.Update(item);
                }
            }                            
            unitOfWork.Save();
        }
	}
}
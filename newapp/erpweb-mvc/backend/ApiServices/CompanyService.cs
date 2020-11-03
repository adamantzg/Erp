using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model.Dal.New;
using erp.DAL.EF.New;
using erp.Model;

namespace backend.ApiServices
{
	public class CompanyService : ICompanyService
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICompanyDAL companyDAL;
		private readonly IFileService fileService;
		private readonly IFileTypeDal fileTypeDal;

		public CompanyService(IUnitOfWork unitOfWork, ICompanyDAL companyDAL, IFileService fileService, 
			IFileTypeDal fileTypeDal)
		{
			this.unitOfWork = unitOfWork;
			this.companyDAL = companyDAL;
			this.fileService = fileService;
			this.fileTypeDal = fileTypeDal;
		}

		public void ProcessFileUrls(IList<Company> companies)
		{
			var types = fileTypeDal.GetAll();
			foreach(var c in companies)
			{
				if(c.Files?.Count > 0)
				{
					foreach(var fi in c.Files)
					{
						fi.url = fileService.GetFileUrl(types, fi);
					}
				}
			}
		}

	}
}
using erp.Model.Dal.New;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using erp.DAL.EF.New;
using backend.ApiServices;
using erp.Model;

namespace backend.Controllers
{
	[Authorize]
	[RoutePrefix("api/company")]
	public class CompanyApiController : ApiController
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICompanyDAL companyDAL;
		private readonly IFileService fileService;
		private readonly ICompanyService companyService;
		private readonly IUserDAL userDAL;
		private readonly IPermissionDAL permissionDAL;
		private readonly IRoleDAL roleDAL;
		private readonly IBrandsDAL brandsDAL;
		private readonly IAccountService accountService;
		private readonly IFileTypeDal fileTypeDal;
		private readonly IUploadedFileHandler uploadedFileHandler;

		public CompanyApiController(IUnitOfWork unitOfWork, IFileTypeDal fileTypeDal,
			IUploadedFileHandler uploadedFileHandler, ICompanyDAL companyDAL, IFileService fileService,
			ICompanyService companyService, IUserDAL userDAL, IPermissionDAL permissionDAL, IRoleDAL roleDAL,
			IBrandsDAL brandsDAL, IAccountService accountService)
		{
			this.unitOfWork = unitOfWork;
			this.companyDAL = companyDAL;
			this.fileService = fileService;
			this.companyService = companyService;
			this.userDAL = userDAL;
			this.permissionDAL = permissionDAL;
			this.roleDAL = roleDAL;
			this.brandsDAL = brandsDAL;
			this.accountService = accountService;
			this.fileTypeDal = fileTypeDal;
			this.uploadedFileHandler = uploadedFileHandler;
		}

		[Route("getFactories")]
		[HttpGet]
		public object getFactories()
		{
			return companyDAL.GetFactories().OrderBy(f => f.factory_code);
		}

		[Route("getClients")]
		[HttpGet]
		public object getClients()
		{
			return companyDAL.GetClients();
		}

		[Route("updateBulk")]
		[HttpPut]
		public object updateBulk(IList<Company> companies)
		{
			var files = companies.SelectMany(p => p.Files).Distinct(new FileComparer()).ToList();
			fileService.HandleFiles(files);
			foreach (var c in companies)
			{
				var ids = c.Files.Select(f => f.id).ToList();
				var file_ids = c.Files.Where(f => !string.IsNullOrEmpty(f.file_id)).Select(f => f.file_id).ToList();
				c.Files = files.Where(f => ids.Contains(f.id) || file_ids.Contains(f.file_id)).ToList();
				//unitOfWork.CompanyRepository.Update(c);
				companyDAL.UpdateFiles(c, File_type.Certificate);
			}
			//unitOfWork.Save();
			var types = fileTypeDal.GetAll();
			companyService.ProcessFileUrls(companies);
			return companies;

		}

		[Route("removeFile")]
		[HttpDelete]
		public void removeFile(int companyId, int fileId)
		{
			companyDAL.RemoveFile(companyId, fileId);
		}

		[Route("getByIds")]
		[HttpGet]
		public object GetByIds(string ids)
		{
			var idList = company.Common.Utilities.GetIdsFromString(ids);
			return unitOfWork.CompanyRepository.Get(c => idList.Contains(c.user_id)).ToList();
		}

		[Route("getFactoriesForUser")]
		[HttpGet]
		public List<Company> GetFactoriesForUser()
		{
			var user = accountService.GetCurrentUser();
			if (user != null)
			{
				return companyDAL.GetCompaniesForUser(user.userid).OrderBy(c => c.factory_code).ToList();
			}
			return null;
		}

		[Route("getFactoriesForClient")]
		[HttpGet]
		public List<Company> GetFactoriesForClient(int clientId)
		{
			var clientIds = new List<int>();
			if (clientId != -1)
				clientIds.Add(clientId);
			else
			{
				//Brands
				clientIds.AddRange(brandsDAL.GetAll().Where(b => b.user_id != null).Select(b => b.user_id.Value));
			}
			return companyDAL.GetFactoriesForClients(clientIds).Where(f => !string.IsNullOrEmpty(f.factory_code))
				.OrderBy(f => f.factory_code).ToList();

		}

	}
}
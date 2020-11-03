using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using erp.Model;
using erp.DAL.EF.New;
using erp.Model.Dal.New;
using backend.ApiServices;

namespace backend.Controllers
{
	[Authorize(Roles = "Administrator")]
	[RoutePrefix("api/file")]
	public class FileApiController : ApiController
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IFileTypeDal fileTypeDal;
		private readonly IUploadedFileHandler uploadedFileHandler;
		private readonly IFileService fileService;

		public FileApiController(IUnitOfWork unitOfWork, IFileTypeDal fileTypeDal,
			IUploadedFileHandler uploadedFileHandler, IFileService fileService)
		{
			this.unitOfWork = unitOfWork;
			this.fileTypeDal = fileTypeDal;
			this.uploadedFileHandler = uploadedFileHandler;
			this.fileService = fileService;
		}

		[Route("getFiles")]
		[HttpGet]
		public object getFiles(int? type_id = null)
		{
			var fileTypes = fileTypeDal.GetAll();
			return unitOfWork.FileRepository.Get(f => type_id == null || f.type_id == type_id).
			Select(f => GetUIObject(fileTypes, f));
		}

		[Route("getFilesForCompanies")]
		[HttpGet]
		public object getFilesForCompanies(int? type_id = null)
		{
			var fileTypes = fileTypeDal.GetAll();
			return unitOfWork.FileRepository.Get(f => (type_id == null || f.type_id == type_id) && f.Companies.Any(), includeProperties: "Companies").
			Select(f => GetUIObject(fileTypes, f));
		}

		[Route("getFilesForMastProducts")]
		[HttpGet]
		public object getFilesForMastProducts(int? type_id = null)
		{
			var fileTypes = fileTypeDal.GetAll();
			return unitOfWork.FileRepository.Get(f => (type_id == null || f.type_id == type_id) && f.MastProducts.Any(), includeProperties: "MastProducts").
			Select(f => GetUIObject(fileTypes, f));
		}

		[Route("delete")]
		[HttpDelete]
		public void deleteFile(int id)
		{
			unitOfWork.FileRepository.DeleteByIds(new[] { id });
		}

		[Route("update")]
		[HttpPut]
		public void updateFile(File file)
		{
			unitOfWork.FileRepository.Update(file);
			unitOfWork.Save();
		}

		private object GetUIObject(List<File_type> fileTypes, File f)
		{
			return new
			{
				f.id,
				f.name,
				f.description,
				f.type_id,
				url = fileService.GetFileUrl(fileTypes, f)
			};
		}

	}

	public class FileComparer : IEqualityComparer<File>
	{
		public bool Equals(File x, File y)
		{
			return x.id == y.id || x.file_id == y.file_id;
		}

		public int GetHashCode(File obj)
		{
			return obj.id > 0 ? obj.id : obj.file_id.Sum(c => c);
		}
	}
}
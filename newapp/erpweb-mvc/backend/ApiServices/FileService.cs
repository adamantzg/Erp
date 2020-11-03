using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model.Dal.New;
using erp.DAL.EF.New;
using erp.Model;
using System.Web.Hosting;

namespace backend.ApiServices
{
	public class FileService : IFileService
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IFileTypeDal fileTypeDal;
		private readonly IUploadedFileHandler uploadedFileHandler;

		public FileService(IUnitOfWork unitOfWork, IFileTypeDal fileTypeDal, IUploadedFileHandler uploadedFileHandler)
		{
			this.unitOfWork = unitOfWork;
			this.fileTypeDal = fileTypeDal;
			this.uploadedFileHandler = uploadedFileHandler;
		}

		public void HandleFiles(IList<File> files)
		{
			var types = fileTypeDal.GetAll();
			foreach (var file in files)
			{
				if (!string.IsNullOrEmpty(file.file_id))
				{
					var data = uploadedFileHandler.GetTempFile(file.file_id);
					if (data != null)
					{
						var path = types.FirstOrDefault(t=>t.id == file.type_id)?.path ?? "";
						file.name = System.IO.Path.GetFileName(
							uploadedFileHandler.WriteFile(file.name, 
							HostingEnvironment.MapPath(System.IO.Path.Combine(Properties.Settings.Default.imagesRootFolder, path)), data));
					}
				}
				if(file.id <= 0)
				{
					unitOfWork.FileRepository.Insert(file);
				}
			}
			unitOfWork.Save();
		
		}

		public string GetFileUrl(IList<File_type> fileTypes, File f)
		{
			return WebUtilities.CombineUrls(Properties.Settings.Default.imagesRootFolder, fileTypes?.FirstOrDefault(t=>t.id == f.type_id)?.path ?? "",f.name);
		}
	}
}
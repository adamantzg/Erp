using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class FileTypeDal : GenericDal<File_type>, IFileTypeDal
	{
		public FileTypeDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM file_type";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM file_type WHERE id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO file_type (id,name,path,previewpath) VALUES(@id,@name,@path,@previewpath)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE file_type SET name = @name,path = @path,previewpath = @previewpath WHERE id = @id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM file_type WHERE id = @id";
		}

		protected override bool IsAutoKey => false;
	}
}

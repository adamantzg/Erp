
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class ReturnsImagesDAL : GenericDal<Returns_images>, IReturnsImagesDAL
    {
		public ReturnsImagesDAL(IDbConnection conn) : base(conn)
		{
		}

		public List<Returns_images> GetByReturn(int return_id, int file_category=0)
		{
			var filterFileCat = file_category > 0 ? " AND file_category = @file_category":"";
			return conn.Query<Returns_images>(
				$"SELECT * FROM returns_images WHERE return_id = @return_id{filterFileCat}",
				new {return_id, file_category}).ToList();
		}

		protected override string IdField => "image_unique";
		
		protected override string GetAllSql()
		{
			return "SELECT * FROM returns_images";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO returns_images (return_id,return_image,user_type,cc_use,added_by,added_date,file_category) 
				VALUES(@return_id,@return_image,@user_type,@cc_use,@added_by,@added_date,@file_category)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE returns_images SET return_id = @return_id,return_image = @return_image,user_type = @user_type,
				cc_use = @cc_use,added_by = @added_by,added_date = @added_date WHERE image_unique = @image_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM returns_images WHERE image_unique = @id";
		}
	}
}
			
			
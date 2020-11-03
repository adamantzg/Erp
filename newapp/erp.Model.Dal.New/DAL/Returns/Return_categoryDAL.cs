
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class ReturnCategoryDAL : GenericDal<Return_category>, IReturnCategoryDAL
	{
		public ReturnCategoryDAL(IDbConnection conn) : base(conn)
		{
		}
		
		protected override string IdField => "returncategory_id";
		protected override bool IsAutoKey => false;

		protected override string GetAllSql()
		{
			return "SELECT * FROM return_category";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM return_category WHERE returncategory_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO return_category (returncategory_id,category_name,category_code) 
				VALUES(@returncategory_id,@category_name,@category_code)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE return_category SET category_name = @category_name,category_code = @category_code 
					WHERE returncategory_id = @returncategory_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM return_category WHERE returncategory_id = @id";
		}
	}
}
			
			
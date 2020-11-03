
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
    public class AdminPagesDAL : GenericDal<Admin_pages>, IAdminPagesDAL
    {
		public AdminPagesDAL(IDbConnection conn) : base(conn)
		{
		}

		public List<Admin_pages> GetForUser(int user_id)
		{
			return conn.Query<Admin_pages>(
				@"SELECT admin_pages.* FROM admin_pages INNER JOIN user_pages ON admin_pages.page_id = user_pages.page_id WHERE userid = @user_id",
				new {user_id}).ToList();
        }
		
		protected override string GetAllSql()
		{
			return "SELECT * FROM admin_pages";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM admin_pages WHERE page_id = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO admin_pages (page_id,page_type,top_level,sub_level,sub_sub_level,notes,page_URL,parameter1,parameter1_value,URL_value) 
			VALUES(@page_id,@page_type,@top_level,@sub_level,@sub_sub_level,@notes,@page_URL,@parameter1,@parameter1_value,@URL_value)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE admin_pages SET page_type = @page_type,top_level = @top_level,sub_level = @sub_level,sub_sub_level = @sub_sub_level,
				notes = @notes,page_URL = @page_URL,parameter1 = @parameter1,parameter1_value = @parameter1_value,URL_value = @URL_value 
				WHERE page_id = @page_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM admin_pages WHERE page_id = @id";
		}

		protected override bool IsAutoKey => false;
		protected override string IdField => "page_id";
	}
}
			
			

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using erp.Model.Dal.New;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class AdminPagesNewDAL : GenericDal<Admin_pages_new>, IAdminPagesNewDAL
    {
		protected override string GetAllSql()
		{
			return "SELECT * FROM admin_pages_new";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM admin_pages_new WHERE page_id = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO admin_pages_new (page_id,parent_id,page_title,page_type,notes,page_URL,parameter1,parameter1_value,URL_value,hide_menu,path) 
				VALUES(@page_id,@parent_id,@page_title,@page_type,@notes,@page_URL,@parameter1,@parameter1_value,@URL_value,@hide_menu,@path)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE admin_pages_new SET parent_id = @parent_id,page_title = @page_title,page_type = @page_type,notes = @notes,page_URL = @page_URL,
				parameter1 = @parameter1,parameter1_value = @parameter1_value,URL_value = @URL_value,hide_menu = @hide_menu,path = @path 
				WHERE page_id = @page_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM admin_pages_new WHERE page_id = @id";
		}

		protected override bool IsAutoKey => false;
		protected override string IdField => "page_id";

		public List<Admin_pages_new> GetForUser(int user_id)
		{
			return conn.Query<Admin_pages_new>(
				@"SELECT admin_pages_new.* FROM admin_pages_new INNER JOIN user_pages ON admin_pages_new.page_id = user_pages.page_id 
					WHERE userid = @user_id ORDER BY admin_pages_new.page_id",
				new {user_id}).ToList();
        }

	    public void GetParent(List<Admin_pages_new> list, int? parent_id)
        {
            if (parent_id == null) return;
	        var page = conn
		        .QueryFirstOrDefault("SELECT admin_pages_new.* FROM admin_pages_new WHERE page_id = @parent_id",
			        new {parent_id}).ToList();
            list.Add(page);
	        GetParent(list, page.parent_id);
        }
		
		public AdminPagesNewDAL(IDbConnection conn) : base(conn)
		{
		}
	}
}
			
			
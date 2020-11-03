
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
    public class ClientPagesAllocatedDAL : GenericDal<Client_pages_allocated>, IClientPagesAllocatedDAL
    {
		protected override string GetAllSql()
		{
			return "SELECT * FROM client_pages_allocated";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM client_pages_allocated WHERE userpage_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO client_pages_allocated (userid,page_id,option1) VALUES(@userid,@page_id,@option1)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE client_pages_allocated SET userid = @userid,page_id = @page_id,option1 = @option1 WHERE userpage_id = @userpage_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM client_pages_allocated WHERE userpage_id = @id";
		}

		protected override string IdField => "userpage_id";

		public List<Client_pages_allocated> GetByPageAndUser(int user_id, string page_Url = "")
		{
			return conn.Query<Client_page, Client_pages_allocated, Client_pages_allocated>(
				@"SELECT client_pages.*, client_pages_allocated.*                    
                FROM  client_pages_allocated 
                    INNER JOIN client_pages ON client_pages_allocated.page_id = client_pages.page_id
                WHERE (page_URL = @page_Url OR @page_Url = '') AND userid = @user_id",
				(p, pa) =>
				{
					pa.Page = p;
					return pa;
				}, new {page_Url, user_id}, splitOn: "userpage_id").ToList();
            
        }
		
		public ClientPagesAllocatedDAL(IDbConnection conn) : base(conn)
		{
		}
	}
}
			
			

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class ClientPagesDAL : GenericDal<Client_page>
	{
		protected override string GetAllSql()
		{
			return "SELECT * FROM client_pages";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM client_pages WHERE page_id = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO client_pages (page_id,page_type,top_level,sub_level,sub_sub_level,notes,page_URL,parameter1,parameter1_value,URL_value) 
				VALUES(@page_id,@page_type,@top_level,@sub_level,@sub_sub_level,@notes,@page_URL,@parameter1,@parameter1_value,@URL_value)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE client_pages SET page_type = @page_type,top_level = @top_level,sub_level = @sub_level,sub_sub_level = @sub_sub_level,notes = @notes,page_URL = @page_URL,parameter1 = @parameter1,parameter1_value = @parameter1_value,URL_value = @URL_value WHERE page_id = @page_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM client_pages WHERE page_id = @id";
		}

		protected override bool IsAutoKey => false;
		protected override string IdField => "page_id";
		
		public ClientPagesDAL(IDbConnection conn) : base(conn)
		{
		}
	}
}
			
			
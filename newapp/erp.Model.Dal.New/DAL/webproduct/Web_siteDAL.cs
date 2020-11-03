
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class WebSiteDal : IWebSiteDal
    {
	    private MySqlConnection conn;

	    public WebSiteDal(IDbConnection conn)
	    {
		    this.conn = (MySqlConnection) conn;
	    }

		public List<Web_site> GetAll()
		{
			return conn.Query<Web_site>("SELECT * FROM web_site").ToList();
		}
		
		
		public Web_site GetById(int id)
		{
			return conn.QueryFirst<Web_site>("SELECT * FROM web_site WHERE id = @id", new {id});
		}

        public Web_site GetByBrandId(int id)
        {
	        return conn.QueryFirst<Web_site>("SELECT * FROM web_site WHERE brand_id = @id", new {id});
        }

        public Web_site GetByCode(string code)
        {
	        return conn.QueryFirst<Web_site>("SELECT * FROM web_site WHERE code = @code", new {code});
        }
		
	
		public void Create(Web_site o)
        {
            string insertsql = @"INSERT INTO web_site (name,code,brand_id,url) VALUES(@name,@code,@brand_id,@url)";
	        conn.Execute(insertsql, o);
	        o.id = conn.ExecuteScalar<int>("SELECT last_insert_id()");
        }
		
		
		public void Update(Web_site o)
		{
			string updatesql = @"UPDATE web_site SET name = @name,code = @code,brand_id = @brand_id,url = @url WHERE id = @id";
			conn.Execute(updatesql, o);
		}
		
		public void Delete(int id)
		{
			conn.Execute("DELETE FROM web_site WHERE id = @id", new {id});
		}
		
	}
}
			
			
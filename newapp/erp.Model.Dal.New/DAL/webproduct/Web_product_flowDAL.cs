
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
    public class WebProductFlowDal : IWebProductFlowDal
    {
	    private MySqlConnection conn;
	
		public WebProductFlowDal(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}

		public List<Web_product_flow> GetAll()
		{
			return conn.Query<Web_product_flow>("SELECT * FROM web_product_flow").ToList();
		}

        public List<Web_product_flow> GetForProduct(int web_unique, IDbConnection conn = null)
        {
	        return (conn ?? this.conn).Query<Web_product_flow>("SELECT * FROM web_product_flow WHERE web_unique = @web_unique",
		        new {web_unique}).ToList();
        }
		
		
		public Web_product_flow GetById(int id)
		{
			return conn.QueryFirstOrDefault<Web_product_flow>("SELECT * FROM web_product_flow WHERE id = @id", new {id});
		}
		
		public void Create(Web_product_flow o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_product_flow (web_unique,part_id,pressure_id,value) VALUES(@web_unique,@part_id,@pressure_id,@value)";
	        conn.Execute(insertsql, o, tr);
        }
		
		
		public void Update(Web_product_flow o, IDbTransaction tr = null)
		{
			string updatesql = @"UPDATE web_product_flow SET web_unique = @web_unique,part_id = @part_id,pressure_id = @pressure_id,value = @value WHERE id = @id";
			conn.Execute(updatesql, o, tr);
		}
		
		public void Delete(int id,IDbTransaction tr = null)
		{
			conn.Execute("DELETE FROM web_product_flow WHERE id = @id", new {id}, tr);
		}

        public void DeleteMissing(int web_unique, IList<int> ids = null, IDbTransaction tr = null)
        {
	        conn.Execute(
		        $"DELETE FROM web_product_flow WHERE web_unique = @web_unique {(ids != null && ids.Count > 0 ? " AND id NOT IN @ids" : "")}",
		        new {web_unique, ids}, tr);
        }
		
		
	}
}
			
			
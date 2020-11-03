
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ContinentsDAL : IContinentsDAL
    {
		private MySqlConnection conn;

		public ContinentsDAL(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}
	
		public List<Continents> GetAll(bool all = false)
		{
			return conn.Query<Continents>($"SELECT * FROM continents {(all ? "" : "WHERE status=1")}").ToList();			
		}
		

		
		public Continents GetById(int id)
		{
			return conn.QueryFirstOrDefault<Continents>("SELECT * FROM continents WHERE id = @id", new {id});			
		}
        public Continents GetByCode(string code)
        {
            return conn.QueryFirstOrDefault<Continents>("SELECT * FROM continents WHERE code = @code", new {code});
        }

		
		public void Create(Continents o)
        {
            string insertsql = @"INSERT INTO continents (name,status,code) VALUES(@name,@status,@code)";

			conn.Execute(insertsql, o);
			o.id = conn.ExecuteScalar<int>("SELECT LAST_INSERT_ID()");
		}
						
		public void Update(Continents o)
		{
            string updatesql = @"UPDATE continents SET name=@name,status=@status,code=@code  WHERE id=@id";

			conn.Execute(updatesql, o);
		}
		
		public  void Delete(int country_id)
		{
			conn.Execute("DELETE FROM continents WHERE id = @country_id", new { country_id});
		}

        public List<Continents> GetForBrand(int brandid)
        {
			return conn.Query<Continents>(@"SELECT c.* FROM continents AS C INNER JOIN dealers AS d ON d.user_country=c.ISO2 
                                INNER JOIN dealer_brandstatus AS db ON db.dealer_id=d.user_id
                                INNER JOIN brands AS b ON b.brand_id=db.brand_id
                                WHERE d.user_country IS NOT NULL AND b.brand_id=@brandid ORDER BY db.brand_status ASC, c.country_id ASC",
								new {brandid }).ToList();
            
        }
	}
}
			
			
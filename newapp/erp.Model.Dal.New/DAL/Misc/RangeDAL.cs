
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class RangeDAL : IRangeDAL
    {
		private MySqlConnection conn;

		private ICompanyDAL companyDal;

		public RangeDAL(IDbConnection conn, ICompanyDAL companyDal)
		{
			this.conn = (MySqlConnection) conn;
			this.companyDal = companyDal;
		}
	
		public List<Range> GetAll()
		{
			return conn.Query<Range>("SELECT * FROM ranges").ToList();			
		}		
		
		public Range GetById(int id)
		{
			return conn.QueryFirstOrDefault<Range>("SELECT * FROM ranges WHERE rangeid = @id", new {id});			
		}

        public List<Range> GetByCompanyId(int id, bool combined = true)
        {
	        var company = companyDal.GetById(id);
	        if (company != null)
	        {
		        return conn.Query<Range>($@"SELECT ranges.* FROM ranges WHERE LENGTH(TRIM(range_desc)) > 0 AND  rangeid 
                                            IN (SELECT cprod_range FROM cust_products INNER JOIN users ON cust_products.cprod_user = users.user_id 
                                            WHERE {(combined ? "users.combined_factory = @id" : "users.user_id = @id")})",
			        new {id}).ToList();
	        }
	        return null;            
        }	
				
		public void Create(Range o)
        {
            string insertsql = @"INSERT INTO ranges (rangeid,range_name,range_desc,range_image,category1,forecast_percentage,user_id) VALUES(@rangeid,@range_name,@range_desc,@range_image,@category1,@forecast_percentage,@user_id)";
		    o.rangeid = conn.ExecuteScalar<int>("SELECT MAX(rangeid)+1 FROM ranges");
			conn.Execute(insertsql, o);           
		}		
				
		public void Update(Range o)
		{
			string updatesql = @"UPDATE ranges SET range_name = @range_name,range_desc = @range_desc,range_image = @range_image,category1 = @category1,forecast_percentage = @forecast_percentage,user_id = @user_id WHERE rangeid = @rangeid";
			conn.Execute(updatesql, o);
		}
		
		public void Delete(int rangeid)
		{
			conn.Execute("DELETE FROM ranges WHERE rangeid = @rangeid", new {rangeid});			
		}
	}
}
			
			
			
			
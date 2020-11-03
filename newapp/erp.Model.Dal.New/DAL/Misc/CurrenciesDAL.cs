
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
    public class CurrenciesDAL : ICurrenciesDAL
    {
	    private MySqlConnection conn;

	    public CurrenciesDAL(IDbConnection conn)
	    {
		    this.conn = (MySqlConnection) conn;
	    }
	
		public List<Currencies> GetAll()
		{
			return conn.Query<Currencies>("SELECT * FROM currencies").ToList();
		}
		
		public Currencies GetById(int id)
		{
			return conn.QueryFirst<Currencies>("SELECT * FROM currencies WHERE curr_code = @id", new {id});
		}
		
		public void Create(Currencies o)
        {
            string insertsql = @"INSERT INTO currencies (curr_desc,curr_symbol,curr_exch1,curr_exch2,curr_exch3) VALUES(@curr_desc,@curr_symbol,@curr_exch1,@curr_exch2,@curr_exch3)";

	        conn.Execute(insertsql, o);
			o.curr_code = conn.ExecuteScalar<int>("SELECT curr_code FROM currencies WHERE curr_code = LAST_INSERT_ID()");
           
		}
		
		public void Update(Currencies o)
		{
			string updatesql = @"UPDATE currencies SET curr_desc = @curr_desc,curr_symbol = @curr_symbol,curr_exch1 = @curr_exch1,curr_exch2 = @curr_exch2,curr_exch3 = @curr_exch3 WHERE curr_code = @curr_code";
			conn.Execute(updatesql, o);
		}
		
		public  void Delete(int curr_code)
		{
			conn.Execute("DELETE FROM currencies WHERE curr_code = @curr_code", new {curr_code});
		}
	}
}
			
			
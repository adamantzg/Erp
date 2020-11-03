
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
    public class PaymentDetailsDAL : GenericDal<Payment_details>, IPaymentDetailsDAL
    {
		public PaymentDetailsDAL(IDbConnection conn) : base(conn)
		{
		}

		protected override string IdField => "payment_details_id";

		

        public List<Payment_details> GetForCompany(int company_id)
        {
	        return conn.Query<Payment_details>("SELECT * FROM payment_details WHERE company_id = @company_id",
		        new {company_id}).ToList();
        }
		

		protected override string GetAllSql()
		{
			return "SELECT * FROM payment_details";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM payment_details WHERE payment_details_id = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO payment_details (bank_name,address,sort_code,beneficiary_name,beneficiary_accnumber,company_id) 
				VALUES(@bank_name,@address,@sort_code,@beneficiary_name,@beneficiary_accnumber,@company_id)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE payment_details SET bank_name = @bank_name,address = @address,sort_code = @sort_code,
			beneficiary_name = @beneficiary_name,beneficiary_accnumber = @beneficiary_accnumber,company_id = @company_id 
			WHERE payment_details_id = @payment_details_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM payment_details WHERE payment_details_id = @id";
		}
	}
}
			
			
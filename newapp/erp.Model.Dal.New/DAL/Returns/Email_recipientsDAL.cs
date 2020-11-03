
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using Dapper;

namespace erp.Model.Dal.New
{
    public class EmailRecipientsDAL : GenericDal<Email_recipients>, IEmailRecipientsDAL
	{
		public EmailRecipientsDAL(IDbConnection conn) : base(conn)
		{
		}

		public List<Email_recipients> GetByCriteria(int company_id,string area = null,object param1 = null, object param2 = null)
        {
			return conn.Query<Email_recipients>(
				@"SELECT * FROM email_recipients 
				WHERE company_id = @company_id AND (area = @area OR @area IS NULL) 
				AND (param1 = @param1 OR @param1 IS NULL) 
				AND (param2 = @param2 OR @param2 IS NULL)", new {company_id, area, param1, param2}
				).ToList();
            
        }

		protected override string GetAllSql()
		{
			return "SELECT * FROM email_recipients";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM email_recipients WHERE id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO email_recipients (company_id,area,to,cc,bcc,param1,param2) 
			VALUES(@company_id,@area,@to,@cc,@bcc,@param1,@param2)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE email_recipients SET company_id = @company_id,area = @area,to = @to,cc = @cc,bcc = @bcc,param1 = @param1, param2 = @param2 WHERE id = @id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM email_recipients WHERE id = @id";
		}
	}
}
			
			
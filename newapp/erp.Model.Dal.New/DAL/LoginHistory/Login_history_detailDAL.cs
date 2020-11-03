
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
    public class LoginHistoryDetailDAL : GenericDal<Login_history_detail>, ILoginHistoryDetailDAL
    {
		protected override string GetAllSql()
		{
			return "SELECT * FROM login_history_detail";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM login_history_detail WHERE detail_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO login_history_detail (history_id,visit_page,visit_URL,visit_time,cprod_id) VALUES(@history_id,@visit_page,
					@visit_URL,@visit_time,@cprod_id)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE login_history_detail SET history_id = @history_id,visit_page = @visit_page,visit_URL = @visit_URL,
				visit_time = @visit_time,cprod_id = @cprod_id WHERE detail_unique = @detail_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM login_history_detail WHERE detail_unique = @id";
		}

		protected override string IdField => "detail_unique";

	    public List<login_history_page_count> GetPageCount(IList<int> adminTypesToInclude = null,
		    IList<int> adminTypesToExclude = null, DateTime? dateFrom = null, DateTime? dateTo = null,
		    bool groupByPageType = false, IList<int> companyIds = null, string excludedCountries = null)
	    {
		    var exCountriesList = excludedCountries != null
			    ? company.Common.Utilities.GetQuotedStringsFromString(excludedCountries)
			    : null;
		    var adminTypes = adminTypesToExclude ?? adminTypesToInclude;
		    
		    return conn.Query<login_history_page_count>(
			    $@"SELECT login_history.login_username as Username, 
					{(groupByPageType ? "login_history_page.page_type" : "login_history_detail.visit_page AS Url")}, COUNT(*) AS count
                    FROM `login_history_detail` INNER JOIN login_history ON login_history_detail.history_id = login_history.history_id 
                    INNER JOIN userusers ON login_history.login_username = userusers.userusername 
					{(groupByPageType ? " LEFT OUTER JOIN login_history_page ON login_history_detail.visit_page = login_history_page.page_url" : "")}
                    WHERE 
                    (login_history.login_date >= @dateFrom OR @dateFrom IS NULL) AND
                     (login_history.login_date <= @dateTo OR @dateTo IS NULL) 
					{(adminTypesToInclude != null || adminTypesToExclude != null ? 
					  $" AND userusers.admin_type {(adminTypesToExclude != null ? "NOT" : "")} IN @adminTypes" : "")}
				    {(companyIds != null && companyIds.Count > 0 ? " AND login_history.user_id IN @companyIds" : "")}
					{(exCountriesList != null ? $" AND (login_history.login_country NOT IN {string.Join(",",exCountriesList)})" : "")}
                    GROUP BY login_history.login_username, {(groupByPageType ? "login_history_page.page_type" : "login_history_detail.visit_page")}
                    ", new {dateFrom, dateTo, companyIds, adminTypes}).ToList();

        }


		public LoginHistoryDetailDAL(IDbConnection conn) : base(conn)
		{
		}
	}
}
			
			
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
    public class LoginhistoryDAL : GenericDal<Login_history>, ILoginhistoryDAL
    {
	    private IUserDAL userDal;

	    public LoginhistoryDAL(IDbConnection conn, IUserDAL userDal) : base(conn)
	    {
		    this.userDal = userDal;
	    }

        public Login_history GetByCriteria(string session_id, DateTime? login_date)
        {
	        return conn.QueryFirstOrDefault<Login_history>(
		        @"SELECT * FROM login_history WHERE (session_id = @session_id OR @session_id IS NULL) 
					AND ( DATE_ADD(@login_date, INTERVAL '24' HOUR) > NOW() OR @login_date IS NULL )",
		        new {session_id, login_date});
            
        }

        public List<Login_history> GetByCriteria(IList<int> company_ids, DateTime? dateFrom, DateTime? dateTo, IList<int> adminTypesToInclude = null, 
	        IList<int> adminTypesToExclude = null, bool showAllActiveUsers = false,IList<User> activeUsers = null, 
	        string companiesAdminTypesMappings = "", string excludedCountries = null)
        {
            var result = new List<Login_history>();
	        
            if (activeUsers == null || activeUsers.Count == 0)
            {
                activeUsers = userDal.GetAll().Where(u => u.status_flag == 0).ToList();
            }
	        
            var dictCompaniesAdminTypes = new Dictionary<int?, int>();
            if (!string.IsNullOrEmpty(companiesAdminTypesMappings))
            {
                var pairs = companiesAdminTypesMappings.Split(';');
                foreach(var pair in pairs)
                {
                    var parts = pair.Split('|');
                    dictCompaniesAdminTypes[Convert.ToInt32(parts[0])] = Convert.ToInt32(parts[1]);
                }
            }

            var exCountriesList = excludedCountries != null ? company.Common.Utilities.GetQuotedStringsFromString(excludedCountries) : null;
            
            var criteria = new List<string>();
            var sql = $@"SELECT users.*, login_history.* FROM login_history INNER JOIN users ON login_history.user_id = users.user_id";
            if (company_ids != null && company_ids.Count > 0)
                criteria.Add($"login_history.user_id IN @company_ids");
            if (exCountriesList != null)
                criteria.Add($"login_history.login_country NOT IN ({string.Join(",",exCountriesList)})");
			
            criteria.Add("(login_date >= @dateFrom OR @dateFrom IS NULL) AND (login_date <= @dateTo OR @dateTo IS NULL)");
            sql += " WHERE " + string.Join(" AND ", criteria);

	        var data = conn.Query<Company, Login_history, Login_history>(
		        sql,
		        (c, l) =>
		        {
			        l.Company = c;
			        return l;
		        }, new {dateFrom, dateTo, company_ids}, splitOn: "history_id").ToList();

	        foreach (var r in data)
	        {
		        var add = true;
		        r.User = activeUsers.FirstOrDefault(u => u.username == r.login_username);
		        if(dictCompaniesAdminTypes.Count > 0)
			        add = !dictCompaniesAdminTypes.ContainsKey(r.user_id) || (r.User != null && dictCompaniesAdminTypes[r.user_id] == r.User.admin_type);
		        if (adminTypesToInclude != null)
			        add = r.User != null && adminTypesToInclude.Contains(r.User.admin_type ?? 0);
                    
		        if (adminTypesToExclude != null)
			        add = r.User == null || !adminTypesToExclude.Contains(r.User.admin_type ?? 0);
		        if(add)
			        result.Add(r);
	        }

	        return result;

        }

		public List<Company> GetCompanies()
		{
			return conn.Query<Company>(
				@"SELECT DISTINCT users.user_id, user_name, customer_code 
				FROM login_history INNER JOIN users ON login_history.user_id = users.user_id ORDER BY user_name").ToList();
		}

	    protected override string GetAllSql()
	    {
		    return @"SELECT * FROM login_history";
	    }

	    protected override string GetByIdSql()
	    {
		    throw new NotImplementedException();
	    }

	    protected override string GetCreateSql()
	    {
		    return @"INSERT INTO login_history (user_id,login_date,login_username,login_country,website,ip_address,pwd) 
				VALUES(@user_id,@login_date,@login_username,@login_country,@website,@ip_address,@pwd)";
	    }

	    protected override string GetUpdateSql()
	    {
		    return
			    @"UPDATE login_history SET user_id = @user_id,login_date = @login_date,login_username = @login_username,login_country = @login_country,
			website = @website,ip_address = @ip_address,pwd = @pwd,session_id = @session_id WHERE history_id = @history_id";
	    }

	    protected override string GetDeleteSql()
	    {
		    return "DELETE FROM login_history WHERE history_id = @id";
	    }

	    protected override string IdField => "history_id";

	    public override List<Login_history> GetAll()
	    {
		    return conn.Query<Login_history>(GetAllSql() + " WHERE login_date >= @date",
			    new {date = DateTime.Now.AddYears(-1)}).ToList();
            
        }

        public List<Login_history> GetForWebsite(string website, DateTime? dateFrom, DateTime? dateTo)
        {
	        return conn.Query<Login_history>(
		        @"SELECT * FROM login_history WHERE (login_date >= @dateFrom OR @dateFrom IS NULL) AND
                                             (login_date <= @dateTo OR @dateTo IS NULL) AND login_history.website=@website",
		        new {dateFrom, dateTo, website = website.ToUpper()}).ToList();
        }

        
        public override void Create(Login_history o, IDbTransaction tr = null)
        {
            o.session_id = Guid.NewGuid().ToString();

	        base.Create(o, tr);

        }

		public Dictionary<string, DateTime?> GetLastLoginDates(IList<int> company_ids, DateTime? from)
		{
			return conn.Query<LastLoginDictEntry>(@"
				SELECT login_username, MAX(login_date) lastlogin FROM login_history 
				WHERE user_id IN @company_ids 
				AND (login_date >= @from OR @from IS NULL) 
				GROUP BY login_username", new { company_ids, from }).ToDictionary(d=>d.login_username, d=>d.lastlogin);
		}
	}

	public class LastLoginDictEntry
	{
		public string login_username { get; set;}
		public DateTime? lastlogin { get; set; }
	}
}

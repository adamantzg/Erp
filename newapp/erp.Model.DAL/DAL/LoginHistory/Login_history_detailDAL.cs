
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Login_history_detailDAL
	{
	
		public static List<Login_history_detail> GetAll()
		{
			var result = new List<Login_history_detail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM login_history_detail", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Login_history_detail GetById(int id)
		{
			Login_history_detail result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM login_history_detail WHERE detail_unique = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
		
	
		public static Login_history_detail GetFromDataReader(MySqlDataReader dr)
		{
			Login_history_detail o = new Login_history_detail();
		
			o.detail_unique =  (int) dr["detail_unique"];
			o.history_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"history_id"));
			o.visit_page = string.Empty + Utilities.GetReaderField(dr,"visit_page");
			o.visit_URL = string.Empty + Utilities.GetReaderField(dr,"visit_URL");
			o.visit_time = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"visit_time"));
			o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cprod_id"));
			
			return o;

		}
		
		
		public static void Create(Login_history_detail o)
        {
            string insertsql = @"INSERT INTO login_history_detail (history_id,visit_page,visit_URL,visit_time,cprod_id) VALUES(@history_id,@visit_page,@visit_URL,@visit_time,@cprod_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = new MySqlCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT LAST_INSERT_ID()";
                o.detail_unique = Convert.ToInt32(cmd.ExecuteScalar());
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Login_history_detail o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@detail_unique", o.detail_unique);
			cmd.Parameters.AddWithValue("@history_id", o.history_id);
			cmd.Parameters.AddWithValue("@visit_page", o.visit_page);
			cmd.Parameters.AddWithValue("@visit_URL", o.visit_URL);
			cmd.Parameters.AddWithValue("@visit_time", o.visit_time);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
		}
		
		public static void Update(Login_history_detail o)
		{
			string updatesql = @"UPDATE login_history_detail SET history_id = @history_id,visit_page = @visit_page,visit_URL = @visit_URL,visit_time = @visit_time,cprod_id = @cprod_id WHERE detail_unique = @detail_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int detail_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM login_history_detail WHERE detail_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", detail_unique);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<login_history_page_count> GetPageCount(IList<int> adminTypesToInclude = null, IList<int> adminTypesToExclude = null, DateTime? dateFrom = null, DateTime? dateTo = null, 
            bool groupByPageType = false, IList<int> companyIds = null, string excludedCountries = null)
        {
            var exCountriesList = excludedCountries != null ? company.Common.Utilities.GetQuotedStringsFromString(excludedCountries) : null;

            var result = new List<login_history_page_count>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT login_history.login_username, {2} COUNT(*) AS count
                                            FROM `login_history_detail` INNER JOIN login_history ON login_history_detail.history_id = login_history.history_id 
                                            INNER JOIN userusers ON login_history.login_username = userusers.userusername {3}
                                            WHERE 
                                            (login_history.login_date >= @dateFrom OR @dateFrom IS NULL) AND
                                             (login_history.login_date <= @dateTo OR @dateTo IS NULL) {0} {4}
                                            GROUP BY login_history.login_username, {1}
                                            ", 
                                            adminTypesToInclude != null || adminTypesToExclude != null ? 
                                                        string.Format(" AND userusers.admin_type {1} IN ({0})", 
                                                            Utils.CreateParametersFromIdList(cmd, adminTypesToExclude ?? adminTypesToInclude,"types_"),
                                                            adminTypesToExclude != null ? "NOT" : "") : string.Empty ,
                                            groupByPageType ? "login_history_page.page_type" : "login_history_detail.visit_page",
                                            groupByPageType ? "login_history_page.page_type," : "login_history_detail.visit_page,",
                                            groupByPageType ? " LEFT OUTER JOIN login_history_page ON login_history_detail.visit_page = login_history_page.page_url" : "",
                                            companyIds != null && companyIds.Count > 0 ? string.Format(" AND (login_history.user_id IN ({0}))",
                                                        Utils.CreateParametersFromIdList(cmd, companyIds,"comp_")) : "",
                                            exCountriesList != null ? $" AND (login_history.login_country NOT IN {string.Join(",",exCountriesList)})" : ""
                                            );
                cmd.Parameters.AddWithValue("@dateFrom", dateFrom != null ? (object)dateFrom.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@dateTo", dateTo != null ? (object)dateTo.Value : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var r = new login_history_page_count
                    {
                        Username = string.Empty + dr["login_username"],
                        Count = Convert.ToInt32(dr["count"])
                    };
                    if (groupByPageType)
                        r.page_type = Utilities.FromDbValue<int>(dr["page_type"]);
                    else
                    {
                        r.Url = string.Empty + dr["visit_page"];
                    }
                    result.Add(r);
                    
                }
                dr.Close();
            }
            return result;
        }
		
		
	}
}
			
			
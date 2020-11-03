using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSUtil;
using LogQuery = MSUtil.LogQueryClassClass;
using LogRecordSet = MSUtil.ILogRecordset;
using LogRecord = MSUtil.ILogRecord;
using IISW3CInputFormat = MSUtil.COMW3CInputContextClassClass;

namespace LogParser
{
    public class Parser
    {
        public static List<UserGroupedResult> GetUserStats(string logPath, DateTime? from = null, DateTime? to = null,
            IList<string> userNames = null,string excludedExtensions = null, string excludedPages = null)
        {
            var result = new List<UserGroupedResult>();
            var whereList = new List<string>();
            
            if(from != null)
                whereList.Add(string.Format(" date >= '{0}'",from.Value.ToString("yyyy-MM-dd")));
            if (to != null)
                whereList.Add(string.Format(" date <= '{0}'", to.Value.ToString("yyyy-MM-dd")));
            if (userNames != null)
            {
                whereList.Add(string.Format(" cs-username IN ({0})",
                    string.Join(";", userNames.Select(l => "'" + l + "'"))));
            }
            if (excludedExtensions != null)
            {
                var list = excludedExtensions.ToLower().Split(',');
                //whereExclusions = string.Join(" AND ",
                //    list.Select(l => string.Format(" cs-uri-stem NOT LIKE ('%{0}')", l)));
                whereList.Add(string.Format(" TO_LOWERCASE(EXTRACT_EXTENSION(cs-uri-stem)) NOT IN ({0})",
                    string.Join(";", list.Select(l => "'" + l + "'"))));
            }

            if (excludedPages != null)
            {
                var list = excludedPages.Split(',');
                
                whereList.AddRange(list.Select(l=>string.Format(" cs-uri-stem NOT LIKE '%{0}%'",l)));
            }

            string query = string.Format("SELECT cs-username, COUNT(*) AS pages FROM {0} {1} GROUP BY cs-username", Path.Combine(logPath,"*.log"),
                whereList.Count > 0 ? " WHERE " + string.Join(" AND ",whereList) : "");

            var logQuery = new LogQueryClassClass();
            var recordSet = logQuery.Execute(query, new COMIISW3CInputContextClassClass());
            for (; !recordSet.atEnd(); recordSet.moveNext())
            {
                // Get the current record
                var logRecord = recordSet.getRecord();

                if (!logRecord.isNull(0))
                {
                    result.Add(new UserGroupedResult{UserName = string.Empty + logRecord.getValue("cs-username"),Count = Convert.ToInt32(logRecord.getValue("pages"))});
                }
            }
            return result;
        }

        public static List<UserPageResult> GetUserPageData(string userName, string logPath, DateTime? from = null,
            DateTime? to = null, string excludedExtensions = null, string excludedPages = null)
        {
            var result = new List<UserPageResult>();
            var whereList = new List<string>();
            whereList.Add(string.Format(" cs-username = '{0}'",userName));
            if (from != null)
                whereList.Add(string.Format(" date >= '{0}'", from.Value.ToString("yyyy-MM-dd")));
            if (to != null)
                whereList.Add(string.Format(" date <= '{0}'", to.Value.ToString("yyyy-MM-dd")));
            if (excludedExtensions != null)
            {
                var list = excludedExtensions.ToLower().Split(',');
                //whereExclusions = string.Join(" AND ",
                //    list.Select(l => string.Format(" cs-uri-stem NOT LIKE ('%{0}')", l)));
                whereList.Add(string.Format(" TO_LOWERCASE(EXTRACT_EXTENSION(cs-uri-stem)) NOT IN ({0})",
                    string.Join(";", list.Select(l => "'" + l + "'"))));
            }

            if (excludedPages != null)
            {
                var list = excludedPages.Split(',');
                
                whereList.AddRange(list.Select(l => string.Format(" cs-uri-stem NOT LIKE '%{0}%'", l)));
            }

            string query = string.Format("SELECT cs-uri-stem, COUNT(*) AS count FROM {0} {1} GROUP BY cs-uri-stem", Path.Combine(logPath, "*.log"),
                whereList.Count > 0 ? " WHERE " + string.Join(" AND ", whereList) : "");
            var logQuery = new LogQueryClassClass();
            var recordSet = logQuery.Execute(query, new COMIISW3CInputContextClassClass());
            for (; !recordSet.atEnd(); recordSet.moveNext())
            {
                // Get the current record
                var logRecord = recordSet.getRecord();

                if (!logRecord.isNull(0))
                {
                    result.Add(new UserPageResult { Page = string.Empty + logRecord.getValue("cs-uri-stem"), Count = Convert.ToInt32(logRecord.getValue("count")) });
                }
            }
            return result;
        }


		public static List<PageDetails> GetUserPageDetails(string userName, string page, string logPath, DateTime? from = null,
			DateTime? to = null, string excludedExtensions = null, string excludedPages = null)
		{
			var result = new List<PageDetails>();
			var whereList = new List<string>();
			whereList.Add(string.Format(" cs-username = '{0}'", userName));
			whereList.Add($" cs-uri-stem = '{page}' ");
			if (from != null)
				whereList.Add(string.Format(" date >= '{0}'", from.Value.ToString("yyyy-MM-dd")));
			if (to != null)
				whereList.Add(string.Format(" date <= '{0}'", to.Value.ToString("yyyy-MM-dd")));
			if (excludedExtensions != null)
			{
				var list = excludedExtensions.ToLower().Split(',');
				//whereExclusions = string.Join(" AND ",
				//    list.Select(l => string.Format(" cs-uri-stem NOT LIKE ('%{0}')", l)));
				whereList.Add(string.Format(" TO_LOWERCASE(EXTRACT_EXTENSION(cs-uri-stem)) NOT IN ({0})",
					string.Join(";", list.Select(l => "'" + l + "'"))));
			}

			if (excludedPages != null)
			{
				var list = excludedPages.Split(',');

				whereList.AddRange(list.Select(l => string.Format(" cs-uri-stem NOT LIKE '%{0}%'", l)));
			}

			string query = string.Format("SELECT cs-uri-stem, date, time FROM {0} {1} ORDER BY date, time", Path.Combine(logPath, "*.log"),
				whereList.Count > 0 ? " WHERE " + string.Join(" AND ", whereList) : "");
			var logQuery = new LogQueryClassClass();
			var recordSet = logQuery.Execute(query, new COMIISW3CInputContextClassClass());
			for (; !recordSet.atEnd(); recordSet.moveNext())
			{
				// Get the current record
				var logRecord = recordSet.getRecord();

				if (!logRecord.isNull(0))
				{
					var date = (logRecord.getValue("date") as DateTime?).Value.ToShortDateString();
					var time = (logRecord.getValue("time") as DateTime?).Value.ToShortTimeString();
					result.Add( new PageDetails { LogDateTime = DateTime.Parse(date + " " + time), Page = string.Empty + logRecord.getValue("cs-uri-stem") });
				}
			}
			return result;
		}


	}

    public class UserPageResult
    {
        public string Page { get; set; }
        public int Count { get; set; }
    }

    public class PageDetails
    {
        public DateTime LogDateTime { get; set; }
        public string Page { get; set; }
    }

    public class UserGroupedResult
    {
        public string UserName { get; set; }
        public int Count { get; set; }
    }
}

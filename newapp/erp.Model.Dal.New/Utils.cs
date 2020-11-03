using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace erp.Model.Dal.New
{
    public class Utils
    {
		public const string ExcludedCprodBrandCats = "115,207,307,608,708,912,8081";

        public static string CreateParametersFromIdList(MySqlCommand cmd, IList<int> ids, string paramPrefix = "id", int startIndex = 0)
        {
            var paramList = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                string paramName = "@" + paramPrefix + (i + startIndex + 1);
                paramList.Add(paramName);
                var prm = new MySqlParameter(paramName, ids[i]);
                cmd.Parameters.Add(prm);
            }
            return String.Join(",", paramList);
        }

        public static string CreateParametersFromIdList<T>(MySqlCommand cmd, IList<T> valueList, string paramPrefix = "id", int startIndex = 0)
        {
            var paramList = new List<string>();
            for (int i = 0; i < valueList.Count; i++)
            {
                string paramName = "@" + paramPrefix + (i + startIndex + 1);
                paramList.Add(paramName);
                var prm = new MySqlParameter(paramName, valueList[i]);
                cmd.Parameters.Add(prm);
            }
            return String.Join(",", paramList);
        }

        public static string CreateWhereClauseFromIdList<T>(MySqlCommand cmd,string field, IList<T> valueList,
                                                            string paramPrefix = "id", int startIndex = 0,bool negate = false)
        {
            return valueList != null && valueList.Count > 0
                       ? string.Format(" AND {0} {1} IN ({2})", field, negate ? " NOT " : string.Empty,
                       CreateParametersFromIdList(cmd, valueList, paramPrefix, startIndex)) : string.Empty;
        }

		

        
    }

    
}

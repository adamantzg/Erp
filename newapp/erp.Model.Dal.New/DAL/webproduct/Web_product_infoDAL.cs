
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class WebProductInfoDal : IWebProductInfoDal
    {
	    private MySqlConnection conn;

	    public WebProductInfoDal(IDbConnection conn)
	    {
		    this.conn = (MySqlConnection) conn;
	    }

		public List<Web_product_info> GetAll()
		{
			return conn.Query<Web_product_info>("SELECT * FROM web_product_info").ToList();
		}

        public List<Web_product_info> GetForProduct(int web_unique, IDbConnection conn = null,int? language_id = null)
        {
	        return (conn ?? this.conn).Query<Web_product_info>(
		        GetSelect(@"SELECT web_product_info.* {0} FROM web_product_info {1} 
					WHERE web_unique = @web_unique ORDER BY `order`",
			        language_id != null, commaBeforeFields: true, commaAfterFields: false),
		        new {web_unique, language_id}).ToList();
        }
		
		
		public Web_product_info GetById(int id)
		{
			return conn.QueryFirstOrDefault<Web_product_info>("SELECT * FROM web_product_info WHERE id = @id", new {id});
		}
		
		public void Create(Web_product_info o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_product_info (web_unique,type,value,`order`) VALUES(@web_unique,@type,@value,@order)";
	        conn.Execute(insertsql, o,tr);
        }
		
		public void Update(Web_product_info o,IDbTransaction tr = null)
		{
			string updatesql = @"UPDATE web_product_info SET web_unique = @web_unique,type = @type,value = @value, `order`= @order WHERE id = @id";

			conn.Execute(updatesql, o,tr);
		}
		
		public void Delete(int id, IDbTransaction tr = null)
		{
			conn.Execute("DELETE FROM web_product_info WHERE id = @id", new {id},tr);
		}

        public void DeleteAll()
        {
	        conn.Execute("DELETE FROM web_product_info");
        }

        public void DeleteMissing(int web_unique, IList<int> ids = null, IDbTransaction tr = null)
        {
	        conn.Execute(
		        $"DELETE FROM web_product_info WHERE web_unique = @web_unique {(ids != null && ids.Count > 0 ? " AND id NOT IN @ids" : "")}",
		        new {web_unique, ids});

        }

        private string GetSelect(string initialSql, bool localize = false, bool commaBeforeFields = false, bool commaAfterFields = true)
        {
            List<string> fields = new List<string>();
            string join = string.Empty;
            if (localize)
            {
                fields.Add(GetTranslationFields(false));
                join = GetTranslationJoin();
            }

            return string.Format(initialSql, (commaBeforeFields && fields.Count > 0 ? ", " : "") + string.Join(",", fields.ToArray()) + (commaAfterFields && fields.Count > 0 ? "," : ""), join);
        }

        private string GetTranslationJoin()
        {
            return @" LEFT OUTER JOIN web_product_info_translate ON (web_product_info.id = web_product_info_translate.info_id AND web_product_info_translate.language_id = @lang)";
        }

        private string GetTranslationFields(bool productOnly = true)
        {
            return @"web_product_info_translate.`value`";
        }
		
		
	}
}
			
			
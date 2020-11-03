
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{


	
	public class WebProductComponentDal : IWebProductComponentDal
	{
		private MySqlConnection conn;

		public WebProductComponentDal(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}

		public List<Web_product_component> GetAll()
		{
			return conn.Query<Web_product_component>("SELECT * FROM web_product_component").ToList();
		}
		
		
		public Web_product_component GetById(int id)
		{
			return conn.Query<Web_product_component>("SELECT * FROM web_product_component WHERE web_unique = @id",
				new {id}).FirstOrDefault();
		}
		
		
		public void Create(Web_product_component o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_product_component (web_unique,cprod_id,qty,order) VALUES(@web_unique,@cprod_id,@qty,@order)";
	        conn.Execute(insertsql, o, tr);
        }
		
		
		public void Update(Web_product_component o)
		{
			string updatesql = @"UPDATE web_product_component SET qty = @qty,order = @order WHERE cprod_id = @cprod_id AND web_unique = @web_unique";
			conn.Execute(updatesql, o);
		}
		
		public void Delete(int cprod_id,int web_unique)
		{
			conn.Execute("DELETE FROM web_product_component WHERE cprod_id = @id AND web_unique = @web_unique", new {cprod_id, web_unique});
		}

        public void DeleteForProduct(int web_unique, IDbTransaction tr = null)
        {
	        conn.Execute("DELETE FROM web_product_component WHERE web_unique = @web_unique", new { web_unique}, tr);
        }


        public List<Web_product_component> GetForProduct(int web_unique, IDbConnection conn = null, int? language_id = null)
        {
            var result = new List<Web_product_component>();
            var sql = GetSelect(@"SELECT web_product_component.*,mast_products.*,cust_products.*,cust_products_extradata.* {0} 
                        FROM web_product_component INNER JOIN cust_products ON web_product_component.cprod_id = cust_products.cprod_id 
						INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
						LEFT OUTER JOIN cust_products_extradata ON cust_products.cprod_id = cust_products_extradata.cprod_id
						{1}	
                        WHERE web_product_component.web_unique = @web_unique ", language_id != null,
		        commaBeforeFields: true, commaAfterFields: false);
	        return (conn ?? this.conn).Query<Web_product_component, Mast_products, Cust_products, cust_products_extradata, Web_product_component>(sql,
		        (wpc, mp, cp,cpex) =>
		        {
			        wpc.Component = cp;
					cp.ExtraData = cpex;
			        wpc.Component.MastProduct = mp;
			        return wpc;
		        },
		        new {web_unique, lang = language_id},splitOn: "mast_id, cprod_id, cprod_id").ToList();
                
        }

        private string GetSelect(string initialSql, bool localize = false, bool commaBeforeFields = false, bool commaAfterFields = true)
        {
            var fields = new List<string>();
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
            return @" LEFT OUTER JOIN cust_products_translate ON (cust_products.cprod_id = cust_products_translate.cprod_id AND cust_products_translate.language_id = @lang)";
        }

        private string GetTranslationFields(bool productOnly = true)
        {
            return @"cust_products_translate.cprod_id,
                    cust_products_translate.lang,
                    cust_products_translate.cprod_name";
        }
	}
}
			
			

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Web_product_componentDAL
	{
	
		public static List<Web_product_component> GetAll()
		{
			var result = new List<Web_product_component>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_component", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Web_product_component GetById(int id)
		{
			Web_product_component result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM web_product_component WHERE cprod_id = @id", conn);
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
		
	
		public static Web_product_component GetFromDataReader(MySqlDataReader dr)
		{
			Web_product_component o = new Web_product_component();
		
			o.web_unique =  (int) dr["web_unique"];
			o.cprod_id =  (int) dr["cprod_id"];
		    o.qty = Utilities.FromDbValue<int>(dr["qty"]);
		    o.order = Utilities.FromDbValue<int>(dr["order"]);
		    o.Component = Cust_productsDAL.GetFromDataReader(dr);
			
			return o;

		}
		
		
		public static void Create(Web_product_component o, IDbTransaction tr = null)
        {
            string insertsql = @"INSERT INTO web_product_component (web_unique,cprod_id,qty,order) VALUES(@web_unique,@cprod_id,@qty,@order)";

		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
		    try
		    {
		        if (tr == null)
		            conn.Open();

                var cmd = Utils.GetCommand(insertsql, (MySqlConnection)conn, (MySqlTransaction)tr);

		        BuildSqlParameters(cmd, o);
		        cmd.ExecuteNonQuery();

		    }
		    finally
		    {
                if(tr == null)
                    conn.Dispose();
		    }
        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Web_product_component o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
		    cmd.Parameters.AddWithValue("@qty", o.qty);
		    cmd.Parameters.AddWithValue("@order", o.order);
        }
		
		public static void Update(Web_product_component o)
		{
			string updatesql = @"UPDATE web_product_component SET qty = @qty,order = @order WHERE cprod_id = @cprod_id AND web_unique = @web_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int cprod_id,int web_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM web_product_component WHERE cprod_id = @id AND web_unique = @web_unique" , conn);
                cmd.Parameters.AddWithValue("@id", cprod_id);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.ExecuteNonQuery();
            }
		}

        public static void DeleteForProduct(int web_unique, IDbTransaction tr = null)
        {
            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            try
            {
                if (tr == null)
                    conn.Open();
                var cmd = Utils.GetCommand("DELETE FROM web_product_component WHERE web_unique = @web_unique", (MySqlConnection) conn,(MySqlTransaction) tr);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if(tr == null)
                    conn.Dispose();
            }
        }


        public static List<Web_product_component> GetForProduct(int web_unique, IDbConnection conn = null, int? language_id = null)
        {
            var result = new List<Web_product_component>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            try
            {

                var cmd = Utils.GetCommand(GetSelect(@"SELECT web_product_component.*,cust_products.*,mast_products.* {0} 
                                   FROM web_product_component INNER JOIN cust_products ON web_product_component.cprod_id = cust_products.cprod_id INNER JOIN mast_products ON 
                                   cust_products.cprod_mast = mast_products.mast_id {1}
                                   WHERE web_product_component.web_unique = @web_unique ",language_id != null,commaBeforeFields:true,commaAfterFields:false), (MySqlConnection) conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                if (language_id != null)
                    cmd.Parameters.AddWithValue("@lang", language_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var r = GetFromDataReader(dr);
                    r.Component.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    //r.Component.ProductType = Product_typeDAL.GetById(firstComp.Component.prod;
                    result.Add(r);
                }
                dr.Close();
            }
            finally
            {
                if (dispose)
                    conn.Dispose();
            }
            return result;
        }

        private static string GetSelect(string initialSql, bool localize = false, bool commaBeforeFields = false, bool commaAfterFields = true)
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

        private static string GetTranslationJoin()
        {
            return @" LEFT OUTER JOIN cust_products_translate ON (cust_products.cprod_id = cust_products_translate.cprod_id AND cust_products_translate.language_id = @lang)";
        }

        private static string GetTranslationFields(bool productOnly = true)
        {
            return @"cust_products_translate.cprod_id,
                    cust_products_translate.lang,
                    cust_products_translate.cprod_name";
        }
	}
}
			
			
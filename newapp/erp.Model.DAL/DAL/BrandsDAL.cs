
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public class BrandsDAL
	{
	
		public static List<Brand> GetAll(bool eb_brands_only = true)
		{
			var result = new List<Brand>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM brands WHERE (@eb_brand IS NULL OR eb_brand = @eb_brand)", conn);
				cmd.Parameters.AddWithValue("@eb_brand", eb_brands_only ? (object)1 : DBNull.Value);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}

        public static List<Brand> GetByIds(IList<int> ids )
        {
            var result = new List<Brand>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format("SELECT * FROM brands WHERE brand_id IN ({0})",
                                                Utils.CreateParametersFromIdList(cmd, ids));
                
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Brand> GetByCompanyIds(IList<int> ids)
        {
            var result = new List<Brand>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("", conn);
                cmd.CommandText = string.Format("SELECT * FROM brands WHERE user_id IN ({0})",
                                                Utils.CreateParametersFromIdList(cmd, ids));

                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Brand GetById(int id)
		{
			Brand result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("SELECT * FROM brands WHERE brand_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					result = GetFromDataReader(dr);
				}
				dr.Close();
			}
			return result;
		}
	
		public static Brand GetFromDataReader(MySqlDataReader dr)
		{
			var o = new Brand();
		
			o.brand_id =  (int) dr["brand_id"];
			o.brandname = string.Empty + dr["brandname"];
			o.user_id = Utilities.FromDbValue<int>(dr["user_id"]);
			o.dealerstatus_view = string.Empty + dr["dealerstatus_view"];
			o.code = string.Empty + dr["code"];
			o.image = string.Empty + dr["image"];
			o.eb_brand = Utilities.FromDbValue<int>(dr["eb_brand"]);
			o.category_flag = Utilities.FromDbValue<int>(dr["category_flag"]);
			o.dealerstatus_manual = Utilities.BoolFromLong(dr["dealerstatus_manual"]);
			o.show_as_eb_default = Utilities.BoolFromLong(dr["show_as_eb_default"]);
			o.show_as_eb_products = Utilities.BoolFromLong(dr["show_as_eb_products"]);
			o.brand_group = Utilities.FromDbValue<int>(dr["brand_group"]);
			o.dealersearch_view = Utilities.GetStringOrNull(dr["dealersearch_view"]);
			return o;

		}
		
		public static void Create(Brand o)
		{
			string insertsql = @"INSERT INTO brands (brandname,user_id,dealerstatus_view,code) VALUES(@brandname,@user_id,@dealerstatus_view,@code)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
				BuildSqlParameters(cmd,o);
				cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT brand_id FROM brands WHERE brand_id = LAST_INSERT_ID()";
				o.brand_id = (int) cmd.ExecuteScalar();
				
			}
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Brand o, bool forInsert = true)
		{
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@brand_id", o.brand_id);
			cmd.Parameters.AddWithValue("@brandname", o.brandname);
			cmd.Parameters.AddWithValue("@user_id", o.user_id);
			cmd.Parameters.AddWithValue("@dealerstatus_view", o.dealerstatus_view);
			cmd.Parameters.AddWithValue("@code", o.code);
		}
		
		public static void Update(Brand o)
		{
			string updatesql = @"UPDATE brands SET brandname = @brandname,user_id = @user_id,dealerstatus_view = @dealerstatus_view,code = @code WHERE brand_id = @brand_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
				BuildSqlParameters(cmd,o, false);
				cmd.ExecuteNonQuery();
			}
		}
		
		public static void Delete(int brand_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM brands WHERE brand_id = @id" , conn);
				cmd.Parameters.AddWithValue("@id", brand_id);
				cmd.ExecuteNonQuery();
			}
		}

		public static Brand GetByCode(string brand_code)
		{
			Brand result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("SELECT * FROM brands WHERE code = @code", conn);
				cmd.Parameters.AddWithValue("@code", brand_code);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					result = GetFromDataReader(dr);
				}
				dr.Close();
			}
			return result;
		}

		public static Brand GetByCompanyId(int company_id)
		{
			Brand result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM brands WHERE user_id = @company_id", conn);
				cmd.Parameters.AddWithValue("@company_id", company_id);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					result = GetFromDataReader(dr);
				}
				dr.Close();
			}
			return result;
		}

		public static List<Brand> GetBrandsByCompanyId(int company_id)
		{
			List<Brand> result = new List<Brand>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM brands WHERE user_id = @company_id" , conn);
				cmd.Parameters.AddWithValue("@company_id", company_id);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					var b = GetFromDataReader(dr);
					result.Add(b);

					if (b.brand_group != null)
					{
						dr.Close();
						cmd.CommandText = "SELECT * FROM brands WHERE COALESCE(user_id,0) <> @company_id AND brand_group = @group";
						cmd.Parameters.AddWithValue("@group", b.brand_group);
						dr = cmd.ExecuteReader();
						while (dr.Read())
						{
							result.Add(GetFromDataReader(dr));
						}
					}
				}
				dr.Close();
			}
			return result;
		}
	}
}
			
			
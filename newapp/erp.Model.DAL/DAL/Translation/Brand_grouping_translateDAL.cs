
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Brand_grouping_translateDAL
	{
	
		public static List<Brand_grouping_translate> GetAll()
		{
			var result = new List<Brand_grouping_translate>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM brand_grouping_translate", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Brand_grouping_translate GetById(int id, string lang)
		{
			Brand_grouping_translate result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM brand_grouping_translate WHERE brand_group = @id AND lang = @lang", conn);
				cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@lang", lang);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Brand_grouping_translate GetFromDataReader(MySqlDataReader dr)
		{
			Brand_grouping_translate o = new Brand_grouping_translate();
		
			o.brand_group =  (int) dr["brand_group"];
			o.lang = string.Empty + Utilities.GetReaderField(dr,"lang");
			o.group_desc = string.Empty + Utilities.GetReaderField(dr,"group_desc");
			
			return o;

		}
		
		
		public static void Create(Brand_grouping_translate o)
        {
            string insertsql = @"INSERT INTO brand_grouping_translate (brand_group, lang,group_desc) VALUES(@brand_group, @lang,@group_desc)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Brand_grouping_translate o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@brand_group", o.brand_group);
			cmd.Parameters.AddWithValue("@lang", o.lang);
			cmd.Parameters.AddWithValue("@group_desc", o.group_desc);
		}
		
		public static void Update(Brand_grouping_translate o)
		{
			string updatesql = @"UPDATE brand_grouping_translate SET group_desc = @group_desc WHERE brand_group = @brand_group AND lang = @lang";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int lang)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM brand_grouping_translate WHERE lang = @id" , conn);
                cmd.Parameters.AddWithValue("@id", lang);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
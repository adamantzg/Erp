using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public class JobsDAL
	{
		public static List<Jobs> GetAll()
		{
			var result = new List<Jobs>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM jobs", conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}		
		
		public static Jobs GetById(int id)
		{
			Jobs result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("SELECT * FROM jobs WHERE id = @id", conn);
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
	
		public static Jobs GetFromDataReader(MySqlDataReader dr)
		{
			var o = new Jobs();
		
			o.id =  (int) dr["id"];
            o.position = string.Empty + dr["position"];
            o.description = string.Empty + dr["description"];
            o.company = string.Empty + dr["company"];
            o.qualifications = string.Empty + dr["qualifications"];
            o.location = string.Empty + dr["location"];
            o.city = string.Empty + dr["city"];
            o.date_posted = (DateTime)dr["date_posted"];
            o.date_valid = Utilities.FromDbValue<DateTime>(dr["date_valid"]);
            o.language = Utilities.FromDbValue<int>(dr["language"]);
            o.type = Utilities.FromDbValue<int>(dr["type"]);
			o.type2 = Utilities.FromDbValue<int>(dr["type2"]);
            o.googlemap_link = string.Empty + dr["googlemap_link"];
			return o;

		}
		
		public static void Create(Jobs o)
		{
            string insertsql = @"INSERT INTO jobs (position,description,company,qualifications,location,city,date_posted,date_valid,language,type,type2,googlemap_link) 
                                VALUES(@position,@description,@company,@qualifications,@location,@city,@date_posted,@date_valid,@language,@type,@type2,@googlemap_link)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
				BuildSqlParameters(cmd,o);
				cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT id FROM jobs WHERE id = LAST_INSERT_ID()";
				o.id = (int) cmd.ExecuteScalar();
				
			}
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Jobs o, bool forInsert = true)
		{
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
            cmd.Parameters.AddWithValue("@position", o.position);
            cmd.Parameters.AddWithValue("@description", o.description);
            cmd.Parameters.AddWithValue("@company", o.company);
            cmd.Parameters.AddWithValue("@qualifications", o.qualifications);
            cmd.Parameters.AddWithValue("@location", o.location);
            cmd.Parameters.AddWithValue("@city", o.city);
            cmd.Parameters.AddWithValue("@date_posted", o.date_posted);
            cmd.Parameters.AddWithValue("@date_valid", o.date_valid);
            cmd.Parameters.AddWithValue("@language", o.language);
            cmd.Parameters.AddWithValue("@type", o.type);
            cmd.Parameters.AddWithValue("@type2", o.type2);
            cmd.Parameters.AddWithValue("@googlemap_link", o.googlemap_link);
		}
		
		public static void Update(Jobs o)
		{
            string updatesql = @"UPDATE jobs SET position=@position,description=@description,company=@company,qualifications=@qualifications,location=@location,city=@city,date_posted=@date_posted,
                                date_valid=@date_valid,language=@language,type=@type,type2=@type2,googlemap_link=@googlemap_link WHERE id = @id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand(updatesql, conn);
				BuildSqlParameters(cmd,o, false);
				cmd.ExecuteNonQuery();
			}
		}
		
		public static void Delete(int id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("DELETE FROM jobs WHERE id = @id" , conn);
				cmd.Parameters.AddWithValue("@id", id);
				cmd.ExecuteNonQuery();
			}
		}
	}
}
			
			
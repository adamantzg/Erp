using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
	public class InstructionsDAL
	{
		public static List<Instructions> GetAll()
		{
			var result = new List<Instructions>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var cmd = Utils.GetCommand("SELECT * FROM instructions", conn);
				var dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}		
		
		public static Instructions GetById(int id)
		{
			Instructions result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = Utils.GetCommand("SELECT * FROM instructions LEFT OUTER JOIN languages ON instructions.language_id = languages.language_id WHERE unique_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					result = GetFromDataReader(dr);
				}
				dr.Close();
			    if (result != null)
			    {
			        if (result.language_id != null)
			        {
			            result.Language = LanguagesDAL.GetFromDataReader(dr);
			            result.Language.Countries = CountriesDAL.GetByLanguage(result.language_id.Value, conn);
			        }
			    }
                    
			}
			return result;
		}

        public static List<Instructions> GetByWebUniqueId(int web_unique, IDbConnection conn = null)
        {
            var result = new List<Instructions>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            //using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) 
            {

                MySqlCommand cmd = Utils.GetCommand(@"SELECT instructions.unique_id,instructions.mast_id, instructions.instruction_filename, instructions.language_id
                                    FROM instructions
                                    INNER JOIN mast_products ON mast_products.mast_id = instructions.mast_id
                                    INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast
                                    INNER JOIN web_product_component ON web_product_component.cprod_id = cust_products.cprod_id
                                    WHERE web_product_component.web_unique = @web_unique", (MySqlConnection) conn);
                cmd.Parameters.AddWithValue("@web_unique", web_unique);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    var ins = GetFromDataReader(dr);
                    if (ins.language_id != null) {
                            ins.Language = LanguagesDAL.GetFromDataReader(dr);
                            
                        }
                    result.Add(ins);
                }
                dr.Close();
                foreach (var ins in result)
                {
                    if(ins.language_id != null)
                        ins.Language.Countries = CountriesDAL.GetByLanguage(ins.language_id.Value, conn);
                }
            }
            if(dispose)
                conn.Dispose();
            return result;
        }

		public static List<Instructions> GetForComponents(List<int> cprod_ids)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				return conn.Query<Instructions>(@"SELECT instructions.*, languages.name as languageName FROM instructions
													   INNER JOIN mast_products ON instructions.mast_id = mast_products.mast_id
													   INNER JOIN cust_products ON mast_products.mast_id = cust_products.cprod_mast
														LEFT OUTER JOIN languages ON instructions.language_id = languages.language_id
													   WHERE cust_products.cprod_id IN @ids", new { ids = cprod_ids}).ToList();
			}
		}

		public static List<Instructions> GetByMastId(int mast_id)
        {
            var result = new List<Instructions>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = Utils.GetCommand(@"SELECT
                                                        instructions.*
                                                        FROM
                                                        instructions
                                                       INNER JOIN mast_products ON instructions.mast_id = mast_products.mast_id
                                                       WHERE
                                                        instructions.mast_id=@mast_id
                                                        ", conn);
                cmd.Parameters.AddWithValue("mast_id",mast_id);
                var dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    var ins = GetFromDataReader(dr);
                    result.Add(ins);
                }
            }
            return result;
        }
	
		public static Instructions GetFromDataReader(MySqlDataReader dr)
		{
			var o = new Instructions();

            o.unique_id = (int)dr["unique_id"];
            o.instruction_filename = string.Empty + dr["instruction_filename"];
            o.language_id = Utilities.FromDbValue<int>(dr["language_id"]);
            o.mast_id = Utilities.FromDbValue<int>(dr["mast_id"]);
			return o;

		}
		
		public static void Create(Instructions o)
		{
            string insertsql = @"INSERT INTO instructions (instruction_filename,language_id,mast_id) VALUES(@instruction_filename,@language_id,@mast_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				
				MySqlCommand cmd = Utils.GetCommand(insertsql, conn);
				BuildSqlParameters(cmd,o);
				cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT unique_id FROM instructions WHERE unique_id = LAST_INSERT_ID()";
                o.unique_id = (int)cmd.ExecuteScalar();
				
			}
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Instructions o, bool forInsert = true)
		{
			
			if(!forInsert)
                cmd.Parameters.AddWithValue("@unique_id", o.unique_id);
            cmd.Parameters.AddWithValue("@language_id", o.language_id);
            cmd.Parameters.AddWithValue("@mast_id", o.mast_id);
            cmd.Parameters.AddWithValue("@instruction_filename", o.instruction_filename);
		}
		
		public static void Update(Instructions o)
		{
            string updatesql = @"UPDATE instructions SET instruction_filename=@instruction_filename,language_id=@language_id,mast_id=@mast_id WHERE unique_id = @id";

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
                MySqlCommand cmd = Utils.GetCommand("DELETE FROM instructions WHERE unique_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				cmd.ExecuteNonQuery();
			}
		}
	}
}
			
			
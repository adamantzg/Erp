
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_question_typeDAL
	{
	
		public static List<Survey_question_type> GetAll()
		{
			var result = new List<Survey_question_type>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question_type", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Survey_question_type GetById(int id)
		{
			Survey_question_type result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question_type WHERE question_type_id = @id", conn);
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
		
	
		public static Survey_question_type GetFromDataReader(MySqlDataReader dr)
		{
			Survey_question_type o = new Survey_question_type();
		
			o.question_type_id =  (int) dr["question_type_id"];
			o.name = string.Empty + Utilities.GetReaderField(dr,"name");
			
			return o;

		}
		
		
		public static void Create(Survey_question_type o)
        {
            string insertsql = @"INSERT INTO survey_question_type (question_type_id,name) VALUES(@question_type_id,@name)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(question_type_id)+1 FROM survey_question_type", conn);
                o.question_type_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_question_type o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@question_type_id", o.question_type_id);
			cmd.Parameters.AddWithValue("@name", o.name);
		}
		
		public static void Update(Survey_question_type o)
		{
			string updatesql = @"UPDATE survey_question_type SET name = @name WHERE question_type_id = @question_type_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int question_type_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_question_type WHERE question_type_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", question_type_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
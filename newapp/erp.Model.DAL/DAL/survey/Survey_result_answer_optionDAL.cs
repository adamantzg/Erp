
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_result_answer_optionDAL
	{
	
		public static List<Survey_result_answer_option> GetAll()
		{
			var result = new List<Survey_result_answer_option>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result_answer_option", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Survey_result_answer_option> GetForAnswer(int answer_id)
        {
            var result = new List<Survey_result_answer_option>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result_answer_option WHERE answer_id = @answer_id", conn);
                cmd.Parameters.AddWithValue("@answer_id", answer_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Survey_result_answer_option GetById(int id)
		{
			Survey_result_answer_option result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result_answer_option WHERE question_option_id = @id", conn);
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
		
	
		public static Survey_result_answer_option GetFromDataReader(MySqlDataReader dr)
		{
			Survey_result_answer_option o = new Survey_result_answer_option();
		
			o.answer_id =  (int) dr["answer_id"];
			o.question_option_id =  (int) dr["question_option_id"];
			
			return o;

		}
		
		
		public static void Create(Survey_result_answer_option o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO survey_result_answer_option (answer_id,question_option_id) VALUES(@answer_id,@question_option_id)";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();
            var cmd = Utils.GetCommand(insertsql,conn,tr);
			BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
            
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_result_answer_option o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@answer_id", o.answer_id);
			cmd.Parameters.AddWithValue("@question_option_id", o.question_option_id);
		}
		
		public static void Update(Survey_result_answer_option o)
		{
			string updatesql = @"UPDATE survey_result_answer_option SET  WHERE question_option_id = @question_option_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int question_option_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_result_answer_option WHERE question_option_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", question_option_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
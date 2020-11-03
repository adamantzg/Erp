
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_question_optionDAL
	{
	
		public static List<Survey_question_option> GetAll()
		{
			var result = new List<Survey_question_option>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question_option", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Survey_question_option> GetForQuestion(int question_id)
        {
            var result = new List<Survey_question_option>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question_option WHERE question_id = @question_id", conn);
                cmd.Parameters.AddWithValue("@question_id", question_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Survey_question_option GetById(int id)
		{
			Survey_question_option result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question_option WHERE question_option_id = @id", conn);
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
		
	
		public static Survey_question_option GetFromDataReader(MySqlDataReader dr)
		{
			Survey_question_option o = new Survey_question_option();
		
			o.question_option_id =  (int) dr["question_option_id"];
			o.question_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"question_id"));
			o.option_text = string.Empty + Utilities.GetReaderField(dr,"option_text");
			o.option_value = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"option_value"));
			
			return o;

		}
		
		
		public static void Create(Survey_question_option o)
        {
            string insertsql = @"INSERT INTO survey_question_option (question_option_id,question_id,option_text,option_value) VALUES(@question_option_id,@question_id,@option_text,@option_value)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                var cmd = Utils.GetCommand("SELECT MAX(question_option_id)+1 FROM survey_question_option", conn);
                o.question_option_id = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_question_option o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@question_option_id", o.question_option_id);
			cmd.Parameters.AddWithValue("@question_id", o.question_id);
			cmd.Parameters.AddWithValue("@option_text", o.option_text);
			cmd.Parameters.AddWithValue("@option_value", o.option_value);
		}
		
		public static void Update(Survey_question_option o)
		{
			string updatesql = @"UPDATE survey_question_option SET question_id = @question_id,option_text = @option_text,option_value = @option_value WHERE question_option_id = @question_option_id";

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
				var cmd = Utils.GetCommand("DELETE FROM survey_question_option WHERE question_option_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", question_option_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
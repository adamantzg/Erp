
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_result_answerDAL
	{
	
		public static List<Survey_result_answer> GetAll()
		{
			var result = new List<Survey_result_answer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result_answer", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Survey_result_answer> GetForDefinition(int def_id, int? question_id = null)
        {
            var result = new List<Survey_result_answer>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM survey_result_answer INNER JOIN survey_result ON survey_result_answer.result_id = survey_result.result_id INNER JOIN dealers ON
                                              survey_result.dealer_id = dealers.user_id
                                              WHERE surveydef_id = @def_id AND (question_id = @question_id OR @question_id IS NULL)", conn);
                cmd.Parameters.AddWithValue("@def_id", def_id);
                cmd.Parameters.AddWithValue("@question_id", Utilities.ToDBNull(question_id));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var a = GetFromDataReader(dr);
                    a.Result = Survey_resultDAL.GetFromDataReader(dr);
                    a.Result.Dealer = new Dealer{user_name = string.Empty + dr["user_name"],user_type = Utilities.FromDbValue<int>(dr["user_type"])};
                    result.Add(a);
                }
                dr.Close();
                foreach (var r in result)
                {
                    r.Options = Survey_result_answer_optionDAL.GetForAnswer(r.answer_id);
                }
            }
            return result;
        }
		
		
		public static Survey_result_answer GetById(int id)
		{
			Survey_result_answer result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result_answer WHERE answer_id = @id", conn);
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
		
	
		public static Survey_result_answer GetFromDataReader(MySqlDataReader dr)
		{
			Survey_result_answer o = new Survey_result_answer();
		
			o.answer_id =  (int) dr["answer_id"];
			o.question_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"question_id"));
			o.value = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"value"));
			o.result_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"result_id"));
			o.comment = string.Empty + Utilities.GetReaderField(dr,"comment");
			
			return o;

		}
		
		
		public static void Create(Survey_result_answer o, MySqlTransaction tr = null)
        {
            string insertsql = @"INSERT INTO survey_result_answer (answer_id,question_id,value,result_id,comment) VALUES(@answer_id,@question_id,@value,@result_id,@comment)";

		    var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if(tr == null)
                conn.Open();
            
            var cmd = Utils.GetCommand(insertsql, conn,tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT answer_id FROM survey_result_answer WHERE answer_id = LAST_INSERT_ID()";
            o.answer_id = (int)cmd.ExecuteScalar();

            if (o.Options != null)
            {
                foreach (var opt in o.Options)
                {
                    opt.answer_id = o.answer_id;
                    Survey_result_answer_optionDAL.Create(opt,tr);
                }
            }
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_result_answer o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@answer_id", o.answer_id);
			cmd.Parameters.AddWithValue("@question_id", o.question_id);
			cmd.Parameters.AddWithValue("@value", o.value);
			cmd.Parameters.AddWithValue("@result_id", o.result_id);
			cmd.Parameters.AddWithValue("@comment", o.comment);
		}
		
		public static void Update(Survey_result_answer o)
		{
			string updatesql = @"UPDATE survey_result_answer SET question_id = @question_id,value = @value,result_id = @result_id,comment = @comment WHERE answer_id = @answer_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int answer_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_result_answer WHERE answer_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", answer_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
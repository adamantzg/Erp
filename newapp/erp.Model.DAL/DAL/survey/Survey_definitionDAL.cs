
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_definitionDAL
	{
	
		public static List<Survey_definition> GetAll()
		{
			var result = new List<Survey_definition>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_definition", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Survey_definition GetById(int id)
		{
			Survey_definition result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_definition WHERE surveydef_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
                result.Questions = Survey_questionDAL.GetForSurvey(id);
                foreach (var q in result.Questions)
                {
                    q.Children = Survey_questionDAL.GetChildren(q.question_id);
                    q.Options = Survey_question_optionDAL.GetForQuestion(q.question_id);
                }
            }
			return result;
		}
		
	
		public static Survey_definition GetFromDataReader(MySqlDataReader dr)
		{
			Survey_definition o = new Survey_definition();
		
			o.surveydef_id =  (int) dr["surveydef_id"];
			o.title = string.Empty + Utilities.GetReaderField(dr,"title");
			o.datecreated = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"datecreated"));
		    o.ViewName = string.Empty + dr["viewname"];
            o.result_title = string.Empty + dr["result_title"];
			return o;

		}
		
		
		public static void Create(Survey_definition o)
        {
            string insertsql = @"INSERT INTO survey_definition (title,datecreated) VALUES(@title,@datecreated)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT surveydef_id FROM survey_definition WHERE surveydef_id = LAST_INSERT_ID()";
                o.surveydef_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_definition o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@surveydef_id", o.surveydef_id);
			cmd.Parameters.AddWithValue("@title", o.title);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
		}
		
		public static void Update(Survey_definition o)
		{
			string updatesql = @"UPDATE survey_definition SET title = @title,datecreated = @datecreated WHERE surveydef_id = @surveydef_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int surveydef_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_definition WHERE surveydef_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", surveydef_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			
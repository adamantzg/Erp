
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_questionDAL
	{
	
		public static List<Survey_question> GetAll()
		{
			var result = new List<Survey_question>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Survey_question> GetForSurvey(int survey_id)
        {
            var result = new List<Survey_question>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question WHERE surveydef_id = @survey_id", conn);
                cmd.Parameters.AddWithValue("@survey_id", survey_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Survey_question> GetChildren(int parent_id)
        {
            var result = new List<Survey_question>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question WHERE parent_id = @parent_id", conn);
                cmd.Parameters.AddWithValue("@parent_id", parent_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Survey_question GetById(int id)
		{
			Survey_question result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_question WHERE question_id = @id", conn);
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
		
	
		public static Survey_question GetFromDataReader(MySqlDataReader dr)
		{
			Survey_question o = new Survey_question();
		
			o.question_id =  (int) dr["question_id"];
			o.surveydef_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"surveydef_id"));
			o.parent_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"parent_id"));
			o.question_type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"question_type"));
			o.text = string.Empty + Utilities.GetReaderField(dr,"text");
			o.uitype = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"uitype"));
			o.order = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"order"));
			o.comment = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"comment")) != null ? (bool?) Convert.ToBoolean(Utilities.GetReaderField(dr,"comment")) : null;
			o.optionset_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"optionset_id"));
			o.commentlabel = string.Empty + Utilities.GetReaderField(dr,"commentlabel");
		    o.yaxislabel = string.Empty + dr["yaxislabel"];
			
			return o;

		}
		
		
		public static void Create(Survey_question o)
        {
            string insertsql = @"INSERT INTO survey_question (surveydef_id,group_id,question_type,text,uitype,order,comment,optionset_id,commentlabel) VALUES(@surveydef_id,@group_id,@question_type,@text,@uitype,@order,@comment,@optionset_id,@commentlabel)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT question_id FROM survey_question WHERE question_id = LAST_INSERT_ID()";
                o.question_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_question o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@question_id", o.question_id);
			cmd.Parameters.AddWithValue("@surveydef_id", o.surveydef_id);
			cmd.Parameters.AddWithValue("@group_id", o.parent_id);
			cmd.Parameters.AddWithValue("@question_type", o.question_type);
			cmd.Parameters.AddWithValue("@text", o.text);
			cmd.Parameters.AddWithValue("@uitype", o.uitype);
			cmd.Parameters.AddWithValue("@order", o.order);
			cmd.Parameters.AddWithValue("@comment", o.comment);
			cmd.Parameters.AddWithValue("@optionset_id", o.optionset_id);
			cmd.Parameters.AddWithValue("@commentlabel", o.commentlabel);
		}
		
		public static void Update(Survey_question o)
		{
			string updatesql = @"UPDATE survey_question SET surveydef_id = @surveydef_id,group_id = @group_id,question_type = @question_type,text = @text,uitype = @uitype,order = @order,comment = @comment,optionset_id = @optionset_id,commentlabel = @commentlabel WHERE question_id = @question_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int question_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_question WHERE question_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", question_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			